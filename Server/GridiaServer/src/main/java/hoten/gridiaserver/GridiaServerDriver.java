package hoten.gridiaserver;

import hoten.gridiaserver.map.Coord;
import hoten.gridiaserver.serving.ServingGridia;
import java.io.File;
import java.io.IOException;
import java.nio.file.Paths;
import java.util.Random;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
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
        
//        for (int i = 0; i < 10; i++) {
//            server.createCreatureRandomly(Math.random() > .5 ? 100 : 109);
//        }
        
        server.createCreature(1, new Coord(0, -1, 0));

        /*Random r = new Random();
         int size = server.tileMap.size;
         for (int i = 0; i < size; i++) {
         for (int j = 0; j < size; j++) {
         for (int k = 0; k < server.tileMap.depth; k++) {
         if (r.nextFloat() > 0.9 && server.tileMap.getFloor(i, j, k) != 1) {
         server.changeItem(new Coord(i, j, k), server.contentManager.createItemInstance(10));
         }
         }
         }
         }*/
        Executors.newScheduledThreadPool(1).scheduleAtFixedRate(() -> {
            /*server.creatures.values().stream().filter(cre -> !cre.belongsToPlayer).forEach(cre -> {
             server.moveCreatureRandomly(cre);
             });*/
            try {
                moveStraight();
            } catch (Exception e) {
                e.printStackTrace();
            }
            if (server.anyPlayersOnline()) {
                /*for (int i = 0; i < 20; i++) {
                 Coord randCoord = randomCoord();
                 if (server.tileMap.getTile(randCoord).floor != 1) {
                 server.changeItem(randCoord, server.contentManager.createItemInstance((int) (Math.random() * 10)));
                 }
                 }*/
                //server.createCreatureRandomly(Math.random() > .5 ? 100 : 109);
            }
        }, 0, 2000, TimeUnit.MILLISECONDS);
    }
    private static boolean movingLeft = true;
    
    private static void moveStraight() {
        Creature cre = server.creatures.get(1);
        Coord loc = cre.location;
        System.out.println("loc = " + loc);
        if (movingLeft) {
            server.moveCreatureTo(cre, loc.add(-1, 0, 0));
            if (cre.location.x <= 0) {
                movingLeft = false;
            }
        } else {
            server.moveCreatureTo(cre, loc.add(1, 0, 0));
            if (cre.location.x >= 10) {
                movingLeft = true;
            }
        }
    }
    
    public static Coord randomCoord() {
        int size = server.tileMap.size;
        int x = (int) (Math.random() * size);
        int y = (int) (Math.random() * size);
        int z = (int) (Math.random() * server.tileMap.depth);
        return new Coord(x, y, z);
    }
}
