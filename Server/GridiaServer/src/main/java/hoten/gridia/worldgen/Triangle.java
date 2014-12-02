package hoten.gridia.worldgen;

import java.util.Arrays;
import java.util.Collections;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.stream.Collectors;

public class Triangle {

    public final Edge ab, bc, ca;
    public final Vector2 pointA, pointB, pointC;

    public Triangle(Vector2 a, Vector2 b, Vector2 c) {
        pointA = a;
        pointB = b;
        pointC = c;
        ab = new Edge(a, b);
        bc = new Edge(b, c);
        ca = new Edge(c, a);
    }

    public boolean hasCommonEdge(Triangle other) {
        return !Collections.disjoint(Arrays.asList(ab, bc, ca), Arrays.asList(other.ab, other.bc, other.ca));
    }

    public boolean hasEdge(Edge edge) {
        return ab.equals(edge) || bc.equals(edge) || ca.equals(edge);
    }

    // algorithm: calculate intersection of two perpendicular bisectors
    public Vector2 computeCircumcenter() {
        List<Edge> viableEdges = Arrays.asList(ab, bc, ca).stream()
                .filter(edge -> edge.slope() != 0)
                .limit(2)
                .collect(Collectors.toList());

        Edge de = viableEdges.get(0);
        Edge ef = viableEdges.get(1);

        Vector2 deMid = de.midpoint();
        Vector2 efMid = ef.midpoint();

        float deSlope = -1 / de.slope();
        float efSlope = -1 / ef.slope();

        float deIntercept = deMid.y - deSlope * deMid.x;
        float efIntercept = efMid.y - efSlope * efMid.x;

        float x = (deIntercept - efIntercept) / (efSlope - deSlope);
        float y = deSlope * x + deIntercept;

        return new Vector2(Math.round(x), Math.round(y));
    }
}
