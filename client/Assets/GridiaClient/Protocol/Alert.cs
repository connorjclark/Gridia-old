namespace Gridia.Protocol
{
    using System;

    using Newtonsoft.Json.Linq;

    using Serving;

    class Alert : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        #region Methods

        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var message = (string) data["message"];
            Locator.Get<HelpMenu>().AddAlert(message);
        }

        #endregion Methods
    }
}