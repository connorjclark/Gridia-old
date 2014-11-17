using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public abstract class Renderable
    {
        public RenderableContainer Parent { get; set; }
        private Rect _rect;
        public Rect Rect
        {
            get { return _rect; }
            protected set
            {
                if (!value.Equals(_rect)) 
                {
                    _rect = value;
                    Dirty = true;
                }
            }
        }

        public float Width
        {
            get { return _rect.width; }
            set { if (value != _rect.width) { _rect.width = value; Dirty = true; } }
        }

        public float Height
        {
            get { return _rect.height; }
            set { if (value != _rect.height) { _rect.height = value; Dirty = true; } }
        }

        public float X
        {
            get { return _rect.x; }
            set { if (value != _rect.x) { _rect.x = value; Dirty = true; } }
        }

        public float Y
        {
            get { return _rect.y; }
            set { if (value != _rect.y) { _rect.y = value; Dirty = true; } }
        }

        public bool Dirty { get; set; }

        public Color32 Color { get; set; }

        public Action OnClick { private get; set; }
        public Action OnRightClick { private get; set; }

        public Renderable(Rect rect)
        {
            Rect = rect;
            Color = new Color32(255, 255, 255, 255);
        }

        public virtual void Render()
        {
            if (Event.current.type == EventType.MouseUp && Rect.Contains(Event.current.mousePosition)) 
            {
                if (Event.current.button == 0)
                {
                    OnClick();
                }
                else if (Event.current.button == 1)
                {
                    OnRightClick();
                }
            }
        }
    }
}
