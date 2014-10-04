using System;

namespace Gridia
{
    public class Tile
    {
        public int Floor { get; set; }
        public ItemInstance Item { get; set; }
        public int Creature { get; set; }
    }

    public class TileMap
    {
        private Tile[] tiles;
        public readonly int size;

        public TileMap (int size)
        {
            this.size = size;
            InitializeTiles ();
        }

        public Tile GetTile (int x, int y)
        {
            return tiles [Wrap (y) * size + Wrap (x)];
        }

        public void SetFloor (int floor, int x, int y)
        {
            GetTile (x, y).Floor = floor;
        }

        public void SetItem (ItemInstance item, int x, int y)
        {
            GetTile (x, y).Item = item;
        }

        public void SetCreature (int creature, int x, int y)
        {
            GetTile (x, y).Creature = creature;
        }

        private void InitializeTiles ()
        {
            Random r = new Random ();
            tiles = new Tile[size * size];
            for (int i = 0; i < tiles.Length; i++) {
                Tile tile = new Tile ();
                tile.Floor = 8;
                tile.Creature = -1;
                //tile.Item = ContentManager.Singleton.GetItem(10).GetInstance();
                tile.Item = ContentManager.Singleton.GetItem(r.Next(ContentManager.Singleton.ItemCount)).GetInstance();
                tiles [i] = tile;
            }
        }

        private int Wrap (int value)
        {
            int mod = value % size;
            return mod < 0 ? size + mod : mod;
        }
    }
}