package com.hoten.gridia.serving;

import com.hoten.gridia.scripting.*;
import groovy.lang.Binding;
import groovy.lang.GroovyShell;
import groovy.lang.Script;
import groovy.util.DelegatingScript;
import com.hoten.gridia.content.ContentManager;
import com.hoten.gridia.map.Coord;
import com.hoten.gridia.Container;
import com.hoten.gridia.Container.ContainerFactory;
import com.hoten.gridia.Container.ContainerType;
import com.hoten.gridia.CreatureImage;
import com.hoten.gridia.CustomPlayerImage;
import com.hoten.gridia.DefaultCreatureImage;
import com.hoten.gridia.GridiaServerDriver;
import com.hoten.gridia.ItemWrapper;
import com.hoten.gridia.ItemWrapper.ContainerItemWrapper;
import com.hoten.gridia.ItemWrapper.WorldItemWrapper;
import com.hoten.gridia.content.ItemInstance;
import com.hoten.gridia.Player;
import com.hoten.gridia.Player.PlayerFactory;
import com.hoten.gridia.content.ItemUse;
import com.hoten.gridia.content.ItemUseException;
import com.hoten.gridia.content.Monster;
import com.hoten.gridia.content.WorldContentLoader;
import com.hoten.gridia.map.Sector;
import com.hoten.gridia.map.Tile;
import com.hoten.gridia.map.TileMap;
import com.hoten.gridia.serializers.GridiaGson;
import com.hoten.servingjava.filetransferring.ServingFileTransferring;
import java.io.File;
import java.io.IOException;
import java.net.Socket;
import com.hoten.servingjava.message.Message;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Random;
import java.util.concurrent.ConcurrentHashMap;
import java.util.function.Consumer;
import java.util.function.Function;
import java.util.function.Predicate;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.util.stream.Collectors;
import org.codehaus.groovy.control.CompilerConfiguration;

public class ServingGridia extends ServingFileTransferring<ConnectionToGridiaClientHandler> {

    public static ServingGridia instance; // :(

    private final ScriptExecutor scriptExecutor = new ScriptExecutor();
    private final EventDispatcher eventDispatcher = new EventDispatcher();
    private GroovyShell shell;
    public final GridiaMessageToClientBuilder messageBuilder = new GridiaMessageToClientBuilder();
    public final TileMap tileMap;
    public final ContentManager contentManager;
    public final ScriptableUsageProcessing usageProcessor;
    public final Map<Integer, Entity> creatures = new ConcurrentHashMap();
    private final Random random = new Random();
    public final PlayerFactory playerFactory;
    public final ContainerFactory containerFactory;
    public final String worldName;
    public final String mapName;
    public final String version = "alpha-1.6-dev";
    public final File worldTopDirectory;
    public final Entity worldEntity = new Entity();

    public ServingGridia(File world, String mapName, int port, File clientDataFolder, String localDataFolderName) throws IOException {
        super(port, clientDataFolder, localDataFolderName);
        worldTopDirectory = world;
        worldName = world.getName();
        this.mapName = mapName;
        contentManager = new WorldContentLoader(world).load();
        usageProcessor = new ScriptableUsageProcessing(contentManager, eventDispatcher);
        GridiaGson.initialize(contentManager, this);
        tileMap = TileMap.loadMap(world, mapName);
        playerFactory = new PlayerFactory(world);
        containerFactory = new ContainerFactory(world);
        setUpScripting();
        instance = this;
    }

    private void setUpScripting() throws IOException {
        CompilerConfiguration compilerConfiguration = new CompilerConfiguration();
        compilerConfiguration.setScriptBaseClass(DelegatingScript.class.getName());
        shell = new GroovyShell(GridiaServerDriver.class.getClassLoader(), new Binding(), compilerConfiguration);
        addScript(new File(worldTopDirectory, "scripts/ScriptLoader.groovy"));
    }

    public Script addScript(File scriptFile) throws IOException {
        return addScript(scriptFile, worldEntity);
    }

    public Script addScript(String scriptText, String scriptName) throws IOException {
        return addScript(scriptText, scriptName, worldEntity);
    }

    public Script addScript(File scriptFile, Entity entity) throws IOException {
        DelegatingScript script = (DelegatingScript) shell.parse(scriptFile);
        return addScript(script, scriptFile.getName(), entity);
    }

    public Script addScript(String scriptText, String scriptName, Entity entity) throws IOException {
        DelegatingScript script = (DelegatingScript) shell.parse(scriptText);
        return addScript(script, scriptName, entity);
    }

    private Script addScript(DelegatingScript script, String scriptName, Entity entity) throws IOException {
        GridiaScript gridiaScript = new GridiaScript(this, eventDispatcher, entity, scriptName);
        script.setDelegate(gridiaScript);
        scriptExecutor.addScript(script);
        return script;
    }

    public void dispatchEvent(String type, Entity target, Object... eventParams) {
        if (target == null) {
            target = worldEntity;
        }
        if (eventParams.length % 2 == 1) {
            throw new RuntimeException("Expected an even amount of event parameters.");
        }
        Map event = new HashMap();
        for (int i = 0; i < eventParams.length; i += 2) {
            event.put(eventParams[i], eventParams[i + 1]);
        }
        eventDispatcher.dispatch(type, target, event);
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
            Entity cre = client.player.creature;
            sendToAll(messageBuilder.chat(cre.getAttribute("name") + " has left the building.", cre.location));
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
                .map(client -> client.player.getUsername())
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
        super.sendTo(message, c -> c.player != null);
    }

    @Override
    public void sendTo(Message message, Predicate<ConnectionToGridiaClientHandler> pred) {
        super.sendTo(message, c -> c.player != null && pred.test(c));
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
        creatures.values().forEach(cre -> sendTo(messageBuilder.addCreature(cre), client));
    }

    public void dropContainerNear(Container container, Coord location) {
        List<ItemInstance> items = container.getItems();
        items.stream().forEach((item) -> addItemNear(item, location, 10, true));
        for (int i = 0; i < items.size(); i++) {
            container.deleteSlot(i);
        }
    }

    public void removeCreature(Entity cre) {
        Sector sector = tileMap.getSectorOf(cre.location);
        creatures.remove(cre.id);
        tileMap.getTile(cre.location).cre = null;
        cre.retire();
        cre.removeScripts();
        sendToClientsWithSectorLoaded(messageBuilder.removeCreature(cre), sector);
    }

    public void moveCreatureTo(Entity cre, Coord loc, int timeInMillisecondsToMove, boolean isTeleport) {
        moveCreatureTo(cre, loc, timeInMillisecondsToMove, isTeleport, false);
    }

    public void moveCreatureTo(Entity cre, Coord loc, int timeInMillisecondsToMove, boolean isTeleport, boolean onRaft) {
        moveCreatureTo(cre, loc, timeInMillisecondsToMove, isTeleport, onRaft, false);
    }

    public void moveCreatureTo(Entity cre, Coord loc, int timeInMillisecondsToMove, boolean isTeleport, boolean onRaft, boolean tellMover) {
        if (loc.equals(cre.location)) {
            return;
        }
        // move into
        if (!isTeleport) {
            Entity creatureMovedInto = tileMap.getCreature(loc);
            if (creatureMovedInto != null) {
                dispatchEvent("MovedInto", creatureMovedInto, "entity", cre);
                return;
            }
        }
        cre.setAttribute("justTeleported", false);
        Sector sectorBefore = tileMap.getSectorOf(cre.location);
        sendToClientsWithSectorLoaded(messageBuilder.moveCreature(cre, 0, false, onRaft, tellMover), sectorBefore);
        tileMap.wrap(loc);
        Sector sector = tileMap.getSectorOf(loc);
        tileMap.getTile(cre.location).cre = null;
        tileMap.getTile(loc).cre = cre;
        cre.location = loc;
        sendTo(messageBuilder.moveCreature(cre, timeInMillisecondsToMove, isTeleport, onRaft, tellMover), client -> client.hasSectorLoaded(sector) || client.player.creature == cre);
        sendTo(messageBuilder.removeCreature(cre), client -> client.player.creature != cre && !client.hasSectorLoaded(sector) && client.hasSectorLoaded(sectorBefore));
    }

    public void moveCreatureTo(Entity cre, Coord loc, boolean isTeleport) {
        moveCreatureTo(cre, loc, 200, isTeleport);
    }

    public void updateCreatureImage(Entity cre) {
        Sector sector = tileMap.getSectorOf(cre.location);
        sendToClientsWithSectorLoaded(messageBuilder.updateCreatureImage(cre), sector);
    }

    public void createCreatureRandomly(int image) {
        Coord c = new Coord(random.nextInt(tileMap.size / 10), random.nextInt(tileMap.size / 10), 0);
        if (tileMap.walkable(c.x, c.y, c.z)) {
            createCreature(image, c);
        }
    }

    public Entity createCreature(Monster mold, Coord loc, boolean friendly) {
        Entity cre = createCreature(mold.image, mold.name, loc, false, friendly);
        List<ItemInstance> items = new ArrayList<>();
        mold.drops.forEach(itemDrop -> items.add(new ItemInstance(itemDrop)));
        cre.setAttribute("inventory", containerFactory.createOnlyInMemory(ContainerType.Inventory, items));
        return cre;
    }

    public Entity createCreature(Monster mold, Coord loc) {
        return createCreature(mold, loc, false);
    }

    public Entity createCreature(int image, Coord loc) {
        return createCreature(new DefaultCreatureImage(image), "Monster", loc, false, false);
    }

    public Entity createCreature(CreatureImage image, Coord loc) {
        return createCreature(image, "Monster", loc, false, false);
    }

    public Entity createCreature(CreatureImage image, String name, Coord loc, boolean belongsToPlayer, boolean friendly) {
        Entity cre = createCreatureQuietly(image, name, loc, belongsToPlayer, friendly);
        registerCreature(cre);
        return cre;
    }

    public Entity getCreature(int id) {
        return creatures.get(id);
    }

    // :(
    public void attachScriptsToCreature(Entity creature) {
        try {
            if (!creature.getBoolean("belongsToPlayer") && creature.getAttribute("name") != "Bill" && creature.getAttribute("name") != "Kitty") {
                addScript(new File(worldTopDirectory, "scripts/RandomWalk.groovy"), creature);
            }
            if (creature.getBoolean("isFriendly")) {
                addScript(new File(worldTopDirectory, "scripts/Friendly.groovy"), creature);
            }
            addScript(new File(worldTopDirectory, "scripts/Life.groovy"), creature);
        } catch (IOException ex) {
            Logger.getLogger(ServingGridia.class.getName()).log(Level.SEVERE, null, ex);
        }
    }

    public void registerCreature(Entity creature) {
        Sector sector = tileMap.getSectorOf(creature.location);
        tileMap.getTile(creature.location).cre = creature;
        sendToClientsWithSectorLoaded(messageBuilder.addCreature(creature), sector);
        creatures.put(creature.id, creature);
        attachScriptsToCreature(creature);
    }

    // :( creating creatures need refactoring, standardization
    public Entity createCreatureQuietly(CreatureImage image, String name, Coord loc, boolean belongsToPlayer, boolean friendly) {
        Entity cre = new Entity();
        cre.setAttribute("belongsToPlayer", belongsToPlayer);
        cre.setAttribute("isFriendly", friendly);
        cre.setAttribute("name", name);
        cre.setAttribute("image", image);
        cre.location = loc;
        return cre;
    }

    public CustomPlayerImage createDefaultCreatureImage() {
        CustomPlayerImage image = new CustomPlayerImage();
        image.bareArms = (int) (Math.random() * 10);
        image.bareHead = (int) (Math.random() * 100);
        image.bareChest = (int) (Math.random() * 10);
        image.bareLegs = (int) (Math.random() * 10);
        return image;
    }

    public void announce(String from, String message, Coord loc) {
        sendToAll(messageBuilder.chat(from, message, loc));
    }

    public void announce(String from, String message, Coord loc, Entity to) {
        sendToFirst(messageBuilder.chat(from, message, loc), client -> client.player != null && client.player.creature == to);
    }

    public void alert(Entity to, String message) {
        sendToFirst(messageBuilder.alert(message), client -> client.player != null && client.player.creature == to);
    }

    public void alertToAll(String message) {
        sendToAll(messageBuilder.alert(message));
    }

    public void announceNewPlayer(ConnectionToGridiaClientHandler client, Player player) {
        String chatMessage = String.format("%s has joined the game!", player.creature.getAttribute("name"));
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
    public ItemInstance addItem(ItemInstance itemToAdd, Coord loc) {
        ItemInstance currentItem = tileMap.getTile(loc).item;
        boolean willStack = ItemInstance.stackable(currentItem, itemToAdd);
        if (currentItem.getItem().id != 0 && !willStack) {
            return null;
        }
        ItemInstance itemToSet = itemToAdd.add(currentItem.getQuantity());
        changeItem(loc, itemToSet);
        return itemToSet;
    }

    //attempts to add an item at location, but if it is occupied, finds a nearby location
    //goes target, leftabove target, above target, rightabove target, left target, right target, leftbelow target...
    public ItemInstance addItemNear(ItemInstance item, Coord loc, int range, boolean includeTargetLocation) {
        int x0 = loc.x;
        int y0 = loc.y;
        ItemInstance itemAdded;
        for (int offset = includeTargetLocation ? 0 : 1; offset <= range; offset++) {
            for (int y1 = y0 - offset; y1 <= offset + y0; y1++) {
                if (y1 == y0 - offset || y1 == y0 + offset) {
                    for (int x1 = x0 - offset; x1 <= offset + x0; x1++) {
                        Coord newLoc = tileMap.wrap(new Coord(x1, y1, loc.z));
                        if ((itemAdded = addItem(item, newLoc)) != null) {
                            return itemAdded;
                        }
                    }
                } else {
                    Coord newLoc = tileMap.wrap(new Coord(x0 - offset, y1, loc.z));
                    if ((itemAdded = addItem(item, newLoc)) != null) {
                        return itemAdded;
                    }
                    newLoc = tileMap.wrap(new Coord(x0 + offset, y1, loc.z));
                    if ((itemAdded = addItem(item, newLoc)) != null) {
                        return itemAdded;
                    }
                }
            }
        }
        return null;
    }

    public ItemInstance addItemNear(int index, ItemInstance item, int bufferzone) {
        return addItemNear(item, tileMap.getCoordFromIndex(index), bufferzone, true);
    }

    public void updateContainerSlot(Container container, int slotIndex) {
        Message message = messageBuilder.updateContainerSlot(container, slotIndex);
        Function<ConnectionToGridiaClientHandler, Integer> invId = client
                -> ((Container) (client.player.creature.getAttribute("inventory"))).id;
        sendTo(message, client -> invId.apply(client) == container.id || client.player.equipment.id == container.id);
        sendTo(message, client -> invId.apply(client) == container.id
                || client.player.equipment.id == container.id
                || client.player.openedContainers.contains(container.id)
        );
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
            containerFactory.save((Container) player.creature.getAttribute("inventory"));
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
        try {
            usageProcessor.processUsage(use, toolWrapper, focusWrapper, connection.player.creature);
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
        } catch (ItemUseException ex) {
            String failureMessage = ex.getMessage();
            if (use.failureMessage != null) {
                failureMessage = use.failureMessage + "\n" + failureMessage;
            }
            connection.send(messageBuilder.chat(failureMessage, connection.getPlayer().creature.location));
        }
    }

    public Player getPlayerWithName(String playerName) {
        return _clients.stream()
                .filter(client -> client.player != null && client.player.getUsername().equals(playerName))
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
        return addItemNear(tileMap.getItem(loc), loc, 10, false) != null;
    }

    public void teleport(Entity creature, Coord coord) {
        playAnimation("WarpOut", creature.location);
        moveCreatureTo(creature, coord, true);
        playAnimation("WarpIn", coord);
    }

    public String getOwnerName(int owner) {
        if (owner == Tile.OWNER_SERVER) {
            return "SERVER";
        }
        if (owner == Tile.OWNER_UNCLAIMED) {
            return "UNCLAIMED";
        }
        return playerFactory._usernameToId.getKey(owner);
    }
}
