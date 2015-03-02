using System;
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

            var usernameLabel = new Label(Vector2.zero, "Username: ");
            _displayList.AddChild(usernameLabel);

            var usernameInput = new TextField(new Vector2(80, 0), "usernameInput", 300, 20)
            {
                MaxChars = 20,
                Text = SceneManager.GetArguement<String>("username")
            };
            _displayList.AddChild(usernameInput);

            var passwordLabel = new Label(new Vector2(0, 30), "Password: ");
            _displayList.AddChild(passwordLabel);

            var passwordInput = new TextField(new Vector2(80, 30), "passwordInput", 300, 20)
            {
                PasswordField = true,
                Text = SceneManager.GetArguement<String>("password")
            };
            _displayList.AddChild(passwordInput);

            var registerButton = new Button(new Vector2(0, 60), "Create Account");
            _displayList.AddChild(registerButton);

            var backButton = new Button(new Vector2(140, 60), "Cancel");
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
            if (ErrorMessage == null) return;
            const int width = 400;
            const int height = 75;
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
