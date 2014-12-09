package hoten.gridia.serving;

import hoten.gridia.content.ContentManager;
import hoten.gridia.map.Coord;
import hoten.gridia.Creature;
import hoten.gridia.Container;
import hoten.gridia.CreatureImage;
import hoten.gridia.CustomPlayerImage;
import hoten.gridia.DefaultCreatureImage;
import hoten.gridia.GridiaServerDriver;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.Player;
import hoten.gridia.content.Monster;
import hoten.gridia.map.Sector;
import hoten.gridia.map.Tile;
import hoten.gridia.map.TileMap;
import hoten.gridia.serializers.GridiaGson;
import hoten.serving.ServingSocket;
import java.io.File;
import java.io.IOException;
import java.net.Socket;
import static hoten.gridia.serving.GridiaProtocols.Clientbound.*;
import hoten.serving.message.JsonMessageBuilder;
import hoten.serving.message.Message;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Random;
import java.util.concurrent.ConcurrentHashMap;

public class ServingGridia extends ServingSocket<ConnectionToGridiaClientHandler> {

    public static ServingGridia instance; // :(

    public final GridiaMessageToClientBuilder messageBuilder = new GridiaMessageToClientBuilder(this::outbound);
    public final TileMap tileMap;
    public final ContentManager contentManager;
    public final Map<Integer, Creature> creatures = new ConcurrentHashMap();
    private final Random random = new Random();
    public boolean devMode = false; // :(

    public ServingGridia(String worldName, String mapName, int port, File clientDataFolder, String localDataFolderName) throws IOException {
        super(port, new GridiaProtocols(), clientDataFolder, localDataFolderName);
        contentManager = new ContentManager(worldName);
        GridiaGson.initialize(contentManager);
        tileMap = TileMap.loadMap(worldName + "/" + mapName);
        instance = this;
    }

    @Override
    protected ConnectionToGridiaClientHandler makeNewConnection(Socket newConnection) throws IOException {
        return new ConnectionToGridiaClientHandler(this, newConnection);
    }

    public boolean anyPlayersOnline() {
        return !_clients.isEmpty();
    }

    public void sendToClientsWithSectorLoadedBut(Message message, Sector sector, ConnectionToGridiaClientHandler client) {
        sendTo(message, c -> c.hasSectorLoaded(sector) && client != c);
    }

    public void sendToClientsWithSectorLoaded(Message message, Sector sector) {
        sendTo(message, c -> c.hasSectorLoaded(sector));
    }

    public void sendToClientsWithAreaLoaded(Message message, int destIndex) {
        sendToClientsWithAreaLoaded(message, tileMap.getCoordFromIndex(destIndex));
    }

    public void sendToClientsWithAreaLoaded(Message message, Coord loc) {
        sendTo(message, c -> c.hasSectorLoaded(tileMap.getSectorOf(loc)));
    }

    public void sendCreatures(ConnectionToGridiaClientHandler client) {
        creatures.values().forEach(cre -> {
            sendTo(messageBuilder.addCreature(cre), client);
        });
    }

    public void hurtCreature(Creature cre, int lifePoints) {
        sendToClientsWithAreaLoaded(messageBuilder.animation(1, cre.location), cre.location);
        cre.life -= lifePoints;
        if (cre.life <= 0) {
            sendToClientsWithAreaLoaded(messageBuilder.animation(45, cre.location), cre.location);
            dropCreatureInventory(cre);
            addItemNear(cre.location, contentManager.createItemInstance(1022), 10);
            if (cre.belongsToPlayer) {
                moveCreatureTo(cre, GridiaServerDriver.getPlayerSpawn(), true);
            } else {
                removeCreature(cre);
            }
        }
    }

    public void dropCreatureInventory(Creature cre) {
        if (cre.inventory != null) {
            List<ItemInstance> items = cre.inventory.getItems();
            items.stream().forEach((item) -> {
                addItemNear(cre.location, item, 10);
            });
            for (int i = 0; i < items.size(); i++) {
                cre.inventory.deleteSlot(i);
            }
        }
    }

    public void removeCreature(Creature cre) {
        Sector sector = tileMap.getSectorOf(cre.location);
        creatures.remove(cre.id);
        tileMap.getTile(cre.location).cre = null;
        Creature.uniqueIds.retire(cre.id);
        sendToClientsWithSectorLoaded(messageBuilder.removeCreature(cre), sector);
    }

    public void moveCreatureTo(Creature cre, Coord loc, int timeInMillisecondsToMove, boolean isTeleport) {
        cre.justTeleported = false;
        sendToClientsWithSectorLoaded(messageBuilder.moveCreature(cre, 0, false), tileMap.getSectorOf(cre.location));
        tileMap.wrap(loc);
        Sector sector = tileMap.getSectorOf(loc);
        tileMap.getTile(cre.location).cre = null;
        tileMap.getTile(loc).cre = cre;
        cre.location = loc;
        sendTo(messageBuilder.moveCreature(cre, timeInMillisecondsToMove, isTeleport), client -> {
            return client.hasSectorLoaded(sector) || client.player.creature == cre;
        });
    }

    public void moveCreatureTo(Creature cre, Coord loc, boolean isTeleport) {
        moveCreatureTo(cre, loc, 200, isTeleport);
    }

    public void updateCreatureImage(Creature cre) {
        Sector sector = tileMap.getSectorOf(cre.location);
        sendToClientsWithSectorLoaded(messageBuilder.updateCreatureImage(cre), sector);
    }

    public void moveCreatureRandomly(Creature cre) {
        int x = cre.location.x;
        int y = cre.location.y;
        int diff = random.nextBoolean() ? 1 : -1;
        if (random.nextBoolean()) {
            x += diff;
        } else {
            y += diff;
        }
        if (tileMap.walkable(x, y, cre.location.z)) {
            moveCreatureTo(cre, new Coord(x, y, cre.location.z), false);
        }
    }

    public void createCreatureRandomly(int image) {
        Coord c = new Coord(random.nextInt(tileMap.size / 10), random.nextInt(tileMap.size / 10), 0);
        if (tileMap.walkable(c.x, c.y, c.z)) {
            createCreature(image, c);
        }
    }

    public Creature createCreature(Monster mold, Coord loc) {
        Creature cre = createCreature(mold.image, mold.name, loc);
        List<ItemInstance> items = new ArrayList<>();
        mold.drops.forEach(itemDrop -> {
            items.add(new ItemInstance(itemDrop));
        });
        cre.inventory = new Container(items);
        return cre;
    }

    public Creature createCreature(int image, Coord loc) {
        return createCreature(new DefaultCreatureImage(image), "Monster", loc);
    }

    public Creature createCreature(CreatureImage image, String name, Coord loc) {
        Creature cre = new Creature();
        cre.name = name;
        cre.image = image;
        cre.location = loc;
        Sector sector = tileMap.getSectorOf(cre.location);
        tileMap.getTile(cre.location).cre = cre;
        sendToClientsWithSectorLoaded(messageBuilder.addCreature(cre), sector);
        creatures.put(cre.id, cre);
        return cre;
    }

    public Creature createCreatureForPlayer(String name) {
        CustomPlayerImage image = new CustomPlayerImage();
        image.bareArms = (int) (Math.random() * 10);
        image.bareHead = (int) (Math.random() * 100);
        image.bareChest = (int) (Math.random() * 10);
        image.bareLegs = (int) (Math.random() * 10);
        Creature cre = createCreature(image, name, GridiaServerDriver.getPlayerSpawn());
        cre.belongsToPlayer = true;
        return cre;
    }

    public void announceNewPlayer(ConnectionToGridiaClientHandler client, Player player) {
        Message message = new JsonMessageBuilder()
                .protocol(outbound(Chat))
                .set("msg", String.format("%s has joined the game!", player.creature.name))
                .build();
        sendToAllBut(message, client);
    }

    public void moveItem(Coord from, Coord to) {
        ItemInstance fromItem = tileMap.getItem(from);
        ItemInstance toItem = tileMap.getItem(to);
        if (toItem.data.id == 0) {
            changeItem(from, ItemInstance.NONE);
            changeItem(to, fromItem);
        }
    }

    public void changeItem(Coord loc, ItemInstance item) {
        tileMap.setItem(item, loc);
        updateTile(loc);
    }

    public void changeFloor(Coord loc, int floor) {
        tileMap.setFloor(loc, floor);
        updateTile(loc);
    }

    public void reduceItemQuantity(Coord loc, int amount) {
        ItemInstance item = tileMap.getItem(loc);
        item.quantity -= amount;
        if (item.quantity == 0) {
            changeItem(loc, ItemInstance.NONE);
        } else {
            updateTile(loc);
        }
    }

    public void changeItem(int index, ItemInstance item) {
        changeItem(tileMap.getCoordFromIndex(index), item);
    }

    public void updateTile(Coord loc) {
        Tile tile = tileMap.getTile(loc);
        sendToClientsWithAreaLoaded(messageBuilder.updateTile(loc, tile), loc);
    }

    public void updateTile(int index) {
        updateTile(tileMap.getCoordFromIndex(index));
    }

    //adds item only if it is to an empty tile or if it would stack
    public boolean addItem(Coord loc, ItemInstance itemToAdd) {
        ItemInstance currentItem = tileMap.getTile(loc).item;
        boolean willStack = ItemInstance.stackable(currentItem, itemToAdd);
        if (currentItem.data.id != 0 && !willStack) {
            return false;
        }
        int q = currentItem.quantity + itemToAdd.quantity;
        itemToAdd.quantity = q;
        changeItem(loc, itemToAdd);
        return true;
    }

    //attempts to add an item at location, but if it is occupied, finds a nearby location
    //goes target, leftabove target, above target, rightabove target, left target, right target, leftbelow target...
    public boolean addItemNear(Coord loc, ItemInstance item, int bufferzone) {
        int x0 = loc.x;
        int y0 = loc.y;
        for (int offset = 0; offset <= bufferzone; offset++) {
            for (int y1 = y0 - offset; y1 <= offset + y0; y1++) {
                if (y1 == y0 - offset || y1 == y0 + offset) {
                    for (int x1 = x0 - offset; x1 <= offset + x0; x1++) {
                        Coord newLoc = tileMap.wrap(new Coord(x1, y1, loc.z));
                        if (addItem(newLoc, item)) {
                            return true;
                        }
                    }
                } else {
                    Coord newLoc = tileMap.wrap(new Coord(x0 - offset, y1, loc.z));
                    if (addItem(newLoc, item)) {
                        return true;
                    }
                    newLoc = tileMap.wrap(new Coord(x0 + offset, y1, loc.z));
                    if (addItem(newLoc, item)) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public boolean addItemNear(int index, ItemInstance item, int bufferzone) {
        return addItemNear(tileMap.getCoordFromIndex(index), item, bufferzone);
    }

    public void updateContainerSlot(Container container, int slotIndex) {
        Message message = messageBuilder.updateContainerSlot(container, slotIndex);
        sendTo(message, client -> client.player.creature.inventory.id == container.id || client.player.equipment.id == container.id);
    }
}
