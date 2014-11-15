using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class InventoryGUI : GridiaWindow
    {
        public List<ItemInstance> Inventory
        {
            set
            {
                _itemRenderables = new List<ItemRenderable>();
                _slots.RemoveChildren();
                for (int i = 0; i < value.Count; i++)
                {
                    var itemRend = new ItemRenderable(new Rect(0, 0, 32 * _scale, 32 * _scale), value[i]);
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
        private ExtendibleGridGUI _slots = new ExtendibleGridGUI(new Rect(0, 0, 0, 0));
        private float _scale; // :(

        public InventoryGUI(Rect rect, float scale)
            : base(rect, "Inventory")
        {
            ResizeOnVertical = false;
            Inventory = new List<ItemInstance>();
            _scale = scale;
        }

        protected override void RenderContents()
        {
            _slots.Render();
        }

        public override void Render() {
            base.Render();
            RenderTooltip(); // :(
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

        private void RenderTooltip() 
        {
            if (_slots.MouseOverTile != -1)
            {
                var itemOver = _itemRenderables[_slots.MouseOverTile].Item.Item.Name;
                var rect = _slots.MouseOverRect;
                var globalRect = new Rect(Event.current.mousePosition.x + rect.x, Event.current.mousePosition.y + rect.y, rect.width, rect.height);
                GUI.Box(globalRect, itemOver);
            }
        }
    }
}
