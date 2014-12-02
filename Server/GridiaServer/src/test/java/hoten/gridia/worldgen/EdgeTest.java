package hoten.gridia.worldgen;

import org.junit.Test;
import static org.junit.Assert.*;

public class EdgeTest {

    public EdgeTest() {
    }

    @Test
    public void testMidpoint() {
        assertEquals(new Vector2(5, 5), new Edge(new Vector2(0, 0), new Vector2(10, 10)).midpoint());
    }

    @Test
    public void testSlope() {
        assertEquals(2, new Edge(new Vector2(0, 0), new Vector2(5, 10)).slope(), 0.0001);
    }
}
