using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            _itemRenderables = new List<ItemRenderable>();

            _slots.RemoveAllChildren();
            for (int i = 0; i < items.Count; i++)
            {
                var pos = new Vector2(0, i * 32);
                var itemRend = new ItemRenderable(pos, items[i]);
                var slotIndex = i;
                itemRend.OnDoubleClick = () =>
                {
                    Locator.Get<ConnectionToGridiaServerHandler>().UnequipItem(slotIndex);
                };
                _itemRenderables.Add(itemRend);
                _slots.AddChild(itemRend);
            }
        }
    }
}
