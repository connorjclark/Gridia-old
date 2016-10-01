namespace Gridia.Protocol
{
    using Newtonsoft.Json.Linq;

    using Serving;

    class TileUpdate : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        #region Methods

        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var game = connection.GetGame();
            var item = (int)data["item"];
            var quantity = (int)data["quantity"];
            var floor = (int)data["floor"];
            var x = (int)data["loc"]["x"];
            var y = (int)data["loc"]["y"];
            var z = (int)data["loc"]["z"];

            game.TileMap.SetItem(Locator.Get<ContentManager>().GetItem(item).GetInstance(quantity), x, y, z);
            game.TileMap.SetFloor(floor, x, y, z);
        }

        #endregion Methods
    }
}