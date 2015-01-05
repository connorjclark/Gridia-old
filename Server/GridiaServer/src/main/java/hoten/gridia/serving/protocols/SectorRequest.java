package hoten.gridia.serving.protocols;

import com.google.gson.JsonObject;
import hoten.gridia.map.Sector;
import hoten.gridia.serving.ConnectionToGridiaClientHandler;
import hoten.gridia.serving.ServingGridia;
import hoten.serving.message.JsonMessageHandler;
import java.io.IOException;

public class SectorRequest extends JsonMessageHandler<ConnectionToGridiaClientHandler> {

    @Override
    protected void handle(ConnectionToGridiaClientHandler connection, JsonObject data) throws IOException {
        ServingGridia server = connection.getServer();
        int sx = data.get("x").getAsInt();
        int sy = data.get("y").getAsInt();
        int sz = data.get("z").getAsInt();
        
        Sector sector = server.tileMap.getSector(sx, sy, sz);
        connection.addToLoadedSectors(sector);
        connection.send(server.messageBuilder.sectorRequest(sector));
    }
}
