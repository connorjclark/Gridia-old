using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class InventoryWindow : GridiaWindow
    {
        public List<ItemInstance> Items
        {
            set
            {
                ViewItems(value);
            }
        }

        public int SlotSelected { get { return _slots.TileSelected; } set { _slots.TileSelected = value; } }
        public int SlotSelectedX { get { return _slots.TileSelectedX; } set { _slots.TileSelectedX = value; } }
        public int SlotSelectedY { get { return _slots.TileSelectedY; } set { _slots.TileSelectedY = value; } }
        // :(
        public int MouseDownSlot { get; private set; }
        public int MouseUpSlot { get; private set; }
        public int MouseOverSlot { get; private set; }

        private List<ItemRenderable> _itemRenderables;
        private ExtendibleGrid _slots = new ExtendibleGrid(Vector2.zero); // :(

        public InventoryWindow(Vector2 pos)
            : base(pos, "Inventory")
        {
            ResizeOnVertical = false;
            Items = new List<ItemInstance>();
            AddChild(_slots);
        }

        public override void Render()
        {
            if (Event.current.type == EventType.Layout) 
            {
                MouseDownSlot = MouseUpSlot = MouseOverSlot = -1;
            }
            base.Render();
        }

        public void ViewItems(List<ItemInstance> items) 
        {
            _itemRenderables = new List<ItemRenderable>();

            _slots.RemoveAllChildren();
            for (int i = 0; i < items.Count; i++)
            {
                var itemRend = new ItemRenderable(Vector2.zero, items[i]);
                var slotIndex = i;
                itemRend.OnRightClick = () =>
                {
                    //Locator.Get<GridiaDriver>().OpenRecipeBook(itemRend.Item);
                };
                itemRend.OnMouseDown = () => MouseDownSlot = slotIndex;
                itemRend.OnClick = () =>
                {
                    MouseUpSlot = slotIndex;
                    SlotSelected = slotIndex;
                };
                itemRend.OnMouseOver = () => MouseOverSlot = slotIndex;
                itemRend.OnDoubleClick = () =>
                {
                    EquipItemAt(slotIndex);
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

        public void EquipItemAtCurrentSelection()
        {
            EquipItemAt(SlotSelected);
        }

        private void EquipItemAt(int slotIndex)
        {
            Locator.Get<ConnectionToGridiaServerHandler>().EquipItem(slotIndex);
        }

        protected override void Resize()
        {
            base.Resize();
            _slots.FitToWidth(Width - BorderSize * 2);

            var availableHeight = Screen.height - BorderSize * 2;
            int maxTilesColumn = Mathf.FloorToInt(availableHeight / _slots.GetTileHeight());
            while (_slots.TilesColumn > maxTilesColumn)
            {
                _slots.SetTilesAcross(_slots.TilesAcross + 1);
            }
        }
    }
}
