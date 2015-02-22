package uk.labbookpages.wav;

import java.io.*;
import java.util.Collection;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.sound.sampled.AudioSystem;
import javax.sound.sampled.UnsupportedAudioFileException;
import org.apache.commons.io.FileUtils;

public class WavTest {

    public static void main(String[] args) throws IOException, WavFileException {
        Collection<File> wavs = FileUtils.listFiles(new File("worlds/demo-world/clientdata/sound"), new String[]{"wav", "WAV"}, true);
        wavs.forEach(file -> {
            try {
                if (file.getName().contains("crik")) {
                    float[] data = readWav(file);
                    System.out.println("data[0] = " + data[0]);
                    System.out.println("data[100] = " + data[100]);
                    System.out.println("data[1000] = " + data[1000]);
                    System.out.println("data[5000] = " + data[5000]);
                    System.out.println("data[10000] = " + data[10000]);
                    System.out.println("data[11285] = " + data[11285]);
                    System.out.println("good wav: " + file);
                }
                if (file.getName().contains("ice")) {
                    AudioSystem.getAudioInputStream(file).getFormat();
                    System.out.println(AudioSystem.getAudioInputStream(file).getFormat());
                }
            } catch (IOException | WavFileException | UnsupportedAudioFileException ex) {
                System.out.println("bad wav: " + file);
                Logger.getLogger(WavTest.class.getName()).log(Level.SEVERE, null, ex);
            }
        });
    }

    private static float[] doublesToFloats(double[] doubles) {
        float[] floats = new float[doubles.length];
        for (int i = 0; i < doubles.length; i++) {
            floats[i] = (float) doubles[i];
        }
        return floats;
    }

    private static float[] readWav(File file) throws IOException, WavFileException {
        WavFile wavFile = WavFile.openWavFile(file);
        wavFile.display();
        int numChannels = wavFile.getNumChannels();
        int numFrames = (int) wavFile.getNumFrames();
        double[] buffer = new double[numChannels * numFrames];
        wavFile.readFrames(buffer, numFrames);
        wavFile.close();
        return doublesToFloats(buffer);
    }
}
