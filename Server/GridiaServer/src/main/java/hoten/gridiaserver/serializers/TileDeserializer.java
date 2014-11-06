package hoten.gridiaserver.serializers;

import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;
import hoten.gridiaserver.content.ItemInstance;
import hoten.gridiaserver.map.Tile;
import java.lang.reflect.Type;

public class TileDeserializer implements JsonDeserializer<Tile> {

    @Override
    public Tile deserialize(final JsonElement json, final Type typeOfT, final JsonDeserializationContext context) throws JsonParseException {
        JsonObject jsonObject = json.getAsJsonObject();
        Tile tile = new Tile();
        tile.floor = jsonObject.get("floor").getAsInt();
        tile.item = context.deserialize(jsonObject, ItemInstance.class);
        return tile;
    }
}
