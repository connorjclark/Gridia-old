package com.hoten.gridia;

import com.hoten.gridia.content.ContentManager;
import com.hoten.gridia.content.WorldContentLoader;
import com.hoten.gridia.map.TileMap;
import com.hoten.gridia.serializers.GridiaGson;
import com.hoten.gridia.serving.ServingGridia;
import com.hoten.gridia.worldgen.MapGenerator;
import com.hoten.servingjava.ServingPolicyFile;
import com.hoten.servingjava.message.MessageHandler;
import java.io.File;
import java.io.IOException;
import java.util.Arrays;
import java.util.Scanner;
import org.apache.commons.io.FileUtils;

public class GridiaServerDriver {

    private static final int DEFAULT_PORT = 1044;
    private static ServingGridia server;

    public static void main(String[] args) throws IOException {
        MessageHandler.loadMessageHandlers(Arrays.asList("com.hoten.gridia.serving.protocols"));
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
        server.startServer();
        
        try {
            new ServingPolicyFile(port).start();
        } catch (IOException ex) {
            System.out.println("Error starting policy file socket. Port 843 is probably already taken. Webplayer version will not work. Check for any applications running on that port (Dropbox?)");
            System.out.println(ex);
        }

        System.out.println("Server started on port " + port);
    }
}
