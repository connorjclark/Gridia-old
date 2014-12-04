package hoten.gridia;

import hoten.gridia.content.ContentManager;
import hoten.gridia.content.Monster;
import hoten.gridia.map.Coord;
import hoten.gridia.map.TileMap;
import hoten.gridia.serializers.GridiaGson;
import hoten.gridia.serving.ServingGridia;
import hoten.gridia.worldgen.MapGenerator;
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
        if (args.length == 0) {
            args = "TestWorld RoachCity".split("\\s+");
        }

        String worldName = args[0];
        String mapName = args[1];

        if (args.length != 2) {
            int numPoints = Integer.parseInt(args[2]);
            int numLloydRelaxations = Integer.parseInt(args[3]);
            long seed = Long.parseLong(args[4]);

            int mapSize = Integer.parseInt(args[5]);
            int mapDepth = Integer.parseInt(args[6]);
            int mapSectorSize = Integer.parseInt(args[7]);

            ContentManager contentManager = new ContentManager(worldName);
            GridiaGson.initialize(contentManager);
            MapGenerator mapGenerator = new MapGenerator(contentManager, numPoints, numLloydRelaxations, seed);
            TileMap tileMap = mapGenerator.generate(worldName + "/" + mapName, mapSize, mapDepth, mapSectorSize);
            tileMap.save();
        }

        File clientDataDir = Paths.get(worldName, "clientdata").toFile();
        String localDataDirName = worldName;
        server = new ServingGridia(worldName, mapName, port, clientDataDir, localDataDirName);
        server.startServer();
        System.out.println("Server started.");

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
