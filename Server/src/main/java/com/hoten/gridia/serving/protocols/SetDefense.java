package com.hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import com.hoten.gridia.Player;
import com.hoten.gridia.scripting.Entity;
import com.hoten.gridia.serving.ConnectionToGridiaClientHandler;
import com.hoten.servingjava.message.JsonMessageHandler;

import java.io.IOException;
import java.util.Arrays;

public class SetDefense extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) throws IOException {
        Player player = connection.getPlayer();
        int defensePoints = data.get("defense").getAsInt();

        player.creature.setAttribute("defense", defensePoints);
    }
}
