using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class ToolTipRenderable : Renderable
    {
        public static ToolTipRenderable instance = new ToolTipRenderable(Vector2.zero);

        public String ToolTip { get; set; }

        private ToolTipRenderable(Vector2 pos)
            : base(pos) { }

        public override void Render()
        {
            base.Render();
            if (ToolTip != null)
            {
                var toolTip = ToolTip;
                GUI.Window(100, Rect, windowId =>
                {
                    var width = 200;
                    var height = 30;
                    GUI.Box(new Rect((Rect.width - width) / 2, (Rect.height - height) / 2, width, height), toolTip);
                    GUI.BringWindowToFront(windowId);
                }, "");
                ToolTip = null;
            }
        }
    }
}
