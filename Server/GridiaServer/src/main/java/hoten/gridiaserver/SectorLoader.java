package hoten.gridiaserver;

import com.google.gson.Gson;
import hoten.serving.fileutils.FileUtils;
import java.io.File;

public class SectorLoader {

    public Sector load(int sectorSize, int x, int y, int z) {
        String path = String.format("TestWorld/json-world/%s,%s,%s.sector", x, y, z);
        String json = FileUtils.readTextFile(new File(path));
        Gson gson = new Gson();
        Tile[][] test = gson.fromJson(json, Tile[][].class);
        return new Sector(test, x, y, z);
    }
}
