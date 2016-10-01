namespace Gridia.Protocol
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Serving;

    using UnityEngine;

    class Container : JsonMessageHandler<ConnectionToGridiaServerHandler>
    {
        #region Methods

        protected override void Handle(ConnectionToGridiaServerHandler connection, JObject data)
        {
            var backToJson = JsonConvert.SerializeObject(data["items"]); // :(
            var items = JsonConvert.DeserializeObject<List<ItemInstance>>(backToJson, new ItemInstanceConverter());
            var id = (int)data["id"];
            var type = (String)data["type"];
            var tabGfxItemId = (int)data["tabGfxItemId"];

            if (type == "Inventory")
            {
                // TODO for some reason this is getting reset. check server for why.
                GameState.Instance.InventoryContainerId = id;
            }

            if (type == "Equipment")
            {
                GameState.Instance.EquipmentContainerId = id;
            }

            MainThreadQueue.Add(() =>
            {
                GameState.Instance.SetContainerItems(id, items);
            });
        }

        #endregion Methods
    }
}