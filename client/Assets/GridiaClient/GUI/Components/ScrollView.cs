namespace Gridia
{
    using UnityEngine;

    public class ScrollView : Renderable
    {
        #region Constructors

        public ScrollView(Vector2 pos, float width, float height, Renderable view)
            : base(pos)
        {
            _rect.width = width;
            _rect.height = height;
            View = view;
        }

        #endregion Constructors

        #region Properties

        public Vector2 ScrollPosition
        {
            get; set;
        }

        public Renderable View
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override void Render()
        {
            base.Render();
            ScrollPosition = GUI.BeginScrollView(Rect, ScrollPosition, new Rect(0, 0, Width - 20, View.Height));
            View.Render();
            GUI.EndScrollView();
        }

        #endregion Methods
    }
}