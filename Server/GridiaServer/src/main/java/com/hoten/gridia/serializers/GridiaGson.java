package com.hoten.gridia.serializers;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.hoten.gridia.CreatureImage;
import com.hoten.gridia.content.ContentManager;
import com.hoten.gridia.content.ItemInstance;
import com.hoten.gridia.map.Tile;
import com.hoten.gridia.serving.ServingGridia;

public class GridiaGson {

    private static Gson _gson;

    public static Gson get() {
        return _gson;
    }

    public static void initialize(ContentManager contentManager, ServingGridia servingGridia) {
        _gson = new GsonBuilder()
                .registerTypeAdapter(ItemInstance.class, new ItemInstanceSerializer())
                .registerTypeAdapter(ItemInstance.class, new ItemInstanceDeserializer(contentManager))
                .registerTypeAdapter(Tile.class, new TileDeserializer(servingGridia))
                .registerTypeAdapter(Tile.class, new TileSerializer())
                .registerTypeAdapter(CreatureImage.class, new InterfaceAdapter<>())
                .create();
    }
}
