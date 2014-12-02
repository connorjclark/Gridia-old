package hoten.gridia.worldgen;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.logging.Level;
import java.util.logging.Logger;

public class VoronoiGraphFactory {

    // http://www.qhull.org/html/qvoronoi.htm
    public VoronoiGraph create(int numPoints, int bounds) {
        List<Vector2> points = new ArrayList<>();
        Set<Edge> edges = new HashSet<>();

        String cmd = String.format("cd bin/qhull && rbox %d D2 z B%d O%d | qvoronoi p Fn && exit", numPoints, bounds / 2, bounds / 2);
        ProcessBuilder builder = new ProcessBuilder("cmd.exe", "/c", cmd)
                .redirectErrorStream(true);

        List<String> output = new ArrayList();
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

        int numVoronoiPoints = Integer.parseInt(output.get(1));
        int size = 1;
        output.subList(2, 2 + numVoronoiPoints).forEach(line -> {
            String[] split = line.trim().split("\\s+");
            int x = (int) (Double.parseDouble(split[0]) * size);
            int y = (int) (Double.parseDouble(split[1]) * size);
            points.add(new Vector2(x, y));
        });

        int vertexIndex = 0;

        for (int i = 3 + numVoronoiPoints; i < 3 + numVoronoiPoints * 2; i++) {
            String line = output.get(i);
            String[] split = line.split(" ");
            Vector2 a = points.get(vertexIndex++);
            Arrays.asList(split).stream().skip(1).forEach(strIndex -> {
                int index = Integer.parseInt(strIndex);
                if (index >= 0) {
                    Vector2 b = points.get(index);
                    edges.add(new Edge(a, b));
                }
            });
        }

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
