package hoten.gridiaserver.serializers;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import hoten.gridiaserver.content.ContentManager;
import hoten.gridiaserver.content.ItemInstance;
import hoten.gridiaserver.map.Tile;

public class GridiaGson {

    private static Gson _gson;

    public static Gson get() {
        return _gson;
    }

    public static void initialize(ContentManager contentManager) {
        if (_gson != null) {
            throw new RuntimeException("GridiaGson already initialized.");
        }
        _gson = new GsonBuilder()
                .registerTypeAdapter(ItemInstance.class, new ItemInstanceDeserializer(contentManager))
                .registerTypeAdapter(Tile.class, new TileDeserializer())
                .create();
    }
}
