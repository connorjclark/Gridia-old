package com.hoten.gridia.serializers;

import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonElement;
import com.google.gson.JsonParseException;
import com.google.gson.JsonSerializationContext;
import com.hoten.gridia.scripting.Entity;
import java.lang.reflect.Type;
import java.util.Map;

public class EntityGsonAdapter extends GsonAdapter<com.hoten.gridia.scripting.Entity> {

    @Override
    public JsonElement serialize(Entity item, Type type, JsonSerializationContext jsc) {
        return jsc.serialize(item.getAttribute("storage"));
    }

    @Override
    public Entity deserialize(JsonElement json, Type typeOfT, JsonDeserializationContext context) throws JsonParseException {
        Map storage = context.deserialize(json, Map.class);
        com.hoten.gridia.scripting.Entity entity = new com.hoten.gridia.scripting.Entity();
        entity.setAttribute("storage", storage);
        return entity;
    }
}
