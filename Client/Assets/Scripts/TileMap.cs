using System;
using UnityEngine;

namespace Gridia
{
    public class Tile
    {
        public int Floor { get; set; }
        public ItemInstance Item { get; set; }
    }

    public class TileMap
    {
        private Tile[] _tiles;
        public ConcurrentDictionary<int, Creature> Creatures = new ConcurrentDictionary<int, Creature>();
        private readonly Sector[, ,] _sectors;
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
            var sector = _sectors[sx, sy, z];
            if (sector == null)
            {
                Locator.Get<ConnectionToGridiaServerHandler>().SectorRequest(sx, sy, z);
            }
            return sector;
        }

        public void CreateCreature(int id, String name, CreatureImage image, int x, int y, int z)
        {
            if (!Creatures.HasKey(id))
            {
                Creatures[id] = new Creature(id, name, image, x, y, z);
            }
        }

        public void RemoveCreature(int id)
        {
            Creatures.Remove(id);
        }

        public void MoveCreature(int id, int x, int y, int z, bool onRaft, long time)
        {
            var cre = GetCreature(id);
            if (cre != null)
            {
                cre.AddPositionSnapshot(new Vector3(x, y, z), onRaft, time);
            }
        }

        public Creature GetCreature(int id) 
        {
            Creature cre;
            Creatures.TryGetValue(id, out cre);
            if (cre == null)
            {
                Locator.Get<ConnectionToGridiaServerHandler>().CreatureRequest(id);
            }
            return cre;
        }

        public Creature GetCreatureAt(Vector3 loc) 
        {
            return Creatures.ValuesToList().Find(cre => 
            {
                var pos = cre.Position;
                return (int)pos.x == loc.x && (int)pos.y == loc.y && pos.z == loc.z;
            });
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
                var tileFacade = new Tile {Item = new ItemInstance(Locator.Get<ContentManager>().GetItem(0))};
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

        public bool Walkable(Vector3 destination)
        {
            return Walkable((int)destination.x, (int)destination.y, (int)destination.z);
        }

        public bool Walkable(int x, int y, int z) {
            var tile = GetTile(x, y, z);
            var loc = new Vector3(x, y, z);
            return GetCreatureAt(loc) == null && tile.Item.Item.Walkable && tile.Floor != 0;
        }

        public int Wrap (int value)
        {
            var mod = value % Size;
            return mod < 0 ? Size + mod : mod;
        }

        public Vector3 Wrap(Vector3 coord) 
        {
            return new Vector3(Wrap((int)coord.x), Wrap((int)coord.y), coord.z);
        }

        public int ToIndex(Vector3 v) {
            var x = Wrap((int)v.x);
            var y = Wrap((int)v.y);
            var z = (int)v.z;
            return x + y * Size + z * Area;
        }

        //given the map wraps around, returns the signed distance between two coords
        public float WrappedDistBetweenX(Vector3 loc1, Vector3 loc2)
        {
            var d1 = loc1.x - loc2.x;
            var d2 = -(Size - loc1.x + loc2.x);
            var d3 = Size + loc1.x - loc2.x;

            if (Math.Abs(d1) < Math.Abs(d2))
            {
                return Math.Abs(d1) < Math.Abs(d3) ? d1 : d3;
            }
            else
            {
                return Math.Abs(d2) < Math.Abs(d3) ? d2 : d3;
            }
        }

        public float WrappedDistBetweenY(Vector3 loc1, Vector3 loc2)
        {
            var d1 = loc1.y - loc2.y;
            var d2 = -(Size - loc1.y + loc2.y);
            var d3 = Size + loc1.y - loc2.y;

            if (Math.Abs(d1) < Math.Abs(d2))
            {
                return Math.Abs(d1) < Math.Abs(d3) ? d1 : d3;
            }
            else
            {
                return Math.Abs(d2) < Math.Abs(d3) ? d2 : d3;
            }
        }
    }
}