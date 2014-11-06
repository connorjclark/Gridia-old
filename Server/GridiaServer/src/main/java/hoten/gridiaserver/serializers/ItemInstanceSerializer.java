package hoten.gridiaserver.serializers;

import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;
import hoten.gridiaserver.content.ItemInstance;
import java.lang.reflect.Type;

public class ItemInstanceSerializer implements JsonSerializer<ItemInstance> {

    @Override
    public JsonElement serialize(ItemInstance item, Type type, JsonSerializationContext jsc) {
        JsonObject obj = new JsonObject();
        obj.addProperty("type", item.data.id);
        obj.addProperty("quantity", item.quantity);
        return obj;
    }
}
