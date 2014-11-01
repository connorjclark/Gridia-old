package hoten.gridiaserver;

import com.google.gson.JsonObject;
import hoten.serving.Protocols;
import hoten.serving.SocketHandler;
import java.io.DataInputStream;
import java.io.IOException;
import java.net.Socket;

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
        throw new UnsupportedOperationException("Not supported yet."); //To change body of generated methods, choose Tools | Templates.
    }

    @Override
    protected void handleData(int type, DataInputStream data) throws IOException {
        throw new UnsupportedOperationException("Not supported yet."); //To change body of generated methods, choose Tools | Templates.
    }
}
