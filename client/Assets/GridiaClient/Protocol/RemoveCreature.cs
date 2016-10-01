using Newtonsoft.Json.Linq;
using Serving;

namespace Gridia.Protocol
{
    class RemoveCreature : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var id = (int) data["id"];

            connection.GetGame().RemoveCreature(id);
        }
    }
}
