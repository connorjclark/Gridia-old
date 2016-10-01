using System;
using UnityEngine;

namespace Gridia
{
    public class Toggle : Renderable
    {
        public String Label { get; set; }
        public Action<bool> OnToggle { private get; set; }
        private bool Selected { get; set; }

        public Toggle(Vector2 pos, String label, bool initialState)
            : base(pos)
        {
            
            Selected = initialState;
            Label = label;
        }

        public override void Render()
        {
            base.Render();
            var textSize = GUI.skin.label.CalcSize(new GUIContent(Label));
            _rect.width = textSize.x + 20; // :(
            _rect.height = textSize.y;
            var newState = GUI.Toggle(Rect, Selected, Label);
            if (newState != Selected)
            {
                Selected = newState;
                OnToggle(Selected);
            }
        }
    }
}
