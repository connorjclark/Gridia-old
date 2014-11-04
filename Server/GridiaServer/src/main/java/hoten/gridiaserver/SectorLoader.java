package hoten.gridiaserver;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.JsonArray;
import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;
import hoten.serving.fileutils.FileUtils;
import java.io.File;
import java.lang.reflect.Type;

public class SectorLoader {

    public static class ItemInstanceDeserializer implements JsonDeserializer<ItemInstance> {

        private final ContentManager _contentManager;

        ItemInstanceDeserializer(ContentManager contentManager) {
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

    public static class ItemInstanceSerializer implements JsonSerializer<ItemInstance> {

        @Override
        public JsonElement serialize(ItemInstance item, Type type, JsonSerializationContext jsc) {
            JsonObject obj = new JsonObject();
            obj.addProperty("type", item.data.id);
            obj.addProperty("quantity", item.quantity);
            return obj;
        }
    }

    public static class TileDeserializer implements JsonDeserializer<Tile> {

        @Override
        public Tile deserialize(final JsonElement json, final Type typeOfT, final JsonDeserializationContext context) throws JsonParseException {
            JsonObject jsonObject = json.getAsJsonObject();
            Tile tile = new Tile();
            tile.floor = jsonObject.get("floor").getAsInt();
            tile.item = context.deserialize(jsonObject, ItemInstance.class);
            return tile;
        }
    }

    private final Gson _gson;

    public SectorLoader(ContentManager contentManager) {
        _gson = new GsonBuilder()
                .registerTypeAdapter(ItemInstance.class, new ItemInstanceDeserializer(contentManager))
                .registerTypeAdapter(Tile.class, new TileDeserializer())
                .create();
    }

    public Sector load(int sectorSize, int x, int y, int z) {
        String path = String.format("TestWorld/json-world/%s,%s,%s.sector", x, y, z);
        String json = FileUtils.readTextFile(new File(path));
        Tile[][] tiles = _gson.fromJson(json, Tile[][].class);
        return new Sector(tiles, x, y, z);
    }
}
