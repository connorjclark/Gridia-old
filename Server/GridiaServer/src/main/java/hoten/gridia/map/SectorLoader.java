package hoten.gridia.map;

import java.io.File;

public interface SectorLoader {

    Sector load(File map, int sectorSize, int x, int y, int z);
}
