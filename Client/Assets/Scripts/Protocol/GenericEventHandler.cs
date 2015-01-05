using Newtonsoft.Json.Linq;
using Serving;

namespace Gridia.Protocol
{
    class GenericEventHandler : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            connection.GenericEventHandler(data);
        }
    }
}
