namespace Gridia
{
    public class ItemInstance
    {
        public Item Item { get; set; }
        public int Quantity { get; set; }
        
        public ItemInstance (Item item, int quantity = 1)
        {
            Item = item;
            Quantity = quantity;
        }
    }
}