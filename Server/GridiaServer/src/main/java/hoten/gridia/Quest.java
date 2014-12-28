package hoten.gridia;

import hoten.gridia.content.ItemInstance;
import hoten.gridia.map.Coord;
import hoten.gridia.serving.ServingGridia;
import java.util.ArrayList;
import java.util.List;
import java.util.function.Predicate;

public abstract class Quest implements Runnable {

    protected final ServingGridia _server;

    public Quest(ServingGridia server) {
        _server = server;
    }

    protected List<Creature> getCreaturesInArea(Coord areaLocation, int size, Predicate<Creature> selector) {
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

    protected ItemInstance removeItemFromInventory(Creature creature, int itemId) {
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
}
