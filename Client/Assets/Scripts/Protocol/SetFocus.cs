using Newtonsoft.Json.Linq;
using Serving;

namespace Gridia.Protocol
{
    class SetFocus : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            ServerSelection.gameInitWaitHandle.WaitOne(); // :(
            GridiaConstants.IS_ADMIN = (bool)data["isAdmin"];
            var id = (int)data["id"];

            connection.GetGame().view.FocusId = id;
        }
    }
}
