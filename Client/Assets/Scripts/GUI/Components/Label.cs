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

        public Label(Vector2 pos, String text, bool centered = false)
            : base(pos)
        {
            Text = text;
            Centered = centered;
        }

        public override void Render()
        {
            base.Render();
            var textSize = GUI.skin.label.CalcSize(new GUIContent(Text));
            _rect.width = textSize.x;
            _rect.height = textSize.y;
            if (Centered)
            {
                X = (Width - textSize.x) / 2;
                Y = (Height - textSize.y) / 2;
            }
            GUI.Label(Rect, Text);
        }
    }
}
