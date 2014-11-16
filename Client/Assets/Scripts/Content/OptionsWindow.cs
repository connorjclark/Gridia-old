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
        private Rect _nextSong;

        public OptionsWindow(Rect rect)
            : base(rect, "Options") 
        {
            _logout = new Rect(0, 0, 80, 30);
            _nextSong = new Rect(0, 35, 80, 30);
        }

        protected override void RenderContents()
        {
            if (GUI.Button(_logout, "Log out")) 
            {
                Application.Quit();
            }
            if (GUI.Button(_nextSong, "Next song"))
            {
                Locator.Get<SoundPlayer>().EndCurrentSong();
            }
        }
    }
}
