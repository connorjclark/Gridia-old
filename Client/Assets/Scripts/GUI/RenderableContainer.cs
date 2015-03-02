using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gridia
{
    public class RenderableContainer : Renderable
    {
        public int NumChildren { get { return Children.Count; } }
        protected List<Renderable> Children = new List<Renderable>();

        public RenderableContainer(Vector2 pos)
            : base(pos) { }

        public override void Render()
        {
            lock (Children)
            {
                if (Children.Any(c => c.Dirty))
                {
                    CalculateRect();
                    Dirty = true;
                    Children.ForEach(c => c.Dirty = false);
                }
                GUI.BeginGroup(new Rect(X, Y, Int32.MaxValue, Int32.MaxValue));
                foreach (var child in Children.ToList())
                {
                    child.Render();
                }
                GUI.EndGroup();
            }
        }

        public override void HandleEvents() 
        {
            GUI.BeginGroup(new Rect(X, Y, Int32.MaxValue, Int32.MaxValue));
            base.HandleEvents();
            lock (Children)
            {
                foreach (var child in Children.ToList())
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
                lock (Children) Children.Add(child);
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
            lock (Children)
            {
                return Children[index];
            }
        }

        public int GetIndexOfChild(Renderable child)
        {
            lock (Children)
            {
                return Children.IndexOf(child);
            }
        }

        public void RemoveChild(Renderable child) 
        {
            lock (Children)
            {
                if (!Children.Remove(child))
                {
                    throw new Exception("Child not found: " + child);
                }
                Dirty = true;
            }
            child.Parent = null;
        }

        public void RemoveChildAt(int index) 
        {
            lock (Children) Children.RemoveAt(index);
            Dirty = true;
        }

        public void RemoveAllChildren() 
        {
            lock (Children) Children.Clear();
            Dirty = true;
        }

        public void CalculateRect() 
        {
            var width = 0f;
            var height = 0f;
            foreach (var child in Children)
            {
                width = Math.Max(width, (child.Width + child.X) );
                height = Math.Max(height, (child.Height + child.Y) );
            }
            _rect.width = width / TrueScale.x;
            _rect.height = height / TrueScale.y;
        }
    }
}
