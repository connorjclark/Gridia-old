package hoten.gridia.serving.protocol;

import com.google.gson.JsonObject;
import hoten.gridia.Player;
import hoten.gridia.serving.ConnectionToGridiaClientHandler;
import hoten.gridia.serving.ServingGridia;
import hoten.serving.message.JsonMessageHandler;
import java.io.IOException;

public class Register extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) throws IOException {
        ServingGridia server = connection.getServer();
        String username = data.get("username").getAsString();
        String passwordHash = data.get("passwordHash").getAsString();

        try {
            new Login().doLogin(connection, server.playerFactory.create(server, username, passwordHash));
        } catch (Player.PlayerFactory.BadRegistrationException ex) {
            connection.send(server.messageBuilder.genericEventListener(ex.getMessage()));
        }
    }
}
