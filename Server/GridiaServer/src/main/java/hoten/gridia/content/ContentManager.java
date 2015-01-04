package hoten.gridia.content;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.reflect.TypeToken;
import hoten.gridia.content.Item.ItemClass;
import hoten.gridia.serializers.ItemDeserializer;
import hoten.gridia.serializers.MonsterDeserializer;
import java.io.File;
import java.io.IOException;
import java.lang.reflect.Type;
import java.util.Arrays;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;
import org.apache.commons.io.FileUtils;

public class ContentManager {

    private final List<Item> _items;
    private final Map<Item, List<ItemUse>> _itemUses;
    private final List<Monster> _monsters;
    private final File _content;

    public ContentManager(File world) throws IOException {
        _content = new File(world, "clientdata/content");
        _items = loadItems();
        _monsters = loadMonsters();
        ItemInstance.NONE.data = _items.get(0);
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

    public Item getItemByName(String name) {
        return _items.stream()
                .filter(item -> item != null && item.name.equalsIgnoreCase(name))
                .findFirst().get();
    }

    // cache?
    public Item getRandomItemOfClassByRarity(ItemClass itemClass) {
        List<Item> itemsOfClass = _items.stream()
                .filter(item -> item != null && item.itemClass == itemClass)
                .collect(Collectors.toList());
        int totalRarity = itemsOfClass.stream()
                .mapToInt(item -> item.rarity)
                .sum();
        int targetRarity = (int) (Math.random() * totalRarity);

        int raritySeen = 0;
        for (Item item : itemsOfClass) {
            raritySeen += item.rarity;
            if (raritySeen > targetRarity) {
                return item;
            }
        }

        return ItemInstance.NONE.data;
    }

    public Monster getMonster(int id) {
        if (_monsters.size() <= id || 0 > id) {
            return null;
        }
        return _monsters.get(id);
    }

    public List<ItemUse> getItemUses(Item tool, Item focus) {
        List<ItemUse> uses = _itemUses.get(tool);
        if (uses == null) {
            return Arrays.asList();
        }
        return uses.stream()
                .filter(u -> (u.focus == focus.id && u.focusSubType == null) || (u.focusSubType != null && u.focusSubType.equals(focus.subType)))
                .collect(Collectors.toList());
    }

    public ItemUse getItemUse(Item tool, Item focus, int index) {
        return _itemUses.get(tool).stream()
                .filter(u -> (u.focus == focus.id && u.focusSubType == null) || (u.focusSubType != null && u.focusSubType.equals(focus.subType)))
                .skip(index)
                .findFirst()
                .get();
    }

    private String loadContentFile(String contentName) throws IOException {
        return FileUtils.readFileToString(new File(_content, contentName + ".json"));
    }

    private List<Item> loadItems() throws IOException {
        String json = loadContentFile("items");
        Type type = new TypeToken<List<Item>>() {
        }.getType();
        return load(json, type);
    }

    private List<Monster> loadMonsters() throws IOException {
        String json = loadContentFile("monsters");
        Type type = new TypeToken<List<Monster>>() {
        }.getType();
        return load(json, type);
    }

    private Map<Item, List<ItemUse>> loadItemUses() throws IOException {
        String json = loadContentFile("itemuses");
        Type type = new TypeToken<List<ItemUse>>() {
        }.getType();
        List<ItemUse> usesList = load(json, type);
        return usesList.stream()
                .collect(Collectors.groupingBy(u -> getItem(u.tool), Collectors.mapping(u -> u, Collectors.toList())));
    }

    private List load(String json, Type type) {
        Gson gson = new GsonBuilder()
                .registerTypeAdapter(Item.class, new ItemDeserializer()) // :(
                .registerTypeAdapter(Monster.class, new MonsterDeserializer(this))
                .create();
        return gson.fromJson(json, type);
    }
}
