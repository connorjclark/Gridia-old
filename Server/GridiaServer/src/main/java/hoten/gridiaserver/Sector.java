package hoten.gridiaserver;

public class Sector {
    private final int _x, _y, _z;
    public final Tile[][] _tiles;
    
    public Sector(Tile[][] tiles, int x, int y, int z) {
        _tiles = tiles;
        _x = x;
        _y = y;
        _z = z;
    }
    
    public Tile getTile(int x, int y) {
        return _tiles[x][y];
    }
}
