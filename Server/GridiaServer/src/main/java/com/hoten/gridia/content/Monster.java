package com.hoten.gridia.content;

import com.hoten.gridia.CreatureImage;
import java.util.List;

public class Monster implements Cloneable {

    public int id;
    public CreatureImage image;
    public String name;
    public List<ItemInstance> drops;

    @Override
    public Monster clone() throws CloneNotSupportedException {
        return (Monster) super.clone();
    }
}
