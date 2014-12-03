package hoten.gridia.worldgen;

import hoten.gridia.worldgen.DelaunayGraphFactory.DelaunayGraph;
import hoten.gridia.worldgen.VoronoiGraphFactory.VoronoiGraph;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Toolkit;
import java.io.IOException;
import java.util.List;
import java.util.ArrayList;
import java.util.Random;
import java.util.Set;
import javax.swing.JFrame;
import javax.swing.JPanel;
import javax.swing.WindowConstants;
import static org.junit.Assert.*;
import org.junit.Before;
import org.junit.Test;

public class GraphBuildIT {

    public static void main(String[] args) throws IOException {
        DelaunayGraphFactory delaunayFactory = new DelaunayGraphFactory();
        VoronoiGraphFactory voronoiFactory = new VoronoiGraphFactory();
        
        VoronoiGraph voronoiGraph = voronoiFactory.create(3000, 2000, 2);

        Dimension screenSize = Toolkit.getDefaultToolkit().getScreenSize();
        final List<Vector2> randomPoints = randomPoints(300, 400);
        final DelaunayGraph delaunayGraph = delaunayFactory.create(randomPoints);
        //final VoronoiGraph voronoiGraph = voronoiFactory.create(delaunayGraph);
        JFrame frame = new JFrame();
        JPanel panel = new JPanel() {
            @Override
            public void paint(Graphics g) {
                //drawPoints(g, randomPoints);
                //drawEdges(g, delaunayGraph.getEdges());
                //drawTriangles(g, delaunayGraph.getTriangles());
                drawEdges(g, voronoiGraph.getEdges());
            }
        };
        panel.setPreferredSize(screenSize);
        frame.add(panel);
        frame.pack();
        frame.setVisible(true);
        frame.setDefaultCloseOperation(WindowConstants.EXIT_ON_CLOSE);
    }

    private static List<Vector2> randomPoints(int amount, int bound) {
        List<Vector2> result = new ArrayList<>();
        Random random = new Random(0);
        for (int i = 0; i < amount; i++) {
            result.add(new Vector2(random.nextInt(bound), random.nextInt(bound)));
        }
        return result;
    }

    private static void drawPoints(Graphics g, List<Vector2> points) {
        int radius = 4;
        g.setColor(Color.BLACK);
        points.forEach(point -> {
            g.fillOval(point.x - radius, point.y - radius, radius * 2, radius * 2);
        });
    }

    private static void drawEdges(Graphics g, Set<Edge> edges) {
        g.setColor(Color.red);
        edges.forEach(edge -> {
            drawEdge(g, edge);
        });
    }

    private static void drawEdge(Graphics g, Edge edge) {
        Vector2 a = edge.a;
        Vector2 b = edge.b;
        g.drawLine(a.x, a.y, b.x, b.y);
    }

    private static void drawTriangles(Graphics g, Set<Triangle> triangles) {
        g.setColor(Color.red);
        triangles.forEach(tri -> {
            drawEdge(g, tri.ab);
            drawEdge(g, tri.bc);
            drawEdge(g, tri.ca);
        });
    }

    @Before
    public void setUp() {
    }

    @Test
    public void testCanary() {
        assertTrue(true);
    }
}
