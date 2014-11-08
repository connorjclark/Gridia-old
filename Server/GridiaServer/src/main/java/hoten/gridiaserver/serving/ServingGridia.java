package hoten.gridiaserver.serving;

import hoten.gridiaserver.content.ContentManager;
import hoten.gridiaserver.map.Coord;
import hoten.gridiaserver.Creature;
import hoten.gridiaserver.Inventory;
import hoten.gridiaserver.content.ItemInstance;
import hoten.gridiaserver.Player;
import hoten.gridiaserver.map.Sector;
import hoten.gridiaserver.map.SectorLoader;
import hoten.gridiaserver.map.SectorSaver;
import hoten.gridiaserver.map.Tile;
import hoten.gridiaserver.map.TileMap;
import hoten.gridiaserver.serializers.GridiaGson;
import hoten.serving.ServingSocket;
import java.io.File;
import java.io.IOException;
import java.net.Socket;
import static hoten.gridiaserver.serving.GridiaProtocols.Clientbound.*;
import hoten.serving.message.JsonMessageBuilder;
import hoten.serving.message.Message;
import java.util.Map;
import java.util.Random;
import java.util.concurrent.ConcurrentHashMap;

public class ServingGridia extends ServingSocket<ConnectionToGridiaClientHandler> {

    public static ServingGridia instance; // :(

    public final GridiaMessageToClientBuilder messageBuilder = new GridiaMessageToClientBuilder(this::outbound);
    public final TileMap tileMap;
    public final ContentManager contentManager;
    public final Map<Integer, Creature> creatures = new ConcurrentHashMap();
    private final Random random = new Random();

    public ServingGridia(int port, File clientDataFolder, String localDataFolderName) throws IOException {
        super(port, new GridiaProtocols(), clientDataFolder, localDataFolderName);
        contentManager = new ContentManager("TestWorld");
        GridiaGson.initialize(contentManager);
        SectorLoader sectorLoader = new SectorLoader();
        SectorSaver sectorSaver = new SectorSaver();
        tileMap = new TileMap(100, 1, 20, sectorLoader, sectorSaver);
        tileMap.loadAll();
        instance = this;
    }

    @Override
    protected ConnectionToGridiaClientHandler makeNewConnection(Socket newConnection) throws IOException {
        return new ConnectionToGridiaClientHandler(this, newConnection);
    }

    public boolean anyPlayersOnline() {
        return !_clients.isEmpty();
    }

    public void sendToClientsWithSectorLoadedBut(Message message, Sector sector, ConnectionToGridiaClientHandler client) {
        sendTo(message, c -> c.hasSectorLoaded(sector) && client != c);
    }

    public void sendToClientsWithSectorLoaded(Message message, Sector sector) {
        sendTo(message, c -> c.hasSectorLoaded(sector));
    }

    public void sendToClientsWithAreaLoaded(Message message, Coord loc) {
        sendTo(message, c -> c.hasSectorLoaded(tileMap.getSectorOf(loc)));
    }

    public void sendCreatures(ConnectionToGridiaClientHandler client) {
        creatures.values().forEach(cre -> {
            sendTo(messageBuilder.addCreature(cre), client);
        });
    }

    public void removeCreature(Creature cre) {
        Sector sector = tileMap.getSectorOf(cre.location);
        creatures.remove(cre.id);
        tileMap.getTile(cre.location).cre = null;
        Creature.uniqueIds.retire(cre.id);
        sendToClientsWithSectorLoaded(messageBuilder.removeCreature(cre), sector);
    }

    public void moveCreatureTo(Creature cre, Coord loc) {
        tileMap.wrap(loc);
        Sector sector = tileMap.getSectorOf(loc);
        tileMap.getTile(cre.location).cre = null;
        tileMap.getTile(loc).cre = cre;
        cre.location = loc;
        sendToClientsWithSectorLoaded(messageBuilder.moveCreature(cre), sector);
    }

    public void movePlayerTo(ConnectionToGridiaClientHandler client, Creature cre, Coord loc) {
        tileMap.wrap(loc);
        Sector sector = tileMap.getSectorOf(loc);
        tileMap.getTile(cre.location).cre = null;
        tileMap.getTile(loc).cre = cre;
        cre.location = loc;
        sendToClientsWithSectorLoadedBut(messageBuilder.moveCreature(cre), sector, client);
    }

    public void moveCreatureRandomly(Creature cre) {
        int x = tileMap.wrap(cre.location.x);
        int y = tileMap.wrap(cre.location.y);
        int diff = random.nextBoolean() ? 1 : -1;
        if (random.nextBoolean()) {
            x += diff;
        } else {
            y += diff;
        }
        if (tileMap.walkable(x, y, cre.location.z)) {
            moveCreatureTo(cre, new Coord(x, y, cre.location.z));
        }
    }

    public void createCreatureRandomly(int image) {
        Coord c = new Coord(random.nextInt(tileMap.size / 10), random.nextInt(tileMap.size / 10), 0);
        if (tileMap.walkable(c.x, c.y, c.z)) {
            createCreature(image, c);
        }
    }

    public Creature createCreature(int image, Coord loc) {
        Creature cre = new Creature();
        cre.image = image;
        cre.location = loc;
        Sector sector = tileMap.getSectorOf(cre.location);
        tileMap.getTile(cre.location).cre = cre;
        sendToClientsWithSectorLoaded(messageBuilder.addCreature(cre), sector);
        creatures.put(cre.id, cre);
        return cre;
    }

    public Creature createCreatureForPlayer() {
        Creature cre = createCreature((int) (Math.random() * 100), new Coord(random.nextInt(4), random.nextInt(4), 0));
        cre.belongsToPlayer = true;
        return cre;
    }

    public void announceNewPlayer(ConnectionToGridiaClientHandler client, Player player) {
        Message message = new JsonMessageBuilder()
                .protocol(outbound(Chat))
                .set("msg", String.format("%s has joined the game!", player.username))
                .build();
        sendToAllBut(message, client);
    }

    public void moveItem(Coord from, Coord to) {
        ItemInstance fromItem = tileMap.getItem(from);
        ItemInstance toItem = tileMap.getItem(to);
        if (toItem.data.id == 0) {
            changeItem(from, ItemInstance.NONE);
            changeItem(to, fromItem);
        }
    }

    public void changeItem(Coord loc, ItemInstance item) {
        tileMap.setItem(item, loc);
        updateTile(loc);
    }

    private void updateTile(Coord loc) {
        Tile tile = tileMap.getTile(loc);
        sendToClientsWithAreaLoaded(messageBuilder.updateTile(loc, tile), loc);
    }

    //adds item only if it is to an empty tile or if it would stack
    public boolean addItem(Coord loc, ItemInstance itemToAdd) {
        ItemInstance currentItem = tileMap.getTile(loc).item;
        boolean willStack = ItemInstance.stackable(currentItem, itemToAdd);
        if (currentItem.data.id != 0 && !willStack) {
            return false;
        }
        int q = currentItem.quantity + itemToAdd.quantity;
        itemToAdd.quantity = q;
        changeItem(loc, itemToAdd);
        return true;
    }

    public void updateInventorySlot(Inventory inventory, int slotIndex) {
        Message message = messageBuilder.updateInventorySlot(inventory, slotIndex);
        sendTo(message, client -> client.player.inventory.id == inventory.id);
    }
}
