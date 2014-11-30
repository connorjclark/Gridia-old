package hoten.gridiaserver.serializers;

import com.google.gson.Gson;
import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;
import hoten.gridiaserver.DefaultCreatureImage;
import hoten.gridiaserver.content.Monster;
import java.lang.reflect.Type;

public class MonsterDeserializer implements JsonDeserializer<Monster> {

    @Override
    public Monster deserialize(final JsonElement json, final Type typeOfT, final JsonDeserializationContext context) throws JsonParseException {
        JsonObject jsonObject = json.getAsJsonObject();
        Monster monster = new Monster();
        int spriteIndex = jsonObject.has("image") ? jsonObject.get("image").getAsInt() : 0;
        if (jsonObject.has("image_type")) {
            int imageType = jsonObject.get("image_type").getAsInt();
            if (imageType == 1) {
                monster.image = new DefaultCreatureImage(spriteIndex, 1, 2);
            } else if (imageType == 2) {
                monster.image = new DefaultCreatureImage(spriteIndex - 1, 2, 2);
            }
        } else {
            monster.image = new DefaultCreatureImage(spriteIndex);
        }
        return monster;
    }
}
