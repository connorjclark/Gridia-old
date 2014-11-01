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
            //_tiles = tiles;
            InitializeTiles();
        }

        private void InitializeTiles()
        {
            int Size = 20;
            _tiles = new Tile[Size, Size];
            for (int x = 0; x < Size; x++) 
            {
                for (int y = 0; y < Size; y++)
                {
                    Tile tile = new Tile();
                    tile.Floor = 8;
                    tile.Item = Locator.Get<ContentManager>().GetItem(0).GetInstance();
                    _tiles[x, y] = tile;
                }
            }
        }

        public Tile GetTile(int x, int y)
        {
            return _tiles[x, y];
        }
    }
}
