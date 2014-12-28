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
        public bool PasswordField { get; set; }
        public int MaxChars { get; set; }
        public String TextFieldName { get; set; }
        public Action<String> OnEnter { private get; set; }

        public TextField(Vector2 pos, String textFieldName, float width, float height)
            : base(pos)
        {
            Text = "";
            TextFieldName = textFieldName;
            _rect.width = width;
            _rect.height = height;
        }

        public override void Render()
        {
            base.Render();
            GUI.SetNextControlName(TextFieldName);
            if (PasswordField)
            {
                Text = GUI.PasswordField(Rect, Text, '*', MaxChars);
            }
            else
            {
                Text = GUI.TextField(Rect, Text, MaxChars);
            }
            if (Text != "" && Event.current.type == EventType.keyDown && Event.current.character == '\n')
            {
                OnEnter(Text);
                Text = "";
            }
        }
    }
}
