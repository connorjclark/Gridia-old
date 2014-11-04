package hoten.gridiaserver;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.JsonArray;
import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;
import hoten.serving.fileutils.FileUtils;
import java.io.File;
import java.lang.reflect.Type;

public class SectorLoader {

    private final Gson _gson;

    public SectorLoader(ContentManager contentManager) {
        SectorTilesDeserializer tilesDeserializer = new SectorTilesDeserializer(contentManager);
        _gson = new GsonBuilder()
                .registerTypeAdapter(Tile[][].class, tilesDeserializer)
                .create();
    }

    public Sector load(int sectorSize, int x, int y, int z) {
        String path = String.format("TestWorld/json-world/%s,%s,%s.sector", x, y, z);
        String json = FileUtils.readTextFile(new File(path));
        JsonObject jObj = _gson.fromJson("{ tiles : " + json + "}", JsonElement.class).getAsJsonObject(); // :(
        Tile[][] tiles = _gson.fromJson(jObj, Tile[][].class);
        return new Sector(tiles, x, y, z);
    }
}

class SectorTilesDeserializer implements JsonDeserializer<Tile[][]> {

    private final ContentManager _contentManager;

    SectorTilesDeserializer(ContentManager contentManager) {
        _contentManager = contentManager;
    }

    @Override
    public Tile[][] deserialize(final JsonElement json, final Type typeOfT, final JsonDeserializationContext context) throws JsonParseException {
        Gson gson = new Gson();
        JsonObject jsonObject = json.getAsJsonObject();
        JsonArray jsonTiles = jsonObject.get("tiles").getAsJsonArray();
        int size = jsonTiles.size();
        Tile[][] tiles = new Tile[size][size];
        for (int i = 0; i < size; i++) {
            JsonArray jsonTilesInner = jsonTiles.get(0).getAsJsonArray();
            for (int j = 0; j < size; j++) {
                Tile tile = gson.fromJson(jsonTilesInner.get(j), Tile.class);
                int itemType = jsonTilesInner.get(j).getAsJsonObject().get("type").getAsInt();
                int quantity = jsonTilesInner.get(j).getAsJsonObject().get("quantity").getAsInt();
                tile.item = _contentManager.createItemInstance(itemType, quantity);
                tiles[i][j] = tile;
            }
        }
        return tiles;
    }
}
