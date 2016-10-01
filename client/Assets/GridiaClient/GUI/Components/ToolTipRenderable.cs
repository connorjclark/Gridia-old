using System;
using UnityEngine;

namespace Gridia
{
    public class ToolTipRenderable : Renderable
    {
        private static ToolTipRenderable _instance;
        public static ToolTipRenderable Instance
        {
            get { return _instance ?? (_instance = new ToolTipRenderable(Vector2.zero)); }
        }

        public String ToolTipMessage { get; set; }

        private ToolTipRenderable(Vector2 pos)
            : base(pos) { }

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
    }
}
