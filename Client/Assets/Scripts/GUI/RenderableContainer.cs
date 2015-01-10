using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class RenderableContainer : Renderable
    {
        public int NumChildren { get { return _children.Count; } }
        protected List<Renderable> _children = new List<Renderable>();

        public RenderableContainer(Vector2 pos)
            : base(pos) { }

        public override void Render()
        {
            if (_children.Any(c => c.Dirty)) 
            {
                CalculateRect();
                Dirty = true;
                _children.ForEach(c => c.Dirty = false);
            }

            GUI.BeginGroup(new Rect(X, Y, Int32.MaxValue, Int32.MaxValue));
            lock (_children)
            {
                foreach (var child in _children.ToList())
                {
                    child.Render();
                }
            }
            GUI.EndGroup();
        }

        public override void HandleEvents() 
        {
            GUI.BeginGroup(new Rect(X, Y, Int32.MaxValue, Int32.MaxValue));
            base.HandleEvents();
            lock (_children)
            {
                foreach (var child in _children.ToList())
                {
                    child.HandleEvents();
                }
            }
            GUI.EndGroup();
        }

        public virtual void AddChild(Renderable child) 
        {
            if (child.Parent == null)
            {
                child.Parent = this;
                lock (_children) _children.Add(child);
                Dirty = true;
                child.Dirty = true;
            }
            else 
            {
                throw new Exception("Child already belongs to a container: " + child);
            }
        }

        public Renderable GetChildAt(int index) 
        {
            return _children[index];
        }

        public int GetIndexOfChild(Renderable child) 
        {
            return _children.IndexOf(child);
        }

        public void RemoveChild(Renderable child) 
        {
            lock (_children)
            {
                if (!_children.Remove(child))
                {
                    throw new Exception("Child not found: " + child);
                }
                Dirty = true;
            }
            child.Parent = null;
        }

        public void RemoveChildAt(int index) 
        {
            lock (_children) _children.RemoveAt(index);
            Dirty = true;
        }

        public void RemoveAllChildren() 
        {
            lock (_children) _children.Clear();
            Dirty = true;
        }

        protected void CalculateRect() 
        {
            var width = 0f;
            var height = 0f;
            foreach (var child in _children)
            {
                width = Math.Max(width, (child.Width + child.X) );
                height = Math.Max(height, (child.Height + child.Y) );
            }
            _rect.width = width / TrueScale.x;
            _rect.height = height / TrueScale.y;
        }
    }
}
