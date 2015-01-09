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

public abstract class Quest implements Runnable {

    protected final ServingGridia _server;

    public Quest(ServingGridia server) {
        _server = server;
    }

    protected List<Creature> getCreaturesInArea(Coord areaLocation, int width, int height, Predicate<Creature> selector) {
        List<Creature> creaturesInArea = new ArrayList<>();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Coord loc = areaLocation.add(x, y, 0);
                Creature cre = _server.tileMap.getCreature(loc);
                if (cre != null && selector.test(cre)) {
                    creaturesInArea.add(cre);
                }
            }
        }
        return creaturesInArea;
    }

    protected ItemInstance removeItemFromInventory(Creature creature, int itemId) {
        ItemInstance item = _server.contentManager.createItemInstance(itemId, 0);
        for (int i = 0; i < creature.inventory.size(); i++) {
            ItemInstance itemAtSlot = creature.inventory.get(i);
            if (itemAtSlot.getData().id == itemId) {
                item = item.add(itemAtSlot.getQuantity());
                creature.inventory.deleteSlot(i);
            }
        }
        return item;
    }

    protected Monster cloneMonsterAndStripName(Monster monster) {
        try {
            Monster cloned = monster.clone();
            cloned.name = "";
            return cloned;
        } catch (CloneNotSupportedException ex) {
            Logger.getLogger(GridiaServerDriver.class.getName()).log(Level.SEVERE, null, ex);
            return monster;
        }
    }

    protected void spawnInArea(Monster monster, int amount, Coord location, int width, int height) {
        Random random = new Random();
        for (int i = 0; i < amount; i++) {
            Coord loc = location.add(random.nextInt(width), random.nextInt(height), 0);
            if (_server.tileMap.walkable(loc) && _server.tileMap.getFloor(loc) != 0) {
                _server.createCreature(monster, loc);
            }
        }
    }

    protected List<Creature> getPlayersInArea(Coord location, int width, int height) {
        return getCreaturesInArea(location, width, height, creature -> creature.belongsToPlayer);
    }
}
