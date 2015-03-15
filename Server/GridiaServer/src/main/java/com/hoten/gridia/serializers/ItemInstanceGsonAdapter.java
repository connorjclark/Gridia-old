package com.hoten.gridia.serializers;

import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;
import com.google.gson.JsonSerializationContext;
import com.hoten.gridia.Creature;
import com.hoten.gridia.content.ContentManager;
import com.hoten.gridia.content.ItemInstance;
import com.hoten.gridia.map.Tile;
import com.hoten.gridia.serving.ServingGridia;
import java.lang.reflect.Type;

public class ItemInstanceGsonAdapter extends GsonAdapter<ItemInstance> {

   private final ContentManager _contentManager;

    public ItemInstanceGsonAdapter(ContentManager contentManager) {
        _contentManager = contentManager;
    }

    @Override
    public JsonElement serialize(ItemInstance item, Type type, JsonSerializationContext jsc) {
        JsonObject obj = new JsonObject();
        obj.addProperty("type", item.getItem().id);
        obj.addProperty("quantity", item.getQuantity());
        JsonObject data = item.getData();
        if (data != null && !data.entrySet().isEmpty()) {
            obj.add("data", item.getData());
        }
        return obj;
    }

    @Override
    public ItemInstance deserialize(final JsonElement json, final Type typeOfT, final JsonDeserializationContext context) throws JsonParseException {
        JsonObject jsonObject = json.getAsJsonObject();
        int itemType = jsonObject.get("type").getAsInt();
        int quantity = jsonObject.get("quantity").getAsInt();
        JsonObject data = jsonObject.has("data") ? jsonObject.get("data").getAsJsonObject() : null;
        return _contentManager.createItemInstance(itemType, quantity, data);
    }
}
