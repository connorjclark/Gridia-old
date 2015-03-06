package hoten.gridia.serving;

import groovy.lang.Binding;
import groovy.lang.GroovyShell;
import groovy.lang.Script;
import groovy.util.DelegatingScript;
import hoten.gridia.content.ContentManager;
import hoten.gridia.map.Coord;
import hoten.gridia.Creature;
import hoten.gridia.Container;
import hoten.gridia.Container.ContainerFactory;
import hoten.gridia.Container.ContainerType;
import hoten.gridia.CreatureImage;
import hoten.gridia.CustomPlayerImage;
import hoten.gridia.DefaultCreatureImage;
import hoten.gridia.GridiaServerDriver;
import hoten.gridia.ItemWrapper;
import hoten.gridia.ItemWrapper.ContainerItemWrapper;
import hoten.gridia.ItemWrapper.WorldItemWrapper;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.Player;
import hoten.gridia.Player.PlayerFactory;
import hoten.gridia.content.ItemUse;
import hoten.gridia.content.Monster;
import hoten.gridia.content.UsageProcessor;
import hoten.gridia.content.WorldContentLoader;
import hoten.gridia.map.Sector;
import hoten.gridia.map.Tile;
import hoten.gridia.map.TileMap;
import hoten.gridia.serializers.GridiaGson;
import com.hoten.servingjava.filetransferring.ServingFileTransferring;
import java.io.File;
import java.io.IOException;
import java.net.Socket;
import com.hoten.servingjava.message.Message;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Random;
import java.util.concurrent.ConcurrentHashMap;
import java.util.function.Consumer;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.util.stream.Collectors;
import org.codehaus.groovy.control.CompilerConfiguration;

public class ServingGridia extends ServingFileTransferring<ConnectionToGridiaClientHandler> {

    public static ServingGridia instance; // :(

    private final hoten.gridia.scripting.ScriptExecutor scriptExecutor = new hoten.gridia.scripting.ScriptExecutor();
    private final hoten.gridia.scripting.EventDispatcher eventDispatcher = new hoten.gridia.scripting.EventDispatcher();
    private GroovyShell shell;
    public final GridiaMessageToClientBuilder messageBuilder = new GridiaMessageToClientBuilder();
    public final TileMap tileMap;
    public final ContentManager contentManager;
    public final Map<Integer, Creature> creatures = new ConcurrentHashMap();
    private final Random random = new Random();
    public final PlayerFactory playerFactory;
    public final ContainerFactory containerFactory;
    public final String worldName;
    public final String version = "alpha-1.4-dev";
    public final File worldTopDirectory;

    public ServingGridia(File world, String mapName, int port, File clientDataFolder, String localDataFolderName) throws IOException {
        super(port, clientDataFolder, localDataFolderName);
        worldTopDirectory = world;
        worldName = world.getName();
        contentManager = new WorldContentLoader(world).load();
        GridiaGson.initialize(contentManager, this);
        tileMap = TileMap.loadMap(world, mapName);
        playerFactory = new PlayerFactory(world);
        containerFactory = new ContainerFactory(world);
        setUpScripting();
        Arrays.asList("Login", "Stairs", "Growth", "FloorDamage").forEach(scriptName -> {
            try {
                addScript(new File(worldTopDirectory, "scripts/" + scriptName + ".groovy"));
            } catch (IOException ex) {
                Logger.getLogger(ServingGridia.class.getName()).log(Level.SEVERE, null, ex);
            }
        });
        instance = this;
    }

    private void setUpScripting() {
        CompilerConfiguration compilerConfiguration = new CompilerConfiguration();
        compilerConfiguration.setScriptBaseClass(DelegatingScript.class.getName());
        shell = new GroovyShell(GridiaServerDriver.class.getClassLoader(), new Binding(), compilerConfiguration);
    }

    public Script addScript(File scriptFile) throws IOException {
        DelegatingScript script = (DelegatingScript) shell.parse(scriptFile);
        return addScript(script);
    }

    public Script addScript(String scriptText) throws IOException {
        DelegatingScript script = (DelegatingScript) shell.parse(scriptText);
        return addScript(script);
    }

    private Script addScript(DelegatingScript script) throws IOException {
        script.setDelegate(new hoten.gridia.scripting.GridiaScript(this, eventDispatcher));
        scriptExecutor.addScript(script);
        return script;
    }

    public void dispatchEvent(String type, Object... eventParams) {
        Map event = new HashMap();
        for (int i = 0; i < eventParams.length; i += 2) {
            event.put(eventParams[i], eventParams[i + 1]);
        }
        eventDispatcher.dispatch(type, event);
    }

    @Override
    public void setupNewClient(ConnectionToGridiaClientHandler newClient) throws IOException {
        super.setupNewClient(newClient);
        System.out.println("Client has connected.");
        newClient.send(messageBuilder.initialize(version, worldName, tileMap.size, tileMap.depth, tileMap.sectorSize));
    }

    @Override
    protected void onClientClose(ConnectionToGridiaClientHandler client) throws IOException {
        if (client.player != null) {
            removeCreature(client.player.creature);
            savePlayer(client.player);
            Creature cre = client.player.creature;
            sendToAll(messageBuilder.chat(cre.name + " has left the building.", cre.location));
            playAnimation("WarpOut", cre.location);
        }
    }

    @Override
    protected ConnectionToGridiaClientHandler makeNewConnection(Socket newConnection) throws IOException {
        return new ConnectionToGridiaClientHandler(newConnection, this);
    }

    public String whoIsOnline() {
        return "Players online: " + _clients.stream()
                .filter(client -> client.player != null)
                .map(client -> client.player.accountDetails.username)
                .collect(Collectors.joining(", "));
    }

    // :(
    public void grow() {
        tileMap.forAllTilesLoaded(x -> y -> z -> tile -> {
            if (tile.item.getItem().growthDelta != 0) {
                tile.item.age += 1;
                if (tile.item.age >= tile.item.getItem().growthDelta) {
                    changeItem(new Coord(x, y, z), contentManager.createItemInstance(tile.item.getItem().growthItem));
                }
            }
        });
    }

    public boolean anyPlayersOnline() {
        return !_clients.isEmpty();
    }

    @Override
    public void sendToAll(Message message) {
        sendTo(message, c -> c.player != null);
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
        sendToClientsWithAreaLoaded(messageBuilder.animation("Attack", cre.location), cre.location);
        cre.life -= lifePoints;
        if (cre.life <= 0) {
            sendToClientsWithAreaLoaded(messageBuilder.animation("diescream", cre.location), cre.location);
            addItemNear(cre.location, contentManager.createItemInstance(1022), 10, true);
            if (cre.belongsToPlayer) {
                moveCreatureTo(cre, tileMap.getDefaultPlayerSpawn(), true);
            } else {
                dropCreatureInventory(cre);
                removeCreature(cre);
            }
        }
    }

    public void dropCreatureInventory(Creature cre) {
        if (cre.inventory != null) {
            List<ItemInstance> items = cre.inventory.getItems();
            items.stream().forEach((item) -> {
                addItemNear(cre.location, item, 10, true);
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
        cre.retire();
        sendToClientsWithSectorLoaded(messageBuilder.removeCreature(cre), sector);
    }

    public void moveCreatureTo(Creature cre, Coord loc, int timeInMillisecondsToMove, boolean isTeleport) {
        moveCreatureTo(cre, loc, timeInMillisecondsToMove, isTeleport, false);
    }

    public void moveCreatureTo(Creature cre, Coord loc, int timeInMillisecondsToMove, boolean isTeleport, boolean onRaft) {
        cre.justTeleported = false;
        Sector sectorBefore = tileMap.getSectorOf(cre.location);
        sendToClientsWithSectorLoaded(messageBuilder.moveCreature(cre, 0, false, onRaft), sectorBefore);
        tileMap.wrap(loc);
        Sector sector = tileMap.getSectorOf(loc);
        tileMap.getTile(cre.location).cre = null;
        tileMap.getTile(loc).cre = cre;
        cre.location = loc;
        sendTo(messageBuilder.moveCreature(cre, timeInMillisecondsToMove, isTeleport, onRaft), client -> {
            return client.hasSectorLoaded(sector) || client.hasSectorLoaded(sectorBefore) || (client.player != null && client.player.creature == cre);
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
        cre.inventory = containerFactory.createOnlyInMemory(ContainerType.Inventory, items);
        return cre;
    }

    public Creature createCreature(int image, Coord loc) {
        return createCreature(new DefaultCreatureImage(image), "Monster", loc);
    }

    public Creature createCreature(CreatureImage image, Coord loc) {
        return createCreature(image, "Monster", loc);
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

    // :(
    public Creature createCreatureQuietly(CreatureImage image, String name, Coord loc) {
        Creature cre = new Creature();
        cre.name = name;
        cre.image = image;
        cre.location = loc;
        creatures.put(cre.id, cre);
        return cre;
    }

    public Creature createCreatureForPlayer(String name, Coord location) {
        CustomPlayerImage image = new CustomPlayerImage();
        image.bareArms = (int) (Math.random() * 10);
        image.bareHead = (int) (Math.random() * 100);
        image.bareChest = (int) (Math.random() * 10);
        image.bareLegs = (int) (Math.random() * 10);
        Creature cre = createCreature(image, name, location);
        cre.belongsToPlayer = true;
        return cre;
    }

    public void announce(String from, String message) {
        sendToAll(messageBuilder.chat(from, message, new Coord(0, 0, 0)));
    }

    public void announceNewPlayer(ConnectionToGridiaClientHandler client, Player player) {
        String chatMessage = String.format("%s has joined the game!", player.creature.name);
        sendToAllBut(messageBuilder.chat(chatMessage, player.creature.location), client);
    }

    public void moveItem(Coord from, Coord to) {
        ItemInstance fromItem = tileMap.getItem(from);
        ItemInstance toItem = tileMap.getItem(to);
        if (toItem.getItem().id == 0) {
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
        changeItem(loc, tileMap.getItem(loc).remove(amount));
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
        if (currentItem.getItem().id != 0 && !willStack) {
            return false;
        }
        changeItem(loc, itemToAdd.add(currentItem.getQuantity()));
        return true;
    }

    //attempts to add an item at location, but if it is occupied, finds a nearby location
    //goes target, leftabove target, above target, rightabove target, left target, right target, leftbelow target...
    public boolean addItemNear(Coord loc, ItemInstance item, int bufferzone, boolean includeTargetLocation) {
        int x0 = loc.x;
        int y0 = loc.y;
        for (int offset = includeTargetLocation ? 0 : 1; offset <= bufferzone; offset++) {
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
        return addItemNear(tileMap.getCoordFromIndex(index), item, bufferzone, true);
    }

    public void updateContainerSlot(Container container, int slotIndex) {
        Message message = messageBuilder.updateContainerSlot(container, slotIndex);
        sendTo(message, client -> client.player != null && (client.player.creature.inventory.id == container.id || client.player.equipment.id == container.id));
        sendTo(message, client -> {
            if (client.player == null) {
                return false;
            }
            return client.player.creature.inventory.id == container.id
                    || client.player.equipment.id == container.id
                    || client.player.openedContainers.contains(container.id);
        });
    }

    public void save() throws IOException {
        sendToAll(messageBuilder.chat("Saving world...", new Coord(0, 0, 0)));
        tileMap.save();
        for (ConnectionToGridiaClientHandler client : _clients) {
            savePlayer(client.player);
        }
        containerFactory.saveAll();
        sendToAll(messageBuilder.chat("Saved!", new Coord(0, 0, 0)));
    }

    public void savePlayer(Player player) throws IOException {
        if (player != null) {
            playerFactory.save(player);
            containerFactory.save(player.creature.inventory);
            containerFactory.save(player.equipment);
        }
    }

    public ItemWrapper getItemFrom(Player player, int from, int index) {
        if (from == 0) {
            Coord location = tileMap.getCoordFromIndex(index);
            return new WorldItemWrapper(this, location);
        } else {
            try {
                Container container = containerFactory.get(from);
                return new ContainerItemWrapper(container, index);
            } catch (IOException ex) {
                throw new RuntimeException("Invalid source", ex);
            }
        }
    }

    public void executeItemUse(
            ConnectionToGridiaClientHandler connection,
            ItemUse use,
            ItemWrapper toolWrapper,
            ItemWrapper focusWrapper,
            int destIndex // :(
    ) throws IOException {
        boolean success = new UsageProcessor(contentManager).processUsage(use, toolWrapper, focusWrapper);

        if (success) {
            if (use.animation != null) {
                Coord loc = tileMap.getCoordFromIndex(destIndex);
                sendToClientsWithAreaLoaded(messageBuilder.animation(use.animation, loc), destIndex);
            }
            if (use.surfaceGround != -1) {
                Coord loc = tileMap.getCoordFromIndex(destIndex);
                changeFloor(loc, use.surfaceGround);
            }
            if (use.successMessage != null) {
                connection.send(messageBuilder.chat(use.successMessage, connection.getPlayer().creature.location));
            }
        } else {
            if (use.failureMessage != null) {
                connection.send(messageBuilder.chat(use.failureMessage, connection.getPlayer().creature.location));
            } else {
                connection.send(messageBuilder.chat("Something went wrong...", connection.getPlayer().creature.location));
            }
        }
    }

    public Player getPlayerWithName(String playerName) {
        return _clients.stream()
                .filter(client -> client.player != null && client.player.accountDetails.username.equals(playerName))
                .map(client -> client.player)
                .findFirst().orElse(null);
    }

    public void playAnimation(String name, Coord loc) {
        Message animMessage = messageBuilder.animation(name, loc);
        sendToAll(animMessage);
    }

    public void forEachClient(Consumer<ConnectionToGridiaClientHandler> consumer) {
        _clients.forEach(consumer);
    }

    public boolean moveItemOutOfTheWay(Coord loc) {
        return addItemNear(loc, tileMap.getItem(loc), 10, false);
    }
}
