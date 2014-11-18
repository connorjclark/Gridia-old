using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class TextArea : Renderable
    {
        public String Text { get; set; }
        public Vector2 ScrollPosition { get; set; }

        public TextArea(Vector2 pos, float width, float height)
            : base(pos)
        {
            Text = "";
            _rect.width = width;
            _rect.height = height;
        }

        public override void Render()
        {
            base.Render();
            var textHeight = GUI.skin.GetStyle("TextArea").CalcSize(new GUIContent(Text)).y;
            ScrollPosition = GUI.BeginScrollView(Rect, ScrollPosition, new Rect(0, 0, Width - 20, textHeight));
            GUI.TextArea(new Rect(0, 0, Rect.width, Math.Max(textHeight, Height)), Text);
            GUI.EndScrollView();
        }
    }
}
