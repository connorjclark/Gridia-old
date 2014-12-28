package hoten.gridia;

import hoten.gridia.content.Monster;
import hoten.gridia.map.Coord;
import hoten.gridia.serving.ServingGridia;
import java.util.ArrayList;
import java.util.List;

public class RandomDungeonQuest extends Quest {

    private class Area {

        public final Coord location;
        public final int width, height, numberToSpawn;

        public Area(int x, int y, int z, int width, int height, int numberToSpawn) {
            location = new Coord(x, y, z);
            this.width = width;
            this.height = height;
            this.numberToSpawn = numberToSpawn;
        }
    }

    public int arenaTickRate = 5000;
    private final List<Area> _areas = new ArrayList<>();
    private final Area _entireArea = new Area(-14, 114, 1, 43, 43, 0);

    public RandomDungeonQuest(ServingGridia server) {
        super(server);
        _areas.add(new Area(5, 137, 1, 13, 13, 5)); // first room
        _areas.add(new Area(1, 114, 1, 21, 8, 10)); // big room
        _areas.add(new Area(-14, 126, 1, 13, 16, 14)); // hall
    }

    @Override
    public void run() {
        if (getPlayersInArea(_entireArea.location, _entireArea.width, _entireArea.height).isEmpty()) {
            List<Creature> monsters = getCreaturesInArea(_entireArea.location, _entireArea.width, _entireArea.height, creature -> !creature.belongsToPlayer);
            monsters.forEach(monster -> {
                _server.removeCreature(monster);
            });
        } else {
            _areas.forEach(area -> {
                if (getPlayersInArea(area.location, area.width, area.height).size() > 0) {
                    int numCurrently = getCreaturesInArea(area.location, area.width, area.height, creature -> !creature.belongsToPlayer).size();
                    if (numCurrently == 0) {
                        Monster randomMonster = getRandomMonster();
                        spawnInArea(cloneMonsterAndStripName(randomMonster), area.numberToSpawn, area.location, area.width, area.height);
                        Coord middle = area.location.add(area.width / 2, area.height / 2, 0);
                        _server.sendToClientsWithAreaLoaded(_server.messageBuilder.chat("A gang of " + randomMonster.name + " appear!", middle), middle);
                    }
                }
            });
        }
    }

    private Monster getRandomMonster() {
        while (true) {
            Monster monster = _server.contentManager.getMonster((int) (Math.random() * 812));
            if (monster != null) {
                return monster;
            }
        }
    }
}
