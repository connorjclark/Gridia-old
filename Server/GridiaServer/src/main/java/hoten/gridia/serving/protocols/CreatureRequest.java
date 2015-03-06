package hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import hoten.gridia.Creature;
import hoten.gridia.serving.ConnectionToGridiaClientHandler;
import hoten.gridia.serving.ServingGridia;
import com.hoten.servingjava.message.JsonMessageHandler;
import java.io.IOException;

public class CreatureRequest extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) throws IOException {
        ServingGridia server = connection.getServer();
        int id = data.get("id").getAsInt();
        
        Creature cre = server.creatures.get(id);
        if (cre != null) {
            connection.send(server.messageBuilder.addCreature(cre));
        }
    }
}
