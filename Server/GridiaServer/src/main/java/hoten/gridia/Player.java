package hoten.gridia;

import hoten.gridia.Container.ContainerType;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.serializers.GridiaGson;
import hoten.gridia.serving.ServingGridia;
import hoten.serving.fileutils.FileUtils;
import java.io.File;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.stream.Collectors;

public class Player {

    public static class PlayerFactory {

        public static class BadLoginException extends Exception {

            public BadLoginException() {
                super();
            }
        }

        public static class BadRegistrationException extends Exception {

            public BadRegistrationException() {
                super();
            }
        }

        private String dir;

        public PlayerFactory(String worldName) {
            dir = worldName + "/Players/";
        }

        public Player load(ServingGridia server, String username, String passwordHash) throws BadLoginException {
            File dataFile = new File(dir + username + ".json");
            if (!dataFile.exists()) {
                throw new BadLoginException();
            }
            String json = FileUtils.readTextFile(dataFile);
            Player player = GridiaGson.get().fromJson(json, Player.class);

            if (!player.passwordHash.equals(passwordHash)) {
                throw new BadLoginException();
            }

            Container container = server.containerFactory.load(player);
            player.creature = server.createCreatureForPlayer(username);

            return player;
        }

        public Player create(ServingGridia server, String username, String passwordHash) throws BadRegistrationException {
            if (username.length() < 5 || new File(dir + username + ".json").exists()) {
                throw new BadRegistrationException();
            }

            Player newPlayer = new Player();
            newPlayer.creature = server.createCreatureForPlayer(username);
            newPlayer.passwordHash = passwordHash;

            // fake an inventory
            List<ItemInstance> inv = new ArrayList();
            inv.addAll(Arrays.asList(
                    57, 335, 277, 280, 1067, 900, 1068, 826, 1974,
                    1974, 1039, 171, 902, 901, 339, 341,
                    29, 19, 18, 12, 913, 34, 140
            ).stream()
                    .map(i -> {
                        int quantity = server.contentManager.getItem(i).stackable ? 1000 : 1;
                        return server.contentManager.createItemInstance(i, quantity);
                    })
                    .collect(Collectors.toList()));
            while (inv.size() < 30) {
                inv.add(server.contentManager.createItemInstance(0));
            }
            newPlayer.creature.inventory = server.containerFactory.create(ContainerType.Inventory, inv);

            return newPlayer;
        }
    }

    public Creature creature;
    public Container equipment;
    public String passwordHash;

    private Player() {
    }
}
