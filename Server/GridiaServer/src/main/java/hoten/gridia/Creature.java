package hoten.gridia;

import hoten.gridia.map.Coord;
import hoten.gridia.uniqueidentifiers.UniqueIdentifiers;

public class Creature {

    private static final UniqueIdentifiers uniqueIds = new UniqueIdentifiers();

    public final int id = uniqueIds.next();
    public String name;
    public Coord location = new Coord(0, 0, 0);
    public CreatureImage image;
    public boolean belongsToPlayer;
    public Container inventory;
    public int life = 3;
    public boolean justTeleported; // :(
    public boolean isFriendly;
    public String friendlyMessage = "Hello!";

    public void retire() {
        uniqueIds.retire(id);
    }
}
