package hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import hoten.gridia.Player;
import hoten.gridia.serving.ConnectionToGridiaClientHandler;
import hoten.gridia.serving.ServingGridia;
import hoten.serving.message.JsonMessageHandler;
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
        connection.send(server.messageBuilder.container(thePlayer.creature.inventory, -199));
        connection.send(server.messageBuilder.container(thePlayer.equipment, -999));

        server.playWarpAnimation(thePlayer.creature.location);

        thePlayer.updatePlayerImage(server);

        server.dispatchEvent("onPlayerLogin", "player", thePlayer);

        connection.send(server.messageBuilder.chat(server.whoIsOnline(), thePlayer.creature.location));
    }
}
