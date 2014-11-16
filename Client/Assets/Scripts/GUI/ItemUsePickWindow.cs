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
                _picks.RemoveChildren();
                for (int i = 0; i < value.Count; i++)
                {
                    var item = Locator.Get<ContentManager>().GetItem(value[i].products[0]).GetInstance(1);
                    var itemRend = new ItemRenderable(new Rect(0, 0, 32 * _scale, 32 * _scale), item);
                    _useRenderables.Add(itemRend);
                    _picks.AddChild(itemRend);
                }

                Width = _picks.Width + BorderSize * 2;
                Height = _picks.Height + BorderSize * 2;
                X = (Screen.width / 2 - Width) / 2;
                Y = (Screen.height - Height) / 2;
            }
        }
        private ExtendibleGrid _picks = new ExtendibleGrid(new Rect(0, 0, 0, 0));
        private List<ItemRenderable> _useRenderables;
        private float _scale;

        public ItemUsePickWindow(Rect rect, float scale)
            : base(rect, "Item use pick")
        {
            _scale = scale;
            Resizeable = false;
        }

        protected override void RenderContents()
        {
            _picks.Render();
            // :(
            if (_picks.MouseUpTile != -1)
            {
                Locator.Get<TabbedUI>().Remove(this);
                Locator.Get<ConnectionToGridiaServerHandler>().PickItemUse(_picks.MouseUpTile);
            }
        }
    }
}
