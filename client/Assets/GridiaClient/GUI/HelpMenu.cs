namespace Gridia
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    public class HelpMenu : GridiaWindow
    {
        #region Fields

        private readonly Label _label = new Label(Vector2.zero, "No messages");
        private readonly Button _next;
        private readonly Button _ok;
        private readonly Button _prev;

        #endregion Fields

        #region Constructors

        public HelpMenu(Vector2 pos)
            : base(pos, "Help")
        {
            Resizeable = false;
            Alerts = new List<string>();
            AddChild(_label);
            _prev = MakeButton("<", new Vector2(0, 0), () => ShowAlert(AlertIndex - 1));
            _ok = MakeButton("OK!", new Vector2(80, 0), () => Visible = false);
            _next = MakeButton(">", new Vector2(160, 0), () => ShowAlert(AlertIndex + 1));
            _label.Y = 40;
        }

        #endregion Constructors

        #region Properties

        private int AlertIndex
        {
            get; set;
        }

        private List<string> Alerts
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public void AddAlert(String message)
        {
            Alerts.Add(message);
            ShowAlert(Alerts.Count - 1);
            Visible = true;
        }

        private Button MakeButton(String label, Vector2 pos, Action onClick)
        {
            var button = new Button(pos, label, false) { OnClick = onClick };
            AddChild(button);
            return button;
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

        #endregion Methods
    }
}