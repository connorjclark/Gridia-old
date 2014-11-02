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
        
        for (int i = 0; i < 100; i++) {
            server.createCreature();
        }

        Executors.newScheduledThreadPool(1).scheduleAtFixedRate(() -> {
            System.out.println("tick " + System.currentTimeMillis());
            server.creatures.values().stream().filter(cre -> !cre.belongsToPlayer).forEach(cre -> {
                server.moveCreatureRandomly(cre);
            });
            if (server.creatures.size() < 5) {
                //server.createCreature();
            }
        }, 0, 1, TimeUnit.SECONDS);
    }
}
