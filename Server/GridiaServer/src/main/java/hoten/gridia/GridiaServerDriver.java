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
import hoten.serving.fileutils.FileUtils;
import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.Scanner;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;
import java.util.function.Predicate;
import java.util.logging.Level;
import java.util.logging.Logger;

public class GridiaServerDriver {

    private static final int port = 1234;
    private static ServingGridia server;

    public static void main(String[] args) throws IOException {
        File splash = new File("splash.txt");
        if (splash.exists()) {
            System.out.println(FileUtils.readTextFile(splash));
        }
        runServerMenu();
    }

    private static void runServerMenu() throws IOException {
        Scanner scanner = new Scanner(System.in);

        System.out.println("Load which world?\n");
        File[] worlds = new File("worlds/").listFiles(file -> file.isDirectory());
        for (int i = 0; i < worlds.length; i++) {
            System.out.println(i + 1 + ") " + worlds[i].getName());
        }
        int worldSelection = promptInt(scanner, "");
        File world = worlds[worldSelection - 1];

        System.out.println("Load which map, or generate a new one?\n");
        File[] maps = new File(world, "maps").listFiles(file -> file.isDirectory());
        if (maps != null) {
            for (int i = 0; i < maps.length; i++) {
                System.out.println(i + 1 + ") " + maps[i].getName());
            }
        }
        System.out.println("0) Generate Map");

        int choice = promptInt(scanner, "");
        switch (choice) {
            case 0:
                String mapName = promptString(scanner, "Map name?");
                File map = new File(world, "maps/" + mapName);

                int numPoints = promptInt(scanner, "How many voronoi points? (recommended: 30000)");
                int numLloydRelaxations = promptInt(scanner, "How many lloyd relaxations? (recommended: at least 1)");
                long seed = promptString(scanner, "Seed? (can be anything)").hashCode();

                int mapSectorSize = promptInt(scanner, "Sector size? (recommended: 20)");
                int mapSize = promptInt(scanner, "Map size? (MUST be a multiple of sector size!)");
                int mapDepth = promptInt(scanner, "Depth? (at least 1, recommended: 2)");

                ContentManager contentManager = new ContentManager(world);
                GridiaGson.initialize(contentManager);
                MapGenerator mapGenerator = new MapGenerator(contentManager, numPoints, numLloydRelaxations, seed);
                TileMap tileMap = mapGenerator.generate(map, mapSize, mapDepth, mapSectorSize);
                tileMap.save();
                System.out.println("Map created!");
                main(null);
                break;
            default:
                loadWorld(world, maps[choice - 1].getName());
                break;
        }
    }

    private static int promptInt(Scanner scanner, String prompt) {
        while (true) {
            System.out.println(prompt);
            try {
                return Integer.parseInt(scanner.nextLine());
            } catch (NumberFormatException ex) {
                System.out.println("Value must be an integer.");
            }
        }
    }

    private static String promptString(Scanner scanner, String prompt) {
        System.out.println(prompt);
        return scanner.nextLine();
    }

    private static void loadWorld(File world, String mapName) throws IOException {
        File clientDataDir = new File(world, "clientdata");
        String localDataDirName = world.getName();
        server = new ServingGridia(world, mapName, port, clientDataDir, localDataDirName);
        server.tileMap.loadAll(); // :(
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

        // save
        Executors.newScheduledThreadPool(1).scheduleAtFixedRate(() -> {
            if (server.anyPlayersOnline()) {
                server.tileMap.save();
            }
        }, 12, 12, TimeUnit.HOURS);

        // grow items
        Executors.newScheduledThreadPool(1).scheduleAtFixedRate(() -> {
            server.grow();
        }, 10, 10, TimeUnit.SECONDS);

        // hard code the roach quest for the presentation
        if (mapName.equals("DemoCity")) {
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
                        if (playersInArena.size() >= 1) {
                            startArena();
                        }
                    }
                }
            }, 0, arenaTickRate, TimeUnit.MILLISECONDS);
        }

        System.out.println("Server started.");
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
            if (!playersInArena.isEmpty()) {
                Creature winner = null;
                int highestAntenae = 0;

                for (Creature player : playersInArena) {
                    int amount = removeItemFromInventory(player, 447).quantity;
                    if (amount >= highestAntenae) {
                        winner = player;
                        highestAntenae = amount;
                    }
                }

                playersInArena.remove(winner);

                playersInArena.forEach(creature -> {
                    server.moveCreatureTo(creature, loserTeleportLocation, true);
                });

                server.moveCreatureTo(winner, winnerTeleportLocation, true);

                server.sendToAll(server.messageBuilder.chat("Game over! Winner: " + winner.name + "\nMost Antenae: " + highestAntenae, winnerTeleportLocation));
            } else {
                server.sendToAll(server.messageBuilder.chat("Game over! Winner: None\nMost Antenae: 0", winnerTeleportLocation));
            }
            clearArena();
        } else {
            sayMessageInArena("Seconds left: " + timeLeft / 1000);
            timeLeft -= arenaTickRate;
        }
    }

    private static void sayMessageInArena(String msg) {
        Coord middleOfArena = arenaLocation.add(arenaSize / 2, arenaSize / 2, 0);
        server.sendToAll(server.messageBuilder.chat(msg, middleOfArena));
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
        Monster roachData = server.contentManager.getMonster(42);
        try {
            roachData = roachData.clone();
            roachData.name = "";
        } catch (CloneNotSupportedException ex) {
            Logger.getLogger(GridiaServerDriver.class.getName()).log(Level.SEVERE, null, ex);
        }
        Random random = new Random();
        for (int i = 0; i < numRoaches; i++) {
            Coord loc = arenaLocation.add(random.nextInt(arenaSize), random.nextInt(arenaSize), 0);
            server.createCreature(roachData, loc);
        }
        sayMessageInArena("BEGIN!");
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
