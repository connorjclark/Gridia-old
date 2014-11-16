using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class OptionsWindow : GridiaWindow
    {
        private Rect _logout;

        public OptionsWindow(Rect rect)
            : base(rect, "Options") 
        {
            _logout = new Rect(0, 0, 80, 30);
        }

        protected override void RenderContents()
        {
            if (GUI.Button(_logout, "Log out")) 
            {
                Application.Quit();
            }
        }
    }
}
