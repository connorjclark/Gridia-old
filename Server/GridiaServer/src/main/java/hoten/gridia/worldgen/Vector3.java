package hoten.gridia.worldgen;

public class Vector3 {

    final int x, y, z;

    public Vector3(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector2 vec2() {
        return new Vector2(x, y);
    }

    public Vector3 sub(Vector3 other) {
        return new Vector3(x - other.x, y - other.y, z - other.z);
    }

    public Vector3 cross(Vector3 other) {
        int newX = y * other.z - z * other.y;
        int newY = z * other.x - x * other.z;
        int newZ = x * other.y - y * other.x;
        return new Vector3(newX, newY, newZ);
    }
}
