package hoten.gridiaserver.serving;

import hoten.gridiaserver.content.ContentManager;
import hoten.gridiaserver.map.Coord;
import hoten.gridiaserver.Creature;
import hoten.gridiaserver.Container;
import hoten.gridiaserver.CreatureImage;
import hoten.gridiaserver.CustomPlayerImage;
import hoten.gridiaserver.DefaultCreatureImage;
import hoten.gridiaserver.content.ItemInstance;
import hoten.gridiaserver.Player;
import hoten.gridiaserver.content.Monster;
import hoten.gridiaserver.map.Sector;
import hoten.gridiaserver.map.SectorLoader;
import hoten.gridiaserver.map.SectorSaver;
import hoten.gridiaserver.map.Tile;
import hoten.gridiaserver.map.TileMap;
import hoten.gridiaserver.serializers.GridiaGson;
import hoten.serving.ServingSocket;
import java.io.File;
import java.io.IOException;
import java.net.Socket;
import static hoten.gridiaserver.serving.GridiaProtocols.Clientbound.*;
import hoten.serving.message.JsonMessageBuilder;
import hoten.serving.message.Message;
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

    public ServingGridia(int port, File clientDataFolder, String localDataFolderName) throws IOException {
        super(port, new GridiaProtocols(), clientDataFolder, localDataFolderName);
        contentManager = new ContentManager("TestWorld");
        GridiaGson.initialize(contentManager);
        SectorLoader sectorLoader = new SectorLoader();
        SectorSaver sectorSaver = new SectorSaver();
        tileMap = new TileMap(100, 1, 20, sectorLoader, sectorSaver);
        tileMap.loadAll();
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

    public void removeCreature(Creature cre) {
        Sector sector = tileMap.getSectorOf(cre.location);
        creatures.remove(cre.id);
        tileMap.getTile(cre.location).cre = null;
        Creature.uniqueIds.retire(cre.id);
        sendToClientsWithSectorLoaded(messageBuilder.removeCreature(cre), sector);
    }

    // :( todo: speed
    public void moveCreatureTo(Creature cre, Coord loc) {
        int timeInMillisecondsToMove = 200;
        sendToClientsWithSectorLoaded(messageBuilder.moveCreature(cre, 0), tileMap.getSectorOf(cre.location));
        tileMap.wrap(loc);
        Sector sector = tileMap.getSectorOf(loc);
        tileMap.getTile(cre.location).cre = null;
        tileMap.getTile(loc).cre = cre;
        cre.location = loc;
        sendToClientsWithSectorLoaded(messageBuilder.moveCreature(cre, timeInMillisecondsToMove), sector);
    }

    public void updateCreaureImage(Creature cre) {
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
            moveCreatureTo(cre, new Coord(x, y, cre.location.z));
        }
    }

    public void createCreatureRandomly(int image) {
        Coord c = new Coord(random.nextInt(tileMap.size / 10), random.nextInt(tileMap.size / 10), 0);
        if (tileMap.walkable(c.x, c.y, c.z)) {
            createCreature(image, c);
        }
    }

    public Creature createCreature(Monster mold, Coord loc) {
        return createCreature(mold.image, loc);
    }

    public Creature createCreature(int image, Coord loc) {
        return createCreature(new DefaultCreatureImage(image), loc);
    }

    public Creature createCreature(CreatureImage image, Coord loc) {
        Creature cre = new Creature();
        cre.image = image;
        cre.location = loc;
        Sector sector = tileMap.getSectorOf(cre.location);
        tileMap.getTile(cre.location).cre = cre;
        sendToClientsWithSectorLoaded(messageBuilder.addCreature(cre), sector);
        creatures.put(cre.id, cre);
        return cre;
    }

    public Creature createCreatureForPlayer() {
        CustomPlayerImage image = new CustomPlayerImage();
        image.bareArms = (int) (Math.random() * 10);
        image.bareHead = (int) (Math.random() * 100);
        image.bareChest = (int) (Math.random() * 10);
        image.bareLegs = (int) (Math.random() * 10);
        Creature cre = createCreature(image, new Coord(50 + random.nextInt(4), 50 + random.nextInt(4), 0));
        cre.belongsToPlayer = true;
        return cre;
    }

    public void announceNewPlayer(ConnectionToGridiaClientHandler client, Player player) {
        Message message = new JsonMessageBuilder()
                .protocol(outbound(Chat))
                .set("msg", String.format("%s has joined the game!", player.username))
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
        sendTo(message, client -> client.player.inventory.id == container.id || client.player.equipment.id == container.id);
    }
}
