package com.hoten.gridia;

import com.hoten.gridia.content.Item;
import com.hoten.gridia.content.ItemInstance;

public class CustomPlayerImage implements CreatureImage {

    public int bareHead, bareArms, bareChest, bareLegs;
    public int head, arms, chest, legs, shield, weapon;

    private int getWearImage(ItemInstance item, int defaultValue) {
        return item.isNothing() ? defaultValue : item.getItem().wearImage;
    }

    public void moldToEquipment(Container equipment) {
        ItemInstance headEquipment = equipment.get(Item.ArmorSpot.Head.ordinal());
        ItemInstance chestEquipment = equipment.get(Item.ArmorSpot.Chest.ordinal());
        ItemInstance legsEquipment = equipment.get(Item.ArmorSpot.Legs.ordinal());
        ItemInstance shieldEquipment = equipment.get(Item.ArmorSpot.Shield.ordinal());
        ItemInstance weaponEquipment = equipment.get(Item.ArmorSpot.Weapon.ordinal());
        head = getWearImage(headEquipment, bareHead);
        chest = getWearImage(chestEquipment, bareChest);
        legs = getWearImage(legsEquipment, bareLegs);
        shield = getWearImage(shieldEquipment, 0);
        weapon = getWearImage(weaponEquipment, 0);
    }
}
