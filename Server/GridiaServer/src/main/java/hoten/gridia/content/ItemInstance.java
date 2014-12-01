package hoten.gridia.content;

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

    public ItemInstance(Item data, int quantity) {
        this.data = data;
        this.quantity = quantity;
    }
    
    public ItemInstance(ItemInstance item) {
        data = item.data;
        quantity = item.quantity;
    }

    @Override
    public String toString() {
        return data.name + (quantity != 1 ? String.format(" (%d)", quantity) : "");
    }
}
