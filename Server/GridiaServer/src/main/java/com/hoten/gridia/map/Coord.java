package com.hoten.gridia.map;

public class Coord {

    public final int x, y, z;

    public Coord(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Coord add(int x1, int y1, int z1) {
        return new Coord(x + x1, y + y1, z + z1);
    }

    public Coord add(Coord other) {
        return new Coord(x + other.x, y + other.y, z + other.z);
    }

    @Override
    public String toString() {
        return String.format("%s, %s, %s", x, y, z);
    }

    @Override
    public boolean equals(Object other) {
        if (!(other instanceof Coord)) {
            return false;
        }
        Coord otherCoord = (Coord) other;
        return otherCoord.x == x && otherCoord.y == y && otherCoord.z == z;
    }

    @Override
    public int hashCode() {
        int hash = 7;
        hash = 89 * hash + this.x;
        hash = 89 * hash + this.y;
        hash = 89 * hash + this.z;
        return hash;
    }
}
