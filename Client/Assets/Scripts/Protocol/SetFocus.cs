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
            GridiaConstants.IS_ADMIN = (bool)data["isAdmin"];
            var id = (int)data["id"];

            game.view.FocusId = id;
            if (GridiaConstants.IS_ADMIN)
            {
                game.InitAdminWindowTab();
            }
        }
    }
}
