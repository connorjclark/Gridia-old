package com.hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import com.hoten.gridia.Player;
import com.hoten.gridia.map.Coord;
import com.hoten.gridia.serializers.GridiaGson;
import com.hoten.gridia.serving.ConnectionToGridiaClientHandler;
import com.hoten.gridia.serving.ServingGridia;
import com.hoten.servingjava.message.JsonMessageHandler;

public class PlayerMove extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) {
        ServingGridia server = connection.getServer();
        Player player = connection.getPlayer();
        Coord loc = GridiaGson.get().fromJson(data.get("loc"), Coord.class);
        boolean onRaft = data.get("onRaft").getAsBoolean();
        int timeForMovement = data.get("timeForMovement").getAsInt();
        
        server.moveCreatureTo(player.creature, server.tileMap.wrap(loc), timeForMovement, false, onRaft);
        player.openedContainers.clear();
    }
}
