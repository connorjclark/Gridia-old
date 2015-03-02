using System;
using UnityEngine;

namespace Gridia 
{
    public class GridiaWindow : RenderableContainer
    {
        private static int _nextWindowId;

        public int WindowId { get; private set; }
        public Renderable Icon { get; set; }
        public bool Resizeable { get; set; }
        public bool Moveable { get; set; }
        public bool Visible { get; set; }
		public bool MouseOver { get; private set; }
		public bool DoubleClick { get; private set; }
		public bool ResizingWindow { get; private set; }
        public bool ResizeOnHorizontal { get; set; }
        public bool ResizeOnVertical { get; set; }
		public float BorderSize { get; set; }
		public String WindowName { get; set; }

        public GridiaWindow(Vector2 pos, String windowName)
            : base(pos)
        {
            _rect.width = _rect.height = 300;
            Resizeable = Moveable = Visible = true;
            ResizeOnHorizontal = ResizeOnVertical = true;
            WindowId = _nextWindowId++;
            WindowName = windowName;
            BorderSize = 25;
        }

        public override void Render() 
        {
            if (Event.current.type == EventType.Layout)
            {
                MouseOver = DoubleClick = false;
            }
            if (Event.current.type == EventType.MouseUp)
            {
                ResizingWindow = false;
            }
            // :(
			if (Event.current.type == EventType.MouseDown) {
				DoubleClick = Event.current.clickCount == 2;
			}
            if (ResizingWindow)
            {
                Resize();
            }
            if (!Visible) return;
            MouseOver = Rect.Contains(Event.current.mousePosition);

            GUI.skin = Skin;

            // what a hack...
            var modifiedRect = GUI.Window(WindowId, new Rect(X, Y, Width + BorderSize * 2, Height + BorderSize * 2), windowId =>
            {
                if (Icon != null)
                {
                    Icon.Width = 4 * BorderSize / 5;
                    Icon.Height = 4 * BorderSize / 5;
                    Icon.X = BorderSize / 5;
                    Icon.Y = BorderSize / 5;
                    Icon.Render();
                }
                if (Moveable) 
                {
                    RenderDrag();
                }
                if (Resizeable)
                {
                    RenderResize();
                }
                GUILayout.BeginArea(new Rect(BorderSize - X, BorderSize - Y, Int32.MaxValue, Int32.MaxValue));
                base.Render();
                GUILayout.EndArea();
            }, WindowName);

            Rect = new Rect(modifiedRect.x, modifiedRect.y, modifiedRect.width - BorderSize * 2, modifiedRect.height - BorderSize * 2);

            if (!ResizingWindow)
            {
                ClampPosition();
            }
        }

        public override void HandleEvents()
        {
            if (!Visible) return;
            GUI.BeginGroup(new Rect(BorderSize, BorderSize, Int32.MaxValue, Int32.MaxValue));
            base.HandleEvents();
            GUI.EndGroup();
        }

        protected virtual void Resize()
        {
            if (ResizeOnHorizontal)
            {
                var newWidth = Event.current.mousePosition.x / TrueScale.x - _rect.x - BorderSize;
                _rect.width = Mathf.Clamp(newWidth, BorderSize * 2, Screen.width - _rect.x);
            }
            if (ResizeOnVertical)
            {
                var newHeight = Event.current.mousePosition.y / TrueScale.y - _rect.y - BorderSize;
                _rect.height = Mathf.Clamp(newHeight, BorderSize * 2, Screen.height - _rect.y);
            }
        }

        protected void RenderDrag()
        {
            GUI.DragWindow(new Rect(0, 0, Width + BorderSize * 2, BorderSize));
        }

        protected void RenderResize() 
        {
            var resizeRect = new Rect(Width, Height + BorderSize, BorderSize * 2, BorderSize);
            if (Event.current.type == EventType.mouseDown && resizeRect.Contains(Event.current.mousePosition))
            {
                ResizingWindow = true;
            }
            GUI.Label(resizeRect, " ◄►");
        }

        private void ClampPosition()
        {
            if (Dirty)
            {
                CalculateRect();
                Dirty = false;
            }
            var maxX = Math.Max(0, Screen.width - Width - BorderSize * 2);
            var maxY = Math.Max(0, Screen.height - Height - BorderSize * 2);
            X = Mathf.Clamp(X, 0, maxX);
            Y = Mathf.Clamp(Y, 0, maxY);
        }
    }
}
