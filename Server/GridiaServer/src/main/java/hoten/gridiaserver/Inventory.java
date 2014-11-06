package hoten.gridiaserver;

import hoten.gridiaserver.content.ItemInstance;
import hoten.gridiaserver.serializers.GridiaGson;
import hoten.serving.fileutils.FileUtils;
import hoten.uniqueidentifiers.FileResourceUniqueIdentifiers;
import hoten.uniqueidentifiers.UniqueIdentifiers;
import java.io.File;
import java.util.Iterator;
import java.util.List;

public class Inventory {

    public static final UniqueIdentifiers uniqueIds = new FileResourceUniqueIdentifiers("TestWorld/containers/");

    public final List<ItemInstance> items;
    public final int id = uniqueIds.next();

    public Inventory(List<ItemInstance> items) {
        this.items = items;
    }
}

class InventoryLoader {

    public Inventory load(int id) {
        String json = FileUtils.readTextFile(new File("TestWorld/containers/" + id));
        return GridiaGson.get().fromJson(json, Inventory.class);
    }
}
