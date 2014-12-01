package hoten.gridia.serializers;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import hoten.gridia.content.ContentManager;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.map.Tile;

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
