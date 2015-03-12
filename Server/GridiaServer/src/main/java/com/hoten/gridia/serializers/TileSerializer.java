package com.hoten.gridia.serializers;

import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;
import com.hoten.gridia.map.Tile;
import java.lang.reflect.Type;

public class TileSerializer implements JsonSerializer<Tile> {

    @Override
    public JsonElement serialize(Tile tile, Type type, JsonSerializationContext jsc) {
        JsonObject obj = new JsonObject();
        obj.addProperty("floor", tile.floor);
        obj.add("item", jsc.serialize(tile.item));
        if (tile.cre != null && !tile.cre.belongsToPlayer) {
            obj.add("cre", jsc.serialize(tile.cre));
        }
        return obj;
    }
}
