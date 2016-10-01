namespace Gridia
{
    using System.Collections.Generic;

    using UnityEngine;

    public class ItemUsePickWindow : GridiaWindow
    {
        #region Fields

        public ExtendibleGrid Picks = new ExtendibleGrid(Vector2.zero); // :(

        private float _scale;
        private List<ItemRenderable> _useRenderables;
        private List<ItemUse> _uses;

        #endregion Fields

        #region Constructors

        public ItemUsePickWindow(Vector2 pos)
            : base(pos, "Item use pick")
        {
            Resizeable = false;
            AddChild(Picks);
            Picks.ShowSelected = true;
        }

        #endregion Constructors

        #region Properties

        public ItemUsePickState ItemUsePickState
        {
            get; set;
        }

        public List<ItemUse> Uses
        {
            set
            {
                _uses = value;
                _useRenderables = new List<ItemRenderable>();
                Picks.RemoveAllChildren();
                for (var i = 0; i < value.Count; i++)
                {
                    var itemToShow = value[i].GetIdentifyingItem();
                    var item = Locator.Get<ContentManager>().GetItem(itemToShow).GetInstance(1);
                    var itemRend = new ItemRenderable(new Vector2(0, 0), item);
                    var index = i;
                    itemRend.OnClick = () => SelectUse(index);
                    _useRenderables.Add(itemRend);
                    Picks.AddChild(itemRend);
                }

                CalculateRect();
                X = (Screen.width / 2 - Width) / 2;
                Y = (Screen.height - Height) / 2;

                SetWindowNameToCurrentSelection();
            }
            get
            {
                return _uses;
            }
        }

        #endregion Properties

        #region Methods

        public void SelectUse()
        {
            SelectUse(Picks.TileSelected);
        }

        public void SetWindowNameToCurrentSelection()
        {
            if (Picks.TileSelected < Uses.Count)
            {
                var productId = Uses[Picks.TileSelected].GetIdentifyingItem();
                var productName = Locator.Get<ContentManager>().GetItem(productId).Name;
                WindowName = productName;
            }
            else
            {
                Picks.TileSelected = 0;
            }
        }

        private void SelectUse(int index)
        {
            Locator.Get<TabbedUI>().Remove(this);
            Locator.Get<ConnectionToGridiaServerHandler>().PickItemUse(index);
            ItemUsePickState.End();
        }

        #endregion Methods
    }
}