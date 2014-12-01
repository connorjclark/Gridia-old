package hoten.gridia.map;

public class Sector {
    public final int sx, sy, sz;
    public final Tile[][] _tiles;
    
    public Sector(Tile[][] tiles, int sx, int sy, int sz) {
        _tiles = tiles;
        this.sx = sx;
        this.sy = sy;
        this.sz = sz;
    }
    
    public Tile getTile(int x, int y) {
        return _tiles[x][y];
    }
}
