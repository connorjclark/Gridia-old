package hoten.gridia.map;

import java.io.File;
import java.io.IOException;

public interface SectorLoader {

    Sector load(File map, int sectorSize, int x, int y, int z) throws IOException;
}
