namespace Gridia
{
    using System;

    using UnityEngine;

    public class Toggle : Renderable
    {
        #region Constructors

        public Toggle(Vector2 pos, String label, bool initialState)
            : base(pos)
        {
            Selected = initialState;
            Label = label;
        }

        #endregion Constructors

        #region Properties

        public String Label
        {
            get; set;
        }

        public Action<bool> OnToggle
        {
            private get; set;
        }

        private bool Selected
        {
            get; set;
        }

        #endregion Properties

        #region Methods

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

        #endregion Methods
    }
}