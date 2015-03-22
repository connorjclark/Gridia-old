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
            var user = (String) data["user"];
            var text = (String) data["text"];
            var x = (int)data["loc"]["x"]; // :(
            var y = (int) data["loc"]["y"];
            var z = (int) data["loc"]["z"];

            var chat = Locator.Get<ChatWindow>();
            chat.Append(user, text);

            Locator.Get<GridiaDriver>().FloatingTexts.Add(new FloatingText(new Vector3(x, y, z), " " + text));
        }
    }
}
