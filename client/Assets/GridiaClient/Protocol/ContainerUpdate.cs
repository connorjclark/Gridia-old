using Newtonsoft.Json.Linq;
using Serving;
using MarkLight.UnityProject;

namespace Gridia.Protocol
{
    class ContainerUpdate : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var id = (int) data["id"];
            var index = (int) data["index"];
            var item = (int) data["item"];
            var quantity = (int) data["quantity"];

            var itemInstance = Locator.Get<ContentManager>().GetItem(item).GetInstance(quantity);
            
            if (id == Main.Instance.InventoryContainerId)
            {
                MainThreadQueue.Add(() =>
                {
                    Main.Instance.SetContainerItem(id, itemInstance, index);
                });
            }

            // var container = Locator.Get<GridiaDriver>().GetOpenContainerWithId(id);
            // if (container != null) 
            // {
            //     container.SetItemAt(index, itemInstance);
            // }
            // else
            // {
            //     UnityEngine.Debug.Log("Null contianer ....");
            // }
        }
    }
}
