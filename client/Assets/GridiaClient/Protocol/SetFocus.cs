namespace Gridia.Protocol
{
    using Newtonsoft.Json.Linq;

    using Serving;

    class SetFocus : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        #region Methods

        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            GridiaDriver.GameInitWaitHandle.WaitOne(); // :(
            var game = connection.GetGame();
            GridiaConstants.IsAdmin = (bool) data["isAdmin"];
            var id = (int) data["id"];

            game.View.FocusId = id;
            if (GridiaConstants.IsAdmin)
            {
                game.InitAdminWindowTab();
            }
        }

        #endregion Methods
    }
}