package hoten.gridia.worldgen;

public class Vector2 {

    public final int x, y;

    public Vector2(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public Vector3 vec3() {
        return new Vector3(x, y, 0);
    }

    @Override
    public int hashCode() {
        int hash = 7 * x + y * 23;
        return hash;
    }

    @Override
    public boolean equals(Object obj) {
        if (obj == null || getClass() != obj.getClass()) {
            return false;
        }
        final Vector2 other = (Vector2) obj;
        if (this.x != other.x) {
            return false;
        }
        return this.y == other.y;
    }

    @Override
    public String toString() {
        return String.format("(%d, %d)", x, y);
    }
}
