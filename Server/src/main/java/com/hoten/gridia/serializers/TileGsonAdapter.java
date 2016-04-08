package com.hoten.gridia.serializers;

import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;
import com.google.gson.JsonSerializationContext;
import com.hoten.gridia.content.ItemInstance;
import com.hoten.gridia.map.Tile;
import com.hoten.gridia.scripting.Entity;
import com.hoten.gridia.serving.ServingGridia;
import java.lang.reflect.Type;

public class TileGsonAdapter extends GsonAdapter<Tile> {

    private final ServingGridia _servingGridia;

    // :( make creature factory
    public TileGsonAdapter(ServingGridia servingGridia) {
        _servingGridia = servingGridia;
    }

    @Override
    public JsonElement serialize(Tile tile, Type type, JsonSerializationContext jsc) {
        JsonObject obj = new JsonObject();
        obj.addProperty("floor", tile.floor);
        obj.add("item", jsc.serialize(tile.item));
        if (tile.cre != null && !tile.cre.getBoolean("belongsToPlayer") && !tile.cre.getBoolean("transient")) {
            obj.add("cre", jsc.serialize(tile.cre));
        }
        if (tile.isUnclaimed()) {
            obj.add("owner", jsc.serialize(tile.getOwner()));
        }
        return obj;
    }

    @Override
    public Tile deserialize(final JsonElement json, final Type typeOfT, final JsonDeserializationContext context) throws JsonParseException {
        JsonObject jsonObject = json.getAsJsonObject();
        Tile tile = new Tile();
        tile.floor = jsonObject.get("floor").getAsInt();
        tile.item = context.deserialize(jsonObject.get("item"), ItemInstance.class);
        if (jsonObject.has("cre") && false) {
            Entity cre = context.deserialize(jsonObject.get("cre"), Entity.class); // :(
            tile.cre = cre;
            _servingGridia.attachScriptsToCreature(cre);
        }
        if (jsonObject.has("owner")) {
            tile.setOwner(jsonObject.get("owner").getAsInt());
        }
        return tile;
    }
}
