using Newtonsoft.Json.Linq;
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
            var sectorSize = connection.GetGame().tileMap.SectorSize;
            var tiles = new Tile[sectorSize, sectorSize];
            var cm = Locator.Get<ContentManager>();

            for (int x = 0; x < sectorSize; x++)
            {
                for (int y = 0; y < sectorSize; y++)
                {
                    var floor = data.ReadInt16();
                    var itemType = data.ReadInt16();
                    var itemQuantity = data.ReadInt16();
                    var tile = new Tile();
                    tile.Floor = floor;
                    tile.Item = cm.GetItem(itemType).GetInstance(itemQuantity);
                    tiles[x, y] = tile;
                }
            }
            game.tileMap.SetSector(new Sector(tiles), sx, sy, sz);

            var numCreatures = data.ReadInt32();
            for (int i = 0; i < numCreatures; i++)
            {
                var id = data.ReadInt16();
                var name = data.ReadJavaUTF();
                var x = data.ReadInt16();
                var y = data.ReadInt16();
                var z = data.ReadInt16();
                var imageType = data.ReadInt16();
                CreatureImage image = null;
                if (imageType == 0)
                {
                    var defaultImage = new DefaultCreatureImage();
                    defaultImage.SpriteIndex = data.ReadInt16();
                    defaultImage.Width = data.ReadInt16();
                    defaultImage.Height = data.ReadInt16();
                    image = defaultImage;
                }
                else if (imageType == 1)
                {
                    var customImage = new CustomPlayerImage();
                    customImage.Head = data.ReadInt16();
                    customImage.Chest = data.ReadInt16();
                    customImage.Legs = data.ReadInt16();
                    customImage.Arms = data.ReadInt16();
                    customImage.Weapon = data.ReadInt16();
                    customImage.Shield = data.ReadInt16();
                    image = customImage;
                }
                game.tileMap.CreateCreature(id, name, image, x, y, z);
            }
        }
    }
}
