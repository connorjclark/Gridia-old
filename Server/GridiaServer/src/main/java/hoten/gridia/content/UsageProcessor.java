package hoten.gridia.content;

import hoten.gridia.Container;
import hoten.gridia.map.Coord;
import hoten.gridia.serving.ServingGridia;
import java.util.ArrayList;
import java.util.List;

public class UsageProcessor {

    public interface ItemWrapper {

        ItemInstance getItemInstance();

        boolean addItemToSource(ItemInstance itemToAdd);

        boolean addItemHere(ItemInstance itemToAdd);

        void changeWrappedItem(ItemInstance newItem);
    }

    public static class WorldItemWrapper implements ItemWrapper {

        private final ServingGridia _server; // :(
        private final Coord _location;
        private final ItemInstance _item;

        public WorldItemWrapper(ServingGridia server, Coord location) {
            _server = server;
            _location = location;
            _item = server.tileMap.getItem(location);
        }

        @Override
        public boolean addItemToSource(ItemInstance itemToAdd) {
            return _server.addItemNear(_location, itemToAdd, 3);
        }

        @Override
        public void changeWrappedItem(ItemInstance newItem) {
            _server.changeItem(_location, newItem);
        }

        @Override
        public boolean addItemHere(ItemInstance itemToAdd) {
            return _server.addItem(_location, itemToAdd);
        }

        @Override
        public ItemInstance getItemInstance() {
            return _item;
        }
    }

    public static class ContainerItemWrapper implements ItemWrapper {

        private final Container _container;
        private final int _slot;
        private final ItemInstance _item;

        public ContainerItemWrapper(Container container, int slot) {
            _container = container;
            _slot = slot;
            _item = slot != -1 ? container.get(slot) : ItemInstance.NONE;
        }

        @Override
        public boolean addItemToSource(ItemInstance itemToAdd) {
            return _container.add(itemToAdd);
        }

        @Override
        public void changeWrappedItem(ItemInstance newItem) {
            _container.set(_slot, newItem);
        }

        @Override
        public boolean addItemHere(ItemInstance itemToAdd) {
            return _container.add(_slot, itemToAdd);
        }

        @Override
        public ItemInstance getItemInstance() {
            return _item;
        }
    }

    public class UsageResult {

        public ItemInstance tool;
        public ItemInstance focus;
        public List<ItemInstance> products;
    }

    private final ContentManager _contentManager;

    public UsageProcessor(ContentManager contentManager) {
        _contentManager = contentManager;
    }

    List<ItemUse> findUsages(ItemInstance tool, ItemInstance focus) {
        return _contentManager.getItemUses(tool.data, focus.data);
    }

    UsageResult getUsageResult(ItemUse usage, ItemInstance tool, ItemInstance focus) {
        UsageResult result = new UsageResult();

        result.tool = new ItemInstance(tool);
        result.focus = new ItemInstance(focus);

        result.tool.quantity -= usage.toolQuantityConsumed;
        result.focus.quantity -= usage.focusQuantityConsumed;

        if (result.tool.quantity <= 0) {
            result.tool = ItemInstance.NONE;
        }
        if (result.focus.quantity <= 0) {
            result.focus = ItemInstance.NONE;
        }

        result.products = new ArrayList<>();
        for (int i = 0; i < usage.products.size(); i++) {
            int itemId = usage.products.get(i);
            int itemQuantity = usage.quantities.get(i);
            result.products.add(_contentManager.createItemInstance(itemId, itemQuantity));
        }

        return result;
    }

    boolean validateUsageResult(UsageResult result) {
        return true;
    }

    void implementResult(UsageResult result, ItemWrapper tool, ItemWrapper focus) {
        if (tool.getItemInstance().data.id != 0) {
            tool.changeWrappedItem(result.tool);
        }
        focus.changeWrappedItem(result.focus);
        result.products.forEach(product -> {
            focus.addItemToSource(product);
        });
    }

    public void processUsage(ItemUse usage, ItemWrapper tool, ItemWrapper focus) {
        UsageResult result = getUsageResult(usage, tool.getItemInstance(), focus.getItemInstance());
        // validate
        implementResult(result, tool, focus);
    }

    // :(
    /*
     public void executeItemUse(
     ConnectionToGridiaClientHandler connection,
     ItemUse use,
     ItemInstance tool,
     ItemInstance focus,
     String source,
     String dest,
     int sourceIndex,
     int destIndex
     ) throws IOException {
     ServingGridia server = connection.getServer();
     Player player = connection.getPlayer();
     if (use.successTool != -1) {
     ItemInstance toolResult = null;
     tool.quantity -= 1;
     if (tool.quantity <= 0) {
     tool = ItemInstance.NONE;
     }
     if (use.successTool != 0) {
     toolResult = server.contentManager.createItemInstance(use.successTool);
     }

     switch (source) {
     case "world":
     server.changeItem(sourceIndex, tool);
     if (toolResult != null) {
     server.addItemNear(server.tileMap.getCoordFromIndex(sourceIndex), toolResult, 3);
     }
     break;
     case "inv":
     player.creature.inventory.set(sourceIndex, tool);
     if (toolResult != null) {
     player.creature.inventory.add(toolResult);
     }
     break;
     }
     }

     if (use.focusQuantityConsumed > 0) {
     if (focus != ItemInstance.NONE) {
     focus.quantity -= use.focusQuantityConsumed;
     }
     switch (dest) {
     case "world":
     server.updateTile(destIndex);
     break;
     }
     }

     if ("world".equals(dest)) {
     for (int i = 0; i < use.products.size(); i++) {
     ItemInstance productInstance = server.contentManager.createItemInstance(use.products.get(i), use.quantities.get(i));
     if (productInstance.data.itemClass == Item.ItemClass.Cave_down) {
     if (player.creature.location.z == server.tileMap.depth - 1) {
     continue;
     }
     Coord below = server.tileMap.getCoordFromIndex(destIndex).add(0, 0, 1);
     server.changeItem(server.tileMap.getIndexFromCoord(below), server.contentManager.createItemInstance(981));
     for (int x = -1; x <= 1; x++) {
     for (int y = -1; y <= 1; y++) {
     server.changeFloor(below.add(x, y, 0), 19);
     }
     }
     } else if (productInstance.data.itemClass == Item.ItemClass.Cave_up) {
     if (player.creature.location.z == 0) {
     continue;
     }
     Coord above = server.tileMap.getCoordFromIndex(destIndex).add(0, 0, -1);
     server.changeItem(server.tileMap.getIndexFromCoord(above), server.contentManager.createItemInstance(980));
     }

     if (i != 0 && use.tool == 0 && player.creature.inventory.canFitItem(productInstance)) {
     player.creature.inventory.add(productInstance);
     } else {
     server.addItemNear(destIndex, productInstance, 4);
     }
     }
     if (use.animation != 0) {
     Coord loc = server.tileMap.getCoordFromIndex(destIndex);
     server.sendToClientsWithAreaLoaded(server.messageBuilder.animation(use.animation, loc), destIndex);
     }
     if (use.surfaceGround != -1) {
     Coord loc = server.tileMap.getCoordFromIndex(destIndex);
     server.changeFloor(loc, use.surfaceGround);
     }
     }

     if (use.successMessage != null) {
     connection.send(server.messageBuilder.chat(use.successMessage, player.creature.location));
     }
     }*/
}
