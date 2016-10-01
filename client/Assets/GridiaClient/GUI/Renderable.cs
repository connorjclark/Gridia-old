using System;
using UnityEngine;

namespace Gridia
{
    public abstract class Renderable
    {
        public GUISkin Skin { get; set; }

        public RenderableContainer Parent { get; set; }
        protected Rect _rect; // :(
        public Rect Rect
        {
            get 
            {
                var scale = TrueScale;
                var scaled = new Rect(_rect.x * scale.x, _rect.y * scale.y, _rect.width * scale.x, _rect.height * scale.y);
                return scaled; 
            }
            set
            {
                var scale = TrueScale;
                var normalized = new Rect(value.x / scale.x, value.y / scale.y, value.width / scale.x, value.height / scale.y);
                if (!normalized.Equals(_rect)) 
                {
                    _rect = normalized;
                    Dirty = true;
                }
            }
        }

        private Vector2 _scale = Vector2.one;
        public Vector2 Scale
        {
            get 
            {
                return _scale;
            } 
            set
            {
                _scale = value;
                Dirty = true;
            }
        }

        public float ScaleXY { set { Dirty = true; _scale = new Vector2(value, value); } }
        public float ScaleX { get { return _scale.x; } set { Dirty = true; _scale.x = value; } }
        public float ScaleY { get { return _scale.y; } set { Dirty = true; _scale.y = value; } }

        public Vector2 TrueScale 
        {
            get 
            {
                var parentScale = Parent != null ? Parent.TrueScale : Vector2.one;
                return new Vector2(parentScale.x * _scale.x, parentScale.y * _scale.y);
            }
        }

        public virtual float Width
        {
            get { return Rect.width; }
            set { if (value != _rect.width) { ScaleX = value / _rect.width; Dirty = true; } }
        }

        public virtual float Height
        {
            get { return Rect.height; }
            set { if (value != _rect.height) { ScaleY = value / _rect.height; Dirty = true; } }
        }

        public float X
        {
            get { return Rect.x; }
            set { if (value != _rect.x) { _rect.x = value / TrueScale.x; Dirty = true; } }
        }

        public float Y
        {
            get { return Rect.y; }
            set { if (value != _rect.y) { _rect.y = value / TrueScale.y; Dirty = true; } }
        }

        public bool Dirty { get; set; }

        public Color32 _color;
        public Color32 Color { get { return _color; } set { _color = value; } }
        public byte Alpha { get { return _color.a; } set { _color.a = value; } }

        // :(
        public Action OnClick { protected get; set; } // really just mouse up... :(
        public Action OnDoubleClick { private get; set; }
        public Action OnMouseDown { private get; set; }
        public Action OnRightClick { private get; set; }
        public Action OnMouseOver { private get; set; }
        public Action OnMouseLeave { private get; set; }
        public Action OnKeyUp { private get; set; }
        public Action OnEnterFrame { private get; set; }
        public Func<String> ToolTip { private get; set; }

        public Renderable(Vector2 pos)
        {
            Rect = new Rect(pos.x, pos.y, 0, 0);
            Color = new Color32(255, 255, 255, 255);
            //Skin = GridiaConstants.Skins[0]; // :(
        }

        protected void ApplySkinAndColor()
        {
            //GUI.skin = Skin;
            Skin = GUI.skin;
            GUI.color = Color;
        }

        public virtual void Render()
        {
            if (OnEnterFrame != null) OnEnterFrame();
            ApplySkinAndColor();
        }

        private bool _mouseOverLastFrame;

        public virtual void HandleEvents()
        {
            var mouse = Event.current.mousePosition;
            bool mouseOver;

            var asGridiaWindow = this as GridiaWindow;
            if (asGridiaWindow != null)
            {
                var borderSize = asGridiaWindow.BorderSize;
                var width = Width + borderSize*2;
                var height = Height + borderSize*2;
                mouseOver = new Rect(-borderSize, -borderSize, width, height).Contains(mouse);
            }
            else
            {
                mouseOver = Rect.Contains(mouse);
            }

            if (mouseOver)
            {
                if (OnMouseOver != null) OnMouseOver();
                if (Event.current.type == EventType.MouseUp)
                {
                    if (Event.current.button == 0)
                    {
                        if (OnClick != null) OnClick();
                    }
                    else if (Event.current.button == 1)
                    {
                        if (OnRightClick != null) OnRightClick();
                    }
                }
                else if (Event.current.type == EventType.MouseDown)
                {
                    if (Event.current.clickCount == 2)
                    {
                        if (OnDoubleClick != null) OnDoubleClick();
                    }
                    else
                    {
                        if (OnMouseDown != null) OnMouseDown();
                    }
                }
                else if (ToolTip != null && ToolTip() != "")
                {
                    RenderTooltip();
                }
            }
            else if (_mouseOverLastFrame && OnMouseLeave != null)
            {
                OnMouseLeave();
            }

            if (Event.current.type == EventType.KeyUp)
            {
                if (OnKeyUp != null) OnKeyUp();
            }

            _mouseOverLastFrame = mouseOver;
        }

        public void RenderTooltip()
        {
            var transitionLowerBound = (float) Screen.height * 1 / 3;
            var transitionUpperBound = (float) Screen.height * 2 / 3;

            var deltaY = 0f;
            var y = Screen.height - Input.mousePosition.y;
            if (y > transitionLowerBound) 
            {
                var ratio = (y - transitionLowerBound) / (transitionUpperBound - transitionLowerBound);
                deltaY = Mathf.Lerp(0, 100, ratio);
            }

            var trueScale = TrueScale;
            var width = 140 * trueScale.x;
            var height = 25 * trueScale.y;
            var globalRect = new Rect(Input.mousePosition.x + 10, Screen.height - Input.mousePosition.y - deltaY + 10, width, height);

            globalRect.x = Math.Min(globalRect.x, Screen.width - globalRect.width);
            globalRect.y = Math.Min(globalRect.y, Screen.height - globalRect.height);

            ToolTipRenderable.Instance.ToolTipMessage = ToolTip();
            ToolTipRenderable.Instance.Rect = globalRect;
        }
    }
}
