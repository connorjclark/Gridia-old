package hoten.gridia.worldgen;

import hoten.gridia.worldgen.DelaunayGraphFactory.DelaunayGraph;
import hoten.gridia.worldgen.VoronoiGraphFactory.VoronoiGraph;
import java.awt.BasicStroke;
import java.awt.Color;
import java.awt.Graphics2D;
import java.awt.Rectangle;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.HashSet;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Random;
import java.util.Set;
import java.util.stream.Collectors;

public class MapGenerator {

    public VoronoiGraph voronoiGraph;
    public DelaunayGraph delaunayGraph;
    public List<Center> centers = new ArrayList<>();
    public List<Corner> corners = new ArrayList<>();
    public List<DoubleEdge> edges = new ArrayList<>();
    public int size;

    public MapGenerator(int size, int numPoints, int numLloydRelaxations) throws IOException {
        this.size = size;
        generate(size, numPoints, numLloydRelaxations);
    }

    private void generate(int size, int numPoints, int numLloydRelaxations) throws IOException {
        List<Vector2> points = randomPoints(numPoints, size);
        voronoiGraph = new VoronoiGraphFactory().create(points, numLloydRelaxations);
        //delaunayGraph = new DelaunayGraphFactory().create(voronoiGraph.getPoints());

        Map<Edge, Edge> ridges = voronoiGraph.getRidges();

        Map<Vector2, Corner> pointCornerMap = new HashMap<>();
        Map<Vector2, Center> pointCenterMap = new HashMap<>();

        // needed?
        voronoiGraph.getPoints().forEach(point -> {
            Center center = new Center(point);
            center.index = centers.size();
            centers.add(center);
            pointCenterMap.put(point, new Center(point));
        });

        ridges.forEach((delaunayEdge, voronoiEdge) -> {
            DoubleEdge edge = new DoubleEdge();

            edges.add(edge);

            edge.v0 = makeCorner(pointCornerMap, voronoiEdge.a);
            edge.v1 = makeCorner(pointCornerMap, voronoiEdge.b);
            edge.d0 = pointCenterMap.get(delaunayEdge.a);
            edge.d1 = pointCenterMap.get(delaunayEdge.b);

            // Centers point to edges. Corners point to edges.
            if (edge.d0 != null) {
                edge.d0.borders.add(edge);
            }
            if (edge.d1 != null) {
                edge.d1.borders.add(edge);
            }
            if (edge.v0 != null) {
                edge.v0.protrudes.add(edge);
            }
            if (edge.v1 != null) {
                edge.v1.protrudes.add(edge);
            }

            // Centers point to centers.
            if (edge.d0 != null && edge.d1 != null) {
                edge.d0.neighbors.add(edge.d1);
                edge.d1.neighbors.add(edge.d0);
            }

            // Corners point to corners
            if (edge.v0 != null && edge.v1 != null) {
                edge.v0.adjacent.add(edge.v1);
                edge.v1.adjacent.add(edge.v0);
            }

            // Centers point to corners
            if (edge.d0 != null) {
                edge.d0.corners.add(edge.v0);
                edge.d0.corners.add(edge.v1);
            }
            if (edge.d1 != null) {
                edge.d1.corners.add(edge.v0);
                edge.d1.corners.add(edge.v1);
            }

            // Corners point to centers
            if (edge.v0 != null) {
                edge.v0.touches.add(edge.d0);
                edge.v0.touches.add(edge.d1);
            }
            if (edge.v1 != null) {
                edge.v1.touches.add(edge.d0);
                edge.v1.touches.add(edge.d1);
            }

        });

        /*
         Center - Center : delanauy edge
        
         Corner - Corner : voronoi edge
        
         Center
        
         -neighbors (adjacent polygons)
         -borders (bordering edges)
         -corners (polygon corners)
        
         Edge
        
         -d0, d1 (polygons connected by the delaunay edge)
         -v0, v1 (corners connected by the voronoi edge)
        
         Corner (voronoi corner)
        
         -touches (polygons touching)
         -protrudes (edges touching)
         -adjacent (connected corners)
        
         */
        improveCorners();
        assignCornerElevations();
        assignOceanCoastAndLand();
        redistributeElevations(corners.stream().filter(c -> !c.ocean && !c.coast).collect(Collectors.toList()));
        assignPolygonElevations();
        calculateDownslopes();
        createRivers();
        assignCornerMoisture();
        redistributeMoisture(corners.stream().filter(c -> !c.ocean && !c.coast).collect(Collectors.toList()));
        assignPolygonMoisture();
        assignBiomes();
    }

    private void drawTriangle(Graphics2D g, Corner c1, Corner c2, Center center) {
        int[] x = new int[3];
        int[] y = new int[3];
        x[0] = (int) center.loc.x;
        y[0] = (int) center.loc.y;
        x[1] = (int) c1.loc.x;
        y[1] = (int) c1.loc.y;
        x[2] = (int) c2.loc.x;
        y[2] = (int) c2.loc.y;
        g.fillPolygon(x, y, 3);
    }

    private boolean closeEnough(double d1, double d2, double diff) {
        return Math.abs(d1 - d2) <= diff;
    }

    private DoubleEdge edgeWithCenters(Center c1, Center c2) {
        for (DoubleEdge e : c1.borders) {
            if (e.d0 == c2 || e.d1 == c2) {
                return e;
            }
        }
        return null;
    }

    public void paint(Graphics2D g) {
        paint(g, false, true, true, false, false, true);
    }

    //also records the area of each voronoi cell
    public void paint(Graphics2D g, boolean drawBiomes, boolean drawRivers, boolean drawSites, boolean drawCorners, boolean drawDelaunay, boolean drawVoronoi) {
        Random r = new Random();

        final int numSites = centers.size();
        Color[] defaultColors = null;
        if (!drawBiomes) {
            defaultColors = new Color[numSites];
            for (int i = 0; i < defaultColors.length; i++) {
                defaultColors[i] = new Color(r.nextInt(255), r.nextInt(255), r.nextInt(255));
            }
        }
//draw via triangles
        for (Center c : centers) {
            g.setColor(defaultColors[c.index]);
//only used if Center c is on the edge of the graph. allows for completely filling in the outer polygons
            Corner edgeCorner1 = null;
            Corner edgeCorner2 = null;
            for (Center n : c.neighbors) {
                DoubleEdge e = edgeWithCenters(c, n);
                if (e.v0 == null) {
//outermost voronoi edges aren't stored in the graph
                    continue;
                }
//find a corner on the exterior of the graph
//if this Edge e has one, then it must have two,
//finding these two corners will give us the missing
//triangle to render. this special triangle is handled
//outside this for loop
                Corner cornerWithOneAdjacent = e.v0.border ? e.v0 : e.v1;
                if (cornerWithOneAdjacent.border) {
                    if (edgeCorner1 == null) {
                        edgeCorner1 = cornerWithOneAdjacent;
                    } else {
                        edgeCorner2 = cornerWithOneAdjacent;
                    }
                }
                drawTriangle(g, e.v0, e.v1, c);
            }
//handle the missing triangle
            if (edgeCorner2 != null) {
//if these two outer corners are NOT on the same exterior edge of the graph,
//then we actually must render a polygon (w/ 4 points) and take into consideration
//one of the four corners (either 0,0 or 0,height or width,0 or width,height)
//note: the 'missing polygon' may have more than just 4 points. this
//is common when the number of sites are quite low (less than 5), but not a problem
//with a more useful number of sites.
//TODO: find a way to fix this
                if (closeEnough(edgeCorner1.loc.x, edgeCorner2.loc.x, 1)) {
                    drawTriangle(g, edgeCorner1, edgeCorner2, c);
                } else {
                    int[] x = new int[4];
                    int[] y = new int[4];
                    x[0] = (int) c.loc.x;
                    y[0] = (int) c.loc.y;
                    x[1] = (int) edgeCorner1.loc.x;
                    y[1] = (int) edgeCorner1.loc.y;
//determine which corner this is
                    x[2] = (int) ((closeEnough(edgeCorner1.loc.x, 0, 1) || closeEnough(edgeCorner2.loc.x, 0, .5)) ? 0 : size);
                    y[2] = (int) ((closeEnough(edgeCorner1.loc.y, 0, 1) || closeEnough(edgeCorner2.loc.y, 0, .5)) ? 0 : size);
                    x[3] = (int) edgeCorner2.loc.x;
                    y[3] = (int) edgeCorner2.loc.y;
                    g.fillPolygon(x, y, 4);
                }
            }
        }
        for (DoubleEdge e : edges) {
            if (drawDelaunay) {
                g.setStroke(new BasicStroke(1));
                g.setColor(Color.YELLOW);
                g.drawLine((int) e.d0.loc.x, (int) e.d0.loc.y, (int) e.d1.loc.x, (int) e.d1.loc.y);
            }
            if (drawRivers && e.river > 0) {
                g.setStroke(new BasicStroke(1 + (int) Math.sqrt(e.river * 2)));
                g.setColor(Color.blue);
                g.drawLine((int) e.v0.loc.x, (int) e.v0.loc.y, (int) e.v1.loc.x, (int) e.v1.loc.y);
            }
        }
        if (drawSites) {
            g.setColor(Color.BLACK);
            for (Center s : centers) {
                g.fillOval((int) (s.loc.x - 2), (int) (s.loc.y - 2), 4, 4);
            }
        }
        if (drawCorners) {
            g.setColor(Color.WHITE);
            for (Corner c : corners) {
                g.fillOval((int) (c.loc.x - 2), (int) (c.loc.y - 2), 4, 4);
            }
        }
        g.setColor(Color.WHITE);
        g.drawRect(0, 0, size, size);
    }

    public String getBiome(Center center) {
        return "blue";
    }

    private void assignBiomes() {
        for (Center center : centers) {
            center.biome = getBiome(center);
        }
    }

    private void assignPolygonMoisture() {
        for (Center center : centers) {
            double total = 0;
            for (Corner c : center.corners) {
                total += c.moisture;
            }
            center.moisture = total / center.corners.size();
        }
    }

    private void redistributeMoisture(List<Corner> landCorners) {
        Collections.sort(landCorners, (Corner o1, Corner o2) -> {
            if (o1.moisture > o2.moisture) {
                return 1;
            } else if (o1.moisture < o2.moisture) {
                return -1;
            }
            return 0;
        });
        for (int i = 0; i < landCorners.size(); i++) {
            landCorners.get(i).moisture = (double) i / landCorners.size();
        }
    }

    private void assignCornerMoisture() {
        LinkedList<Corner> queue = new LinkedList();
        for (Corner c : corners) {
            if ((c.water || c.river > 0) && !c.ocean) {
                c.moisture = c.river > 0 ? Math.min(3.0, (0.2 * c.river)) : 1.0;
                queue.push(c);
            } else {
                c.moisture = 0.0;
            }
        }
        while (!queue.isEmpty()) {
            Corner c = queue.pop();
            for (Corner a : c.adjacent) {
                double newM = .9 * c.moisture;
                if (newM > a.moisture) {
                    a.moisture = newM;
                    queue.add(a);
                }
            }
        }
        // Salt water
        for (Corner c : corners) {
            if (c.ocean || c.coast) {
                c.moisture = 1.0;
            }
        }
    }

    private DoubleEdge lookupEdgeFromCorner(Corner c, Corner downslope) {
        for (DoubleEdge e : c.protrudes) {
            if (e.v0 == downslope || e.v1 == downslope) {
                return e;
            }
        }
        return null;
    }

    private void createRivers() {
        Random random = new Random(); // :(
        for (int i = 0; i < 100; i++) {
            Corner c = corners.get(random.nextInt(corners.size()));
            if (c.ocean || c.elevation < 0.3 || c.elevation > 0.9) {
                continue;
            }
            while (!c.coast) {
                if (c == c.downslope) {
                    break;
                }
                DoubleEdge edge = lookupEdgeFromCorner(c, c.downslope);
                if (!edge.v0.water || !edge.v1.water) {
                    edge.river++;
                    c.river++;
                    c.downslope.river++; // TODO: fix double count
                }
                c = c.downslope;
            }
        }
    }

    private void calculateDownslopes() {
        for (Corner c : corners) {
            Corner down = c;
            for (Corner a : c.adjacent) {
                if (a.elevation <= down.elevation) {
                    down = a;
                }
            }
            c.downslope = down;
        }
    }

    private void assignPolygonElevations() {
        for (Center center : centers) {
            double total = 0;
            for (Corner c : center.corners) {
                total += c.elevation;
            }
            center.elevation = total / center.corners.size();
        }
    }

    private void redistributeElevations(List<Corner> landCorners) {
        Collections.sort(landCorners, (Corner o1, Corner o2) -> {
            if (o1.elevation > o2.elevation) {
                return 1;
            } else if (o1.elevation < o2.elevation) {
                return -1;
            }
            return 0;
        });
        final double SCALE_FACTOR = 1.1;
        for (int i = 0; i < landCorners.size(); i++) {
            double y = (double) i / landCorners.size();
            double x = Math.sqrt(SCALE_FACTOR) - Math.sqrt(SCALE_FACTOR * (1 - y));
            x = Math.min(x, 1);
            landCorners.get(i).elevation = x;
        }
        for (Corner c : corners) {
            if (c.ocean || c.coast) {
                c.elevation = 0.0;
            }
        }
    }

    private void assignOceanCoastAndLand() {
        LinkedList<Center> queue = new LinkedList();
        final double waterThreshold = .3;
        for (final Center center : centers) {
            int numWater = 0;
            for (final Corner c : center.corners) {
                if (c.border) {
                    center.border = center.water = center.ocean = true;
                    queue.add(center);
                }
                if (c.water) {
                    numWater++;
                }
            }
            center.water = center.ocean || ((double) numWater / center.corners.size() >= waterThreshold);
        }
        while (!queue.isEmpty()) {
            final Center center = queue.pop();
            for (final Center n : center.neighbors) {
                if (n.water && !n.ocean) {
                    n.ocean = true;
                    queue.add(n);
                }
            }
        }
        for (Center center : centers) {
            boolean oceanNeighbor = false;
            boolean landNeighbor = false;
            for (Center n : center.neighbors) {
                oceanNeighbor |= n.ocean;
                landNeighbor |= !n.water;
            }
            center.coast = oceanNeighbor && landNeighbor;
        }
        for (Corner c : corners) {
            int numOcean = 0;
            int numLand = 0;
            for (Center center : c.touches) {
                numOcean += center.ocean ? 1 : 0;
                numLand += !center.water ? 1 : 0;
            }
            c.ocean = numOcean == c.touches.size();
            c.coast = numOcean > 0 && numLand > 0;
            c.water = c.border || ((numLand != c.touches.size()) && !c.coast);
        }
    }

    private boolean isWater(Vector2 loc) {
        return false;
    }

    private void assignCornerElevations() {
        LinkedList<Corner> queue = new LinkedList();
        for (Corner c : corners) {
            c.water = isWater(c.loc);
            if (c.border) {
                c.elevation = 0;
                queue.add(c);
            } else {
                c.elevation = Double.MAX_VALUE;
            }
        }
        while (!queue.isEmpty()) {
            Corner c = queue.pop();
            for (Corner a : c.adjacent) {
                double newElevation = 0.01 + c.elevation;
                if (!c.water && !a.water) {
                    newElevation += 1;
                }
                if (newElevation < a.elevation) {
                    a.elevation = newElevation;
                    queue.add(a);
                }
            }
        }
    }

    private void improveCorners() {
        List<Vector2> newLocations = new ArrayList<>();
        corners.forEach(corner -> {
            Vector2 newLoc = corner.border ? corner.loc : corner.touches.stream()
                    .map(center -> center.loc)
                    .reduce((acc, e) -> acc.add(e))
                    .get().div(corner.touches.size());
            newLocations.add(newLoc);
        });
        for (int i = 0; i < newLocations.size(); i++) {
            corners.get(i).loc = newLocations.get(i);
        }
    }

    private Corner makeCorner(Map<Vector2, Corner> pointCornerMap, Vector2 point) {
        Corner corner = pointCornerMap.get(point);
        if (corner == null) {
            corner = new Corner();
            corner.loc = point;
            corner.border = false; // :(
            corners.add(corner);
            pointCornerMap.put(point, corner);
        }
        return corner;
    }

    public static class DoubleEdge {

        public Center d0, d1;
        public Corner v0, v1;
        public int river;
    }

    public static class Corner {

        public int river;
        public boolean water, coast, ocean;
        public double elevation, moisture;
        public Vector2 loc;
        public boolean border;
        public Corner downslope;
        public List<DoubleEdge> protrudes = new ArrayList<>();
        public Set<Center> touches = new NonNullSet<>();
        public Set<Corner> adjacent = new NonNullSet<>();
    }

    public static class Center {

        public int index;
        public String biome;
        public double elevation, moisture;
        public boolean water, ocean, border, coast;
        public Vector2 loc;
        public Set<Center> neighbors = new NonNullSet<>();
        public List<DoubleEdge> borders = new ArrayList<>();
        public Set<Corner> corners = new NonNullSet<>();

        private Center(Vector2 point) {
            loc = point;
        }

    }

    private static class NonNullSet<T> extends HashSet<T> {

        @Override
        public boolean add(T e) {
            if (e == null) {
                return false;
            }
            return super.add(e);
        }
    }

    private List<Vector2> randomPoints(int numPoints, int bound) {
        List<Vector2> result = new ArrayList<>();
        Random random = new Random(0);
        for (int i = 0; i < numPoints; i++) {
            result.add(new Vector2(random.nextInt(bound), random.nextInt(bound)));
        }
        return result;
    }
}
