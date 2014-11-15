using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public abstract class Renderable
    {
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

        public Renderable(Rect rect)
        {
            Rect = rect;
        }

        public bool Dirty { get; set; }

        public abstract void Render();
    }
}
