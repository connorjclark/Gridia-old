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

public class ServingGridia extends ServingSocket<ConnectionToGridiaClientHandler> {

    private final List<Creature> creatures = new ArrayList();

    public ServingGridia(int port, File clientDataFolder, String localDataFolderName) throws IOException {
        super(port, new GridiaProtocols(), clientDataFolder, localDataFolderName);
    }

    public void createCreature() {
        Creature creature = new Creature();
        creature.location.set(2, 2);
        creatures.add(creature);
        
        Message message = new JsonMessageBuilder()
                .protocol(outbound(AddCreature))
                .set("id", creature.id)
                .set("loc", creature.location)
                .build();
        sendToAll(message);
    }

    @Override
    protected ConnectionToGridiaClientHandler makeNewConnection(Socket newConnection) throws IOException {
        return new ConnectionToGridiaClientHandler(this, newConnection);
    }
}
