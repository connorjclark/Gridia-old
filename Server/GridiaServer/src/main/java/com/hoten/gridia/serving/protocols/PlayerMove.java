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
        Coord delta = GridiaGson.get().fromJson(data.get("delta"), Coord.class);
        boolean onRaft = data.get("onRaft").getAsBoolean();
        int timeForMovement = data.get("timeForMovement").getAsInt();
        
        Coord loc = server.tileMap.wrap(player.creature.location.add(delta));
        server.moveCreatureTo(player.creature, loc, timeForMovement, false, onRaft);
        player.openedContainers.clear();
    }
}
