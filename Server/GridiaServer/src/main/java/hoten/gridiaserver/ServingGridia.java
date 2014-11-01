package hoten.gridiaserver;

import hoten.serving.ServingSocket;
import java.io.File;
import java.io.IOException;
import java.net.Socket;
import java.util.ArrayList;
import java.util.List;
import static hoten.gridiaserver.GridiaProtocols.Clientbound.*;
import hoten.serving.JsonMessageBuilder;
import hoten.serving.Message;
import java.util.Random;

public class ServingGridia extends ServingSocket<ConnectionToGridiaClientHandler> {

    public final TileMap tileMap;
    public final List<Creature> creatures = new ArrayList();
    private final Random random = new Random();

    public ServingGridia(int port, File clientDataFolder, String localDataFolderName) throws IOException {
        super(port, new GridiaProtocols(), clientDataFolder, localDataFolderName);
        tileMap = new TileMap(100, 1, 20);
        tileMap.loadAll();
    }

    public void moveCreatures() {
        creatures.forEach(cre -> {
            moveCreature(cre);
        });
    }
    
    public void sendCreatures(ConnectionToGridiaClientHandler client) {
        creatures.forEach(cre -> {
            sendTo(createCreatureMessage(cre), client);
        });
    }

    private void moveCreature(Creature cre) {
        int x = cre.location.x;
        int y = cre.location.y;
        int diff = random.nextInt(2) * (random.nextBoolean() ? 1 : -1);
        if (random.nextBoolean()) {
            x += diff;
        } else {
            y += diff;
        }
        cre.location.set(x, y);

        Message message = new JsonMessageBuilder()
                .protocol(outbound(MoveCreature))
                .set("id", cre.id)
                .set("loc", cre.location)
                .build();
        sendToAll(message);
    }

    public void createCreature() {
        Creature cre = new Creature();
        cre.location.set(random.nextInt(10), random.nextInt(10));
        creatures.add(cre);
        sendToAll(createCreatureMessage(cre));
    }

    public Message createCreatureMessage(Creature cre) {
        return new JsonMessageBuilder()
                .protocol(outbound(AddCreature))
                .set("id", cre.id)
                .set("loc", cre.location)
                .build();
    }

    @Override
    protected ConnectionToGridiaClientHandler makeNewConnection(Socket newConnection) throws IOException {
        return new ConnectionToGridiaClientHandler(this, newConnection);
    }
}
