package hoten.gridia;

import hoten.gridia.content.Monster;
import hoten.gridia.map.Coord;
import hoten.gridia.serving.ServingGridia;
import java.util.List;

// hard code the roach quest :(
public class RoachQuest extends Quest {

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
        super(server);
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
                    int amount = removeItemFromInventory(player, 447).getQuantity();
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

    private void clearArena() {
        arenaIsGoing = false;
        List<Creature> monsters = getCreaturesInArea(arenaLocation, arenaSize, arenaSize, creature -> !creature.belongsToPlayer);
        monsters.forEach(monster -> {
            _server.removeCreature(monster);
        });
    }

    private void spawnRoaches() {
        int numCurrently = getCreaturesInArea(arenaLocation, arenaSize, arenaSize, creature -> "".equals(creature.name)).size();
        Monster roachData = cloneMonsterAndStripName(_server.contentManager.getMonster(42));
        spawnInArea(roachData, numRoaches - numCurrently, arenaLocation, arenaSize, arenaSize);
    }

    private void startArena() {
        arenaIsGoing = true;
        timeLeft = arenaDuration;
        spawnRoaches();
        sayMessageInArena("BEGIN!");
    }

    private List<Creature> getPlayersInArena() {
        return getPlayersInArea(arenaLocation, arenaSize, arenaSize);
    }
}
