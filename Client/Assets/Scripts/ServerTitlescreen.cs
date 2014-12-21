using System;
using System.Collections;
using UnityEngine;

namespace Gridia
{
    public class ServerTitlescreen : MonoBehaviour
    {
        private RenderableContainer _displayList;
        private bool LoadGame { get; set; }
        private String ErrorMessage { get; set; }

        public void Start()
        {
            _displayList = new RenderableContainer(Vector2.zero);

            var usernameInput = new TextField(Vector2.zero, "usernameInput", 300, 20);
            _displayList.AddChild(usernameInput);

            var passwordInput = new TextField(Vector2.zero, "passwordInput", 300, 20);
            passwordInput.Y = 30;
            _displayList.AddChild(passwordInput);

            var loginButton = new Button(Vector2.zero, "Login");
            loginButton.Y = 60;
            _displayList.AddChild(loginButton);

            var registerButton = new Button(Vector2.zero, "Register");
            registerButton.X = 60;
            registerButton.Y = 60;
            _displayList.AddChild(registerButton);

            loginButton.OnClick = () =>
            {
                if (ErrorMessage == null)
                {
                    var passwordHash = passwordInput.Text;
                    Locator.Get<ConnectionToGridiaServerHandler>().Login(usernameInput.Text, passwordHash);
                }
            };
            registerButton.OnClick = () =>
            {
                if (ErrorMessage == null)
                {
                    var args = new Hashtable();
                    args["username"] = usernameInput.Text;
                    args["password"] = passwordInput.Text;
                    SceneManager.LoadScene("Register", args);
                }
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
