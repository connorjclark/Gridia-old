using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class TextField : Renderable
    {
        public String Text { get; set; }
        public Action<String> OnEnter { private get; set; }

        public TextField(Vector2 pos, float width, float height)
            : base(pos)
        {
            Text = "";
            _rect.width = width;
            _rect.height = height;
        }

        public override void Render()
        {
            base.Render();
            Text = GUI.TextField(Rect, Text);
            if (Text != "" && Event.current.type == EventType.keyDown && Event.current.character == '\n')
            {
                OnEnter(Text);
                Text = "";
            }
        }
    }
}
