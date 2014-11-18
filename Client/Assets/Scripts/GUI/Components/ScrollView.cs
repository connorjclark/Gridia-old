using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class ScrollView : Renderable
    {
        public Renderable View { get; set; }
        public Vector2 ScrollPosition { get; set; }

        public ScrollView(Vector2 pos, float width, float height, Renderable view)
            : base(pos)
        {
            _rect.width = width;
            _rect.height = height;
            View = view;
        }

        public override void Render()
        {
            base.Render();
            ScrollPosition = GUI.BeginScrollView(Rect, ScrollPosition, new Rect(0, 0, Width - 20, View.Height));
            View.Render();
            GUI.EndScrollView();
        }
    }
}
