package hoten.gridiaserver.content;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import hoten.serving.fileutils.FileUtils;
import java.io.File;
import java.lang.reflect.Type;
import java.util.Arrays;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

public class ContentManager {

    private final List<Item> _items;
    private final Map<Item, List<ItemUse>> _itemUses;
    private final String _path;

    public ContentManager(String worldName) {
        _path = String.format("%s/clientdata/content/", worldName);
        _items = loadItems();
        _items.set(0, ItemInstance.NONE.data);
        _itemUses = loadItemUses();
    }

    public ItemInstance createItemInstance(int id, int quantity) {
        if (id == 0) {
            return ItemInstance.NONE;
        }
        Item item = getItem(id);
        if (item == null) {
            System.err.println("null item: " + id);
            return ItemInstance.NONE;
        }
        return new ItemInstance(item, quantity);
    }

    public ItemInstance createItemInstance(int id) {
        return createItemInstance(id, 1);
    }

    public Item getItem(int id) {
        if (id == -1) {
            return _items.get(0);
        }
        return _items.get(id);
    }

    public List<ItemUse> getItemUses(Item tool, Item focus) {
        List<ItemUse> uses = _itemUses.get(tool);
        if (uses == null) {
            return Arrays.asList();
        }
        return uses.stream()
                .filter(u -> u.focus == focus.id)
                .collect(Collectors.toList());
    }

    public ItemUse getItemUse(Item tool, Item focus, int index) {
        return _itemUses.get(tool).stream()
                .filter(u -> u.focus == focus.id)
                .skip(index)
                .findFirst()
                .get();
    }

    private List<Item> loadItems() {
        String json = FileUtils.readTextFile(new File(_path + "items.txt"));
        Type type = new TypeToken<List<Item>>() {
        }.getType();
        return load(json, type);
    }

    private Map<Item, List<ItemUse>> loadItemUses() {
        String json = FileUtils.readTextFile(new File(_path + "itemuses.txt"));
        Type type = new TypeToken<List<ItemUse>>() {
        }.getType();
        List<ItemUse> usesList = load(json, type);
        return usesList.stream()
                .collect(Collectors.groupingBy(u -> getItem(u.tool), Collectors.mapping(u -> u, Collectors.toList())));
    }

    private List load(String json, Type type) {
        Gson gson = new Gson();
        return gson.fromJson(json, type);
    }
}
