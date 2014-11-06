package hoten.gridiaserver.serving;

import com.google.gson.Gson;
import com.google.gson.JsonObject;
import hoten.gridiaserver.map.Coord;
import hoten.gridiaserver.content.ItemInstance;
import hoten.gridiaserver.Player;
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
    private Player _player;

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
        _player = new Player();
        _player.username = System.currentTimeMillis() + "";
        _player.creature = _server.createCreatureForPlayer();
        _server.announceNewPlayer(this, _player);
        send(_messageBuilder.initialize(_server.tileMap.size, _server.tileMap.depth, _server.tileMap.sectorSize));
        send(_messageBuilder.setFocus(_player.creature.id));

        // fake an inventory
        List<ItemInstance> inv = new ArrayList();
        for (int i = 0; i < 20; i++) {
            inv.add(_server.contentManager.createItemInstance((int) (Math.random() * 100)));
        }
        send(_messageBuilder.inventory(inv));
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
        }
    }

    @Override
    protected void handleData(int type, DataInputStream data) throws IOException {
        throw new UnsupportedOperationException("Not supported yet.");
    }

    @Override
    protected synchronized void close() {
        super.close();
        _server.removeCreature(_player.creature);
        _server.sendToAll(_messageBuilder.chat(_player.username + " has left the building."));
    }

    private void ProcessPlayerMove(JsonObject data) throws IOException {
        Coord loc = _gson.fromJson(data.get("loc"), Coord.class);
        _server.movePlayerTo(this, _player.creature, loc);
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

    private void ProcessMoveItem(JsonObject data) throws IOException {
        Gson gson = new Gson();
        Coord from = gson.fromJson(data.get("from"), Coord.class);
        Coord to = gson.fromJson(data.get("to"), Coord.class);
        _server.moveItem(from, to);
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

        _server.sendToAll(_messageBuilder.chat(_player.username + " says: " + msg));
    }
}
