using System;
using UnityEngine;

namespace Gridia
{
    public class ToolTipRenderable : Renderable
    {
        public static ToolTipRenderable Instance = new ToolTipRenderable(Vector2.zero);

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
                const int width = 200;
                const int height = 30;
                GUI.Box(new Rect((Rect.width - width) / 2, (Rect.height - height) / 2, width, height), toolTip);
                GUI.BringWindowToFront(windowId);
            }, "");
            ToolTipMessage = null;
        }
    }
}
