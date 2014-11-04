package hoten.gridiaserver;

import java.io.File;
import java.io.IOException;
import java.nio.file.Paths;
import java.util.Random;
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

        for (int i = 0; i < 10; i++) {
            server.createCreature();
        }

        Random r = new Random();
        int size = server.tileMap.size;
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                for (int k = 0; k < server.tileMap.depth; k++) {
                    if (r.nextFloat() > 0.9) {
                        server.tileMap.setItem(server.contentManager.createItemInstance(10), i, j, k);
                    }
                }
            }
        }

        Executors.newScheduledThreadPool(1).scheduleAtFixedRate(() -> {
            System.out.println("tick " + System.currentTimeMillis());
            server.creatures.values().stream().filter(cre -> !cre.belongsToPlayer).forEach(cre -> {
                server.moveCreatureRandomly(cre);
            });
            for (int i = 0; i < 100; i++) {
                server.changeItem(randomCoord(), server.contentManager.createItemInstance((int) (Math.random() * 10)));
            }
        }, 0, 1, TimeUnit.SECONDS);
    }

    public static Coord randomCoord() {
        int size = server.tileMap.size;
        int x = (int) (Math.random() * size);
        int y = (int) (Math.random() * size);
        int z = (int) (Math.random() * server.tileMap.depth);
        return new Coord(x, y, z);
    }
}
