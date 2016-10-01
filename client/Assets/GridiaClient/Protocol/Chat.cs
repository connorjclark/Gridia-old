namespace Gridia.Protocol
{
    using System;

    using Newtonsoft.Json.Linq;

    using Serving;

    using UnityEngine;

    class Chat : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        #region Methods

        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var user = (String) data["user"];
            var text = (String) data["text"];
            var x = (int)data["loc"]["x"]; // :(
            var y = (int) data["loc"]["y"];
            var z = (int) data["loc"]["z"];

            var chat = Locator.Get<ChatWindow>();
            MainThreadQueue.Add(() => chat.Append(user, text));

            Locator.Get<GridiaDriver>().FloatingTexts.Add(new FloatingText(new Vector3(x, y, z), " " + text));
        }

        #endregion Methods
    }
}