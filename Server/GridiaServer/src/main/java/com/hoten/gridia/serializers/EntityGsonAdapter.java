package com.hoten.gridia.serializers;

import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;
import com.google.gson.JsonPrimitive;
import com.google.gson.JsonSerializationContext;
import com.google.gson.reflect.TypeToken;
import com.hoten.gridia.Container;
import com.hoten.gridia.map.Coord;
import com.hoten.gridia.scripting.Entity;
import java.lang.reflect.Type;
import java.util.Map;
import java.util.logging.Level;
import java.util.logging.Logger;

public class EntityGsonAdapter extends GsonAdapter<Entity> {

    private void serializeWithType(JsonSerializationContext jsc, JsonObject into, String name, Object value) {
        if (value.getClass() != Container.class) {
            JsonObject jo2 = new JsonObject();
            jo2.addProperty("class", value.getClass().getName());
            jo2.add("value", jsc.serialize(value));
            into.add(name, jo2);
        }
    }

    @Override
    public JsonElement serialize(Entity entity, Type type, JsonSerializationContext jsc) {
        JsonObject jo = new JsonObject();
        Map<String, Object> storage = (Map) entity.getAttribute("storage");
        storage.forEach((key, value) -> {
            serializeWithType(jsc, jo, key, value);
        });
        if (entity.location != null) {
            serializeWithType(jsc, jo, "location", entity.location);
        }
        return jo;
    }

    @Override
    public Entity deserialize(JsonElement json, Type typeOfT, JsonDeserializationContext context) throws JsonParseException {
        Type storageType = new TypeToken<Map<String, Map<String, JsonElement>>>() {
        }.getType();
        Map<String, Map> storage = context.deserialize(json, storageType);
        Entity entity = new Entity();
        storage.forEach((key, valueAndClass) -> {
            try {
                JsonPrimitive jsonClass = (JsonPrimitive) valueAndClass.get("class");
                Type klass = Class.forName(jsonClass.getAsString());
                Object value = context.deserialize((JsonElement) valueAndClass.get("value"), klass);
                if (!"location".equals(key)) {
                    entity.setAttribute(key, value);
                } else {
                    entity.location = (Coord) value;
                }
            } catch (ClassNotFoundException ex) {
                Logger.getLogger(EntityGsonAdapter.class.getName()).log(Level.SEVERE, null, ex);
            }
        });
        return entity;
    }
}
