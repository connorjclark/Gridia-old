package hoten.gridiaserver;

public class ItemInstance {

    public static final ItemInstance NONE = new ItemInstance(new Item(), 0);

    public final Item data;
    public int quantity;

    public ItemInstance(Item data, int quantity) {
        this.data = data;
    }
}
