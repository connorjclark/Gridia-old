package hoten.gridia.serializers;

import com.google.gson.JsonArray;
import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;
import hoten.gridia.DefaultCreatureImage;
import hoten.gridia.content.Item;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.content.Monster;
import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;

public class MonsterDeserializer implements JsonDeserializer<Monster> {

    private final List<Item> _items; // :(

    public MonsterDeserializer(List<Item> items) {
        _items = items;
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
        if (jsonObject.has("treasure")) {
            JsonArray treasures = jsonObject.get("treasure").getAsJsonArray();
            for (int i = 0; i < treasures.size(); i++) {
                JsonObject treasure = treasures.get(i).getAsJsonObject();
                String itemName = treasure.get("item").getAsString();
                // :(
                if (!itemName.contains("<")) {
                    Item data = _items.stream()
                            .filter(item -> item != null && item.name.equalsIgnoreCase(itemName))
                            .findFirst().get();
                    int quantity = treasure.get("quantity").getAsInt();
                    ItemInstance item = new ItemInstance(data, quantity, null);
                    drops.add(item);
                }
            }
        }
        monster.drops = drops;

        return monster;
    }
}
