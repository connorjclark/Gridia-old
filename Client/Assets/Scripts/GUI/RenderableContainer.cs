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
            if (_children.Any(c => c.Dirty)) 
            {
                _children.ForEach(c => c.Dirty = false);
                CalculateRect();
            }
            GUI.BeginGroup(Rect);
            foreach (var child in _children)
            {
                child.Render();
            }
            GUI.EndGroup();
        }

        public int NumChildren { get { return _children.Count; } }

        public void AddChild(Renderable child) 
        {
            if (child.Parent == null)
            {
                child.Parent = this;
                _children.Add(child);
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

        public void RemoveChild(Renderable child) 
        {
            if (!_children.Remove(child)) 
            {
                throw new Exception("Child not found: " + child);
            }
        }

        public void RemoveChildren() 
        {
            _children.Clear();
        }

        private void CalculateRect() 
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
