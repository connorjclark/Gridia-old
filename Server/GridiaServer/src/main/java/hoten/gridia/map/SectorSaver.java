package hoten.gridia.map;

import hoten.gridia.serializers.GridiaGson;
import hoten.serving.fileutils.FileUtils;
import java.io.File;

public class SectorSaver {

    public void save(File map, Sector sector) {
        String json = GridiaGson.get().toJson(sector._tiles);
        File file = new File(map, String.format("/%d,%d,%d.json", sector.sx, sector.sy, sector.sz));
        FileUtils.saveAs(file, json.getBytes());
    }
}
