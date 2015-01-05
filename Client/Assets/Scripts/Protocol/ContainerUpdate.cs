using Newtonsoft.Json.Linq;
using Serving;
using System;

namespace Gridia.Protocol
{
    class ContainerUpdate : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var type = (String)data["type"];
            int index = (int)data["index"];
            int item = (int)data["item"];
            int quantity = (int)data["quantity"];

            var itemInstance = Locator.Get<ContentManager>().GetItem(item).GetInstance(quantity);
            // :(
            if (type == "Inventory")
            {
                Locator.Get<InventoryWindow>().SetItemAt(index, itemInstance);
            }
            else if (type == "Equipment")
            {
                Locator.Get<EquipmentWindow>().SetItemAt(index, itemInstance);
            }
        }
    }
}
