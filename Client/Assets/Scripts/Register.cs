using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Gridia
{
    public class Register : MonoBehaviour
    {
        private RenderableContainer _displayList;
        private bool LoadGame { get; set; }
        private String ErrorMessage { get; set; }

        public void Start()
        {
            _displayList = new RenderableContainer(Vector2.zero);

            var usernameInput = new TextField(Vector2.zero, "usernameInput", 300, 20);
            usernameInput.Text = SceneManager.GetArguement<String>("username");
            _displayList.AddChild(usernameInput);

            var passwordInput = new TextField(Vector2.zero, "passwordInput", 300, 20);
            passwordInput.Y = 30;
            passwordInput.Text = SceneManager.GetArguement<String>("password");
            _displayList.AddChild(passwordInput);

            var registerButton = new Button(Vector2.zero, "Create Account");
            registerButton.Y = 60;
            _displayList.AddChild(registerButton);

            var backButton = new Button(Vector2.zero, "Cancel");
            backButton.X = 120;
            backButton.Y = 60;
            _displayList.AddChild(backButton);

            registerButton.OnClick = () =>
            {
                Locator.Get<ConnectionToGridiaServerHandler>().Register(usernameInput.Text, passwordInput.Text);
            };
            backButton.OnClick = () =>
            {
                SceneManager.LoadScene("ServerTitlescreen");
            };

            Locator.Get<ConnectionToGridiaServerHandler>().GenericEventHandler = (data) =>
            {
                var message = (String)data["obj"];
                if (message == "success")
                {
                    LoadGame = true;
                }
                else
                {
                    ErrorMessage = message;
                }
            };
        }

        public void OnGUI()
        {
            _displayList.X = (Screen.width - _displayList.Width) / 2;
            _displayList.Y = (Screen.height - _displayList.Height) / 2;
            _displayList.Render();

            // :(
            if (ErrorMessage != null)
            {
                var width = 400;
                var height = 75;
                var x = (Screen.width - width) / 2;
                var y = (Screen.height - height) / 2;
                GUI.Window(0, new Rect(x, y, width, height), id =>
                {
                    if (GUI.Button(new Rect(200 - 50 / 2, 30, 50, 20), "OK"))
                    {
                        ErrorMessage = null;
                    }
                }, ErrorMessage);
            }
        }

        public void Update()
        {
            if (LoadGame)
            {
                SceneManager.LoadScene("Main");
            }
        }
    }
}
