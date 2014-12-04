package hoten.gridia.map;

import hoten.gridia.serializers.GridiaGson;
import hoten.serving.fileutils.FileUtils;
import java.io.File;

public class SectorSaver {

    public void save(String mapName, Sector sector) {
        String path = String.format(mapName + "/%d,%d,%d.json", sector.sx, sector.sy, sector.sz);
        String json = GridiaGson.get().toJson(sector._tiles);
        FileUtils.saveAs(new File(path), json.getBytes());
    }
}
