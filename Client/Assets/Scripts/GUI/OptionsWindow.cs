using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class OptionsWindow : GridiaWindow
    {
        public OptionsWindow(Vector2 pos)
            : base(pos, "Options") 
        {
            Resizeable = false;
            var soundPlayer = Locator.Get<SoundPlayer>();
            MakeButton("Log out", new Rect(0, 0, 80, 30), Application.Quit);
            MakeButton("Next song", new Rect(0, 35, 80, 30), soundPlayer.EndCurrentSong);
            MakeToggle(" Mute music", soundPlayer.MuteMusic, new Rect(0, 70, 100, 30), (value) => soundPlayer.MuteMusic = value);
            MakeToggle(" Mute sfx", soundPlayer.MuteSfx, new Rect(0, 105, 100, 30), (value) => soundPlayer.MuteSfx = value);
        }

        private void MakeButton(String label, Rect rect, Action onClick) 
        {
            var button = new Button(new Vector2(rect.x, rect.y), rect.width, rect.height, label);
            button.OnClick = onClick;
            AddChild(button);
        }

        private void MakeToggle(String label, bool initialState, Rect rect, Action<bool> onToggle) 
        {
            var toggle = new Toggle(new Vector2(rect.x, rect.y), rect.width, rect.height, label, initialState);
            toggle.Rect = rect;
            toggle.OnToggle = onToggle;
            AddChild(toggle);
        }
    }
}
