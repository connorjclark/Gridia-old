using Gridia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class ContainerWindow : GridiaWindow
    {
        public int SlotSelected { get { return _slots.TileSelected; } set { _slots.TileSelected = value; } }
        public int SlotSelectedX { get { return _slots.TileSelectedX; } set { _slots.TileSelectedX = value; } }
        public int SlotSelectedY { get { return _slots.TileSelectedY; } set { _slots.TileSelectedY = value; } }
        // :(
        public int MouseDownSlot { get; private set; }
        public int MouseUpSlot { get; private set; }
        public int MouseOverSlot { get; private set; }

        public int ContainerId { get; private set; }
        private List<ItemRenderable> _itemRenderables;
        private ExtendibleGrid _slots = new ExtendibleGrid(Vector2.zero); // :(

        public ContainerWindow(Vector2 pos)
            : base(pos, "Container")
        {
            ResizeOnVertical = false;
            Set(new List<ItemInstance>(), 0);
            AddChild(_slots);
        }

        // :(
        public void SetWindowNameToCurrentSelection()
        {
            if (_itemRenderables != null && _slots.TileSelected < _slots.NumChildren)
            {
                WindowName = _itemRenderables[_slots.TileSelected].Item.ToString();
            }
            else
            {
                _slots.TileSelected = 0;
            }
        }

        public override void Render()
        {
            if (Event.current.type == EventType.Layout) 
            {
                MouseDownSlot = MouseUpSlot = MouseOverSlot = -1;
                SetWindowNameToCurrentSelection();
            }
            base.Render();
        }

        public void Set(List<ItemInstance> items, int id)
        {
            ContainerId = id;
            ViewItems(items);
        }

        private void ViewItems(List<ItemInstance> items) 
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

            SetWindowNameToCurrentSelection();
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

        public bool HasRaft()
        {
            return _itemRenderables.Any(itemR => itemR.Item.Item.Class == Item.ItemClass.Raft);
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
