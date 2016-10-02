package com.hoten.gridia.content;

import java.io.File;
import java.io.IOException;
import org.apache.commons.io.FileUtils;

public class WorldContentLoader implements ContentLoader {

    private final File _world;

    public WorldContentLoader(File world) {
        _world = world;
    }

    @Override
    public ContentManager load() throws IOException {
        File content = new File(_world, "clientdata/content");
        String itemsJson = FileUtils.readFileToString(new File(content, "items.json"));
        String usagesJson = FileUtils.readFileToString(new File(content, "itemuses.json"));
        String monstersJson = FileUtils.readFileToString(new File(content, "monsters.json"));
        return new JsonContentLoader(itemsJson, usagesJson, monstersJson).load();
    }
}
