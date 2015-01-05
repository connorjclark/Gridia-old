using Newtonsoft.Json.Linq;
using Serving;

namespace Gridia.Protocol
{
    class MoveCreature : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var game = connection.GetGame();
            int id = (int)data["id"];
            int x = (int)data["loc"]["x"];
            int y = (int)data["loc"]["y"];
            int z = (int)data["loc"]["z"];
            long time = (long)data["time"];
            bool isTeleport = (bool)data["isTeleport"];
            bool onRaft = (bool)data["onRaft"];

            if (id != game.view.FocusId || isTeleport)
            {
                game.tileMap.MoveCreature(id, x, y, z, onRaft, time);
                if (isTeleport)
                {
                    var cre = game.tileMap.GetCreature(id);
                    if (cre != null)
                    {
                        cre.ClearSnapshots(1);
                    }
                }
            }
        }
    }
}
