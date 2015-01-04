package hoten.gridia.serving.protocol;

import com.google.gson.JsonObject;
import hoten.gridia.Player;
import hoten.gridia.map.Coord;
import hoten.gridia.serializers.GridiaGson;
import hoten.gridia.serving.ConnectionToGridiaClientHandler;
import hoten.gridia.serving.ServingGridia;
import hoten.serving.message.JsonMessageHandler;

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
    }
}
