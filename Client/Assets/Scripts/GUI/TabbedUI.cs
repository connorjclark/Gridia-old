using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gridia
{
    public class TabbedUI : GridiaWindow
    {
        private readonly List<GridiaWindow> _windows = new List<GridiaWindow>(); // :(
        private readonly ExtendibleGrid _tabs = new ExtendibleGrid(Vector2.zero);

        public TabbedUI(Vector2 pos)
            : base(pos, "Tabs")
        {
            _tabs.TileSelected = -1;
            Moveable = Resizeable = false;
            AddChild(_tabs);
        }

        public override void Render()
        {
            base.Render();
            _windows.ForEach(w => w.Render());
        }

        public override void HandleEvents()
        {
            base.HandleEvents();
            _windows.ForEach(w => w.HandleEvents());
        }

        public void Add(int tabItemSpriteIndex, GridiaWindow window, bool visible) 
        {
            if (!_windows.Contains(window)) 
            {
                window.Visible = visible;
                _windows.Add(window);
                var tab = new ItemImageRenderable(Vector2.zero, tabItemSpriteIndex)
                {
                    ToolTip = () => window.WindowName,
                    OnClick = () => ToggleVisiblity(window)
                };
                _tabs.AddChild(tab);
                SetTabTransparency(_tabs.NumChildren - 1);
                Dirty = true;
                window.Icon = new ItemImageRenderable(Vector2.zero, tabItemSpriteIndex);
            }
        }

        public void Remove(GridiaWindow window) 
        {
            var index = _windows.IndexOf(window);
            if (index == -1) return;
            _windows.RemoveAt(index);
            _tabs.RemoveChildAt(index);
            _tabs.CalculateRect();
            Dirty = true;
            _rect.x = Int32.MaxValue;
        }

        public void ToggleVisiblity(GridiaWindow window)
        {
            ToggleVisiblity(_windows.IndexOf(window));
        }

        public void ToggleVisiblity(int index)
        {
            if (index >= _windows.Count) return;
            _windows[index].Visible = !_windows[index].Visible;
            SetTabTransparency(index);
        }

        public bool MouseOverAny() 
        {
            return MouseOver || _windows.Exists(w => w.MouseOver);
        }

        public bool ResizingAny()
        {
            return ResizingWindow || _windows.Exists(w => w.ResizingWindow);
        }

        private void SetTabTransparency(int index) 
        {
            var tab = _tabs.GetChildAt(index);
            var alpha = (byte)(_windows[index].Visible ? 255 : 100);
            tab.Color = new Color32(255, 255, 255, alpha);
        }

        public int NumWindows()
        {
            return _windows.Count;
        }

        public GridiaWindow GetWindowAt(int index)
        {
            return _windows[index];
        }

        class ItemImageRenderable : SpriteRenderable
        {
            private int SpriteIndex { get; set; }

            public ItemImageRenderable(Vector2 pos, int spriteIndex)
                : base(pos)
            {
                SpriteIndex = spriteIndex;
            }

            public override int GetSpriteIndex()
            {
                return SpriteIndex;
            }

            public override Texture GetTexture(int spriteIndex)
            {
                var textures = Locator.Get<TextureManager>(); // :(
                return textures.Items.GetTextureForSprite(spriteIndex);
            }
        }
    }
}
