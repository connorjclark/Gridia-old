package hoten.gridia.serializers;

import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.map.Tile;
import java.lang.reflect.Type;

public class TileDeserializer implements JsonDeserializer<Tile> {

    @Override
    public Tile deserialize(final JsonElement json, final Type typeOfT, final JsonDeserializationContext context) throws JsonParseException {
        JsonObject jsonObject = json.getAsJsonObject();
        Tile tile = new Tile();
        tile.floor = jsonObject.get("floor").getAsInt();
        tile.item = context.deserialize(jsonObject.get("item"), ItemInstance.class);
        return tile;
    }
}
