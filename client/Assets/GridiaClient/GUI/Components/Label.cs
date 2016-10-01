namespace Gridia
{
    using System;

    using UnityEngine;

    public class Label : Renderable
    {
        #region Fields

        public int TextWidth = 300;

        #endregion Fields

        #region Constructors

        public Label(Vector2 pos, String text, bool centered = false, bool background = false)
            : base(pos)
        {
            Text = text;
            Centered = centered;
            Background = background;
        }

        #endregion Constructors

        #region Properties

        public bool Background
        {
            get; set;
        }

        public bool Centered
        {
            get; set;
        }

        public String Text
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override void Render()
        {
            base.Render();
            _rect.height = Skin.label.CalcHeight(new GUIContent(Text), TextWidth);
            _rect.width = TextWidth;
            var rect = Centered ? new Rect(X - Width / 2, Y, Width, Height) : Rect;
            if (Background)
            {
                GUI.Box(rect, "");
            }
            GUI.Label(rect, Text);
        }

        #endregion Methods
    }
}