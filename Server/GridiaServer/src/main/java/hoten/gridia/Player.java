package hoten.gridia;

import com.google.gson.Gson;
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

            public BadLoginException(String message) {
                super(message);
            }
        }

        public static class BadRegistrationException extends Exception {

            public BadRegistrationException(String message) {
                super(message);
            }
        }

        private final String dir;

        public PlayerFactory(String worldName) {
            dir = worldName + "/Players/";
        }

        public Player load(ServingGridia server, String username, String passwordHash) throws BadLoginException {
            File dataFile = new File(dir + username + ".json");
            if (!dataFile.exists()) {
                throw new BadLoginException("Bad user/password");
            }
            String json = FileUtils.readTextFile(dataFile);
            AccountDetails accountDetails = GridiaGson.get().fromJson(json, AccountDetails.class);

            if (!accountDetails.passwordHash.equals(passwordHash)) {
                throw new BadLoginException("Bad user/password");
            }

            Creature creature = server.createCreatureForPlayer(username);
            creature.inventory = server.containerFactory.load(accountDetails.inventoryId);
            Container equipment = server.containerFactory.load(accountDetails.equipmentId);

            return new Player(accountDetails, creature, equipment);
        }

        public Player create(ServingGridia server, String username, String passwordHash) throws BadRegistrationException {
            if (username.length() < 3) {
                throw new BadRegistrationException("Username must be >2 characters");
            }
            if (new File(dir + username + ".json").exists()) {
                throw new BadRegistrationException("Username already exists");
            }

            AccountDetails accountDetails = new AccountDetails();
            accountDetails.username = username;
            accountDetails.passwordHash = passwordHash;

            Creature creature = server.createCreatureForPlayer(username);

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
            creature.inventory = server.containerFactory.create(ContainerType.Inventory, inv);
            accountDetails.inventoryId = creature.inventory.id;

            // fake equipment
            List<ItemInstance> equipmentItems = new ArrayList();
            equipmentItems.add(server.contentManager.createItemInstance(0));
            equipmentItems.add(server.contentManager.createItemInstance(0));
            equipmentItems.add(server.contentManager.createItemInstance(0));
            equipmentItems.add(server.contentManager.createItemInstance(0));
            equipmentItems.add(server.contentManager.createItemInstance(0));

            Container equipment = server.containerFactory.create(ContainerType.Equipment, equipmentItems);
            accountDetails.equipmentId = equipment.id;
            if (creature.image instanceof CustomPlayerImage) {
                ((CustomPlayerImage) (creature.image)).moldToEquipment(equipment);
            }

            Player player = new Player(accountDetails, creature, equipment);
            save(player);

            return player;
        }

        public void save(Player player) {
            FileUtils.saveAs(new File(dir + player.accountDetails.username + ".json"), new Gson().toJson(player.accountDetails).getBytes());
        }
    }

    public final Creature creature;
    public final Container equipment;
    public final AccountDetails accountDetails;

    private Player(AccountDetails accountDetails, Creature creature, Container equipment) {
        this.accountDetails = accountDetails;
        this.creature = creature;
        this.equipment = equipment;
    }

    public static class AccountDetails {

        public String username, passwordHash;
        public int inventoryId, equipmentId;
        public boolean isAdmin;

        private AccountDetails() {
        }
    }
}
