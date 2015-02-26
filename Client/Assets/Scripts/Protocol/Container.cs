using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serving;
using System;
using System.Collections.Generic;
using UnityEngine;

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
                    var invWindow = Locator.Get<GridiaDriver>().invGui;
                    invWindow.Set(items, id);
                    invWindow.ShowSelected = true;
                    invWindow.SelectedColor = new Color32(255, 255, 0, 50);
                    break;
                case "Equipment":
                    Locator.Get<GridiaDriver>().equipmentGui.Set(items, id);
                    break;
                default:
                    Locator.Get<GridiaDriver>().AddNewContainer(items, id, tabGfxItemId);
                    break;
            }
        }
    }
}
