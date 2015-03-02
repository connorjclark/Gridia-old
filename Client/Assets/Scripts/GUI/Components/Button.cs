using System;
using UnityEngine;

namespace Gridia
{
    public class Button : Renderable
    {
        public String Text { get; set; }

        public Button(Vector2 pos, String text)
            : base(pos)
        {
            Text = text;
        }

        public override void Render()
        {
            base.Render();
            var textSize = GUI.skin.label.CalcSize(new GUIContent(Text));
            _rect.width = textSize.x + 20; // :(
            _rect.height = textSize.y * 2;
            // :(
            if (GUI.Button(Rect, Text) && OnClick != null) 
            {
                OnClick();
            }
        }
    }
}
