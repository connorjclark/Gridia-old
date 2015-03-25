using Newtonsoft.Json.Linq;
using Serving;
using System;

namespace Gridia.Protocol
{
    class Alert : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var message = (string) data["message"];
            Locator.Get<HelpMenu>().AddAlert(message);
        }
    }
}
