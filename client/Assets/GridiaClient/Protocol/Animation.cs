namespace Gridia.Protocol
{
    using Newtonsoft.Json.Linq;

    using Serving;

    using UnityEngine;

    class Animation : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        #region Methods

        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var game = connection.GetGame();
            var name = (string)data["name"];
            var x = (int)data["loc"]["x"];
            var y = (int)data["loc"]["y"];
            var z = (int)data["loc"]["z"];

            if (game.View.Focus == null || z == game.View.Focus.Position.z)
            {
                var coord = new Vector3(x, y, z);
                var anim = Locator.Get<ContentManager>().GetAnimation(name);
                var animRenderable = new AnimationRenderable(coord, anim);
                Locator.Get<GridiaGame>().Animations.Add(animRenderable);
            }
        }

        #endregion Methods
    }
}