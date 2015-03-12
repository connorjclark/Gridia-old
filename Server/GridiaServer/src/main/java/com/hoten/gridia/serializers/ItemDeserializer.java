package com.hoten.gridia.serializers;

import com.google.gson.Gson;
import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;
import com.hoten.gridia.content.Item;
import java.lang.reflect.Type;

public class ItemDeserializer implements JsonDeserializer<Item> {

    @Override
    public Item deserialize(final JsonElement json, final Type typeOfT, final JsonDeserializationContext context) throws JsonParseException {
        Gson gson = new Gson();
        JsonObject jsonObject = json.getAsJsonObject();
        Item item = gson.fromJson(jsonObject, Item.class);
        if (jsonObject.has("class")) {
            String itemClass = jsonObject.get("class").getAsString();
            item.itemClass = Item.ItemClass.valueOf(itemClass);
        }

        if (item.isEquipable()) {
            String armorSpot = jsonObject.get("armorSpot").getAsString();
            item.armorSpot = Item.ArmorSpot.valueOf(armorSpot);
        }

        return item;
    }
}
