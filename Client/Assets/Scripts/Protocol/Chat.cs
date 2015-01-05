using Newtonsoft.Json.Linq;
using Serving;
using System;
using UnityEngine;

namespace Gridia.Protocol
{
    class Chat : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var message = (String)data["msg"];
            var x = (int)data["loc"]["x"]; // :(
            var y = (int)data["loc"]["y"];
            var z = (int)data["loc"]["z"];

            var chat = Locator.Get<ChatWindow>();
            chat.append(message);

            Locator.Get<GridiaDriver>().floatingTexts.Add(new FloatingText(new Vector3(x, y, z), " " + message));
        }
    }
}
