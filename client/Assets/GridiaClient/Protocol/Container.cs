using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serving;
using System;
using System.Collections.Generic;
using UnityEngine;
using MarkLight.UnityProject;

namespace Gridia.Protocol
{
    class Container : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var backToJson = JsonConvert.SerializeObject(data["items"]); // :(
            var items = JsonConvert.DeserializeObject<List<ItemInstance>>(backToJson, new ItemInstanceConverter());
            var id = (int)data["id"];
            var type = (String)data["type"];
            var tabGfxItemId = (int)data["tabGfxItemId"];

            switch (type)
            {
                case "Inventory":
                    Main.Instance.InventoryContainerId = id;
                    Main.Instance.SetContainerItems(id, items);
                    break;
                case "Equipment":
                    //Locator.Get<GridiaDriver>().EquipmentGui.Set(items, id);
                    break;
                default:
                    //Locator.Get<GridiaDriver>().AddNewContainer(items, id, tabGfxItemId);
                    break;
            }
        }
    }
}
