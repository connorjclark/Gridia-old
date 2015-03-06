package hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import hoten.gridia.map.Coord;
import hoten.gridia.serializers.GridiaGson;
import hoten.gridia.serving.ConnectionToGridiaClientHandler;
import hoten.gridia.serving.ServingGridia;
import com.hoten.servingjava.message.JsonMessageHandler;

public class AdminMakeFloor extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) {
        ServingGridia server = connection.getServer();
        Coord loc = GridiaGson.get().fromJson(data.get("loc"), Coord.class);
        int floor = data.get("floor").getAsInt();
        
        server.changeFloor(loc, floor);
    }
}
