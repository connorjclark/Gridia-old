package hoten.gridia.map;

import hoten.gridia.serializers.GridiaGson;
import hoten.serving.fileutils.FileUtils;
import java.io.File;

public class JsonSectorLoader implements SectorLoader {

    @Override
    public Sector load(File map, int sectorSize, int x, int y, int z) {
        String json = FileUtils.readTextFile(new File(map, String.format("/%d,%d,%d.json", x, y, z)));
        Tile[][] tiles = GridiaGson.get().fromJson(json, Tile[][].class);
        return new Sector(tiles, x, y, z);
    }
}
