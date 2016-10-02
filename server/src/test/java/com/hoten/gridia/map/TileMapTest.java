package com.hoten.gridia.map;

import java.io.File;
import org.junit.Before;
import org.junit.Test;
import static org.junit.Assert.*;

public class TileMapTest {

    private TileMap _tileMap;

    @Before
    public void setUp() {
        int size = 100;
        int depth = 1;
        int sectorSize = 20;
        SectorLoader sectorLoader = (int sectorSize1, int x, int y, int z) -> null;
        SectorSaver sectorSaver = new SectorSaver(null);
        _tileMap = new TileMap(size, depth, sectorSize, sectorLoader, sectorSaver);
    }

    @Test
    public void testWrapValueWhenGreaterThanSize() {
        assertEquals(10, _tileMap.wrap(110));
    }

    @Test
    public void testWrapValueWhenGreaterThanSizeByAlot() {
        assertEquals(10, _tileMap.wrap(1110));
    }

    @Test
    public void testWrapValueWhenNegative() {
        assertEquals(90, _tileMap.wrap(-10));
    }

    @Test
    public void testWrapValueWhenNegativeByAlot() {
        assertEquals(90, _tileMap.wrap(-510));
    }
}
