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

        public Button(Rect rect, String text)
            : base(rect)
        {
            Text = text;
        }

        public override void Render()
        {
            base.Render();
            GUI.Button(Rect, Text);
        }
    }
}
