package hoten.gridia.serializers;

import com.google.gson.JsonArray;
import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;
import hoten.gridia.DefaultCreatureImage;
import hoten.gridia.content.ContentManager;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.content.Monster;
import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;

public class MonsterDeserializer implements JsonDeserializer<Monster> {

    private final ContentManager _contentManager;

    public MonsterDeserializer(ContentManager contentManager) {
        _contentManager = contentManager;
    }

    @Override
    public Monster deserialize(final JsonElement json, final Type typeOfT, final JsonDeserializationContext context) throws JsonParseException {
        JsonObject jsonObject = json.getAsJsonObject();
        Monster monster = new Monster();
        monster.id = jsonObject.get("id").getAsInt();
        monster.name = jsonObject.get("name").getAsString();

        int spriteIndex = jsonObject.has("image") ? jsonObject.get("image").getAsInt() - 1 : 0; // :(
        if (jsonObject.has("image_type")) {
            int imageType = jsonObject.get("image_type").getAsInt();
            if (imageType == 1) {
                monster.image = new DefaultCreatureImage(spriteIndex, 1, 2);
            } else if (imageType == 2) {
                monster.image = new DefaultCreatureImage(spriteIndex, 2, 2);
            }
        } else {
            monster.image = new DefaultCreatureImage(spriteIndex);
        }

        List<ItemInstance> drops = new ArrayList<>();
        drops.add(_contentManager.createItemInstance(490)); // :(
        if (jsonObject.has("treasure")) {
            JsonArray treasures = jsonObject.get("treasure").getAsJsonArray();
            for (int i = 0; i < treasures.size(); i++) {
                JsonObject treasure = treasures.get(i).getAsJsonObject();
                String itemName = treasure.get("item").getAsString();
                // :(
                if (!itemName.contains("<")) {
                    int type = _contentManager.getItemByName(itemName).id;
                    int quantity = treasure.get("quantity").getAsInt();
                    ItemInstance item = _contentManager.createItemInstance(type, quantity);
                    drops.add(item);
                }
            }
        }
        monster.drops = drops;

        return monster;
    }
}
