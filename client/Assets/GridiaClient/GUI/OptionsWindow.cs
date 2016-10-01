using System;
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
            MakeButton("Log out", new Vector2(0, 0), () => {
                Locator.Get<ConnectionToGridiaServerHandler>().Close();
                SceneManager.LoadScene("ServerSelection");
            });
            MakeButton("Next song", new Vector2(0, 55), soundPlayer.EndCurrentSong);
            MakeToggle(" Mute music", soundPlayer.MuteMusic, new Vector2(0, 105), (value) => soundPlayer.MuteMusic = value);
            MakeToggle(" Mute sfx", soundPlayer.MuteSfx, new Vector2(0, 130), (value) => soundPlayer.MuteSfx = value);
        }

        private void MakeButton(String label, Vector2 pos, Action onClick) 
        {
            var button = new Button(pos, label) {OnClick = onClick};
            AddChild(button);
        }

        private void MakeToggle(String label, bool initialState, Vector2 pos, Action<bool> onToggle) 
        {
            var toggle = new Toggle(pos, label, initialState) {OnToggle = onToggle};
            AddChild(toggle);
        }
    }
}
