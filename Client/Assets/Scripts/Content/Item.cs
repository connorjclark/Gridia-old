using System;
namespace Gridia
{
    public class Item
    {
        public int Id { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public String Name { get; set; }
        public bool Walkable { get; set; }
        public int Light { get; set; }
        public int[] Animations { get; set; }

        public Item()
        {
            ImageWidth = ImageHeight = 1;
        }

        public ItemInstance GetInstance (int quantity = 1)
        {
            return new ItemInstance (this, quantity);
        }
    }
}