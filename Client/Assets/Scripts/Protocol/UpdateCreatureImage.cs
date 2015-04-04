using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serving;

namespace Gridia.Protocol
{
    class UpdateCreatureImage : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var game = connection.GetGame();
            var id = (int)data["id"];

            UnityEngine.Debug.Log(data);

            var backToJson = JsonConvert.SerializeObject(data["image"]); // :(
            var image = JsonConvert.DeserializeObject<CreatureImage>(backToJson, new CreatureImageConverter());
            game.TileMap.GetCreature(id).Image = image;
        }
    }
}
