package hoten.gridia.worldgen;

public class Edge {

    public final Vector2 a, b;

    public Edge(Vector2 a, Vector2 b) {
        this.a = a;
        this.b = b;
    }

    public Vector2 midpoint() {
        int x = (a.x + b.x) / 2;
        int y = (a.y + b.y) / 2;
        return new Vector2(x, y);
    }

    public float slope() {
        int dx = b.x - a.x;
        int dy = b.y - a.y;
        return (float) dy / dx;
    }

    public double length2() {
        int dx = a.x - b.x;
        int dy = a.y - b.y;
        return dx * dx + dy * dy;
    }

    public double length() {
        return Math.sqrt(length2());
    }

    @Override
    public int hashCode() {
        int hash = 23 * (a.hashCode() + b.hashCode());
        return hash;
    }

    @Override
    public boolean equals(Object obj) {
        if (obj == null || getClass() != obj.getClass()) {
            return false;
        }
        final Edge other = (Edge) obj;
        return (a.equals(other.a) && b.equals(other.b))
                || (a.equals(other.b) && b.equals(other.a));
    }

    @Override
    public String toString() {
        return String.format("%s -> %s", a, b);
    }
}
