package hoten.gridiaserver.map;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import hoten.gridiaserver.content.ContentManager;
import hoten.gridiaserver.content.ItemInstance;
import hoten.gridiaserver.serializers.ItemInstanceDeserializer;
import hoten.gridiaserver.serializers.TileDeserializer;
import hoten.serving.fileutils.FileUtils;
import java.io.File;

public class SectorLoader {

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
