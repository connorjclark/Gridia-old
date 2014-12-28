package hoten.gridia;

import hoten.gridia.content.ItemInstance;
import hoten.gridia.content.Monster;
import hoten.gridia.map.Coord;
import hoten.gridia.serving.ServingGridia;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.function.Predicate;
import java.util.logging.Level;
import java.util.logging.Logger;

// hard code the roach quest
public class RoachQuest implements Runnable {

    private final ServingGridia _server;
    public final int arenaTickRate = 3000;
    private final int arenaDuration = 30 * 1000;
    private final Coord arenaLocation = new Coord(70, 166, 1);
    private final Coord winnerTeleportLocation = new Coord(88, 190, 0);
    private final Coord loserTeleportLocation = new Coord(86, 192, 0);
    private final int arenaSize = 24;
    private final int numRoaches = 50;
    private boolean arenaIsGoing;
    private int timeLeft;

    public RoachQuest(ServingGridia server) {
        _server = server;
    }

    @Override
    public void run() {
        if (_server.anyPlayersOnline()) {
            List<Creature> playersInArena = getPlayersInArena();

            if (arenaIsGoing) {
                if (playersInArena.isEmpty()) {
                    clearArena();
                } else {
                    stepArena();
                }
            } else {
                if (playersInArena.size() >= 1) {
                    startArena();
                }
            }
        }
    }

    private void stepArena() {
        if (timeLeft == 0) {
            List<Creature> playersInArena = getPlayersInArena();
            if (!playersInArena.isEmpty()) {
                Creature winner = null;
                int highestAntenae = 0;

                for (Creature player : playersInArena) {
                    int amount = removeItemFromInventory(player, 447).quantity;
                    if (amount >= highestAntenae) {
                        winner = player;
                        highestAntenae = amount;
                    }
                }

                playersInArena.remove(winner);

                playersInArena.forEach(creature -> {
                    _server.moveCreatureTo(creature, loserTeleportLocation.add((int) (Math.random() * 5), (int) (Math.random() * 6), 0), true);
                });

                _server.moveCreatureTo(winner, winnerTeleportLocation, true);

                _server.sendToAll(_server.messageBuilder.chat("Game over! Winner: " + winner.name + "\nMost Antenae: " + highestAntenae, winnerTeleportLocation));
            } else {
                _server.sendToAll(_server.messageBuilder.chat("Game over! Winner: None\nMost Antenae: 0", winnerTeleportLocation));
            }
            clearArena();
        } else {
            spawnRoaches();
            sayMessageInArena("Seconds left: " + timeLeft / 1000);
            timeLeft -= arenaTickRate;
        }
    }

    private void sayMessageInArena(String msg) {
        Coord middleOfArena = arenaLocation.add(arenaSize / 2, arenaSize / 2, 0);
        _server.sendToAll(_server.messageBuilder.chat(msg, middleOfArena));
    }

    private ItemInstance removeItemFromInventory(Creature creature, int itemId) {
        ItemInstance item = _server.contentManager.createItemInstance(itemId, 0);
        for (int i = 0; i < creature.inventory.size(); i++) {
            ItemInstance itemAtSlot = creature.inventory.get(i);
            if (itemAtSlot.data.id == itemId) {
                item.quantity += itemAtSlot.quantity;
                creature.inventory.deleteSlot(i);
            }
        }
        return item;
    }

    private void clearArena() {
        arenaIsGoing = false;
        List<Creature> monsters = getCreaturesInArea(arenaLocation, arenaSize, creature -> !creature.belongsToPlayer);
        monsters.forEach(monster -> {
            _server.removeCreature(monster);
        });
    }

    private void spawnRoaches() {
        int numCurrently = getCreaturesInArea(arenaLocation, arenaSize, creature -> "".equals(creature.name)).size();
        Monster roachData = _server.contentManager.getMonster(42);
        try {
            roachData = roachData.clone();
            roachData.name = "";
        } catch (CloneNotSupportedException ex) {
            Logger.getLogger(GridiaServerDriver.class.getName()).log(Level.SEVERE, null, ex);
        }
        Random random = new Random();
        for (int i = numCurrently; i < numRoaches; i++) {
            Coord loc = arenaLocation.add(random.nextInt(arenaSize), random.nextInt(arenaSize), 0);
            if (_server.tileMap.walkable(loc) && _server.tileMap.getFloor(loc) != 0) {
                _server.createCreature(roachData, loc);
            }
        }
    }

    private void startArena() {
        arenaIsGoing = true;
        timeLeft = arenaDuration;
        spawnRoaches();
        sayMessageInArena("BEGIN!");
    }

    private List<Creature> getCreaturesInArea(Coord areaLocation, int size, Predicate<Creature> selector) {
        List<Creature> creaturesInArea = new ArrayList<>();
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                Coord loc = areaLocation.add(x, y, 0);
                Creature cre = _server.tileMap.getCreature(loc);
                if (cre != null && selector.test(cre)) {
                    creaturesInArea.add(cre);
                }
            }
        }
        return creaturesInArea;
    }

    private List<Creature> getPlayersInArena() {
        return getCreaturesInArea(arenaLocation, arenaSize, creature -> creature.belongsToPlayer);
    }
}
