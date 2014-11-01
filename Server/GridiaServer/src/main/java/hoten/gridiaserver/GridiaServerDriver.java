package hoten.gridiaserver;

import java.io.File;
import java.io.IOException;
import java.nio.file.Paths;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;

public class GridiaServerDriver {

    private static final int port = 1234;

    public static void main(String[] args) throws IOException {
        File clientDataDir = Paths.get("TestWorld", "clientdata").toFile();
        String localDataDirName = "TestWorld";
        ServingGridia server = new ServingGridia(port, clientDataDir, localDataDirName);
        server.startServer();
        System.out.println("Server started.");

        Executors.newScheduledThreadPool(1).scheduleAtFixedRate(() -> {
            server.moveCreatures();
            server.createCreature();
        }, 0, 1, TimeUnit.SECONDS);
    }
}
