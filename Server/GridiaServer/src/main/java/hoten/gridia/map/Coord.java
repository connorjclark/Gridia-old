package hoten.gridia.map;

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
}
