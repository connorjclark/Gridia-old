package hoten.gridia.map;

import hoten.gridia.serializers.GridiaGson;
import hoten.serving.fileutils.FileUtils;
import java.io.File;

public class JsonSectorLoader implements SectorLoader {

    @Override
    public Sector load(String mapName, int sectorSize, int x, int y, int z) {
        String path = String.format(mapName + "/%d,%d,%d.json", x, y, z);
        String json = FileUtils.readTextFile(new File(path));
        Tile[][] tiles = GridiaGson.get().fromJson(json, Tile[][].class);
        return new Sector(tiles, x, y, z);
    }
}
