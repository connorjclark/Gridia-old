package com.hoten.gridia.serving;

import com.hoten.gridia.Player;
import com.hoten.gridia.map.Sector;
import com.hoten.servingjava.SocketHandler;
import com.hoten.servingjava.SocketHandlerImpl;
import com.hoten.servingjava.message.Message;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.Socket;
import java.util.ArrayList;
import java.util.List;

public class ConnectionToGridiaClientHandler implements SocketHandler {

    private final SocketHandler _socketHandler;
    private final List<Sector> _loadedSectors = new ArrayList(); // :( Player?
    private final ServingGridia _server;
    public Player player;

    public ConnectionToGridiaClientHandler(Socket socket, ServingGridia server) throws IOException {
        _socketHandler = new SocketHandlerImpl(socket);
        _server = server;
    }

    @Override
    public void start(Runnable onConnectionSettled, SocketHandler topLevelSocketHandler) throws IOException, InstantiationException, IllegalAccessException {
        _socketHandler.start(onConnectionSettled, topLevelSocketHandler);
    }

    @Override
    public void send(Message message) throws IOException {
        _socketHandler.send(message);
    }

    @Override
    public void close() {
        _socketHandler.close();
    }

    @Override
    public DataOutputStream getOutputStream() {
        return _socketHandler.getOutputStream();
    }

    @Override
    public DataInputStream getInputStream() {
        return _socketHandler.getInputStream();
    }

    public boolean hasSectorLoaded(Sector sector) {
        return _loadedSectors.contains(sector);
    }

    public void addToLoadedSectors(Sector sector) {
        _loadedSectors.add(sector);
    }

    public ServingGridia getServer() {
        return _server;
    }

    public Player getPlayer() {
        return player;
    }
}
