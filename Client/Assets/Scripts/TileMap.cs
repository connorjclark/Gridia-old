using System;
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
        private List<Creature> _creatures = new List<Creature>();
        private Sector[, ,] _sectors;
        public int Size { get; private set; }
        public int Depth { get; private set; }
        public int SectorSize { get; private set; }
        public int Area { get; private set; }
        public int Volume { get; private set; }
        public int SectorsAcross { get; private set; }
        public int SectorsFloor { get; private set; }
        public int SectorsTotal { get; private set; }

        public TileMap (int size, int depth, int sectorSize)
        {
            if (size % sectorSize != 0)
            {
                throw new ArgumentException("sectorSize must be a factor of size");
            }
            Size = size; //smell?
            Depth = depth;
            SectorSize = sectorSize;
            Area = size * size;
            Volume = Area * depth;
            SectorsAcross = size / sectorSize;
            SectorsFloor = SectorsAcross * SectorsAcross;
            SectorsTotal = SectorsFloor * depth;
            _sectors = new Sector[SectorsAcross, SectorsAcross, depth];
        }

        public void SetSector(Sector sector, int x, int y, int z)
        {
            _sectors[x, y, z] = sector;
        }

        public Sector GetSectorOf(int x, int y, int z) {
            x = Wrap(x);
            y = Wrap(y);
            var sx = x / SectorSize;
            var sy = y / SectorSize;
            Sector sector = _sectors[sx, sy, z];
            if (sector == null)
            {
                Locator.Get<ConnectionToGridiaServerHandler>().RequestSector(sx, sy, z);
            }
            return sector;
        }

        public Tile GetTile(Vector3 loc) 
        {
            return GetTile((int)loc.x, (int)loc.y, (int)loc.z);
        }

        public Tile GetTile (int x, int y, int z)
        {
            x = Wrap(x);
            y = Wrap(y);
            var sector = GetSectorOf(x, y, z);
            if (sector == null) {
                var tileFacade = new Tile();
                tileFacade.Item = new ItemInstance(Locator.Get<ContentManager>().GetItem(0));
                return tileFacade;
            }
            return sector.GetTile(x % SectorSize, y % SectorSize);
        }

        public void SetFloor (int floor, int x, int y, int z)
        {
            GetTile (x, y, z).Floor = floor;
        }

        public void SetItem (ItemInstance item, int x, int y, int z)
        {
            GetTile (x, y, z).Item = item;
        }

        private Tile GetTileOfCreature(Creature creature) {
            var pos = creature.Position;
            return GetTile((int)pos.x, (int)pos.y, (int)pos.z);
        }

        public void AddCreature(Creature creature) {
            GetTileOfCreature(creature).Creature = creature;
            _creatures.Add(creature);
        }

        public bool Walkable(int x, int y, int z) {
            Tile tile = GetTile(x, y, z);
            return tile.Creature == null && !tile.Item.Item.BlockMovement;
        }

        public int Wrap (int value)
        {
            int mod = value % Size;
            return mod < 0 ? Size + mod : mod;
        }

        public int ToIndex(Vector3 v) {
            int x = (int)v.x;
            int y = (int)v.y;
            int z = (int)v.z;
            return x + y * Size + z * Area;
        }
    }
}