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
        private Rect _muteMusic;
        private Rect _muteSfx;
        private SoundPlayer _soundPlayer;

        public OptionsWindow(Rect rect)
            : base(rect, "Options") 
        {
            Resizeable = false;
            _logout = new Rect(0, 0, 80, 30);
            _nextSong = new Rect(0, 35, 80, 30);
            _muteMusic = new Rect(0, 70, 100, 30);
            _muteSfx = new Rect(0, 105, 100, 30);
            _soundPlayer = Locator.Get<SoundPlayer>();
        }

        protected override void RenderContents()
        {

            if (GUI.Button(_logout, "Log out")) 
            {
                Application.Quit();
            }
            if (GUI.Button(_nextSong, "Next song"))
            {
                _soundPlayer.EndCurrentSong();
            }
            _soundPlayer.MuteMusic = GUI.Toggle(_muteMusic, _soundPlayer.MuteMusic, " Mute music");
            _soundPlayer.MuteSfx = GUI.Toggle(_muteSfx, _soundPlayer.MuteSfx, " Mute sfx");
        }
    }
}
