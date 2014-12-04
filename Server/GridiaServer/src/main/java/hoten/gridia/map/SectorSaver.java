package hoten.gridia.map;

import hoten.gridia.serializers.GridiaGson;
import hoten.serving.fileutils.FileUtils;
import java.io.File;

public class SectorSaver {

    public void save(Sector sector) {
        String path = String.format("TestWorld/json-world/%d,%d,%d.sector", sector.sx, sector.sy, sector.sz);
        String json = GridiaGson.get().toJson(sector._tiles);
        FileUtils.saveAs(new File(path), json.getBytes());
    }
}
