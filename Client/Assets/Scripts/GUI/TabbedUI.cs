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
        private List<GridiaWindow> _windows = new List<GridiaWindow>();
        private float _spacing = 5;
        private float _tabSize;

        public TabbedUI(Vector2 position, float scale)
            : base(position, "Tabs")
        {
            Moveable = Resizeable = false;
            _tabSize = scale * GridiaConstants.SPRITE_SIZE;
            var resized = WindowRect;
            resized.height = BorderSize * 2 + _tabSize; // :(
            WindowRect = resized;
        }

        protected override void RenderContents() 
        {
            for (int i = 0; i < _windows.Count; i++)
            {
                var rect = new Rect(i * (_tabSize + _spacing), 0, _tabSize, _tabSize);
                var itemId = _tabItemSprites[i];
                bool tabOpen = _windows[i].Visible;
                if (GUI.Button(rect, "")) {
                    _windows[i].Visible = !tabOpen;
                }
                var alpha = (byte)(tabOpen ? 255 : 50);
                GUI.color = new Color32(255, 255, 255, alpha);
                RenderSlot(rect, Locator.Get<ContentManager>().GetItem(itemId).GetInstance());
            }
        }

        public override void Render()
        {
            base.Render();
            _windows.ForEach(w => w.Render());
        }

        public void Add(int tabItemSprite, GridiaWindow window) 
        {
            _tabItemSprites.Add(tabItemSprite);
            _windows.Add(window);
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
