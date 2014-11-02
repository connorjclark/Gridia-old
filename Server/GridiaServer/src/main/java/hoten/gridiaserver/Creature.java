package hoten.gridiaserver;

import hoten.uniqueidentifiers.UniqueIdentifiers;

public class Creature {

    public static final UniqueIdentifiers uniqueIds = new UniqueIdentifiers();

    public Coord location = new Coord();
    public final int id = uniqueIds.next();
    public boolean belongsToPlayer;
}
