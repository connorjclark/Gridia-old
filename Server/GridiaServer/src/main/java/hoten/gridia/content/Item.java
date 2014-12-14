package hoten.gridia.content;

public class Item {

    public enum ItemClass {

        Normal, Weapon, Wand, Plant, Ore, Ammo, Wall, Armor, Vendor, Shield, Food, Money, Container,
        Jewelry_neck, Jewelry_finger, Jewelry_wrist, Slot, Bridge, Cave_down, Cave_up, Fire, Flag,
        Rune, Raft, Trap, Clothechest, Ball, Goal, Fish
    }

    public enum ArmorSpot {

        Head, Chest, Legs, Weapon, Shield
    }

    // :( make final?
    public int id, wearImage, rarity, growthItem, growthDelta;
    public ItemClass itemClass;
    public ArmorSpot armorSpot;
    public String name, subType;
    public boolean walkable, moveable, stackable;

    public boolean isEquipable() {
        return itemClass == ItemClass.Armor || itemClass == ItemClass.Weapon || itemClass == ItemClass.Shield;
    }
}
