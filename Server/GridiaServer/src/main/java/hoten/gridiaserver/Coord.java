package hoten.gridiaserver;

public class Coord {

    public int x, y, z;

    public Coord() {
    }

    public Coord(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public void set(int x1, int y1, int z1) {
        x = x1;
        y = y1;
        z = z1;
    }
}
