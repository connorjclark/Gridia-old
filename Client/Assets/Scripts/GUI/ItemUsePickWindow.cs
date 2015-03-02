using System.Collections.Generic;
using UnityEngine;

namespace Gridia
{
    public class ItemUsePickWindow : GridiaWindow
    {
        private List<ItemUse> _uses;
        public List<ItemUse> Uses
        {
            set
            {
                _uses = value;
                _useRenderables = new List<ItemRenderable>();
                Picks.RemoveAllChildren();
                for (var i = 0; i < value.Count; i++)
                {
                    var itemToShow = value[i].SuccessTool > 0 ? value[i].SuccessTool : value[i].Products[0];
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
        public ExtendibleGrid Picks = new ExtendibleGrid(Vector2.zero); // :(
        public ItemUsePickState ItemUsePickState { get; set; }
        private List<ItemRenderable> _useRenderables;
        private float _scale;

        public ItemUsePickWindow(Vector2 pos)
            : base(pos, "Item use pick")
        {
            Resizeable = false;
            AddChild(Picks);
            Picks.ShowSelected = true;
        }

        public void SetWindowNameToCurrentSelection() 
        {
            if (Picks.TileSelected < Uses.Count)
            {
                var productId = Uses[Picks.TileSelected].Products.Count != 0 ? Uses[Picks.TileSelected].Products[0] : Uses[Picks.TileSelected].SuccessTool;
                var productName = Locator.Get<ContentManager>().GetItem(productId).Name;
                WindowName = productName;
            }
            else
            {
                Picks.TileSelected = 0;
            }
        }

        public void SelectUse()
        {
            SelectUse(Picks.TileSelected);
        }

        private void SelectUse(int index) 
        {
            Locator.Get<TabbedUI>().Remove(this);
            Locator.Get<ConnectionToGridiaServerHandler>().PickItemUse(index);
            ItemUsePickState.End();
        }
    }
}
