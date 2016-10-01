namespace Gridia
{
    using System;

    using UnityEngine;

    public class TextArea : Renderable
    {
        #region Constructors

        public TextArea(Vector2 pos, float width, float height)
            : base(pos)
        {
            RichText = new RichText();
            _rect.width = width + 20;
            _rect.height = height + 20;
        }

        #endregion Constructors

        #region Properties

        public int MaxLength
        {
            get { return RichText.MaxLength; }
            set { RichText.MaxLength = value; }
        }

        public float MaxScrollY
        {
            get; private set;
        }

        public Vector2 ScrollPosition
        {
            get; set;
        }

        public String Text
        {
            get { return RichText.ToString(); }
        }

        public float TextHeight
        {
            get; private set;
        }

        private RichText RichText
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public void Append(String text)
        {
            RichText.Append(text);
        }

        public override void Render()
        {
            base.Render();
            TextHeight = GUI.skin.GetStyle("TextArea").CalcHeight(new GUIContent(Text), Width - 20);
            ScrollPosition = GUI.BeginScrollView(Rect, ScrollPosition, new Rect(0, 0, Width - 20, TextHeight));
            GUI.TextArea(new Rect(0, 0, Width - 20, Math.Max(Height, TextHeight)), Text);
            GUI.EndScrollView();
            CalculateMaxScrollY();
        }

        private void CalculateMaxScrollY()
        {
            MaxScrollY = GUI.skin.GetStyle("TextArea").CalcHeight(new GUIContent(Text), Width - 20) - Height;
        }

        #endregion Methods
    }
}