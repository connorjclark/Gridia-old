using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serving;
using System;

namespace Gridia.Protocol
{
    class AddCreature : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            int id = (int)data["id"];
            var name = (String)data["name"];

            var backToJson = JsonConvert.SerializeObject(data["image"]); // :(
            var image = JsonConvert.DeserializeObject<CreatureImage>(backToJson, new CreatureImageConverter());

            int x = (int)data["loc"]["x"];
            int y = (int)data["loc"]["y"];
            int z = (int)data["loc"]["z"];

            connection.GetGame().tileMap.CreateCreature(id, name, image, x, y, z);
        }
    }
}
