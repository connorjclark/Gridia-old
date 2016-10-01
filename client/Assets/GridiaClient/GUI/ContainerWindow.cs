namespace Gridia
{
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    public class ContainerWindow : GridiaWindow
    {
        #region Fields

        protected List<ItemRenderable> ItemRenderables;
        protected ExtendibleGrid Slots = new ExtendibleGrid(Vector2.zero); // :(

        #endregion Fields

        #region Constructors

        public ContainerWindow(Vector2 pos)
            : base(pos, "Container")
        {
            ResizeOnVertical = false;
            Set(new List<ItemInstance>(), 0);
            AddChild(Slots);
        }

        #endregion Constructors

        #region Properties

        public int ContainerId
        {
            get; private set;
        }

        // :(
        public int MouseDownSlot
        {
            get; private set;
        }

        public int MouseOverSlot
        {
            get; private set;
        }

        public int MouseUpSlot
        {
            get; private set;
        }

        public Color SelectedColor
        {
            get { return Slots.SelectedColor; }
            set { Slots.SelectedColor = value; }
        }

        public bool ShowSelected
        {
            get { return Slots.ShowSelected; }
            set { Slots.ShowSelected = value; }
        }

        public int SlotSelected
        {
            get { return Slots.TileSelected; } set { Slots.TileSelected = value; }
        }

        public int SlotSelectedX
        {
            get { return Slots.TileSelectedX; } set { Slots.TileSelectedX = value; }
        }

        public int SlotSelectedY
        {
            get { return Slots.TileSelectedY; } set { Slots.TileSelectedY = value; }
        }

        #endregion Properties

        #region Methods

        public void EquipItemAtCurrentSelection()
        {
            EquipItemAt(SlotSelected);
        }

        public ItemInstance GetItemAt(int index)
        {
            return ItemRenderables[index].Item;
        }

        public bool HasRaft()
        {
            return ItemRenderables.Any(itemR => itemR.Item.Item.Class == Item.ItemClass.Raft);
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

        public void SetItemAt(int index, ItemInstance item)
        {
            ItemRenderables[index].Item = item;
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

        private void EquipItemAt(int slotIndex)
        {
            Locator.Get<ConnectionToGridiaServerHandler>().EquipItem(slotIndex);
        }

        #endregion Methods
    }
}