using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class Button : Renderable
    {
        public String Text { get; set; }

        public Button(Vector2 pos, float width, float height, String text)
            : base(pos)
        {
            _rect.width = width;
            _rect.height = height;
            Text = text;
        }

        public override void Render()
        {
            base.Render();
            GUI.Button(Rect, Text);
        }
    }
}
