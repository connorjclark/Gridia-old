package com.hoten.gridia.map;

import java.io.IOException;

public interface SectorLoader {

    Sector load(int sectorSize, int x, int y, int z) throws IOException;
}
