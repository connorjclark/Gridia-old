namespace Gridia
{
    using System;

    public class Item
    {
        #region Constructors

        public Item()
        {
            ImageWidth = ImageHeight = 1;
        }

        #endregion Constructors

        #region Enumerations

        public enum ItemClass
        {
            Normal,
            Weapon,
            Wand,
            Plant,
            Ore,
            Ammo,
            Wall,
            Armor,
            Vendor,
            Shield,
            Food,
            Money,
            Container,
            Jewelry_neck,
            Jewelry_finger,
            Jewelry_wrist,
            Slot,
            Bridge,
            Cave_down,
            Cave_up,
            Fire,
            Flag,
            Rune,
            Raft,
            Trap,
            Clothechest,
            Ball,
            Goal,
            Fish
        }

        #endregion Enumerations

        #region Properties

        public int[] Animations
        {
            get; set;
        }

        public ItemClass Class
        {
            get; set;
        }

        // :( private set?
        public int Id
        {
            get; set;
        }

        public int ImageHeight
        {
            get; set;
        }

        public int ImageWidth
        {
            get; set;
        }

        public int Light
        {
            get; set;
        }

        public String Name
        {
            get; set;
        }

        public bool Walkable
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public ItemInstance GetInstance(int quantity = 1)
        {
            return new ItemInstance (this, quantity);
        }

        #endregion Methods
    }
}