using Newtonsoft.Json.Linq;
using Serving;
using System;

namespace Gridia.Protocol
{
    class RenameCreature : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            int id = (int)data["id"];
            var name = (String)data["name"];

            connection.GetGame().tileMap.GetCreature(id).Name = name;
        }
    }
}
