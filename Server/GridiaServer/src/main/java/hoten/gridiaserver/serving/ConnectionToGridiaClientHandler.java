package hoten.gridiaserver.serving;

import com.google.gson.Gson;
import com.google.gson.JsonObject;
import hoten.gridiaserver.Inventory;
import hoten.gridiaserver.map.Coord;
import hoten.gridiaserver.content.ItemInstance;
import hoten.gridiaserver.Player;
import hoten.gridiaserver.content.Item;
import hoten.gridiaserver.content.ItemUse;
import hoten.gridiaserver.map.Sector;
import hoten.serving.message.Protocols;
import hoten.serving.SocketHandler;
import java.io.DataInputStream;
import java.io.IOException;
import java.net.Socket;
import java.util.ArrayList;
import java.util.List;

public class ConnectionToGridiaClientHandler extends SocketHandler {

    private final List<Sector> _loadedSectors = new ArrayList();
    private final GridiaMessageToClientBuilder _messageBuilder;
    private final Gson _gson = new Gson();
    private final ServingGridia _server;
    public Player player;

    public ConnectionToGridiaClientHandler(ServingGridia server, Socket socket) throws IOException {
        super(socket, new GridiaProtocols(), Protocols.BoundDest.CLIENT);
        _server = server;
        _messageBuilder = server.messageBuilder;
    }

    public boolean hasSectorLoaded(Sector sector) {
        return _loadedSectors.contains(sector);
    }

    @Override
    protected void onConnectionSettled() throws IOException {
        player = new Player();
        player.username = System.currentTimeMillis() + "";
        player.creature = _server.createCreatureForPlayer();
        _server.announceNewPlayer(this, player);
        send(_messageBuilder.initialize(_server.tileMap.size, _server.tileMap.depth, _server.tileMap.sectorSize));
        send(_messageBuilder.setFocus(player.creature.id));

        // fake an inventory
        List<ItemInstance> inv = new ArrayList();
        inv.add(_server.contentManager.createItemInstance(57));
        inv.add(_server.contentManager.createItemInstance(280));
        inv.add(_server.contentManager.createItemInstance(1067));
        for (int i = 0; i < 20; i++) {
            inv.add(_server.contentManager.createItemInstance(0));
        }

        player.inventory = new Inventory(inv);
        send(_messageBuilder.inventory(player.inventory));
    }

    @Override
    protected void handleData(int type, JsonObject data) throws IOException {
        switch (GridiaProtocols.Serverbound.values()[type]) {
            case PlayerMove:
                ProcessPlayerMove(data);
                break;
            case SectorRequest:
                ProcessSectorRequest(data);
                break;
            case CreatureRequest:
                ProcessCreatureRequest(data);
                break;
            case MoveItem:
                ProcessMoveItem(data);
                break;
            case Chat:
                ProcessChat(data);
                break;
            case UseItem:
                ProcessUseItem(data);
                break;
        }
    }

    @Override
    protected void handleData(int type, DataInputStream data) throws IOException {
        throw new UnsupportedOperationException("Not supported yet.");
    }

    @Override
    protected synchronized void close() {
        super.close();
        _server.removeCreature(player.creature);
        _server.sendToAll(_messageBuilder.chat(player.username + " has left the building."));
    }

    private void ProcessPlayerMove(JsonObject data) throws IOException {
        Coord loc = _gson.fromJson(data.get("loc"), Coord.class);
        _server.moveCreatureTo(player.creature, loc);
    }

    private void ProcessSectorRequest(JsonObject data) throws IOException {
        int sx = data.get("x").getAsInt();
        int sy = data.get("y").getAsInt();
        int sz = data.get("z").getAsInt();
        Sector sector = _server.tileMap.getSector(sx, sy, sz);
        _loadedSectors.add(sector);
        send(_messageBuilder.sectorRequest(sector));
    }

    private void ProcessCreatureRequest(JsonObject data) throws IOException {
        int id = data.get("id").getAsInt();
        send(_messageBuilder.addCreature(_server.creatures.get(id)));
    }

    private ItemInstance getItemFrom(String from, int index) {
        switch (from) {
            case "world":
                return _server.tileMap.getItem(_server.tileMap.getCoordFromIndex(index));
            case "inv":
                if (index == -1) {
                    return ItemInstance.NONE;
                }
                return player.inventory.get(index);
            default:
                return ItemInstance.NONE;
        }
    }

    private void removeItemAt(String from, int index) {
        switch (from) {
            case "world":
                _server.changeItem(_server.tileMap.getCoordFromIndex(index), ItemInstance.NONE);
                break;
            case "inv":
                player.inventory.deleteSlot(index);
                break;
        }
    }

    private void ProcessMoveItem(JsonObject data) throws IOException {
        String source = data.get("source").getAsString();
        String dest = data.get("dest").getAsString();
        int sourceIndex = data.get("si").getAsInt();
        int destIndex = data.get("di").getAsInt();

        if (source.equals(dest) && sourceIndex == destIndex) {
            return;
        }

        ItemInstance item = getItemFrom(source, sourceIndex);

        if (item == ItemInstance.NONE) {
            return;
        }

        boolean moveSuccessful = false;
        switch (dest) {
            case "world":
                moveSuccessful = _server.addItem(_server.tileMap.getCoordFromIndex(destIndex), item);
                break;
            case "inv":
                moveSuccessful = player.inventory.add(item, destIndex);
                break;
        }

        if (!moveSuccessful) {
            return;
        }

        removeItemAt(source, sourceIndex);
    }

    private void ProcessChat(JsonObject data) throws IOException {
        String msg = data.get("msg").getAsString();

        if ("!clear".equals(msg)) {
            int size = _server.tileMap.size;
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    for (int k = 0; k < _server.tileMap.depth; k++) {
                        _server.tileMap.setItem(ItemInstance.NONE, i, j, k);
                    }
                }
            }
        }

        _server.sendToAll(_messageBuilder.chat(player.username + " says: " + msg));
    }

    private void ProcessUseItem(JsonObject data) {
        String source = data.get("source").getAsString();
        String dest = data.get("dest").getAsString();
        int sourceIndex = data.get("si").getAsInt();
        int destIndex = data.get("di").getAsInt();
        System.out.println(source + " " + dest);

        ItemInstance tool = getItemFrom(source, sourceIndex);
        ItemInstance focus = getItemFrom(dest, destIndex);

        System.out.println(tool.data.name + " " + focus.data.name);
        List<ItemUse> uses = _server.contentManager.getItemUses(tool.data, focus.data);
        System.out.println(uses.size());
        if (uses.isEmpty()) {
            return;
        }
        ItemUse use = uses.get(0);

        if (use.focusQuantityConsumed > 0) {
            focus.quantity -= use.focusQuantityConsumed;
            switch (dest) {
                case "world":
                    _server.updateTile(destIndex);
                    break;
            }
        }

        if ("world".equals(dest)) {
            ItemInstance item = _server.contentManager.createItemInstance(use.products.get(0));
            _server.addItemNear(_server.tileMap.getCoordFromIndex(destIndex), item, 3);
            use.products.stream()
                    .skip(1)
                    .forEach(product -> {
                        ItemInstance productInstance = _server.contentManager.createItemInstance(product);
                        _server.addItemNear(destIndex, productInstance, 3);
                    });
        }
    }
}
