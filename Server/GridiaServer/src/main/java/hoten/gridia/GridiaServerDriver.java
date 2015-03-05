package hoten.gridia;

import hoten.gridia.content.ContentManager;
import hoten.gridia.content.WorldContentLoader;
import hoten.gridia.map.TileMap;
import hoten.gridia.serializers.GridiaGson;
import hoten.gridia.serving.ServingGridia;
import hoten.gridia.worldgen.MapGenerator;
import hoten.serving.ServingFlashPolicy;
import hoten.serving.message.MessageHandler;
import java.io.File;
import java.io.IOException;
import java.util.Arrays;
import java.util.Scanner;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;
import org.apache.commons.io.FileUtils;

public class GridiaServerDriver {

    private static final int DEFAULT_PORT = 1044;
    private static ServingGridia server;

    public static void main(String[] args) throws IOException {
        MessageHandler.loadMessageHandlers(Arrays.asList("hoten.gridia.serving.protocols"));
        showSplash();
        runServerMenu();
    }

    private static void showSplash() throws IOException {
        File splash = new File("splash.txt");
        if (splash.exists()) {
            System.out.println(FileUtils.readFileToString(splash));
        }
    }

    private static void runServerMenu() throws IOException {
        Scanner scanner = new Scanner(System.in);

        System.out.println(String.format("Server port? (Leave blank for %d)\n", DEFAULT_PORT));
        int port;
        try {
            port = Integer.parseInt(scanner.nextLine());
        } catch (NumberFormatException ex) {
            port = DEFAULT_PORT;
        }

        System.out.println("Load which world?\n");
        File[] worlds = new File("worlds/").listFiles(file -> file.isDirectory());
        for (int i = 0; i < worlds.length; i++) {
            System.out.println(i + 1 + ") " + worlds[i].getName());
        }
        int worldSelection = promptInt(scanner, "");
        File world = worlds[worldSelection - 1];

        System.out.println("Load a map, or generate a new one?\n");
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

                ContentManager contentManager = new WorldContentLoader(world).load();
                GridiaGson.initialize(contentManager, null);
                MapGenerator mapGenerator = new MapGenerator(contentManager, numPoints, numLloydRelaxations, seed);
                TileMap tileMap = mapGenerator.generate(map, mapSize, mapDepth, mapSectorSize);
                tileMap.save();
                System.out.println("Map created!");
                loadWorld(world, mapName, port);
                break;
            default:
                loadWorld(world, maps[choice - 1].getName(), port);
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

    private static void loadWorld(File world, String mapName, int port) throws IOException {
        File clientDataDir = new File(world, "clientdata");
        String localDataDirName = "worlds/" + world.getName() + "/clientdata";
        server = new ServingGridia(world, mapName, port, clientDataDir, localDataDirName);
        server.tileMap.loadAll(); // :(
        server.startServer();

        new ServingFlashPolicy(port).start();

        Executors.newScheduledThreadPool(1).scheduleAtFixedRate(() -> {
            if (server.anyPlayersOnline()) {
                moveMonstersRandomly();
            }
        }, 0, 1500, TimeUnit.MILLISECONDS);

        // roach quest and random monster dungeon
        if (mapName.equals("demo-city")) {
            RoachQuest roachQuest = new RoachQuest(server);
            Executors.newScheduledThreadPool(1).scheduleAtFixedRate(roachQuest, 0, roachQuest.arenaTickRate, TimeUnit.MILLISECONDS);

            RandomDungeonQuest randomDungeonQuest = new RandomDungeonQuest(server);
            Executors.newScheduledThreadPool(1).scheduleAtFixedRate(randomDungeonQuest, 0, randomDungeonQuest.arenaTickRate, TimeUnit.MILLISECONDS);
        }

        System.out.println("Server started on port " + port);
    }

    private static void moveMonstersRandomly() {
        server.creatures.values().stream()
                .filter(cre -> !cre.belongsToPlayer && !cre.name.equals("Stacy the Mayor"))
                .forEach(cre -> {
                    if (Math.random() > 0.5) {
                        server.moveCreatureRandomly(cre);
                    }
                });
    }
}
