package hoten.gridia.serializers;

import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;
import hoten.gridia.Creature;
import hoten.gridia.content.ItemInstance;
import hoten.gridia.map.Tile;
import hoten.gridia.serving.ServingGridia;
import java.lang.reflect.Type;

public class TileDeserializer implements JsonDeserializer<Tile> {

    private final ServingGridia _servingGridia;

    // :( make creature factory
    public TileDeserializer(ServingGridia servingGridia) {
        _servingGridia = servingGridia;
    }

    @Override
    public Tile deserialize(final JsonElement json, final Type typeOfT, final JsonDeserializationContext context) throws JsonParseException {
        JsonObject jsonObject = json.getAsJsonObject();
        Tile tile = new Tile();
        tile.floor = jsonObject.get("floor").getAsInt();
        tile.item = context.deserialize(jsonObject.get("item"), ItemInstance.class);
        if (jsonObject.has("cre") && false) {
            Creature cre = context.deserialize(jsonObject.get("cre"), Creature.class);
            tile.cre = _servingGridia.createCreatureQuietly(cre.image, cre.name, cre.location);
            tile.cre.isFriendly = cre.isFriendly;
            tile.cre.friendlyMessage = cre.friendlyMessage;
            tile.cre.life = cre.life;
        }
        return tile;
    }
}
