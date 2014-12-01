package hoten.gridia;

import hoten.gridia.content.Monster;
import hoten.gridia.map.Coord;
import hoten.gridia.serving.ServingGridia;
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
        for (int i = 0; i < 1000; i++) {
            Coord randCoord = randomCoord();
            if (server.tileMap.getTile(randCoord).floor != 1) {
                int randomItemId = possibleItems.get((int) (possibleItems.size() * Math.random()));
                server.changeItem(randCoord, server.contentManager.createItemInstance(randomItemId));
            }
        }

        Executors.newScheduledThreadPool(1).scheduleAtFixedRate(() -> {
            if (server.anyPlayersOnline() && server.creatures.size() < 100) {
                spawnMonster();
            }
        }, 0, 500, TimeUnit.MILLISECONDS);

        Executors.newScheduledThreadPool(1).scheduleAtFixedRate(() -> {
            if (server.anyPlayersOnline()) {
                moveMonstersRandomly();
            }
        }, 0, 1500, TimeUnit.MILLISECONDS);
    }

    private static void moveMonstersRandomly() {
        server.creatures.values().stream()
                .filter(cre -> !cre.belongsToPlayer)
                .forEach(cre -> {
                    if (Math.random() > 0.5) {
                        server.moveCreatureRandomly(cre);
                    }
                });
    }

    private static void spawnMonster() {
        List<Integer> possibleMonsters = Arrays.asList(120, 5, 63, 1);
        int randomMonsterId = possibleMonsters.get((int) (possibleMonsters.size() * Math.random()));
        Monster mold = server.contentManager.getMonster(randomMonsterId);
        server.createCreature(mold, randomCoord());
    }

    private static Coord randomCoord() {
        int size = server.tileMap.size;
        int x = (int) (Math.random() * size);
        int y = (int) (Math.random() * size);
        int z = (int) (Math.random() * server.tileMap.depth);
        if (!server.tileMap.walkable(x, y, z)) {
            return randomCoord();
        }
        return new Coord(x, y, z);
    }
}
