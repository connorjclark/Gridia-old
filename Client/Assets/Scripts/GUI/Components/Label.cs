using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class Label : Renderable
    {
        public String Text { get; set; }
        public bool Centered { get; set; }

        public Label(Vector2 pos, float width, float height, String text, bool centered = false)
            : base(pos)
        {
            _rect.width = width;
            _rect.height = height;
            Text = text;
            Centered = centered;
        }

        public override void Render()
        {
            base.Render();
            if (Centered)
            {
                var textSize = GUI.skin.GetStyle("Label").CalcSize(new GUIContent(Text));
                var x = X + (Width - textSize.x) / 2;
                var y = Y + (Height - textSize.y) / 2;
                GUI.Label(new Rect(x, y, Width, Height), Text);
            }
            else
            {
                GUI.Label(Rect, Text);
            }
        }
    }
}
