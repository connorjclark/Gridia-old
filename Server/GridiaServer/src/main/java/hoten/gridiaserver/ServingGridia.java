package hoten.gridiaserver;

import hoten.serving.ServingSocket;
import java.io.File;
import java.io.IOException;
import java.net.Socket;
import static hoten.gridiaserver.GridiaProtocols.Clientbound.*;
import hoten.serving.message.JsonMessageBuilder;
import hoten.serving.message.Message;
import java.util.Map;
import java.util.Random;
import java.util.concurrent.ConcurrentHashMap;

public class ServingGridia extends ServingSocket<ConnectionToGridiaClientHandler> {

    public final GridiaMessageToClientBuilder messageBuilder = new GridiaMessageToClientBuilder(this::outbound);
    public final TileMap tileMap;
    public final ContentManager contentManager;
    public final Map<Integer, Creature> creatures = new ConcurrentHashMap();
    private final Random random = new Random();

    public ServingGridia(int port, File clientDataFolder, String localDataFolderName) throws IOException {
        super(port, new GridiaProtocols(), clientDataFolder, localDataFolderName);
        contentManager = new ContentManager("TestWorld");
        tileMap = new TileMap(100, 1, 20);
        tileMap.loadAll();
    }

    @Override
    protected ConnectionToGridiaClientHandler makeNewConnection(Socket newConnection) throws IOException {
        return new ConnectionToGridiaClientHandler(this, newConnection);
    }

    public void sendToClientsWithSectorLoaded(Message message, Sector sector) {
        sendTo(message, client -> client.hasSectorLoaded(sector));
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
        Sector sector = tileMap.getSectorOf(loc);
        tileMap.getTile(cre.location).cre = null;
        tileMap.getTile(loc).cre = cre;
        cre.location = loc;
        sendToClientsWithSectorLoaded(messageBuilder.moveCreature(cre), sector);
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
        if (tileMap.getTile(x, y, cre.location.z).cre != null) {
            return;
        }
        moveCreatureTo(cre, new Coord(x, y, cre.location.z));
    }

    public Creature createCreature() {
        Creature cre = new Creature();
        //cre.location.set(random.nextInt(tileMap.size), random.nextInt(tileMap.size), 0);
        cre.location.set(random.nextInt(tileMap.size / 10), random.nextInt(tileMap.size / 10), 0);
        Sector sector = tileMap.getSectorOf(cre.location);
        tileMap.getTile(cre.location).cre = cre;
        sendToClientsWithSectorLoaded(messageBuilder.addCreature(cre), sector);
        creatures.put(cre.id, cre);
        return cre;
    }

    public Creature createCreatureForPlayer() {
        Creature cre = createCreature();
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
}
