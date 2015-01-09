package hoten.gridia.content;

import java.util.Objects;

public class ItemInstance {

    public static final ItemInstance NONE = new ItemInstance(new Item(), 0);

    public static boolean stackable(ItemInstance item1, ItemInstance item2) {
        if (item1.data.id != item2.data.id || !item1.data.stackable) {
            return false;
        }
        int q = item1.quantity + item2.quantity;
        return q < 1000;
    }

    public Item data;
    public int quantity;
    public int age;

    public ItemInstance(Item data, int quantity) {
        this.data = data;
        this.quantity = quantity;
    }

    public ItemInstance(ItemInstance item) {
        data = item.data;
        quantity = item.quantity;
    }

    @Override
    public int hashCode() {
        int hash = 7;
        hash = 73 * hash + Objects.hashCode(this.data);
        hash = 73 * hash + this.quantity;
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
        if (!Objects.equals(this.data, other.data)) {
            return false;
        }
        if (this.quantity != other.quantity) {
            return false;
        }
        return true;
    }

    @Override
    public String toString() {
        return data.name + (quantity != 1 ? String.format(" (%d)", quantity) : "");
    }
}
