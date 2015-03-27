package com.hoten.gridia.map;

import com.google.gson.Gson;
import com.google.gson.JsonObject;
import com.hoten.gridia.Player;
import com.hoten.gridia.content.ItemInstance;
import java.io.File;
import java.io.IOException;
import java.util.Random;
import java.util.function.Consumer;
import java.util.function.Function;
import java.util.logging.Level;
import java.util.logging.Logger;
import org.apache.commons.io.FileUtils;

public class TileMap {

    public static TileMap loadMap(File world, String mapName) throws IOException {
        File map = new File(world, "maps/" + mapName);
        String json = FileUtils.readFileToString(new File(map, "meta.json"));

        JsonObject metaData = new Gson().fromJson(json, JsonObject.class);

        int size = metaData.get("size").getAsInt();
        int depth = metaData.get("depth").getAsInt();
        int sectorSize = metaData.get("sectorSize").getAsInt();

        TileMap tm = new TileMap(size, depth, sectorSize, new JsonSectorLoader(map), new SectorSaver(map));

        tm._defaultPlayerSpawn = new Gson().fromJson(metaData.get("defaultPlayerSpawn"), Coord.class);

        return tm;
    }

    public final int size, depth, sectorSize, area, volume, sectorsAcross, sectorsFloor, sectorsTotal;
    private Coord _defaultPlayerSpawn = new Coord(498, 543, 0); // :(
    private final Sector[][][] _sectors;
    private final SectorLoader _sectorLoader;
    private final SectorSaver _sectorSaver;

    public TileMap(int size, int depth, int sectorSize, SectorLoader sectorLoader, SectorSaver sectorSaver) {
        if (size % sectorSize != 0) {
            throw new IllegalArgumentException("sectorSize must be a factor of size");
        }
        this.size = size;
        this.depth = depth;
        this.sectorSize = sectorSize;
        area = size * size;
        volume = area * depth;
        sectorsAcross = size / sectorSize;
        sectorsFloor = sectorsAcross * sectorsAcross;
        sectorsTotal = sectorsFloor * depth;
        _sectors = new Sector[sectorsAcross][sectorsAcross][depth];
        _sectorLoader = sectorLoader;
        _sectorSaver = sectorSaver;
    }

    public Coord getDefaultPlayerSpawn() {
        Random random = new Random();
        return _defaultPlayerSpawn.add(random.nextInt(3), random.nextInt(3), 0);
    }

    // :( temporary
    public void loadAll() throws IOException {
        for (int x = 0; x < sectorsAcross; x++) {
            for (int y = 0; y < sectorsAcross; y++) {
                for (int z = 0; z < depth; z++) {
                    Sector sector = _sectors[x][y][z];
                    if (sector == null) {
                        _sectors[x][y][z] = _sectorLoader.load(sectorSize, x, y, z);
                    }
                }
            }
        }
    }

    // :( lol
    public void forAllTilesLoaded(Function<Integer, Function<Integer, Function<Integer, Consumer<Tile>>>> func) {
        for (int sx = 0; sx < sectorsAcross; sx++) {
            for (int sy = 0; sy < sectorsAcross; sy++) {
                for (int sz = 0; sz < depth; sz++) {
                    Sector sector = _sectors[sx][sy][sz];
                    if (sector != null) {
                        for (int x = 0; x < sectorSize; x++) {
                            for (int y = 0; y < sectorSize; y++) {
                                func.apply(x + sx * sectorSize).apply(y + sy * sectorSize).apply(sz).accept(sector._tiles[x][y]);
                            }
                        }
                    }
                }
            }
        }
    }

    public void save() throws IOException {
        for (int x = 0; x < sectorsAcross; x++) {
            for (int y = 0; y < sectorsAcross; y++) {
                for (int z = 0; z < depth; z++) {
                    Sector sector = _sectors[x][y][z];
                    if (sector != null) {
                        _sectorSaver.save(sector);
                    }
                }
            }
        }
    }

    public String getSectorData(int sx, int sy, int sz) {
        Sector sector = getSector(sx, sy, sz);
        Gson gson = new Gson(); // :(
        return gson.toJson(sector._tiles);
    }

    public Sector getSector(int sx, int sy, int sz) {
        Sector sector = _sectors[sx][sy][sz];
        if (sector == null) {
            try {
                return _sectors[sx][sy][sz] = _sectorLoader.load(sectorSize, sx, sy, sz);
            } catch (IOException ex) {
                Logger.getLogger(TileMap.class.getName()).log(Level.SEVERE, null, ex);
            }
        }
        return sector;
    }

    public Sector getSectorOf(Coord loc) {
        return getSectorOf(loc.x, loc.y, loc.z);
    }

    public Sector getSectorOf(int x, int y, int z) {
        x = wrap(x);
        y = wrap(y);
        int sx = x / sectorSize;
        int sy = y / sectorSize;
        return getSector(sx, sy, z);
    }

    public Tile getTile(Coord loc) {
        return getTile(loc.x, loc.y, loc.z);
    }

    public Tile getTile(int x, int y, int z) {
        x = wrap(x);
        y = wrap(y);
        return getSectorOf(x, y, z).getTile(x % sectorSize, y % sectorSize);
    }

    public int getFloor(Coord coord) {
        return getTile(coord).floor;
    }

    public int getFloor(int x, int y, int z) {
        return getTile(x, y, z).floor;
    }

    public void setFloor(int x, int y, int z, int floor) {
        getTile(x, y, z).floor = floor;
    }

    public void setFloor(Coord loc, int floor) {
        getTile(loc).floor = floor;
    }

    public ItemInstance getItem(Coord c) {
        return getTile(c.x, c.y, c.z).item;
    }

    public ItemInstance getItem(int x, int y, int z) {
        return getTile(x, y, z).item;
    }

    public void setItem(ItemInstance item, int x, int y, int z) {
        getTile(x, y, z).item = item;
    }

    public void setItem(ItemInstance item, Coord c) {
        getTile(c.x, c.y, c.z).item = item;
    }

    public com.hoten.gridia.scripting.Entity getCreature(Coord loc) {
        return getTile(loc).cre;
    }

    public int wrap(int value) {
        int mod = value % size;
        return mod < 0 ? size + mod : mod;
    }

    public Coord wrap(Coord loc) {
        return new Coord(wrap(loc.x), wrap(loc.y), loc.z);
    }

    public boolean walkable(Coord coord) {
        Tile tile = getTile(coord);
        return tile.cre == null && tile.item.getItem().walkable && tile.floor != 0;
    }

    public boolean walkable(int x, int y, int z) {
        Tile tile = getTile(x, y, z);
        return tile.cre == null && tile.item.getItem().walkable && tile.floor != 0;
    }

    public Coord getCoordFromIndex(int index) {
        int z = index / area;
        int y = (index / size) % size;
        int x = index % size;
        return new Coord(x, y, z);
    }

    public int getIndexFromCoord(Coord coord) {
        return wrap(coord.x) + wrap(coord.y) * size + wrap(coord.z) * area;
    }

    public boolean inBounds(int x, int y, int z) {
        return x >= 0 && x < size && y >= 0 && y < size && z >= 0 && z < depth;
    }

    public boolean hasRightsTo(Player player, Coord coord) {
        Tile tile = getTile(coord);
        if (tile.getOwner() == player.getPlayerId() || tile.isUnclaimed()) {
            return true;
        }
        Sector sector = getSectorOf(coord);
        return sector.getOwner() == player.getPlayerId() || sector.isUnclaimed();
    }
}
