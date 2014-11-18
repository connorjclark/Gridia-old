using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class Toggle : Renderable
    {
        public String Label { get; set; }
        public Action<bool> OnToggle { private get; set; }
        private bool Selected { get; set; }

        public Toggle(Vector2 pos, float width, float height, String label, bool initialState)
            : base(pos)
        {
            _rect.width = width;
            _rect.height = height;
            Selected = initialState;
            Label = label;
        }

        public override void Render()
        {
            base.Render();
            var newState = GUI.Toggle(Rect, Selected, Label);
            if (newState != Selected)
            {
                Selected = newState;
                OnToggle(Selected);
            }
        }
    }
}
