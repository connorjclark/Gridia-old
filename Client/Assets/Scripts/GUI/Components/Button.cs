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
            _rect.height = 32; // :(
        }

        public override void Render()
        {
            base.Render();
            var textSize = GUI.skin.label.CalcSize(new GUIContent(Text));
            _rect.width = textSize.x + 20; // :(
            _rect.height = 32;
            // :(
            if (GUI.Button(Rect, Text) && OnClick != null) 
            {
                OnClick();
            }
        }
    }
}
