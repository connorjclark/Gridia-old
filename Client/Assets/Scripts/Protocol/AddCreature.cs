using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serving;

namespace Gridia.Protocol
{
    class AddCreature : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var id = (int) data["id"];
            var name = (string) data["name"];

            var backToJson = JsonConvert.SerializeObject(data["image"]); // :(
            var image = JsonConvert.DeserializeObject<CreatureImage>(backToJson, new CreatureImageConverter());

            var x = (int) data["loc"]["x"];
            var y = (int) data["loc"]["y"];
            var z = (int) data["loc"]["z"];

            connection.GetGame().TileMap.CreateCreature(id, name, image, x, y, z);
        }
    }
}
