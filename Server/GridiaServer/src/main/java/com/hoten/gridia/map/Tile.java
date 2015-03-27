package com.hoten.gridia.map;

import com.hoten.gridia.content.ItemInstance;

public class Tile {

    public static final int OWNER_SERVER = -1;
    public static final int OWNER_UNCLAIMED = 0;

    public int floor;
    public ItemInstance item;
    public com.hoten.gridia.scripting.Entity cre;
    private int _owner;

    public boolean isUnclaimed() {
        return _owner == OWNER_UNCLAIMED;
    }

    public int getOwner() {
        return _owner;
    }

    public void setOwner(int owner) {
        _owner = owner;
    }
}
