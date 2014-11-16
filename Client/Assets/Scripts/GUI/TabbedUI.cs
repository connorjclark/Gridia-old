using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class TabbedUI : GridiaWindow
    {
        private List<GridiaWindow> _windows = new List<GridiaWindow>(); // :(
        private float _spacing = 5;
        private float _tabSize;
        private ExtendibleGrid _tabs = new ExtendibleGrid(new Rect(0, 0, 0, 0));

        public TabbedUI(Rect rect, float scale)
            : base(rect, "Tabs")
        {
            Moveable = Resizeable = false;
            _tabSize = scale * GridiaConstants.SPRITE_SIZE;
            Height = BorderSize * 2 + _tabSize;
        }

        protected override void RenderContents() 
        {
            _tabs.Render();

            if (_tabs.MouseUpTile != -1) 
            {
                ToggleVisiblity(_tabs.MouseUpTile);
                var tab = _tabs.GetChildAt(_tabs.MouseUpTile);
                var alpha = (byte)(_windows[_tabs.MouseUpTile].Visible ? 255 : 50);
                tab.Color = new Color32(255, 255, 255, alpha);
            }
        }

        public override void Render()
        {
            base.Render();
            _windows.ForEach(w => w.Render());
        }

        public void Add(int tabItemSprite, GridiaWindow window) 
        {
            if (!_windows.Contains(window)) 
            {
                _windows.Add(window);

                var item = Locator.Get<ContentManager>().GetItem(tabItemSprite).GetInstance();
                var tab = new ItemRenderable(new Rect(0, 0, _tabSize, _tabSize), item);
                _tabs.AddChild(tab);
            }
        }

        public void Remove(GridiaWindow window) 
        {
            var index = _windows.IndexOf(window);
            if (index != -1)
            {
                _windows.RemoveAt(index);
                _tabs.RemoveChildAt(index);
            }
        }

        public void ToggleVisiblity(int index)
        {
            if (index < _windows.Count) 
            {
                _windows[index].Visible = !_windows[index].Visible;
            }
        }

        public bool MouseOverAny() 
        {
            return MouseOver || _windows.Exists(w => w.MouseOver);
        }

        public bool ResizingAny()
        {
            return ResizingWindow || _windows.Exists(w => w.ResizingWindow);
        }
    }
}
