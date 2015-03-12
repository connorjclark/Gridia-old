package com.hoten.gridia;

import com.hoten.gridia.map.Coord;
import com.hoten.gridia.serving.ServingGridia;
import com.hoten.gridia.uniqueidentifiers.UniqueIdentifiers;

public class Creature extends com.hoten.gridia.scripting.Entity {

    private static final UniqueIdentifiers uniqueIds = new UniqueIdentifiers(100);

    public final int id = uniqueIds.next();
    public String name;
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

    public boolean isAlive() {
        return life > 0;
    }

    public void setLocation(Coord newLocation) {
        ServingGridia.instance.moveCreatureTo(this, newLocation, false);
    }
}
