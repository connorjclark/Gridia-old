package hoten.gridia;

import hoten.gridia.content.ItemInstance;
import hoten.gridia.serializers.GridiaGson;
import hoten.gridia.serving.ServingGridia;
import hoten.serving.fileutils.FileUtils;
import hoten.gridia.uniqueidentifiers.FileResourceUniqueIdentifiers;
import hoten.gridia.uniqueidentifiers.UniqueIdentifiers;
import java.io.File;
import java.util.List;

public class Container {

    public enum ContainerType {

        Inventory, Equipment, Other
    }

    public static final UniqueIdentifiers uniqueIds = new FileResourceUniqueIdentifiers("TestWorld/containers/");

    private final List<ItemInstance> _items;
    public final int id;
    public final ContainerType type;

    public Container(List<ItemInstance> items, ContainerType type) {
        _items = items;
        id = uniqueIds.next();
        this.type = type;
    }

    public Container(List<ItemInstance> items) {
        this(items, ContainerType.Inventory);
    }

    public List<ItemInstance> getItems() {
        return _items;
    }

    public ItemInstance get(int index) {
        return _items.get(index);
    }

    public boolean isEmpty(int index) {
        return _items.get(index) == ItemInstance.NONE;
    }

    public void set(int index, ItemInstance item) {
        _items.set(index, item);
        updateSlot(index);
    }

    public boolean add(ItemInstance itemToAdd) {
        return addToFirstAvailableSlot(itemToAdd);
    }

    public boolean add(ItemInstance itemToAdd, int slotIndex) {
        if (slotIndex > _items.size()) {
            return false;
        }
        ItemInstance currentItem = get(slotIndex);
        if (!ItemInstance.stackable(itemToAdd, currentItem) && currentItem.data.id != 0) {
            return false;
        }
        itemToAdd.quantity = itemToAdd.quantity + currentItem.quantity;
        set(slotIndex, itemToAdd);
        return true;
    }

    private boolean addToFirstAvailableSlot(ItemInstance itemToAdd) {
        for (int i = 0; i < _items.size(); i++) {
            if (add(itemToAdd, i)) {
                return true;
            }
        }
        return false;
    }

    public void updateSlot(int slotIndex) {
        ServingGridia.instance.updateContainerSlot(this, slotIndex); // :(
    }

    public void deleteSlot(int slotIndex) {
        set(slotIndex, ItemInstance.NONE);
        updateSlot(slotIndex);
    }

    public void reduceQuantityAt(int slotIndex, int amount) {
        ItemInstance item = get(slotIndex);
        item.quantity -= amount;
        if (item.quantity == 0) {
            set(slotIndex, ItemInstance.NONE);
        } else {
            updateSlot(slotIndex);
        }
    }
}

class InventoryLoader {

    public Container load(int id) {
        String json = FileUtils.readTextFile(new File("TestWorld/containers/" + id));
        return GridiaGson.get().fromJson(json, Container.class);
    }
}
