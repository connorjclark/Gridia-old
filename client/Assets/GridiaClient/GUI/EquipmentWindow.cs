using System.Collections.Generic;
using UnityEngine;

namespace Gridia
{
    // :(
    public class EquipmentWindow : ContainerWindow
    {
        public EquipmentWindow(Vector2 pos)
            : base(pos)
        {
            WindowName = "Equipment";
            Resizeable = false;
        }

        protected override void ViewItems(List<ItemInstance> items) 
        {
            ItemRenderables = new List<ItemRenderable>();

            Slots.RemoveAllChildren();
            for (var i = 0; i < items.Count; i++)
            {
                var pos = new Vector2(0, i * 32);
                var itemRend = new ItemRenderable(pos, items[i]);
                var slotIndex = i;
                itemRend.OnDoubleClick = () =>
                {
                    Locator.Get<ConnectionToGridiaServerHandler>().UnequipItem(slotIndex);
                };
                ItemRenderables.Add(itemRend);
                Slots.AddChild(itemRend);
            }
        }
    }
}
