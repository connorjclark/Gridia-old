package hoten.gridia.worldgen;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.nio.file.Files;
import java.util.LinkedList;
import java.util.List;
import java.util.Queue;
import java.util.logging.Level;
import java.util.logging.Logger;
import java.util.stream.Collectors;

public class QhullBridge {
    
    public File saveAsQhullPoints(List<Vector2> points, String filePath) throws IOException {
        String result = "2\n" + points.size() + "\n" + points.stream().
                map(point -> String.format("%d %d", point.x, point.y))
                .collect(Collectors.joining("\n"));

        File file = new File(filePath);
        Files.write(file.toPath(), result.getBytes());

        return file;
    }
    
    public Queue<String> runQhull(String qhullCmd) {
        String cmd = String.format("cd bin/qhull && %s && exit", qhullCmd);
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
        
        return output;
    }
}
