namespace Gridia
{
    using System;

    using UnityEngine;

    public class ToolTipRenderable : Renderable
    {
        #region Fields

        private static ToolTipRenderable _instance;

        #endregion Fields

        #region Constructors

        private ToolTipRenderable(Vector2 pos)
            : base(pos)
        {
        }

        #endregion Constructors

        #region Properties

        public static ToolTipRenderable Instance
        {
            get { return _instance ?? (_instance = new ToolTipRenderable(Vector2.zero)); }
        }

        public String ToolTipMessage
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override void Render()
        {
            base.Render();
            if (ToolTipMessage == null) return;
            var toolTip = ToolTipMessage;
            _rect.height = Skin.label.CalcHeight(new GUIContent(toolTip), Width);
            GUI.Window(100, Rect, windowId =>
            {
                GUI.Label(new Rect(0, 0, _rect.width, _rect.height), toolTip);
                GUI.BringWindowToFront(windowId);
            }, "");
            ToolTipMessage = null;
        }

        #endregion Methods
    }
}