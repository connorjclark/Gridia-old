package com.hoten.gridia;

import com.hoten.gridia.Container.ContainerType;
import com.hoten.gridia.content.ItemInstance;
import com.hoten.gridia.scripting.Entity;
import com.hoten.gridia.serializers.GridiaGson;
import com.hoten.gridia.serving.ServingGridia;
import com.hoten.gridia.uniqueidentifiers.FileResourceUniqueIdentifiers;
import com.hoten.gridia.uniqueidentifiers.UniqueIdentifiers;
import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.util.stream.Collectors;
import org.apache.commons.collections4.BidiMap;
import org.apache.commons.collections4.bidimap.TreeBidiMap;
import org.apache.commons.io.FileUtils;

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

        public final BidiMap<String, Integer> _usernameToId = new TreeBidiMap<>();
        private final File _dir;
        private final UniqueIdentifiers _uniqueIds;

        public PlayerFactory(File world) {
            _dir = new File(world, "players/");
            _uniqueIds = new FileResourceUniqueIdentifiers(_dir, 100);
            FileUtils.listFiles(_dir, new String[]{"json"}, true).forEach(file -> {
                try {
                    String json = FileUtils.readFileToString(file);
                    Entity creature = GridiaGson.get().fromJson(json, Entity.class);
                    _usernameToId.put((String) creature.getAttribute("username"), (int) creature.getAttribute("playerId"));
                } catch (IOException ex) {
                    Logger.getLogger(Player.class.getName()).log(Level.SEVERE, null, ex);
                }
            });
        }

        public Player load(ServingGridia server, String username, String passwordHash) throws BadLoginException, IOException {
            if (!_usernameToId.containsKey(username)) {
                throw new BadLoginException("Bad user/password");
            }
            String json = FileUtils.readFileToString(new File(_dir, _usernameToId.get(username) + ".json"));
            Entity creature = GridiaGson.get().fromJson(json, Entity.class);

            Container equipment = server.containerFactory.get((int) creature.getAttribute("equipmentId")); // :(
            Player player = new Player(creature, equipment);
            if (!player.getPasswordHash().equals(passwordHash)) {
                throw new BadLoginException("Bad user/password");
            }
            creature.setAttribute("inventory", server.containerFactory.get(player.getInventoryId()));

            server.registerCreature(creature);
            return player;
        }

        public Player create(ServingGridia server, String username, String passwordHash) throws BadRegistrationException, IOException {
            if (username.length() < 3) {
                throw new BadRegistrationException("Username must be at least 3 characters");
            }

            if (_usernameToId.containsKey(username)) {
                throw new BadRegistrationException("Username already exists");
            }

            // fake equipment
            List<ItemInstance> equipmentItems = new ArrayList();
            equipmentItems.add(server.contentManager.createItemInstance(0));
            equipmentItems.add(server.contentManager.createItemInstance(0));
            equipmentItems.add(server.contentManager.createItemInstance(0));
            equipmentItems.add(server.contentManager.createItemInstance(0));
            equipmentItems.add(server.contentManager.createItemInstance(0));
            Container equipment = server.containerFactory.create(ContainerType.Equipment, equipmentItems);

            Entity creature = new Entity();
            Player player = new Player(creature, equipment);
            player.setPlayerId(_uniqueIds.next());
            player.setUsername(username);
            player.setPasswordHash(passwordHash);
            creature.location = server.tileMap.getDefaultPlayerSpawn();
            creature.setAttribute("image", server.createDefaultCreatureImage());
            creature.setAttribute("belongsToPlayer", true);
            player.setIsAdmin(_dir.listFiles() == null || _dir.listFiles().length == 0);

            if (!creature.hasAttribute("name")) {
                creature.setAttribute("name", username);
            }

            // fake an inventory
            int invSize = 40;
            List<ItemInstance> inv = new ArrayList<>();
            inv.addAll(Arrays.asList(
                    0, 0, 335, 277, 280, 1067, 900, 826,
                    1974, 1039, 171, 902, 339, 341,
                    29, 19, 18, 12, 913, 34, 140
            ).stream()
                    .map(i -> {
                        int quantity = server.contentManager.getItem(i).stackable ? 1000 : 1;
                        return server.contentManager.createItemInstance(i, quantity);
                    })
                    .collect(Collectors.toList()));
            while (inv.size() < invSize) {
                inv.add(server.contentManager.createItemInstance(0));
            }

            Container invContainer = server.containerFactory.create(ContainerType.Inventory, inv);
            creature.setAttribute("inventory", invContainer);
            player.setInventoryId(invContainer.id);

            player.setEquipmentId(equipment.id);
            CreatureImage image = (CreatureImage) creature.getAttribute("image");
            if (image instanceof CustomPlayerImage) {
                ((CustomPlayerImage) (image)).moldToEquipment(equipment);
            }

            _usernameToId.put(username, player.getPlayerId());
            save(player);
            server.registerCreature(creature);

            return player;
        }

        public void save(Player player) throws IOException {
            String json = GridiaGson.get().toJson(player.creature);
            FileUtils.writeStringToFile(new File(_dir, player.getPlayerId() + ".json"), json);
        }
    }

    public final Entity creature;
    public final Container equipment;
    public final Set<Integer> openedContainers = new HashSet();
    // :(
    public int useSourceIndex, useDestIndex;
    public int useSource, useDest;

    private Player(Entity creature, Container equipment) {
        this.creature = creature;
        this.equipment = equipment;
    }
    
    public void updatePlayerImage(ServingGridia server) {
        CreatureImage image = (CreatureImage) creature.getAttribute("image");
        if (image instanceof CustomPlayerImage) {
            ((CustomPlayerImage) image).moldToEquipment(equipment);
        }
        server.updateCreatureImage(creature);
    }

    public int getPlayerId() {
        return (int) creature.getAttribute("playerId");
    }

    public void setPlayerId(int id) {
        creature.setAttribute("playerId", id);
    }

    /**
     * @return the username
     */
    public String getUsername() {
        return (String) creature.getAttribute("username");
    }

    /**
     * @param username the username to set
     */
    public void setUsername(String username) {
        creature.setAttribute("username", username);
    }

    /**
     * @return the passwordHash
     */
    public String getPasswordHash() {
        return (String) creature.getAttribute("passwordHash");
    }

    /**
     * @param passwordHash the passwordHash to set
     */
    public void setPasswordHash(String passwordHash) {
        creature.setAttribute("passwordHash", passwordHash);
    }

    /**
     * @return the inventoryId
     */
    public int getInventoryId() {
        return (int) creature.getAttribute("inventoryId");
    }

    /**
     * @param inventoryId the inventoryId to set
     */
    public void setInventoryId(int inventoryId) {
        creature.setAttribute("inventoryId", inventoryId);
    }

    /**
     * @return the equipmentId
     */
    public int getEquipmentId() {
        return (int) creature.getAttribute("equipmentId");
    }

    /**
     * @param equipmentId the equipmentId to set
     */
    public void setEquipmentId(int equipmentId) {
        creature.setAttribute("equipmentId", equipmentId);
    }

    /**
     * @return the isAdmin
     */
    public boolean isAdmin() {
        return creature.getBoolean("isAdmin");
    }

    /**
     * @param isAdmin the isAdmin to set
     */
    public void setIsAdmin(boolean isAdmin) {
        creature.setAttribute("isAdmin", isAdmin);
    }
}
