package hoten.gridia.map;

import hoten.gridia.serializers.GridiaGson;
import java.io.File;
import java.io.IOException;
import org.apache.commons.io.FileUtils;

public class SectorSaver {

    public void save(File map, Sector sector) throws IOException {
        String json = GridiaGson.get().toJson(sector._tiles);
        File file = new File(map, String.format("/%d,%d,%d.json", sector.sx, sector.sy, sector.sz));
        FileUtils.writeStringToFile(file, json);
    }
}
