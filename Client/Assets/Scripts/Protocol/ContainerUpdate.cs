using Newtonsoft.Json.Linq;
using Serving;
using System;

namespace Gridia.Protocol
{
    class ContainerUpdate : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var id = (int)data["id"];
            int index = (int)data["index"];
            int item = (int)data["item"];
            int quantity = (int)data["quantity"];

            var itemInstance = Locator.Get<ContentManager>().GetItem(item).GetInstance(quantity);
            Locator.Get<GridiaDriver>().GetOpenContainerWithId(id).SetItemAt(index, itemInstance);
        }
    }
}
