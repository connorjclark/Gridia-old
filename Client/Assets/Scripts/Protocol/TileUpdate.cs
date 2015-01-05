using Newtonsoft.Json.Linq;
using Serving;

namespace Gridia.Protocol
{
    class TileUpdate : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var game = connection.GetGame();
            var item = (int)data["item"];
            var quantity = (int)data["quantity"];
            var floor = (int)data["floor"];
            var x = (int)data["loc"]["x"];
            var y = (int)data["loc"]["y"];
            var z = (int)data["loc"]["z"];

            game.tileMap.SetItem(Locator.Get<ContentManager>().GetItem(item).GetInstance(quantity), x, y, z);
            game.tileMap.SetFloor(floor, x, y, z);
        }
    }
}
