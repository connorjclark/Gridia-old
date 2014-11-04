package hoten.gridiaserver;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import hoten.serving.fileutils.FileUtils;
import java.io.File;
import java.lang.reflect.Type;
import java.util.List;

public class ContentManager {

    private final List<Item> _items;
    private final String _path;

    public ContentManager(String worldName) {
        _path = String.format("%s/clientdata/content/", worldName);
        _items = loadItems();
    }
    
    public Item getItem(int id) {
        return _items.get(id);
    }

    private List loadItems() {
        String json = FileUtils.readTextFile(new File(_path + "items.txt"));
        Type type = new TypeToken<List<Item>>() {
        }.getType();
        return load(json, type);
    }

    private List load(String json, Type type) {
        return new Gson().fromJson(json, type);
    }
}
