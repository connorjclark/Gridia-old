package hoten.gridia.worldgen;

import hoten.gridia.worldgen.DelaunayGraphFactory.DelaunayGraph;
import hoten.gridia.worldgen.VoronoiGraphFactory.VoronoiGraph;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.Toolkit;
import java.io.IOException;
import java.util.List;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Random;
import java.util.Set;
import java.util.stream.Collectors;
import javax.swing.JFrame;
import javax.swing.JPanel;
import javax.swing.WindowConstants;
import static org.junit.Assert.*;
import org.junit.Before;
import org.junit.Test;

public class MapGeneratorIT {

    public static void main(String[] args) throws IOException {
        final MapGenerator mapGenerator = new MapGenerator(2000, 10000, 2);

        Dimension screenSize = Toolkit.getDefaultToolkit().getScreenSize();
        JFrame frame = new JFrame();
        JPanel panel = new JPanel() {
            int i = 0;

            @Override
            public void paint(Graphics g) {

                g.setColor(Color.gray);
                g.fillRect(0, 0, 10000, 10000);

                /*g.setColor(Color.black);
                 drawPoints(g, mapGenerator.voronoiGraph.getPoints());
                 g.setColor(Color.blue);
                 drawPoints(g, mapGenerator.centers.stream().map(e -> e.loc).collect(Collectors.toList()));

                 g.setColor(Color.black);
                 drawEdges(g, mapGenerator.voronoiGraph.getRidges().keySet());
                 drawEdges(g, mapGenerator.voronoiGraph.getRidges().values());*/
                /*mapGenerator.edges.forEach(edge -> {
                 if (edge.d0 != null && edge.d1 != null) {
                 g.setColor(Color.black);
                 drawEdge(g, new Edge(edge.d0.loc, edge.d1.loc));
                 g.setColor(Color.white);
                 drawEdge(g, new Edge(edge.v0.loc, edge.v1.loc));
                 }
                 });

                 g.setColor(Color.red);
                 drawPoints(g, 3, mapGenerator.voronoiGraph.getPoints());

                 g.setColor(Color.blue);
                 drawPoints(g, 2, mapGenerator.corners.stream().map(e -> e.loc).collect(Collectors.toList()));*/
                mapGenerator.paint((Graphics2D) g.create());
            }
        };
        panel.setPreferredSize(screenSize);
        frame.add(panel);
        frame.pack();
        frame.setVisible(true);
        frame.setDefaultCloseOperation(WindowConstants.EXIT_ON_CLOSE);
    }

    private static void drawPoints(Graphics g, int radius, Collection<Vector2> points) {
        points.forEach(point -> {
            drawPoint(g, radius, point);
        });
    }

    private static void drawPoint(Graphics g, int radius, Vector2 point) {
        g.fillOval(point.x - radius, point.y - radius, radius * 2, radius * 2);
    }

    private static void drawEdges(Graphics g, Collection<Edge> edges) {
        edges.forEach(edge -> {
            drawEdge(g, edge);
        });
    }

    private static void drawEdge(Graphics g, Edge edge) {
        Vector2 a = edge.a;
        Vector2 b = edge.b;
        g.drawLine(a.x, a.y, b.x, b.y);
    }

    private static void drawTriangles(Graphics g, Collection<Triangle> triangles) {
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
