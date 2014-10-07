using System.Collections.Generic;
using UnityEngine;

namespace Gridia
{
    public class Tile
    {
        public int Floor { get; set; }
        public ItemInstance Item { get; set; }
        public Creature Creature { get; set; }
    }

    public class TileMap
    {
        private Tile[] tiles;
        private List<Creature> _creatures;
        public int Size { get; private set; }

        public TileMap (int size)
        {
            Size = size;//smell?
            _creatures = new List<Creature>();
            InitializeTiles ();
        }

        public Tile GetTile (int x, int y)
        {
            return tiles [Wrap (y) * Size + Wrap (x)];
        }

        public void SetFloor (int floor, int x, int y)
        {
            GetTile (x, y).Floor = floor;
        }

        public void SetItem (ItemInstance item, int x, int y)
        {
            GetTile (x, y).Item = item;
        }

        private Tile GetTileOfCreature(Creature creature) {
            return GetTile((int)creature.Position.x, (int)creature.Position.y);
        }

        public void AddCreature(Creature creature) {
            GetTileOfCreature(creature).Creature = creature;
            _creatures.Add(creature);
        }

        public void UpdateCreature(Creature creature, Vector2 newPosition) {
            GetTileOfCreature(creature).Creature = null;
            creature.Position = newPosition;
            GetTileOfCreature(creature).Creature = creature;
        }

        public bool Walkable(int x, int y) {
            return GetTile(x, y).Creature == null;
        }

        private void InitializeTiles ()
        {
            tiles = new Tile[Size * Size];
            for (int i = 0; i < tiles.Length; i++) {
                Tile tile = new Tile ();
                tile.Floor = 8;
                tile.Item = ContentManager.Singleton.GetItem(Random.Range(0, 10)).GetInstance();
                if (Random.Range(0, 10) > 1) tile.Item = ContentManager.Singleton.GetItem(0).GetInstance();
                //tile.Item = ContentManager.Singleton.GetItem(r.Next(ContentManager.Singleton.ItemCount)).GetInstance();
                tiles [i] = tile;
            }
        }

        private int Wrap (int value)
        {
            int mod = value % Size;
            return mod < 0 ? Size + mod : mod;
        }
    }
}