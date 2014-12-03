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

public class VoronoiGraphFactory {

    public VoronoiGraph create(List<Vector2> points, int numLloydRelaxations) throws IOException {
        VoronoiGraph graph = create(points);

        for (int i = 0; i < numLloydRelaxations; i++) {
            points = lloydRelaxation(graph);
            graph = create(points);
        }

        return graph;
    }

    public VoronoiGraph create(List<Vector2> points) throws IOException {
        QhullBridge qhull = new QhullBridge();
        File pointsFile = qhull.saveAsQhullPoints(points, "../../tmp_voronoi_data");

        List<Vector2> voronoiCenters = new ArrayList<>();
        Set<Edge> edges = new HashSet<>();
        List<List<Vector2>> regions = new ArrayList<>();
        Map<Edge, Edge> ridges = new HashMap<>(); // delaunay edge -> voronoi edge

        String cmd = String.format("qvoronoi p Fn FN Fv < %s", pointsFile.getAbsolutePath());
        //String cmd = String.format("qvoronoi Fv < %s", pointsFile.getAbsolutePath());
        Queue<String> output = qhull.runQhull(cmd);

        output.remove();

        int numVoronoiPointsLeft = Integer.parseInt(output.remove());
        System.out.println("numVoronoiPointsLeft = " + numVoronoiPointsLeft);

        while (numVoronoiPointsLeft-- > 0) {
            String line = output.remove();
            String[] split = line.trim().split("\\s+");
            int x = (int) (Double.parseDouble(split[0]));
            int y = (int) (Double.parseDouble(split[1]));
            voronoiCenters.add(new Vector2(x, y));
        }

        output.remove();

        voronoiCenters.forEach(a -> {
            String line = output.remove();
            String[] split = line.split(" ");
            Arrays.asList(split).stream().skip(1).forEach(strIndex -> {
                int index = Integer.parseInt(strIndex);
                if (index >= 0) {
                    Vector2 b = voronoiCenters.get(index);
                    edges.add(new Edge(a, b));
                }
            });
        });

        int numRegionsLeft = Integer.parseInt(output.remove());

        while (numRegionsLeft-- > 0) {
            List<Vector2> region = new ArrayList<>();

            String line = output.remove();
            String[] split = line.split(" ");
            Arrays.asList(split).stream().skip(1).forEach(strIndex -> {
                int index = Integer.parseInt(strIndex);
                if (index >= 0) {
                    region.add(voronoiCenters.get(index));
                }
            });

            regions.add(region);
        }

        int numRidges = Integer.parseInt(output.remove());
        for (int i = 0; i < numRidges; i++) {
            String line = output.remove();
            String[] split = line.split("\\s+");

            Vector2 pointA = points.get(Integer.parseInt(split[1]));
            Vector2 pointB = points.get(Integer.parseInt(split[2]));

            int indexA = Integer.parseInt(split[3]);
            int indexB = Integer.parseInt(split[4]);

            if (indexA != 0 && indexB != 0) {
                Vector2 voronoiCenterA = voronoiCenters.get(Integer.parseInt(split[3]) - 1);
                Vector2 voronoiCenterB = voronoiCenters.get(Integer.parseInt(split[4]) - 1);
                ridges.put(new Edge(pointA, pointB), new Edge(voronoiCenterA, voronoiCenterB));
            }
        }
        System.out.println("numRidgesLeft = " + numRidges);

        return new VoronoiGraph(points, voronoiCenters, edges, regions, ridges);
    }

    private List<Vector2> lloydRelaxation(VoronoiGraph graph) throws IOException {
        List<Vector2> newPoints = new ArrayList<>();

        graph.getRegions().forEach(region -> {
            if (!region.isEmpty()) {
                Vector2 center = region.stream().reduce((acc, point) -> {
                    return point.add(acc);
                }).get().div(region.size());
                newPoints.add(center);
            } else {
                newPoints.add(new Vector2(0, 0));
            }
        });

        return newPoints;
    }

    public static class VoronoiGraph {

        private final List<Vector2> _points;
        private final List<Vector2> _voronoiCenters;
        private final Set<Edge> _edges;
        private final List<List<Vector2>> _regions;
        private final Map<Edge, Edge> _ridges;

        public VoronoiGraph(List<Vector2> points, List<Vector2> voronoiCenters, Set<Edge> edges, List<List<Vector2>> regions, Map<Edge, Edge> ridges) {
            _points = points;
            _voronoiCenters = voronoiCenters;
            _edges = edges;
            _regions = regions;
            _ridges = ridges;
        }

        public List<Vector2> getPoints() {
            return _points;
        }

        public List<Vector2> getVoronoiCenters() {
            return _voronoiCenters;
        }

        public Set<Edge> getEdges() {
            return _edges;
        }

        public List<List<Vector2>> getRegions() {
            return _regions;
        }

        public Map<Edge, Edge> getRidges() {
            return _ridges;
        }
    }
}
