using Newtonsoft.Json.Linq;
using Serving;

namespace Gridia.Protocol
{
    class RenameCreature : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var id = (int) data["id"];
            var name = (string) data["name"];

            connection.GetGame().TileMap.GetCreature(id).Name = name;
        }
    }
}
