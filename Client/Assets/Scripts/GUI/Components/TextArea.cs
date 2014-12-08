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
        public float TextHeight { get; private set; }
        public float MaxScrollY { get; private set; }

        public TextArea(Vector2 pos, float width, float height)
            : base(pos)
        {
            Text = "";
            _rect.width = width + 20;
            _rect.height = height + 20;
        }

        public override void Render()
        {
            base.Render();
            TextHeight = GUI.skin.GetStyle("TextArea").CalcHeight(new GUIContent(Text), Width);
            ScrollPosition = GUI.BeginScrollView(Rect, ScrollPosition, new Rect(0, 0, Width - 20, TextHeight));
            GUI.TextArea(new Rect(0, 0, Width, Math.Max(Height, TextHeight)), Text);
            GUI.EndScrollView();
            CalculateMaxScrollY();
        }

        private void CalculateMaxScrollY() 
        {
            MaxScrollY = GUI.skin.GetStyle("TextArea").CalcHeight(new GUIContent(Text), Width) - Height;
        }
    }
}
