using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gridia
{
    public class HelpMenu : GridiaWindow
    {
        private int AlertIndex { get; set; }
        private List<string> Alerts { get; set; }
        private readonly Button _prev;
        private readonly Button _ok;
        private readonly Button _next;
        private readonly Label _label = new Label(Vector2.zero, "No messages");

        public HelpMenu(Vector2 pos)
            : base(pos, "Help")
        {
            Resizeable = false;
            Alerts = new List<string>();
            AddChild(_label);
            _prev = MakeButton("<", new Vector2(0, 70), () => ShowAlert(AlertIndex - 1));
            _ok = MakeButton("OK!", new Vector2(80, 70), () => Visible = false);
            _next = MakeButton(">", new Vector2(160, 70), () => ShowAlert(AlertIndex + 1));
        }

        public override void Render()
        {
            base.Render();
            _prev.Y = _ok.Y = _next.Y = _label.Height/ScaleY;
        }

        public void AddAlert(String message)
        {
            Alerts.Add(message);
            ShowAlert(Alerts.Count - 1);
            Visible = true;
        }

        private void ShowAlert(int index)
        {
            AlertIndex = Mathf.Clamp(index, 0, Alerts.Count - 1);
            _label.Text = Alerts[AlertIndex];
            var isFirst = AlertIndex == 0;
            var isLast = AlertIndex == Alerts.Count - 1;
            _prev.Active = !isFirst;
            _next.Active = !isLast;
            _prev.Alpha = isFirst ? (byte)150 : (byte)255;
            _next.Alpha = isLast ? (byte)150 : (byte)255;
        }

        private Button MakeButton(String label, Vector2 pos, Action onClick)
        {
            var button = new Button(pos, label, false) { OnClick = onClick };
            AddChild(button);
            return button;
        }
    }
}
