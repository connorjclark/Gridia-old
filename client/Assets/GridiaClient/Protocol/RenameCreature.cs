namespace Gridia.Protocol
{
    using Newtonsoft.Json.Linq;

    using Serving;

    class RenameCreature : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        #region Methods

        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var id = (int) data["id"];
            var name = (string) data["name"];

            connection.GetGame().TileMap.GetCreature(id).Name = name;
        }

        #endregion Methods
    }
}