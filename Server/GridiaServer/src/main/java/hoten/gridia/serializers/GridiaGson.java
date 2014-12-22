package hoten.gridia.serializers;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import hoten.gridia.CreatureImage;
import hoten.gridia.content.ContentManager;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.map.Tile;
import hoten.gridia.serving.ServingGridia;

public class GridiaGson {

    private static Gson _gson;

    public static Gson get() {
        return _gson;
    }

    public static void initialize(ContentManager contentManager, ServingGridia servingGridia) {
        _gson = new GsonBuilder()
                .registerTypeAdapter(ItemInstance.class, new ItemInstanceDeserializer(contentManager))
                .registerTypeAdapter(Tile.class, new TileDeserializer(servingGridia))
                .registerTypeAdapter(Tile.class, new TileSerializer())
                .registerTypeAdapter(ItemInstance.class, new ItemInstanceSerializer())
                //.registerTypeAdapter(CreatureImage.class, new CreatureImageDeserializer())
                .registerTypeAdapter(CreatureImage.class, new InterfaceAdapter<>())
                .create();
    }
}
