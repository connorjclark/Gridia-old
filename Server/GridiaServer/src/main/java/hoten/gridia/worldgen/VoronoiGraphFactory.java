package hoten.gridia.worldgen;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.nio.file.Files;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashSet;
import java.util.LinkedList;
import java.util.List;
import java.util.Queue;
import java.util.Random;
import java.util.Set;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.util.stream.Collectors;

public class VoronoiGraphFactory {

    public VoronoiGraph create(int numPoints, int bounds, int numLloydRelaxations) throws IOException {
        VoronoiGraph graph = create(randomPoints(numPoints, bounds));

        for (int i = 0; i < numLloydRelaxations; i++) {
            List<Vector2> points = lloydRelaxation(graph);
            graph = create(points);
        }

        return graph;
    }

    private List<Vector2> randomPoints(int numPoints, int bound) {
        List<Vector2> result = new ArrayList<>();
        Random random = new Random();
        for (int i = 0; i < numPoints; i++) {
            result.add(new Vector2(random.nextInt(bound), random.nextInt(bound)));
        }
        return result;
    }

    // http://www.qhull.org/html/qvoronoi.htm
    private VoronoiGraph create(List<Vector2> points) throws IOException {
        File pointsFile = saveAsQhullPoints(points, "../../tmp_voronoi_data");

        List<Vector2> voronoiPoints = new ArrayList<>();
        Set<Edge> edges = new HashSet<>();
        List<List<Vector2>> regions = new ArrayList<>();

        String cmd = String.format("cd bin/qhull && qvoronoi p Fn FN < %s && exit", pointsFile.getAbsolutePath());
        ProcessBuilder builder = new ProcessBuilder("cmd.exe", "/c", cmd)
                .redirectErrorStream(true);

        Queue<String> output = new LinkedList();
        try {
            Process qhull = builder.start();
            BufferedReader in = new BufferedReader(new InputStreamReader(qhull.getInputStream()));
            while (true) {
                String line = in.readLine();
                if (line == null) {
                    break;
                }
                output.add(line);
            }
        } catch (IOException ex) {
            Logger.getLogger(VoronoiGraphFactory.class.getName()).log(Level.SEVERE, null, ex);
        }

        output.remove();

        int numVoronoiPointsLeft = Integer.parseInt(output.remove());

        while (numVoronoiPointsLeft-- > 0) {
            String line = output.remove();
            String[] split = line.trim().split("\\s+");
            int x = (int) (Double.parseDouble(split[0]));
            int y = (int) (Double.parseDouble(split[1]));
            voronoiPoints.add(new Vector2(x, y));
        }

        output.remove();

        voronoiPoints.forEach(a -> {
            String line = output.remove();
            String[] split = line.split(" ");
            Arrays.asList(split).stream().skip(1).forEach(strIndex -> {
                int index = Integer.parseInt(strIndex);
                if (index >= 0) {
                    Vector2 b = voronoiPoints.get(index);
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
                    region.add(voronoiPoints.get(index));
                }
            });

            regions.add(region);
        }

        return new VoronoiGraph(voronoiPoints, edges, regions);
    }

    private File saveAsQhullPoints(List<Vector2> points, String filePath) throws IOException {
        String result = "2\n" + points.size() + "\n" + points.stream().
                map(point -> String.format("%d %d", point.x, point.y))
                .collect(Collectors.joining("\n"));

        File file = new File(filePath);
        Files.write(file.toPath(), result.getBytes());

        return file;
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

        private final List<Vector2> _voronoiPoints;
        private final Set<Edge> _edges;
        private final List<List<Vector2>> _regions;

        public VoronoiGraph(List<Vector2> points, Set<Edge> edges, List<List<Vector2>> regions) {
            _voronoiPoints = points;
            _edges = edges;
            _regions = regions;
        }

        public List<Vector2> getVoronoiPoints() {
            return _voronoiPoints;
        }

        public Set<Edge> getEdges() {
            return _edges;
        }

        public List<List<Vector2>> getRegions() {
            return _regions;
        }
    }
}
