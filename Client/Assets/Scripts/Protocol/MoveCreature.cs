using Newtonsoft.Json.Linq;
using Serving;

namespace Gridia.Protocol
{
    class MoveCreature : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var game = connection.GetGame();
            var id = (int) data["id"];
            var x = (int) data["loc"]["x"];
            var y = (int) data["loc"]["y"];
            var z = (int) data["loc"]["z"];
            var time = (long) data["time"] - Creature.RENDER_DELAY;
            var isTeleport = (bool) data["isTeleport"];
            var onRaft = (bool) data["onRaft"];

            if (id != game.View.FocusId || isTeleport)
            {
                game.TileMap.MoveCreature(id, x, y, z, onRaft, time);
                if (isTeleport)
                {
                    var cre = game.TileMap.GetCreature(id);
                    if (cre != null)
                    {
                        cre.ClearSnapshots(1);
                    }
                }
            }
        }
    }
}
