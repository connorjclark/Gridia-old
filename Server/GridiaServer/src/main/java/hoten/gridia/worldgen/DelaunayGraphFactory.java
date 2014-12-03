package hoten.gridia.worldgen;

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Queue;
import java.util.Set;
import java.util.stream.Collectors;

public class DelaunayGraphFactory {

    public DelaunayGraph create(List<Vector2> points) throws IOException {
        Set<Edge> edges = new HashSet<>();
        Set<Triangle> triangles = new HashSet<>();
        Map<Edge, List<Triangle>> edgeTrianglesMap = new HashMap<>();

        QhullBridge qhull = new QhullBridge();
        File pointsFile = qhull.saveAsQhullPoints(points, "../../tmp_voronoi_data");

        String cmd = String.format("qdelaunay i FN < %s", pointsFile.getAbsolutePath());
        Queue<String> output = qhull.runQhull(cmd);
                
        int numTriangles = Integer.parseInt(output.remove());

        for (int i = 0; i < numTriangles; i++) {
            String[] split = output.remove().split("\\s+");
            Vector2 a = points.get(Integer.parseInt(split[0]));
            Vector2 b = points.get(Integer.parseInt(split[1]));
            Vector2 c = points.get(Integer.parseInt(split[2]));

            Triangle triangle = new Triangle(a, b, c);
            triangles.add(triangle);

            edges.add(triangle.ab);
            edges.add(triangle.bc);
            edges.add(triangle.ca);

            addToEdgeTriangleMap(edgeTrianglesMap, triangle.ab, triangle);
            addToEdgeTriangleMap(edgeTrianglesMap, triangle.bc, triangle);
            addToEdgeTriangleMap(edgeTrianglesMap, triangle.ca, triangle);
        }

        //return null;
        return new DelaunayGraph(edges, triangles, edgeTrianglesMap, null);
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
