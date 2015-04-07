package com.hoten.gridia.content;

import com.google.gson.JsonObject;
import java.util.Objects;

public class ItemInstance {

    // :(
    public static ItemInstance NONE;

    public static void setBlankItemInstance(ItemInstance item) {
        if (NONE != null) {
            throw new RuntimeException("Blank item has already been set!");
        }
        NONE = item;
    }

    public static boolean stackable(ItemInstance item1, ItemInstance item2) {
        if (item1._item.id != item2._item.id || !item1._item.stackable) {
            return false;
        }
        int q = item1._quantity + item2._quantity;
        return q <= 100000;
    }

    private final Item _item;
    private final int _quantity;
    public int age;
    private JsonObject _data;

    public ItemInstance(Item item, int quantity, JsonObject data) {
        _item = item;
        _quantity = quantity;
        _data = data;
    }

    public ItemInstance(ItemInstance item) {
        _item = item._item;
        _quantity = item._quantity;
        age = item.age;
    }

    public boolean isNothing() {
        return this == NONE;
    }

    public ItemInstance remove(int amount) {
        int newQuantity = _quantity - amount;
        if (newQuantity <= 0) {
            return ItemInstance.NONE;
        } else {
            return new ItemInstance(_item, newQuantity, _data);
        }
    }

    public ItemInstance add(int amount) {
        return remove(-amount);
    }

    @Override
    public int hashCode() {
        int hash = 7;
        hash = 73 * hash + Objects.hashCode(this._item);
        hash = 73 * hash + this._quantity;
        return hash;
    }

    @Override
    public boolean equals(Object obj) {
        if (obj == null) {
            return false;
        }
        if (!(obj instanceof ItemInstance)) {
            return false;
        }
        final ItemInstance other = (ItemInstance) obj;
        if (!Objects.equals(_item, other._item)) {
            return false;
        }
        return _quantity == other._quantity;
    }

    @Override
    public String toString() {
        return _item.name + (_quantity != 1 ? String.format(" (%d)", _quantity) : "");
    }

    public Item getItem() {
        return _item;
    }

    public int getQuantity() {
        return _quantity;
    }

    public JsonObject getData() {
        if (_data == null) {
            _data = new JsonObject();
        }
        return _data;
    }
}
