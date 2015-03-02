package hoten.gridia.content;

import hoten.gridia.ItemWrapper;
import hoten.gridia.content.Item.ItemClass;
import hoten.gridia.map.Coord;
import java.util.ArrayList;
import java.util.List;

public class UsageProcessor {

    public class UsageResult {

        public ItemInstance tool;
        public ItemInstance focus;
        public ItemInstance successTool;
        public List<ItemInstance> products;
    }

    private final ContentManager _contentManager;

    public UsageProcessor(ContentManager contentManager) {
        _contentManager = contentManager;
    }

    List<ItemUse> findUsages(ItemInstance tool, ItemInstance focus) {
        return _contentManager.getItemUses(tool.getItem(), focus.getItem());
    }

    UsageResult getUsageResult(ItemUse usage, ItemInstance tool, ItemInstance focus) {
        UsageResult result = new UsageResult();

        result.tool = tool.remove(usage.toolQuantityConsumed);
        result.focus = focus.remove(usage.focusQuantityConsumed);

        if (usage.successTool != -1) {
            result.successTool = _contentManager.createItemInstance(usage.successTool, 1, tool.getData());
        }

        // :(
        result.products = new ArrayList<>();
        for (int i = 0; i < usage.products.size(); i++) {
            int itemId = usage.products.get(i);
            int itemQuantity = usage.quantities.get(i);
            result.products.add(_contentManager.createItemInstance(itemId, itemQuantity, focus.getData()));
        }

        return result;
    }

    // :(
    private void makeCaveIfNecessary(UsageResult result, ItemWrapper focus) {
        if (!result.products.isEmpty()) {
            Item firstResult = result.products.get(0).getItem();
            if (firstResult.isCave()) {
                ItemWrapper.WorldItemWrapper focusAsWorld = (ItemWrapper.WorldItemWrapper) focus;
                if (firstResult.itemClass == ItemClass.Cave_down) {
                    ItemInstance below = focusAsWorld.getItemBelow();
                    if (below.getItem().itemClass != ItemClass.Cave_up) {
                        if (below == ItemInstance.NONE || focusAsWorld.moveItemBelow()) {
                            focusAsWorld.setItemBelow(_contentManager.createItemInstance(981));
                        }
                    }
                } else {
                    ItemInstance above = focusAsWorld.getItemAbove();
                    if (above.getItem().itemClass != ItemClass.Cave_down) {
                        if (above == ItemInstance.NONE || focusAsWorld.moveItemAbove()) {
                            focusAsWorld.setItemAbove(_contentManager.createItemInstance(980));
                        }
                    }
                }
            }
        }
    }

    void implementResult(UsageResult result, ItemWrapper tool, ItemWrapper focus) {
        if (tool.getItemInstance() != ItemInstance.NONE) {
            tool.changeWrappedItem(result.tool);
            if (result.successTool != null) {
                tool.addItemToSource(result.successTool);
            }
        }
        focus.changeWrappedItem(result.focus);
        for (int i = 0; i < result.products.size(); i++) {
            if (i == 0 || tool.getItemInstance() != ItemInstance.NONE) {
                focus.addItemToSource(result.products.get(i));
            } else {
                tool.addItemToSource(result.products.get(i));
            }
        }
    }

    private boolean isCaveOrNothing(ItemInstance item) {
        return item.getItem().isCave() || item == ItemInstance.NONE;
    }

    private boolean validate(UsageResult result, ItemUse usage, ItemWrapper tool, ItemWrapper focus) {
        boolean focusIsContainer = focus instanceof ItemWrapper.ContainerItemWrapper;
        if (focusIsContainer && usage.surfaceGround != -1) {
            return false;
        }

        if (!result.products.isEmpty()) {
            Item firstResult = result.products.get(0).getItem();
            if (firstResult.isCave()) {
                if (focusIsContainer) {
                    return false;
                } else {
                    ItemWrapper.WorldItemWrapper focusAsWorld = (ItemWrapper.WorldItemWrapper) focus;
                    if (firstResult.itemClass == ItemClass.Cave_down) {
                        if (focusAsWorld.isLowestLevel()) {
                            return false;
                        }
                    } else if (firstResult.itemClass == ItemClass.Cave_up) {
                        if (focusAsWorld.isHighestLevel()) {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    public boolean processUsage(ItemUse usage, ItemWrapper tool, ItemWrapper focus) {
        UsageResult result = getUsageResult(usage, tool.getItemInstance(), focus.getItemInstance());
        boolean success = validate(result, usage, tool, focus);
        if (success) {
            makeCaveIfNecessary(result, focus);
            implementResult(result, tool, focus);
        }
        return success;
    }
}
