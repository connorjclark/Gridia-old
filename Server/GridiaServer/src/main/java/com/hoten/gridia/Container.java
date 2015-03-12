package com.hoten.gridia;

import com.hoten.gridia.content.ItemInstance;
import com.hoten.gridia.serializers.GridiaGson;
import com.hoten.gridia.serving.ServingGridia;
import com.hoten.gridia.uniqueidentifiers.FileResourceUniqueIdentifiers;
import com.hoten.gridia.uniqueidentifiers.UniqueIdentifiers;
import java.io.File;
import java.io.IOException;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;
import java.util.stream.IntStream;
import org.apache.commons.io.FileUtils;

public class Container {

    public static class ContainerFactory {

        private final UniqueIdentifiers _uniqueIds;
        private final File dir;
        private final Map<Integer, Container> _containers = new HashMap<>();

        public ContainerFactory(File world) {
            dir = new File(world, "containers/");
            _uniqueIds = new FileResourceUniqueIdentifiers(dir, 100);
        }

        public Container get(int id) throws IOException {
            if (_containers.containsKey(id)) {
                return _containers.get(id);
            } else {
                return load(id);
            }
        }

        public boolean exists(int id) {
            return new File(dir, id + ".json").exists();
        }

        private Container load(int id) throws IOException {
            String json = FileUtils.readFileToString(new File(dir, id + ".json"));
            Container container = GridiaGson.get().fromJson(json, Container.class);
            _containers.put(id, container);
            return container;
        }

        public Container create(ContainerType type, List<ItemInstance> items) throws IOException {
            Container container = new Container(_uniqueIds.next(), type, items);
            _containers.put(container.id, container);
            save(container);
            return container;
        }

        public Container create(ContainerType type, int size) throws IOException {
            List<ItemInstance> items = IntStream.range(0, size)
                    .boxed()
                    .map(i -> ItemInstance.NONE)
                    .collect(Collectors.toList());
            return create(type, items);
        }

        public Container createOnlyInMemory(ContainerType type, List<ItemInstance> items) {
            Container container = new Container(_uniqueIds.next(), type, items);
            return container;
        }

        public Container createOnlyInMemory(ContainerType type, int size) {
            List<ItemInstance> items = IntStream.range(0, size)
                    .boxed()
                    .map(i -> ItemInstance.NONE)
                    .collect(Collectors.toList());
            return createOnlyInMemory(type, items);
        }

        public void save(Container container) throws IOException {
            FileUtils.writeStringToFile(new File(dir, container.id + ".json"), GridiaGson.get().toJson(container));
        }

        public void saveAll() throws IOException {
            for (Container container : _containers.values()) {
                save(container);
            }
        }
    }

    public enum ContainerType {

        Inventory, Equipment, Other
    }

    private final List<ItemInstance> _items;
    public final int id;
    public final ContainerType type; // :(

    public Container(int id, ContainerType type, List<ItemInstance> items) {
        this.id = id;
        _items = items;
        this.type = type;
    }

    public boolean containsItemId(int id) {
        return _items.stream()
                .anyMatch(item -> item.getItem().id == id);
    }

    public int size() {
        return _items.size();
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

    public boolean add(int slotIndex, ItemInstance itemToAdd) {
        if (slotIndex == -1) {
            return addToFirstAvailableSlot(itemToAdd);
        }
        if (slotIndex > _items.size()) {
            return false;
        }
        ItemInstance currentItem = get(slotIndex);
        if (!ItemInstance.stackable(itemToAdd, currentItem) && currentItem.getItem().id != 0) {
            return false;
        }
        set(slotIndex, itemToAdd.add(currentItem.getQuantity()));
        return true;
    }

    private boolean addToFirstAvailableSlot(ItemInstance itemToAdd) {
        for (int i = 0; i < _items.size(); i++) {
            if (add(i, itemToAdd)) {
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
        set(slotIndex, get(slotIndex).remove(amount));
    }

    public boolean canFitItem(ItemInstance itemToTest) {
        return _items.stream().
                anyMatch(item -> {
                    return item == ItemInstance.NONE || ItemInstance.stackable(item, itemToTest);
                });
    }
}
