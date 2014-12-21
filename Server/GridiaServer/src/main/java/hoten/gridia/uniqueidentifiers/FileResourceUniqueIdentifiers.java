package hoten.gridia.uniqueidentifiers;

import hoten.serving.fileutils.FileUtils;
import java.io.File;
import java.util.List;
import java.util.Set;
import java.util.stream.Collectors;
import java.util.stream.IntStream;
import org.apache.commons.io.FilenameUtils;

public class FileResourceUniqueIdentifiers extends UniqueIdentifiers {

    public FileResourceUniqueIdentifiers(String dir) {
        super(100);
        new File(dir).mkdirs();
        bufferIdsThatArentClaimed(dir);
    }

    private void bufferIdsThatArentClaimed(String dir) {
        List<File> files = FileUtils.getAllFilesInDirectory(new File(dir));
        Set<Integer> idsInUse = files.stream()
                .map(file -> FilenameUtils.removeExtension(file.getName()))
                .filter(name -> name.matches("\\d+"))
                .map(name -> Integer.parseInt(name))
                .collect(Collectors.toSet());
        int max = idsInUse.stream().max(Integer::compare).orElseGet(() -> 0);
        Set<Integer> oneToMax = IntStream.rangeClosed(1, max).boxed().collect(Collectors.toSet());
        Set<Integer> idsNotInUse = difference(oneToMax, idsInUse);
        _available.addAll(idsNotInUse);
        _nextNewId = max + 1;
    }

    private Set<Integer> difference(final Set<Integer> set1, final Set<Integer> set2) {
        final Set<Integer> larger = set1.size() > set2.size() ? set1 : set2;
        final Set<Integer> smaller = larger.equals(set1) ? set2 : set1;
        return larger.stream().filter(n -> !smaller.contains(n)).collect(Collectors.toSet());
    }
}
