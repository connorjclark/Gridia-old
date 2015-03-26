package com.hoten.gridia.map;

import com.hoten.gridia.serializers.GridiaGson;
import java.io.File;
import java.io.IOException;
import org.apache.commons.io.FileUtils;

public class SectorSaver {

    private final File _map;

    public SectorSaver(File map) {
        _map = map;
    }

    public void save(Sector sector) throws IOException {
        String json = GridiaGson.get().toJson(sector._tiles);
        File file = new File(_map, String.format("%d,%d,%d.json", sector.sx, sector.sy, sector.sz));
        FileUtils.writeStringToFile(file, json);
    }
}
