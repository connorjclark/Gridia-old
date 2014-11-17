using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class RenderableContainer : Renderable
    {
        protected List<Renderable> _children = new List<Renderable>();

        public RenderableContainer(Rect rect)
            : base(rect) 
        {
            
        }

        public override void Render()
        {
            if (Dirty || _children.Any(c => c.Dirty)) 
            {
                Dirty = false;
                _children.ForEach(c => c.Dirty = false);
                CalculateRect();
            }
            GUI.BeginGroup(Rect);
            lock (_children)
            {
                foreach (var child in _children)
                {
                    GUI.color = child.Color;
                    child.Render();
                }
            }
            GUI.EndGroup();
        }

        public int NumChildren { get { return _children.Count; } }

        public virtual void AddChild(Renderable child) 
        {
            if (child.Parent == null)
            {
                child.Parent = this;
                lock (_children) _children.Add(child);
                Dirty = true;
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
                width = Math.Max(width, child.Width + child.X);
                height = Math.Max(height, child.Height + child.Y);
            }
            Width = width;
            Height = height;
        }

        private void EnsureContainsRect(Renderable child) 
        {
            Width = Math.Max(Rect.width, child.Width + child.X);
            Height = Math.Max(Rect.height, child.Height + child.Y);
        }
    }
}
