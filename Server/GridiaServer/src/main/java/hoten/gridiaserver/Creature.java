package hoten.gridiaserver;

import hoten.uniqueidentifiers.UniqueIdentifiers;

public class Creature {

    public static final UniqueIdentifiers uniqueIds = new UniqueIdentifiers();

    public final int id = uniqueIds.next();
    public Coord location = new Coord();
    public int image;
    public boolean belongsToPlayer;
}
