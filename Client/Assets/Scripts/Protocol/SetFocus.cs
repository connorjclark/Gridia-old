using Newtonsoft.Json.Linq;
using Serving;

namespace Gridia.Protocol
{
    class SetFocus : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            ServerSelection.gameInitWaitHandle.WaitOne(); // :(
            var game = connection.GetGame();
            GridiaConstants.IsAdmin = (bool) data["isAdmin"];
            var id = (int) data["id"];

            game.View.FocusId = id;
            if (GridiaConstants.IsAdmin)
            {
                game.InitAdminWindowTab();
            }
        }
    }
}
