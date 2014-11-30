using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class EquipmentWindow : GridiaWindow
    {
        public List<ItemInstance> Items
        {
            set
            {
                ViewItems(value);
            }
        }

        private List<ItemRenderable> _itemRenderables;
        private RenderableContainer _slots = new RenderableContainer(Vector2.zero); // :(

        public EquipmentWindow(Vector2 pos)
            : base(pos, "Equipment")
        {
            Resizeable = false;
            Items = new List<ItemInstance>();
            AddChild(_slots);
        }

        public void ViewItems(List<ItemInstance> items) 
        {
            _itemRenderables = new List<ItemRenderable>();

            _slots.RemoveAllChildren();
            for (int i = 0; i < items.Count; i++)
            {
                var pos = new Vector2(0, i * 32);
                var itemRend = new ItemRenderable(pos, items[i]);
                itemRend.OnRightClick = () =>
                {
                    Locator.Get<GridiaDriver>().OpenRecipeBook(itemRend.Item);
                };
                _itemRenderables.Add(itemRend);
                _slots.AddChild(itemRend);
            }
        }

        public void SetItemAt(int index, ItemInstance item)
        {
            _itemRenderables[index].Item = item;
        }

        public ItemInstance GetItemAt(int index)
        {
            return _itemRenderables[index].Item;
        }
    }
}
