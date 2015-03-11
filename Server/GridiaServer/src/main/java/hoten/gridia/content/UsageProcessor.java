package hoten.gridia.content;

import hoten.gridia.ItemWrapper;
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

    protected void implementResult(UsageResult result, ItemUse usage, ItemWrapper tool, ItemWrapper focus) {
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

    protected void validate(UsageResult result, ItemUse usage, ItemWrapper tool, ItemWrapper focus) throws ItemUseException {
        boolean focusIsContainer = focus instanceof ItemWrapper.ContainerItemWrapper;
        if (focusIsContainer && usage.surfaceGround != -1) {
            throw new ItemUseException("You can't change the ground of a container slot");
        }
    }

    public void processUsage(ItemUse usage, ItemWrapper tool, ItemWrapper focus) throws ItemUseException {
        UsageResult result = getUsageResult(usage, tool.getItemInstance(), focus.getItemInstance());
        validate(result, usage, tool, focus);
        implementResult(result, usage, tool, focus);
    }
}
