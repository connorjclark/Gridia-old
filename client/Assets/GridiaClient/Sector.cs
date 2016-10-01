namespace Gridia
{
    public class Sector
    {
        #region Fields

        private readonly Tile[,] _tiles;

        #endregion Fields

        #region Constructors

        public Sector(Tile[,] tiles)
        {
            _tiles = tiles;
        }

        #endregion Constructors

        #region Methods

        public Tile GetTile(int x, int y)
        {
            return _tiles[x, y];
        }

        #endregion Methods
    }
}