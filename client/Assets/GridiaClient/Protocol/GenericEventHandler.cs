namespace Gridia.Protocol
{
    using Newtonsoft.Json.Linq;

    using Serving;

    class GenericEventHandler : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        #region Methods

        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            connection.GenericEventHandler(data);
        }

        #endregion Methods
    }
}