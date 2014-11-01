package hoten.gridiaserver;

import com.google.gson.JsonObject;
import hoten.serving.Protocols;
import hoten.serving.SocketHandler;
import java.io.DataInputStream;
import java.io.IOException;
import java.net.Socket;
import static hoten.gridiaserver.GridiaProtocols.Clientbound.*;
import hoten.serving.BinaryMessageBuilder;

public class ConnectionToGridiaClientHandler extends SocketHandler {

    private final ServingGridia server;

    public ConnectionToGridiaClientHandler(ServingGridia server, Socket socket) throws IOException {
        super(socket, new GridiaProtocols(), Protocols.BoundDest.CLIENT);
        this.server = server;
    }

    @Override
    protected void onConnectionSettled() throws IOException {
        //server.sendCreatures(this);
    }

    @Override
    protected void handleData(int type, JsonObject data) throws IOException {
        switch (GridiaProtocols.Serverbound.values()[type]) {
            case SectorRequest:
                ProcessSectorRequest(data);
                break;
        }
    }

    @Override
    protected void handleData(int type, DataInputStream data) throws IOException {
        throw new UnsupportedOperationException("Not supported yet."); //To change body of generated methods, choose Tools | Templates.
    }

    private void ProcessSectorRequest(JsonObject data) throws IOException {
        int sx = data.get("x").getAsInt();
        int sy = data.get("y").getAsInt();
        int sz = data.get("z").getAsInt();

        Tile[][] tiles = server.tileMap.getSector(sx, sy, sz)._tiles;
        BinaryMessageBuilder builder = new BinaryMessageBuilder()
                .protocol(outbound(SectorData))
                .writeInt(sx)
                .writeInt(sy)
                .writeInt(sz);
        for (int x = 0; x < tiles.length; x++) {
            for (int y = 0; y < tiles.length; y++) {
                builder.writeShort(tiles[x][y].floor);
                builder.writeShort(tiles[x][y].item);
            }
        }
        send(builder.build());
    }
}
