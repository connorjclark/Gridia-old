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
        private ExtendibleGrid _tabs = new ExtendibleGrid(Vector2.zero);

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
                var tab = new ItemImageRenderable(Vector2.zero, tabItemSpriteIndex);
                tab.ToolTip = () => window.WindowName;
                tab.OnClick = () => ToggleVisiblity(window);
                _tabs.AddChild(tab);
                SetTabTransparency(_tabs.NumChildren - 1);
                Dirty = true;
            }
        }

        public void Remove(GridiaWindow window) 
        {
            var index = _windows.IndexOf(window);
            if (index != -1)
            {
                _windows.RemoveAt(index);
                _tabs.RemoveChildAt(index);
                Dirty = true;
            }
        }

        public void ToggleVisiblity(GridiaWindow window)
        {
            ToggleVisiblity(_windows.IndexOf(window));
        }

        public void ToggleVisiblity(int index)
        {
            if (index < _windows.Count) 
            {
                _windows[index].Visible = !_windows[index].Visible;
                SetTabTransparency(index);
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

        private void SetTabTransparency(int index) 
        {
            var tab = _tabs.GetChildAt(index);
            var alpha = (byte)(_windows[index].Visible ? 255 : 100);
            tab.Color = new Color32(255, 255, 255, alpha);
        }

        class ItemImageRenderable : SpriteRenderable
        {
            public int SpriteIndex { get; set; }

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
                return textures.Items[spriteIndex / GridiaConstants.SPRITES_IN_SHEET];
            }
        }
    }
}
