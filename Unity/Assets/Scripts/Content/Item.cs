using System;

namespace Gridia
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Light { get; set; }
        public int[] Animations { get; set; }

        public ItemInstance GetInstance (int quantity = 1)
        {
            return new ItemInstance (this, quantity);
        }
    }
}