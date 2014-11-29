using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class ItemUsePickWindow : GridiaWindow
    {
        public List<ItemUse> Uses
        {
            set
            {
                _useRenderables = new List<ItemRenderable>();
                Picks.RemoveAllChildren();
                for (int i = 0; i < value.Count; i++)
                {
                    var item = Locator.Get<ContentManager>().GetItem(value[i].products[0]).GetInstance(1);
                    var itemRend = new ItemRenderable(new Vector2(0, 0), item);
                    var index = i;
                    itemRend.OnClick = () => SelectUse(index);
                    _useRenderables.Add(itemRend);
                    Picks.AddChild(itemRend);
                }

                CalculateRect();
                X = (Screen.width / 2 - Width) / 2;
                Y = (Screen.height - Height) / 2;
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

        public override void Render()
        {
            base.Render();
            if (Event.current.type == EventType.KeyDown) 
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    Locator.Get<ItemUsePickWindow>().SelectUse();
                    ItemUsePickState.End();
                }
            }
        }
    }
}
