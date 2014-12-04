package hoten.gridia;

import hoten.gridia.content.ContentManager;
import hoten.gridia.content.Item;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.map.Coord;
import hoten.gridia.map.TileMap;
import hoten.gridia.serializers.GridiaGson;
import hoten.gridia.serving.ServingGridia;
import hoten.gridia.worldgen.MapGenerator;
import java.io.File;
import java.io.IOException;
import java.nio.file.Paths;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;

public class GridiaServerDriver {

    private static final int port = 1234;
    private static ServingGridia server;

    public static void main(String[] args) throws IOException {
        if (args.length == 0) {
            //args = "TestWorld RoachCity 30000 20 51235053089343 1000 2 20".split("\\s+");
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

        Executors.newScheduledThreadPool(1).scheduleAtFixedRate(() -> {
            if (server.anyPlayersOnline()) {
                moveMonstersRandomly();
            }
        }, 0, 1500, TimeUnit.MILLISECONDS);

        // temp code to handle cave up/down
        Executors.newScheduledThreadPool(1).scheduleAtFixedRate(() -> {
            server.creatures.values().forEach(creature -> {
                if (!creature.justTeleported) {
                    ItemInstance itemUnder = server.tileMap.getItem(creature.location);
                    Coord loc = creature.location;
                    if (loc.z != server.tileMap.depth && itemUnder.data.itemClass == Item.ItemClass.Cave_down) {
                        server.moveCreatureTo(creature, loc.add(0, 0, 1));
                        creature.justTeleported = true;
                    } else if (loc.z != 0 && itemUnder.data.itemClass == Item.ItemClass.Cave_up) {
                        server.moveCreatureTo(creature, loc.add(0, 0, -1));
                        creature.justTeleported = true;
                    }
                }
            });
        }, 0, 1000, TimeUnit.MILLISECONDS);

        // save every minute
        Executors.newScheduledThreadPool(1).scheduleAtFixedRate(() -> {
            if (server.anyPlayersOnline()) {
                server.tileMap.save();
            }
        }, 0, 1500, TimeUnit.MILLISECONDS);

        System.out.println("Server started.");
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
}
