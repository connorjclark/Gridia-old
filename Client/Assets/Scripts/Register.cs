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
            backButton.X = 60;
            backButton.Y = 60;
            _displayList.AddChild(backButton);

            registerButton.OnClick = () =>
            {
                var username = usernameInput.Text;
                var passwordHash = passwordInput.Text; // ...
                Locator.Get<ConnectionToGridiaServerHandler>().Register(username, passwordHash);
                //SceneManager.LoadScene("ServerTitlescreen");
            };
            backButton.OnClick = () =>
            {
                SceneManager.LoadScene("ServerTitlescreen");
            };
        }

        public void OnGUI()
        {
            _displayList.X = (Screen.width - _displayList.Width) / 2;
            _displayList.Y = (Screen.height - _displayList.Height) / 2;
            _displayList.Render();
        }
    }
}
