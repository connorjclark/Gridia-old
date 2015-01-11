using System;
using System.Collections;
using UnityEngine;

namespace Gridia
{
    public class ServerTitlescreen : MonoBehaviour
    {
        private RenderableContainer _displayList;
        private bool LoadGame { get; set; }

        public void Start()
        {
            _displayList = new RenderableContainer(Vector2.zero);

            var usernameLabel = new Label(Vector2.zero, "Username: ");
            _displayList.AddChild(usernameLabel);

            var usernameInput = new TextField(new Vector2(80, 0), "usernameInput", 300, 20);
            _displayList.AddChild(usernameInput);

            var passwordLabel = new Label(new Vector2(0, 30), "Password: ");
            _displayList.AddChild(passwordLabel);

            var passwordInput = new TextField(new Vector2(80, 30), "passwordInput", 300, 20);
            passwordInput.PasswordField = true;

            _displayList.AddChild(passwordInput);

            var loginButton = new Button(new Vector2(0, 60), "Login");
            _displayList.AddChild(loginButton);

            var registerButton = new Button(new Vector2(100, 60), "Register");
            _displayList.AddChild(registerButton);

            loginButton.OnClick = () =>
            {
                if (GridiaConstants.ErrorMessage == null)
                {
                    Locator.Get<ConnectionToGridiaServerHandler>().Login(usernameInput.Text, passwordInput.Text);
                }
            };
            registerButton.OnClick = () =>
            {
                if (GridiaConstants.ErrorMessage == null)
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
                    GridiaConstants.ErrorMessage = message;
                }
            };
        }

        public void OnGUI()
        {
            _displayList.X = (Screen.width - _displayList.Width) / 2;
            _displayList.Y = (Screen.height - _displayList.Height) / 2;
            _displayList.Render();

            GridiaConstants.DrawErrorMessage();
        }

        public void Update()
        {
            if (LoadGame)
            {
                SceneManager.LoadScene("Main");
            }
        }

        public void OnApplicationQuit()
        {
            GridiaConstants.OnApplicationQuit();
        }
    }
}
