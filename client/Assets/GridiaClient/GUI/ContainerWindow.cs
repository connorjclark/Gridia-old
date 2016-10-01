using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gridia
{
    public class ContainerWindow : GridiaWindow
    {
        public int SlotSelected { get { return Slots.TileSelected; } set { Slots.TileSelected = value; } }
        public int SlotSelectedX { get { return Slots.TileSelectedX; } set { Slots.TileSelectedX = value; } }
        public int SlotSelectedY { get { return Slots.TileSelectedY; } set { Slots.TileSelectedY = value; } }
        // :(
        public int MouseDownSlot { get; private set; }
        public int MouseUpSlot { get; private set; }
        public int MouseOverSlot { get; private set; }

        public int ContainerId { get; private set; }

        public bool ShowSelected
        {
            get { return Slots.ShowSelected; }
            set { Slots.ShowSelected = value; }
        }

        public Color SelectedColor
        {
            get { return Slots.SelectedColor; }
            set { Slots.SelectedColor = value; }
        }

        protected List<ItemRenderable> ItemRenderables;
        protected ExtendibleGrid Slots = new ExtendibleGrid(Vector2.zero); // :(

        public ContainerWindow(Vector2 pos)
            : base(pos, "Container")
        {
            ResizeOnVertical = false;
            Set(new List<ItemInstance>(), 0);
            AddChild(Slots);
        }

        // :(
        public void SetWindowNameToCurrentSelection()
        {
            if (ItemRenderables != null && Slots.TileSelected < Slots.NumChildren)
            {
                WindowName = ItemRenderables[Slots.TileSelected].Item.ToString();
            }
            else
            {
                Slots.TileSelected = 0;
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

        protected virtual void ViewItems(List<ItemInstance> items) 
        {
            ItemRenderables = new List<ItemRenderable>();

            Slots.RemoveAllChildren();
            ShowSelected = false;
            for (var i = 0; i < items.Count; i++)
            {
                var itemRend = new ItemRenderable(Vector2.zero, items[i]);
                var slotIndex = i;
                itemRend.OnRightClick = () =>
                {
                    Locator.Get<GridiaDriver>().OpenRecipeBook(itemRend.Item);
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
                ItemRenderables.Add(itemRend);
                Slots.AddChild(itemRend);
            }

            SetWindowNameToCurrentSelection();
        }

        public void SetItemAt(int index, ItemInstance item)
        {
            ItemRenderables[index].Item = item;
        }

        public ItemInstance GetItemAt(int index)
        {
            return ItemRenderables[index].Item;
        }

        public void EquipItemAtCurrentSelection()
        {
            EquipItemAt(SlotSelected);
        }

        public bool HasRaft()
        {
            return ItemRenderables.Any(itemR => itemR.Item.Item.Class == Item.ItemClass.Raft);
        }

        private void EquipItemAt(int slotIndex)
        {
            Locator.Get<ConnectionToGridiaServerHandler>().EquipItem(slotIndex);
        }

        protected override void Resize()
        {
            base.Resize();
            Slots.FitToWidth(Width - BorderSize * 2);

            var availableHeight = Screen.height - BorderSize * 2;
            var maxTilesColumn = Mathf.FloorToInt(availableHeight / Slots.GetTileHeight());
            while (Slots.TilesColumn > maxTilesColumn)
            {
                Slots.SetTilesAcross(Slots.TilesAcross + 1);
            }
        }
    }
}
