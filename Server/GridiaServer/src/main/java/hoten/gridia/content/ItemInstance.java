package hoten.gridia.content;

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
        if (item1._data.id != item2._data.id || !item1._data.stackable) {
            return false;
        }
        int q = item1._quantity + item2._quantity;
        return q < 1000;
    }

    private final Item _data;
    private final int _quantity;
    public int age;

    public ItemInstance(Item data, int quantity) {
        this._data = data;
        this._quantity = quantity;
    }

    public ItemInstance(ItemInstance item) {
        _data = item._data;
        _quantity = item._quantity;
        age = item.age;
    }

    public ItemInstance remove(int amount) {
        int newQuantity = _quantity - amount;
        if (newQuantity <= 0) {
            return ItemInstance.NONE;
        } else {
            return new ItemInstance(_data, newQuantity);
        }
    }

    public ItemInstance add(int amount) {
        return remove(-amount);
    }

    @Override
    public int hashCode() {
        int hash = 7;
        hash = 73 * hash + Objects.hashCode(this._data);
        hash = 73 * hash + this._quantity;
        return hash;
    }

    @Override
    public boolean equals(Object obj) {
        if (obj == null) {
            return false;
        }
        if (getClass() != obj.getClass()) {
            return false;
        }
        final ItemInstance other = (ItemInstance) obj;
        if (!Objects.equals(this._data, other._data)) {
            return false;
        }
        if (this._quantity != other._quantity) {
            return false;
        }
        return true;
    }

    @Override
    public String toString() {
        return _data.name + (_quantity != 1 ? String.format(" (%d)", _quantity) : "");
    }

    public Item getData() {
        return _data;
    }

    public int getQuantity() {
        return _quantity;
    }
}
