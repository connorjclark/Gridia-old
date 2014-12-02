package hoten.gridia.worldgen;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import org.junit.Test;
import static org.junit.Assert.*;

public class TriangleTest {

    // http://www.wolframalpha.com/input/?i=circumcenter+of+a+triangle+%2810%2C10%29+%280%2C10%29+%2810%2C0%29
    @Test
    public void testComputeCircumcenter() {
        List<Vector2> points = Arrays.asList(
                new Vector2(10, 10), new Vector2(0, 10), new Vector2(10, 0),
                new Vector2(20, 10), new Vector2(30, 40), new Vector2(10, 30),
                new Vector2(57, 87), new Vector2(65, 32), new Vector2(45, 87)
        );

        List<Vector2> circumcenters = Arrays.asList(
                new Vector2(5, 5),
                new Vector2(25, 25),
                new Vector2(51, 58)
        );

        List<Triangle> triangles = new ArrayList<>();
        for (int i = 0; i < points.size() / 3; i++) {
            Vector2 a = points.get(i * 3);
            Vector2 b = points.get(i * 3 + 1);
            Vector2 c = points.get(i * 3 + 2);
            triangles.add(new Triangle(a, b, c));
        }

        for (int i = 0; i < triangles.size(); i++) {
            Triangle triangle = triangles.get(i);
            assertEquals(circumcenters.get(i), triangle.computeCircumcenter());
        }
    }
}
