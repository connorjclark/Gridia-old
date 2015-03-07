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
            GUI.Window(100, Rect, windowId =>
            {
                var width = Width * 0.9f;
                var height = Height * 0.8f;
                GUI.Box(new Rect((Width - width) / 2, (Height - height) / 2, width, height), toolTip);
                GUI.BringWindowToFront(windowId);
            }, "");
            ToolTipMessage = null;
        }
    }
}
