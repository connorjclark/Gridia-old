using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia 
{
    public abstract class GridiaWindow : Renderable
    {
        private static int _NEXT_WINDOW_ID;

        // :(
        public static void RenderSlot(Rect loc, ItemInstance item)
        {
            GUI.Box(loc, "");

            if (item.Item.Animations == null) return; // :(

            var textures = Locator.Get<TextureManager>(); // :(

            int spriteId = item.Item.Animations[0];
            int textureX = (spriteId % GridiaConstants.SPRITES_IN_SHEET) % GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
            int textureY = 9 - (spriteId % GridiaConstants.SPRITES_IN_SHEET) / GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
            var texCoords = new Rect(textureX / 10.0f, textureY / 10.0f, 1 / 10.0f, 1 / 10.0f); // :( don't hardcode 10
            GUI.DrawTextureWithTexCoords(loc, textures.GetItemsTexture(spriteId / GridiaConstants.SPRITES_IN_SHEET), texCoords);
        }

        public int WindowId { get; private set; }
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

        public GridiaWindow(Rect rect, String windowName)
            : base(rect)
        {
            Resizeable = Moveable = Visible = true;
            ResizeOnHorizontal = ResizeOnVertical = true;
            WindowId = _NEXT_WINDOW_ID++;
            WindowName = windowName;
            BorderSize = 20;
        }

        protected abstract void RenderContents();

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
			if (Event.current.type == EventType.MouseDown) {
				DoubleClick = Event.current.clickCount == 2;
			}
            if (ResizingWindow)
            {
                Resize();
            }
            if (Visible) 
            {
                MouseOver = Rect.Contains(Event.current.mousePosition);

                Rect = GUI.Window(WindowId, Rect, windowId =>
                {
                    if (Moveable) 
                    {
                        RenderDrag();
                    }
                    if (Resizeable)
                    {
                        RenderResize();
                    }
                    GUILayout.BeginArea(new Rect(BorderSize, BorderSize, Int32.MaxValue, Int32.MaxValue));
                    RenderContents();
                    GUILayout.EndArea();
                }, WindowName);

                if (!ResizingWindow)
                {
                    ClampPosition();
                }
            }
        }

        protected virtual void Resize()
        {
            if (ResizeOnHorizontal)
            {
                var newWidth = Event.current.mousePosition.x - X + BorderSize * 2;
                Width = Mathf.Clamp(newWidth, BorderSize * 2, Screen.width - X);
            }
            if (ResizeOnVertical)
            {
                var newHeight = Event.current.mousePosition.y - Y + BorderSize * 2;
                Height = Mathf.Clamp(newHeight, BorderSize * 2, Screen.height - Y);
            }
        }

        protected void RenderDrag()
        {
            GUI.DragWindow(new Rect(0, 0, Width - 40, 20));
        }

        protected void RenderResize() 
        {
            var resizeRect = new Rect(Width - 40, Height - 20, 40, 20);
            if (Event.current.type == EventType.mouseDown && resizeRect.Contains(Event.current.mousePosition))
            {
                ResizingWindow = true;
            }
            GUI.Label(resizeRect, " ◄►");
        }

        private void ClampPosition()
        {
            var maxX = Math.Max(0, Screen.width - Width);
            var maxY = Math.Max(0, Screen.height - Height);
            X = Mathf.Clamp(X, 0, maxX);
            Y = Mathf.Clamp(Y, 0, maxY);
        }
    }
}
