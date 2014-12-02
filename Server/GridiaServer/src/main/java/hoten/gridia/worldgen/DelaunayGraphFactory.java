package hoten.gridia.worldgen;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.stream.Collectors;

public class DelaunayGraphFactory {

    public DelaunayGraph create(List<Vector2> points) {
        int n = points.size();
        int[][] adjMatrix = new int[n][n];
        List<Vector3> expandedPoints = expandPoints(points);

        Set<Edge> edges = new HashSet<>();
        Set<Triangle> triangles = new HashSet<>();
        Map<Edge, List<Triangle>> edgeTrianglesMap = new HashMap<>();

        if (n == 2) {
            adjMatrix[0][1] = 1;
            adjMatrix[1][0] = 1;
            Vector3 first = expandedPoints.get(0);
            Vector3 second = expandedPoints.get(1);
            edges.add(new Edge(first.vec2(), second.vec2()));
            return new DelaunayGraph(edges, triangles, edgeTrianglesMap, adjMatrix);
        }

        for (int i = 0; i < n - 2; i++) {
            for (int j = i + 1; j < n; j++) {
                for (int k = i + 1; k < n; k++) {
                    if (j == k) {
                        continue;
                    }

                    Vector3 iP = expandedPoints.get(i);
                    Vector3 jP = expandedPoints.get(j);
                    Vector3 kP = expandedPoints.get(k);

                    Vector3 ij = iP.sub(jP);
                    Vector3 ik = iP.sub(kP);
                    Vector3 cross = ij.cross(ik);

                    boolean flag = cross.z < 0 && expandedPoints.stream()
                            .limit(n)
                            .allMatch(mP -> {
                                return (mP.x - iP.x) * cross.x + (mP.y - iP.y) * cross.y + (mP.z - iP.z) * cross.z <= 0;
                            });

                    if (!flag) {
                        continue;
                    }

                    Triangle triangle = new Triangle(iP.vec2(), jP.vec2(), kP.vec2());
                    triangles.add(triangle);

                    edges.add(triangle.ab);
                    edges.add(triangle.bc);
                    edges.add(triangle.ca);

                    addToEdgeTriangleMap(edgeTrianglesMap, triangle.ab, triangle);
                    addToEdgeTriangleMap(edgeTrianglesMap, triangle.bc, triangle);
                    addToEdgeTriangleMap(edgeTrianglesMap, triangle.ca, triangle);

                    adjMatrix[i][j] = 1;
                    adjMatrix[j][i] = 1;
                    adjMatrix[k][i] = 1;
                    adjMatrix[i][k] = 1;
                    adjMatrix[j][k] = 1;
                    adjMatrix[k][j] = 1;
                }
            }
        }

        return new DelaunayGraph(edges, triangles, edgeTrianglesMap, adjMatrix);
    }

    private void addToEdgeTriangleMap(Map<Edge, List<Triangle>> edgeTriangleMap, Edge edge, Triangle triangle) {
        List<Triangle> triangles = edgeTriangleMap.get(edge);
        if (triangles == null) {
            triangles = new ArrayList<>();
            edgeTriangleMap.put(edge, triangles);
        }
        triangles.add(triangle);
    }

    private List<Vector3> expandPoints(List<Vector2> points) {
        return points.stream()
                .map(p -> new Vector3(p.x, p.y, p.x * p.x + p.y * p.y))
                .collect(Collectors.toList());
    }

    public static class DelaunayGraph {

        private final Set<Triangle> _triangles;
        private final Set<Edge> _edges;
        private final Map<Edge, List<Triangle>> _edgeTriangleMap;
        private final int[][] _adjMatrix;

        private DelaunayGraph(Set<Edge> graph, Set<Triangle> triangles, Map<Edge, List<Triangle>> edgeTriangleMap, int[][] adjMatrix) {
            _edges = graph;
            _triangles = triangles;
            _edgeTriangleMap = edgeTriangleMap;
            _adjMatrix = adjMatrix;
        }

        public Set<Edge> getEdges() {
            return _edges;
        }

        public Set<Triangle> getTriangles() {
            return _triangles;
        }

        public Map<Edge, List<Triangle>> getEdgeTriangleMap() {
            return _edgeTriangleMap;
        }

        public int[][] getAdjMatrix() {
            return _adjMatrix;
        }
    }
}
