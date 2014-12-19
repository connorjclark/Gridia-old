package hoten.gridia.serializers;

import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;
import hoten.gridia.Player;
import java.lang.reflect.Type;

public class PlayerSerializer implements JsonSerializer<Player> {

    @Override
    public JsonElement serialize(Player player, Type type, JsonSerializationContext jsc) {
        JsonObject obj = new JsonObject();
        obj.addProperty("passwordHash", player.passwordHash);
        obj.addProperty("inventory", player.creature.inventory.id);
        obj.addProperty("equipment", player.equipment.id);
        return obj;
    }
}
