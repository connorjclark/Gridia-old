using System;
using UnityEngine;

namespace Gridia
{
    public class TextArea : Renderable
    {
        public String Text
        {
            get { return RichText.ToString(); }
        }
        public Vector2 ScrollPosition { get; set; }
        public int MaxLength
        {
            get { return RichText.MaxLength; }
            set { RichText.MaxLength = value; }
        }
        public float TextHeight { get; private set; }
        public float MaxScrollY { get; private set; }
        private RichText RichText { get; set; }

        public TextArea(Vector2 pos, float width, float height)
            : base(pos)
        {
            RichText = new RichText();
            _rect.width = width + 20;
            _rect.height = height + 20;
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

        public void Append(String text)
        {
            RichText.Append(text);
        }

        private void CalculateMaxScrollY() 
        {
            MaxScrollY = GUI.skin.GetStyle("TextArea").CalcHeight(new GUIContent(Text), Width - 20) - Height;
        }
    }
}
