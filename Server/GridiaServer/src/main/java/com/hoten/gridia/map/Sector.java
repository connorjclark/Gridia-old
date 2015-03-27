package com.hoten.gridia.map;

import static com.hoten.gridia.map.Tile.OWNER_UNCLAIMED;

public class Sector {

    public final int sx, sy, sz;
    public final Tile[][] _tiles;
    private int _owner;

    public Sector(Tile[][] tiles, int sx, int sy, int sz) {
        _tiles = tiles;
        this.sx = sx;
        this.sy = sy;
        this.sz = sz;
    }

    public boolean isUnclaimed() {
        return _owner == OWNER_UNCLAIMED;
    }

    public int getOwner() {
        return _owner;
    }

    public void setOwner(int owner) {
        _owner = owner;
    }

    public Tile getTile(int x, int y) {
        return _tiles[x][y];
    }
}
