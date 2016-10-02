package com.hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import com.hoten.gridia.Player;
import com.hoten.gridia.map.Coord;
import com.hoten.gridia.map.Tile;
import com.hoten.gridia.serializers.GridiaGson;
import com.hoten.gridia.serving.ConnectionToGridiaClientHandler;
import com.hoten.gridia.serving.ServingGridia;
import com.hoten.servingjava.message.JsonMessageHandler;
import java.io.IOException;

public class Hit extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) throws IOException {
        ServingGridia server = connection.getServer();
        Player player = connection.getPlayer();
        Coord loc = GridiaGson.get().fromJson(data.get("loc"), Coord.class);
        Tile tile = server.tileMap.getTile(loc);
        server.dispatchEvent("MovedInto", tile.cre, "entity", player.creature, "location", loc);
    }
}
