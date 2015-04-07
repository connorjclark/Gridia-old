package com.hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import com.hoten.gridia.Player;
import com.hoten.gridia.map.Coord;
import com.hoten.gridia.serializers.GridiaGson;
import com.hoten.gridia.serving.ConnectionToGridiaClientHandler;
import com.hoten.gridia.serving.ServingGridia;
import com.hoten.servingjava.message.JsonMessageHandler;

public class PerformAction extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) {
        ServingGridia server = connection.getServer();
        Player player = connection.getPlayer();
        Coord loc = GridiaGson.get().fromJson(data.get("loc"), Coord.class);
        int actionId = data.get("id").getAsInt();

        server.dispatchEvent("Action", player.creature, "actionId", actionId, "loc", loc);
    }
}
