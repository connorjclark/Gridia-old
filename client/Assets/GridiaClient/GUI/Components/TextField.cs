namespace Gridia
{
    using System;

    using UnityEngine;

    public class TextField : Renderable
    {
        #region Constructors

        public TextField(Vector2 pos, String textFieldName, float width, float height)
            : base(pos)
        {
            Text = "";
            MaxChars = int.MaxValue;
            TextFieldName = textFieldName;
            _rect.width = width;
            _rect.height = height;
        }

        #endregion Constructors

        #region Properties

        public int MaxChars
        {
            get; set;
        }

        public Action OnEnter
        {
            private get; set;
        }

        public bool PasswordField
        {
            get; set;
        }

        public String Text
        {
            get; set;
        }

        public String TextFieldName
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override void Render()
        {
            base.Render();
            GUI.SetNextControlName(TextFieldName);
            Text = PasswordField ? GUI.PasswordField(Rect, Text, '*', MaxChars) : GUI.TextField(Rect, Text, MaxChars);
            if (OnEnter != null && Text != "" && Event.current.type == EventType.keyDown && Event.current.character == '\n')
            {
                OnEnter();
            }
        }

        #endregion Methods
    }
}