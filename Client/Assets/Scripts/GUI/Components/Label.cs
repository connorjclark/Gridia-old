using System;
using UnityEngine;

namespace Gridia
{
    public class Label : Renderable
    {
        public String Text { get; set; }
        public bool Centered { get; set; }
        public bool Background { get; set; }
        public int TextWidth = 300;

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
            _rect.height = Skin.label.CalcHeight(new GUIContent(Text), TextWidth);
            _rect.width = TextWidth;
            var rect = Centered ? new Rect(X - Width / 2, Y, Width, Height) : Rect;
            if (Background)
            {
                GUI.Box(rect, "");
            }
            GUI.Label(rect, Text);
        }
    }
}
