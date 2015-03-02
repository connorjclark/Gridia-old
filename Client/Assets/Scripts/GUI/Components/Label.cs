using System;
using UnityEngine;

namespace Gridia
{
    public class Label : Renderable
    {
        public String Text { get; set; }
        public bool Centered { get; set; }
        public bool Background { get; set; }

        public Label(Vector2 pos, String text, bool centered = false, bool background = false)
            : base(pos)
        {
            Text = text;
            Centered = centered;
            Background = background;
        }

        public override void Render()
        {
            base.Render();
            var textSize = GUI.skin.label.CalcSize(new GUIContent(Text));
            _rect.width = textSize.x;
            _rect.height = textSize.y;
            var rect = Centered ? new Rect(X - textSize.x / 2, Y, textSize.x, Height) : Rect;
            if (Background)
            {
                GUI.Box(rect, "");
            }
            GUI.Label(rect, Text);
        }
    }
}
