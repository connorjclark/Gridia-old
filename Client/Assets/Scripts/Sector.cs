using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gridia
{
    public class Sector
    {
        private Tile[,] _tiles;

        public Sector(Tile[,] tiles) {
            _tiles = tiles;
        }

        public Tile GetTile(int x, int y)
        {
            return _tiles[x, y];
        }
    }
}
