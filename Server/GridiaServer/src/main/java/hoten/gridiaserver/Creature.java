package hoten.gridiaserver;

public class Creature {

    private static final UniqueIdentifiers uniqueIds = new UniqueIdentifiers();

    public Coord location = new Coord();
    public final int id = uniqueIds.next();
}
