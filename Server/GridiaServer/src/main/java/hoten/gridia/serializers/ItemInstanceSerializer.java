package hoten.gridia.serializers;

import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;
import hoten.gridia.content.ItemInstance;
import java.lang.reflect.Type;

public class ItemInstanceSerializer implements JsonSerializer<ItemInstance> {

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
}
