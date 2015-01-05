package hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import hoten.gridia.Player;
import hoten.gridia.serving.ConnectionToGridiaClientHandler;
import hoten.gridia.serving.ServingGridia;
import hoten.serving.message.JsonMessageHandler;
import hoten.serving.message.Message;
import java.io.IOException;

public class Login extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) throws IOException {
        ServingGridia server = connection.getServer();
        String username = data.get("username").getAsString();
        String passwordHash = data.get("passwordHash").getAsString();
        
        try {
            doLogin(connection, server.playerFactory.load(server, username, passwordHash));
        } catch (Player.PlayerFactory.BadLoginException ex) {
            connection.send(server.messageBuilder.genericEventHandler(ex.getMessage()));
        }
    }
    
    // :(
    public void doLogin(ConnectionToGridiaClientHandler connection, Player thePlayer) throws IOException {
        ServingGridia server = connection.getServer();
        connection.player = thePlayer;
        connection.send(server.messageBuilder.genericEventHandler("success"));
        connection.send(server.messageBuilder.setFocus(thePlayer.creature.id, thePlayer.accountDetails.isAdmin));
        connection.send(server.messageBuilder.container(thePlayer.creature.inventory));
        connection.send(server.messageBuilder.container(thePlayer.equipment));

        Message animMessage = server.messageBuilder.animation(3, thePlayer.creature.location);
        server.sendToClientsWithAreaLoaded(animMessage, thePlayer.creature.location);
        connection.send(animMessage);

        thePlayer.updatePlayerImage(server);
    }
}
