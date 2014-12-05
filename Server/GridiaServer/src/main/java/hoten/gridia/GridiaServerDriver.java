package hoten.gridia;

import hoten.gridia.content.ContentManager;
import hoten.gridia.content.Item;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.content.Monster;
import hoten.gridia.map.Coord;
import hoten.gridia.map.TileMap;
import hoten.gridia.serializers.GridiaGson;
import hoten.gridia.serving.ServingGridia;
import hoten.gridia.worldgen.MapGenerator;
import java.io.File;
import java.io.IOException;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;
import java.util.function.Predicate;

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
                        server.moveCreatureTo(creature, loc.add(0, 0, 1), true);
                        creature.justTeleported = true;
                    } else if (loc.z != 0 && itemUnder.data.itemClass == Item.ItemClass.Cave_up) {
                        server.moveCreatureTo(creature, loc.add(0, 0, -1), true);
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
        }, 0, 1, TimeUnit.MINUTES);

        // hard code the roach quest for the presentation
        Executors.newScheduledThreadPool(1).scheduleAtFixedRate(() -> {
            if (server.anyPlayersOnline()) {
                List<Creature> playersInArena = getPlayersInArena();

                if (arenaIsGoing) {
                    if (playersInArena.isEmpty()) {
                        clearArena();
                    } else {
                        stepArena();
                    }
                } else {
                    if (playersInArena.size() > 0) {
                        startArena();
                    }
                }
            }
        }, 0, 1000, TimeUnit.MILLISECONDS);

        System.out.println("Server started.");
    }

    // hard code spawn location
    private static Coord playerSpawn = new Coord(498, 543, 0);

    public static Coord getPlayerSpawn() {
        Random random = new Random();
        return playerSpawn.add(random.nextInt(3), random.nextInt(3), 0);
    }

    // hard code the roach quest for the presentation
    private static int arenaTickRate = 5000;
    private static int arenaDuration = 30 * 1000;
    private static Coord arenaLocation = new Coord(495, 481, 1);
    private static Coord winnerTeleportLocation = new Coord(550, 485, 1);
    private static Coord loserTeleportLocation = new Coord(550, 490, 1);
    private static int arenaSize = 23;
    private static int numRoaches = 50;
    private static boolean arenaIsGoing;
    private static int timeLeft;

    private static void stepArena() {
        if (timeLeft == 0) {
            List<Creature> playersInArena = getPlayersInArena();
            Creature winner = playersInArena.stream()
                    .max((creature1, creature2) -> {
                        int numAntanea1 = removeItemFromInventory(creature1, 447).quantity;
                        int numAntanea2 = removeItemFromInventory(creature2, 447).quantity;
                        return Integer.compare(numAntanea1, numAntanea2);
                    }).get();

            playersInArena.remove(winner);

            playersInArena.forEach(creature -> {
                server.moveCreatureTo(creature, loserTeleportLocation, true);
            });

            server.moveCreatureTo(winner, winnerTeleportLocation, true);

            server.sendToAll(server.messageBuilder.chat("Game over!"));
            clearArena();
        } else {
            timeLeft -= arenaTickRate;
        }
    }

    private static ItemInstance removeItemFromInventory(Creature creature, int itemId) {
        ItemInstance item = server.contentManager.createItemInstance(itemId, 0);
        for (int i = 0; i < creature.inventory.size(); i++) {
            ItemInstance itemAtSlot = creature.inventory.get(i);
            if (itemAtSlot.data.id == itemId) {
                item.quantity += itemAtSlot.quantity;
                creature.inventory.deleteSlot(i);
            }
        }
        return item;
    }

    private static void clearArena() {
        arenaIsGoing = false;
        List<Creature> monsters = getCreaturesInArea(arenaLocation, arenaSize, creature -> !creature.belongsToPlayer);
        monsters.forEach(monster -> {
            server.removeCreature(monster);
        });
    }

    private static void startArena() {
        arenaIsGoing = true;
        timeLeft = arenaDuration;
        Monster roach = server.contentManager.getMonster(42);
        Random random = new Random();
        for (int i = 0; i < numRoaches; i++) {
            Coord loc = arenaLocation.add(random.nextInt(arenaSize), random.nextInt(arenaSize), 0);
            server.createCreature(roach, loc);
        }
    }

    private static List<Creature> getCreaturesInArea(Coord areaLocation, int size, Predicate<Creature> selector) {
        List<Creature> creaturesInArea = new ArrayList<>();
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                Coord loc = areaLocation.add(x, y, 0);
                Creature cre = server.tileMap.getCreature(loc);
                if (cre != null && selector.test(cre)) {
                    creaturesInArea.add(cre);
                }
            }
        }
        return creaturesInArea;
    }

    private static List<Creature> getPlayersInArena() {
        return getCreaturesInArea(arenaLocation, arenaSize, creature -> creature.belongsToPlayer);
    }

    // end arena code
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
