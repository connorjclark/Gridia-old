using Newtonsoft.Json.Linq;
using Serving;

namespace Gridia.Protocol
{
    class RemoveCreature : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            int id = (int)data["id"];

            connection.GetGame().tileMap.RemoveCreature(id);
        }
    }
}
