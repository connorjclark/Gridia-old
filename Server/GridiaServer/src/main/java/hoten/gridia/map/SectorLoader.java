package hoten.gridia.map;

public interface SectorLoader {

    Sector load(String mapName, int sectorSize, int x, int y, int z);
}
