package com.hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import com.hoten.gridia.Player;
import com.hoten.gridia.serving.ConnectionToGridiaClientHandler;
import com.hoten.gridia.serving.ServingGridia;
import com.hoten.servingjava.message.JsonMessageHandler;
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
            connection.send(server.messageBuilder.genericEventHandler(ex.getMessage()));
        }
    }
}
