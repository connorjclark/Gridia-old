package hoten.gridia.worldgen;

import hoten.gridia.worldgen.DelaunayGraphFactory.DelaunayGraph;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

public class VoronoiGraphFactory {

    public VoronoiGraph create(DelaunayGraph delaunayGraph) {
        Set<Edge> edges = new HashSet<>();
        Set<Vector2> points = new HashSet<>();

        //Set<Triangle> triangles = delaunayGraph.getTriangles();
        Map<Edge, List<Triangle>> delaunayEdgeTrianglesMap = delaunayGraph.getEdgeTriangleMap();

        delaunayGraph.getEdges().forEach(delaunayEdge -> {
            List<Triangle> neighbors = delaunayEdgeTrianglesMap.get(delaunayEdge);

            if (neighbors.size() == 1) {
                // ...
            } else if (neighbors.size() == 2) {
                Vector2 a = neighbors.get(0).computeCircumcenter();
                Vector2 b = neighbors.get(1).computeCircumcenter();
                edges.add(new Edge(a, b));
            }
        });

        return new VoronoiGraph(edges);
    }

    public static class VoronoiGraph {

        private final Set<Edge> _edges;

        public VoronoiGraph(Set<Edge> edges) {
            _edges = edges;
        }

        public Set<Edge> getEdges() {
            return _edges;
        }
    }
}
