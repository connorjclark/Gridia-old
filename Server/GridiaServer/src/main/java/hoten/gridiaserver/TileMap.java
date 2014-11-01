package hoten.gridiaserver;

import com.google.gson.Gson;

public class TileMap {

    private final int _size, _depth, _sectorSize, _area, _volume, _sectorsAcross, _sectorsFloor, _sectorsTotal;
    private final Sector[][][] _sectors;
    private final SectorLoader _sectorLoader = new SectorLoader();
    private final SectorSaver _sectorSaver = new SectorSaver();

    public TileMap(int size, int depth, int sectorSize) {
        if (size % sectorSize != 0) {
            throw new IllegalArgumentException("sectorSize must be a factor of size");
        }
        _size = size;
        _depth = depth;
        _sectorSize = sectorSize;
        _area = size * size;
        _volume = _area * depth;
        _sectorsAcross = size / sectorSize;
        _sectorsFloor = _sectorsAcross * _sectorsAcross;
        _sectorsTotal = _sectorsFloor * depth;
        _sectors = new Sector[_sectorsAcross][_sectorsAcross][depth];
    }

    //temporary
    public void loadAll() {
        for (int x = 0; x < _sectorsAcross; x++) {
            for (int y = 0; y < _sectorsAcross; y++) {
                for (int z = 0; z < _depth; z++) {
                    Sector sector = _sectors[x][y][z];
                    if (sector != null) {
                        _sectors[x][y][z] = _sectorLoader.load(_sectorSize, x, y, z);
                    }
                }
            }
        }
    }

    public void save() {
        for (int x = 0; x < _size; x++) {
            for (int y = 0; y < _size; y++) {
                for (int z = 0; z < _depth; z++) {
                    Sector sector = _sectors[x][y][z];
                    if (sector != null) {
                        _sectorSaver.save(sector);
                    }
                }
            }
        }
    }
    
    public String getSectorData(int sx, int sy, int sz) {
        Sector sector = getSector(sx, sy, sz);
        Gson gson = new Gson(); // :(
        return gson.toJson(sector._tiles);
    }
    
    public Sector getSector(int sx, int sy, int sz) {
        Sector sector = _sectors[sx][sy][sz];
        if (sector == null) {
            _sectors[sx][sy][sz] = sector = _sectorLoader.load(_sectorSize, sx, sy, sz);
        }
        return sector;
    }

    public Sector getSectorOf(int x, int y, int z) {
        int sx = x / _sectorSize;
        int sy = y / _sectorSize;
        return getSector(sx, sy, z);
    }

    public Tile getTile(int x, int y, int z) {
        return getSectorOf(x, y, z).getTile(x % _sectorSize, y % _sectorSize);
    }

    public int getFloor(int x, int y, int z) {
        return getTile(x, y, z).floor;
    }

    public int getItem(int x, int y, int z) {
        return getTile(x, y, z).item;
    }
}
