package hoten.gridia.map;

import hoten.gridia.serializers.GridiaGson;
import java.io.File;
import java.io.IOException;
import org.apache.commons.io.FileUtils;

public class JsonSectorLoader implements SectorLoader {

    @Override
    public Sector load(File map, int sectorSize, int x, int y, int z) throws IOException {
        File file = new File(map, String.format("/%d,%d,%d.json", x, y, z));
        String json = FileUtils.readFileToString(file);
        Tile[][] tiles = GridiaGson.get().fromJson(json, Tile[][].class);
        return new Sector(tiles, x, y, z);
    }
}
