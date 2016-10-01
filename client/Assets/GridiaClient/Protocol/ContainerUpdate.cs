namespace Gridia.Protocol
{
    using Newtonsoft.Json.Linq;

    using Serving;

    class ContainerUpdate : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        #region Methods

        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var id = (int) data["id"];
            var index = (int) data["index"];
            var item = (int) data["item"];
            var quantity = (int) data["quantity"];

            var itemInstance = Locator.Get<ContentManager>().GetItem(item).GetInstance(quantity);

            MainThreadQueue.Add(() =>
            {
                GameState.Instance.SetContainerItem(id, itemInstance, index);
            });
        }

        #endregion Methods
    }
}