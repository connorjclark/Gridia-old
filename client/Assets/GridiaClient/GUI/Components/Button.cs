namespace Gridia
{
    using System;

    using UnityEngine;

    public class Button : Renderable
    {
        #region Constructors

        public Button(Vector2 pos, String text, bool guessSize = true)
            : base(pos)
        {
            Text = text;
            _rect.width = 32; // :(
            _rect.height = 32; // :(
            GuessSize = guessSize;
            Active = true;
        }

        #endregion Constructors

        #region Properties

        public bool Active
        {
            get; set;
        }

        public bool GuessSize
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
            if (GuessSize)
            {
                var textSize = GUI.skin.label.CalcSize(new GUIContent(Text));
                _rect.width = textSize.x + 20; // :(
                _rect.height = 32;
            }
            // :(
            if (GUI.Button(Rect, Text) && OnClick != null && Active)
            {
                if (GridiaConstants.SoundPlayer != null)
                {
                    GridiaConstants.SoundPlayer.PlaySfx("pop_drip");
                }
                OnClick();
            }
        }

        #endregion Methods
    }
}