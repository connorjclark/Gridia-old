using Serving;

namespace Gridia.Protocol
{
    class SectorData : BinaryMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JavaBinaryReader data)
        {
            var game = connection.GetGame();
            var sx = data.ReadInt32();
            var sy = data.ReadInt32();
            var sz = data.ReadInt32();
            var sectorSize = connection.GetGame().TileMap.SectorSize;
            var tiles = new Tile[sectorSize, sectorSize];
            var cm = Locator.Get<ContentManager>();

            for (var x = 0; x < sectorSize; x++)
            {
                for (var y = 0; y < sectorSize; y++)
                {
                    var floor = data.ReadInt16();
                    var itemType = data.ReadInt16();
                    var itemQuantity = data.ReadInt16();
                    var tile = new Tile {Floor = floor, Item = cm.GetItem(itemType).GetInstance(itemQuantity)};
                    tiles[x, y] = tile;
                }
            }
            game.TileMap.SetSector(new Sector(tiles), sx, sy, sz);

            var numCreatures = data.ReadInt32();
            for (var i = 0; i < numCreatures; i++)
            {
                var id = data.ReadInt16();
                var name = data.ReadJavaUTF();
                var x = data.ReadInt16();
                var y = data.ReadInt16();
                var z = data.ReadInt16();
                var imageType = data.ReadInt16();
                CreatureImage image = null;
                switch (imageType)
                {
                    case 0:
                        var defaultImage = new DefaultCreatureImage
                        {
                            SpriteIndex = data.ReadInt16(),
                            Width = data.ReadInt16(),
                            Height = data.ReadInt16()
                        };
                        image = defaultImage;
                        break;
                    case 1:
                        var customImage = new CustomPlayerImage
                        {
                            Head = data.ReadInt16(),
                            Chest = data.ReadInt16(),
                            Legs = data.ReadInt16(),
                            Arms = data.ReadInt16(),
                            Weapon = data.ReadInt16(),
                            Shield = data.ReadInt16()
                        };
                        image = customImage;
                        break;
                }
                game.CreateCreature(id, name, image, x, y, z);
            }
        }
    }
}
