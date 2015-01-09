package hoten.gridia.content;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.reflect.TypeToken;
import hoten.gridia.serializers.ItemDeserializer;
import hoten.gridia.serializers.MonsterDeserializer;
import java.io.IOException;
import java.lang.reflect.Type;
import java.util.List;

public class JsonContentLoader implements ContentLoader {

    private final String _itemsJson, _usagesJson, _monstersJson;

    public JsonContentLoader(String itemsJson, String usagesJson, String monstersJson) {
        _itemsJson = itemsJson;
        _usagesJson = usagesJson;
        _monstersJson = monstersJson;
    }

    @Override
    public ContentManager load() throws IOException {
        List<Item> items = loadItems();
        List<ItemUse> usages = loadItemUses(items);
        List<Monster> monsters = loadMonsters(items);
        return new ContentManager(items, usages, monsters);
    }

    private List<Item> loadItems() throws IOException {
        Type type = new TypeToken<List<Item>>() {
        }.getType();
        return new GsonBuilder()
                .registerTypeAdapter(Item.class, new ItemDeserializer())
                .create()
                .fromJson(_itemsJson, type);
    }

    private List<ItemUse> loadItemUses(List<Item> items) throws IOException {
        Type type = new TypeToken<List<ItemUse>>() {
        }.getType();
        return new Gson().fromJson(_usagesJson, type);
    }

    private List<Monster> loadMonsters(List<Item> items) throws IOException {
        Type type = new TypeToken<List<Monster>>() {
        }.getType();
        return new GsonBuilder()
                .registerTypeAdapter(Item.class, new ItemDeserializer())
                .registerTypeAdapter(Monster.class, new MonsterDeserializer(items))
                .create()
                .fromJson(_monstersJson, type);
    }
}
