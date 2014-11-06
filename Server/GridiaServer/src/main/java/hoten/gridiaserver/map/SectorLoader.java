package hoten.gridiaserver.map;

import hoten.gridiaserver.serializers.GridiaGson;
import hoten.serving.fileutils.FileUtils;
import java.io.File;

public class SectorLoader {

    public Sector load(int sectorSize, int x, int y, int z) {
        String path = String.format("TestWorld/json-world/%s,%s,%s.sector", x, y, z);
        String json = FileUtils.readTextFile(new File(path));
        Tile[][] tiles = GridiaGson.get().fromJson(json, Tile[][].class);
        return new Sector(tiles, x, y, z);
    }
}
