using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class TabbedUI : GridiaWindow
    {
        private List<int> _tabItemSprites = new List<int>();
        private List<bool> _tabEnabled = new List<bool>(); // :(
        private List<GridiaWindow> _windows = new List<GridiaWindow>(); // :(
        private float _spacing = 5;
        private float _tabSize;

        public TabbedUI(Rect rect, float scale)
            : base(rect, "Tabs")
        {
            Moveable = Resizeable = false;
            _tabSize = scale * GridiaConstants.SPRITE_SIZE;
            Height = BorderSize * 2 + _tabSize;
        }

        protected override void RenderContents() 
        {
            int tabsRendered = 0;
            for (int i = 0; i < _windows.Count; i++)
            {
                if (!_tabEnabled[i])
                {
                    continue;
                }
                var rect = new Rect(tabsRendered++ * (_tabSize + _spacing), 0, _tabSize, _tabSize);
                var itemId = _tabItemSprites[i];
                bool tabOpen = _windows[i].Visible;
                if (GUI.Button(rect, "")) {
                    ToggleVisiblity(i);
                }
                var alpha = (byte)(tabOpen ? 255 : 50);
                GUI.color = new Color32(255, 255, 255, alpha);
                RenderSlot(rect, Locator.Get<ContentManager>().GetItem(itemId).GetInstance());
            }
        }

        public override void Render()
        {
            base.Render();
            for (int i = 0; i < _windows.Count; i++)
            {
                if (_tabEnabled[i])
                {
                    _windows[i].Render();
                }
            }
        }

        public void Add(int tabItemSprite, GridiaWindow window, bool enabled = true) 
        {
            _tabItemSprites.Add(tabItemSprite);
            _windows.Add(window);
            _tabEnabled.Add(enabled);
        }

        public void ToggleVisiblity(int index)
        {
            if (index < _windows.Count) 
            {
                _windows[index].Visible = !_windows[index].Visible;
            }
        }

        public void Enable(GridiaWindow window, bool enabled) 
        {
            _tabEnabled[_windows.IndexOf(window)] = enabled;
        }

        public void DisableAll() 
        {
            _windows.ForEach(w => Enable(w, false));
        }

        public void EnableAll()
        {
            _windows.ForEach(w => Enable(w, true));
        }

        public bool IsEnabled(GridiaWindow window) 
        {
            return _tabEnabled[_windows.IndexOf(window)];
        }

        public bool MouseOverAny() 
        {
            return MouseOver || _windows.Exists(w => IsEnabled(w) && w.MouseOver);
        }

        public bool ResizingAny()
        {
            return ResizingWindow || _windows.Exists(w => IsEnabled(w) && w.ResizingWindow);
        }
    }
}
