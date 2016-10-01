namespace Gridia
{
    public class Sector
    {
        private readonly Tile[,] _tiles;

        public Sector(Tile[,] tiles) {
            _tiles = tiles;
        }

        public Tile GetTile(int x, int y)
        {
            return _tiles[x, y];
        }
    }
}
