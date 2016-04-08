package com.hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import com.hoten.gridia.Player;
import com.hoten.gridia.scripting.Entity;
import com.hoten.gridia.serving.ConnectionToGridiaClientHandler;
import com.hoten.servingjava.message.JsonMessageHandler;

import java.io.IOException;
import java.util.Arrays;

public class Attack extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) throws IOException {
        Player player = connection.getPlayer();
        int attackPoints = data.get("attack").getAsInt();

        if (attackPoints > 0) {
            Entity target = (Entity) player.creature.getAttribute("target");
            target.callMethod("hurt", Arrays.asList(attackPoints, player.creature.callMethod("generateDeathReason", Arrays.asList())));
        } else {
            player.creature.callMethod("hurt", Arrays.asList(-attackPoints, "tripped on his fists"));
        }
    }
}
