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

            if (type == "Inventory")
            {
                Locator.Get<GridiaDriver>().invGui.Set(items, id);
            }
            else if (type == "Equipment")
            {
                Locator.Get<GridiaDriver>().equipmentGui.Set(items, id);
            }
            else
            {
                Locator.Get<GridiaDriver>().AddNewContainer(items, id, tabGfxItemId);
            }
        }
    }
}
