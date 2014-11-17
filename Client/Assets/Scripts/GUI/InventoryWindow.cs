using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class InventoryWindow : GridiaWindow
    {
        public List<ItemInstance> Inventory
        {
            set
            {
                _itemRenderables = new List<ItemRenderable>();
                _slots.RemoveAllChildren();
                for (int i = 0; i < value.Count; i++)
                {
                    var itemRend = new ItemRenderable(new Rect(0, 0, 32 * _scale, 32 * _scale), value[i]);
                    var slotIndex = i;
                    itemRend.OnRightClick = () => 
                    {
                        Locator.Get<GridiaDriver>().OpenRecipeBook(itemRend.Item);
                    };
                    _itemRenderables.Add(itemRend);
                    _slots.AddChild(itemRend);
                }

                Width = _slots.Width + BorderSize * 2;
                Height = _slots.Height + BorderSize * 2;
            }
        }

        public int SlotSelected { get { return _slots.TileSelected; } set { _slots.TileSelected = value; } }
        public int MouseDownSlot { get { return _slots.MouseDownTile; } }
        public int MouseUpSlot { get { return _slots.MouseUpTile; } }
        public int MouseOverSlot { get { return _slots.MouseOverTile; } }

        private List<ItemRenderable> _itemRenderables;
        private ExtendibleGrid _slots = new ExtendibleGrid(new Rect(0, 0, 0, 0));
        private float _scale; // :(

        public InventoryWindow(Rect rect, float scale)
            : base(rect, "Inventory")
        {
            ResizeOnVertical = false;
            Inventory = new List<ItemInstance>();
            _scale = scale;
            _slots.TileSelected = 0;
        }

        protected override void RenderContents()
        {
            _slots.Render();
        }

        public void SetItemAt(int index, ItemInstance item)
        {
            _itemRenderables[index].Item = item;
        }

        public ItemInstance GetItemAt(int index)
        {
            return _itemRenderables[index].Item;
        }

        protected override void Resize()
        {
            base.Resize();
            _slots.FitToWidth(Width - BorderSize * 2);
            Width = Math.Max(Width, BorderSize * 2 + _slots.Width);
            Height = BorderSize * 2 + _slots.Height;
        }
    }
}
