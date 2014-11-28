package hoten.gridiaserver;

import hoten.gridiaserver.map.Coord;
import hoten.gridiaserver.serving.ServingGridia;
import java.io.File;
import java.io.IOException;
import java.nio.file.Paths;
import java.util.Arrays;
import java.util.List;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;

public class GridiaServerDriver {

    private static final int port = 1234;
    private static ServingGridia server;

    public static void main(String[] args) throws IOException {
        File clientDataDir = Paths.get("TestWorld", "clientdata").toFile();
        String localDataDirName = "TestWorld";
        server = new ServingGridia(port, clientDataDir, localDataDirName);
        server.startServer();
        System.out.println("Server started.");

        List<Integer> possibleItems = Arrays.asList(263, 260, 188, 264, 575);
        for (int i = 0; i < 2000; i++) {
            Coord randCoord = randomCoord();
            if (server.tileMap.getTile(randCoord).floor != 1) {
                int randomItemId = possibleItems.get((int) (possibleItems.size() * Math.random()));
                server.changeItem(randCoord, server.contentManager.createItemInstance(randomItemId));
            }
        }

        Executors.newScheduledThreadPool(1).scheduleAtFixedRate(() -> {
            // nothing right now
        }, 0, 200, TimeUnit.MILLISECONDS);
    }

    public static Coord randomCoord() {
        int size = server.tileMap.size;
        int x = (int) (Math.random() * size);
        int y = (int) (Math.random() * size);
        int z = (int) (Math.random() * server.tileMap.depth);
        return new Coord(x, y, z);
    }
}
