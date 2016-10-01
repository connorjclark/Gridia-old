namespace Gridia.Protocol
{
    using Newtonsoft.Json.Linq;

    using Serving;

    class RemoveCreature : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        #region Methods

        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var id = (int) data["id"];

            connection.GetGame().RemoveCreature(id);
        }

        #endregion Methods
    }
}