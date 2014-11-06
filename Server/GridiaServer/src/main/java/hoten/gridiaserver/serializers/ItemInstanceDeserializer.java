package hoten.gridiaserver.serializers;

import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;
import hoten.gridiaserver.content.ContentManager;
import hoten.gridiaserver.content.ItemInstance;
import java.lang.reflect.Type;

public class ItemInstanceDeserializer implements JsonDeserializer<ItemInstance> {

    private final ContentManager _contentManager;

    public ItemInstanceDeserializer(ContentManager contentManager) {
        _contentManager = contentManager;
    }

    @Override
    public ItemInstance deserialize(final JsonElement json, final Type typeOfT, final JsonDeserializationContext context) throws JsonParseException {
        JsonObject jsonObject = json.getAsJsonObject();
        int itemType = jsonObject.get("type").getAsInt();
        int quantity = jsonObject.get("quantity").getAsInt();
        return _contentManager.createItemInstance(itemType, quantity);
    }
}
