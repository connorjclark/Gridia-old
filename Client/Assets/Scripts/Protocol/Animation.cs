using Newtonsoft.Json.Linq;
using Serving;
using UnityEngine;

namespace Gridia.Protocol
{
    class Animation : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var game = connection.GetGame();
            var animId = (int)data["anim"];
            var x = (int)data["loc"]["x"];
            var y = (int)data["loc"]["y"];
            var z = (int)data["loc"]["z"];

            if (game.view.Focus == null || z == game.view.Focus.Position.z)
            {
                var coord = new Vector3(x, y, z);
                var anim = Locator.Get<ContentManager>().GetAnimation(animId);
                var animRenderable = new AnimationRenderable(coord, anim);
                Locator.Get<GridiaGame>().animations.Add(animRenderable);
            }
        }
    }
}
