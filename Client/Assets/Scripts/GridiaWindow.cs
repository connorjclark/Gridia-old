using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia 
{
    public abstract class GridiaWindow
    {
        private static int _NEXT_WINDOW_ID;

        public int WindowId { get; private set; }
        public bool MouseOver { get; private set; }
        public bool ResizingWindow { get; private set; }
        public bool ResizeOnHorizontal { get; set; }
        public bool ResizeOnVertical { get; set; }
        public float BorderSize { get; set; }
        public String WindowName { get; set; }
        protected Rect _windowRect;

        public GridiaWindow(Vector2 position, String windowName)
        {
            WindowId = _NEXT_WINDOW_ID++;
            WindowName = windowName;
            _windowRect = new Rect(position.x, position.y, 0, 0);
            ResizeOnHorizontal = ResizeOnVertical = true;
            BorderSize = 20;
        }

        protected abstract void RenderContents();

        public virtual void Render() 
        {
            if (Event.current.type == EventType.Layout)
            {
                MouseOver = false;
            }
            if (Event.current.type == EventType.MouseUp)
            {
                ResizingWindow = false;
            }
            if (ResizingWindow)
            {
                Resize();
            }

            MouseOver = _windowRect.Contains(Event.current.mousePosition);

            _windowRect = GUI.Window(WindowId, _windowRect, windowId =>
            {
                RenderDragAndResize();
                GUILayout.BeginArea(new Rect(BorderSize, BorderSize, Int32.MaxValue, Int32.MaxValue));
                RenderContents();
                GUILayout.EndArea();
            }, WindowName);

            ClampPosition();
        }

        protected virtual void Resize()
        {
            if (ResizeOnHorizontal)
            {
                _windowRect.width = Event.current.mousePosition.x - _windowRect.x + BorderSize * 2;
                _windowRect.width = Mathf.Clamp(_windowRect.width, BorderSize * 2, Screen.width);
            }
            if (ResizeOnVertical)
            {
                _windowRect.height = Event.current.mousePosition.y - _windowRect.y + BorderSize * 2;
                _windowRect.height = Mathf.Clamp(_windowRect.height, BorderSize * 2, Screen.height);
            }
        }

        protected void RenderDragAndResize()
        {
            GUI.DragWindow(new Rect(0, 0, _windowRect.width - 40, 20));
            var resizeRect = new Rect(_windowRect.width - 40, 0, 40, 20);
            if (Event.current.type == EventType.mouseDown && resizeRect.Contains(Event.current.mousePosition))
            {
                ResizingWindow = true;
            }
            GUI.Label(resizeRect, " ◄►");
        }

        private void ClampPosition()
        {
            var maxX = Math.Max(0, Screen.width - _windowRect.width);
            var maxY = Math.Max(0, Screen.height - _windowRect.height);
            _windowRect.x = Mathf.Clamp(_windowRect.x, 0, maxX);
            _windowRect.y = Mathf.Clamp(_windowRect.y, 0, maxY);
        }
    }
}
