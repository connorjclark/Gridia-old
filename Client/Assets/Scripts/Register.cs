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
            _displayList = new RenderableContainer(Vector2.zero) {ScaleXY = GridiaConstants.GuiScale};

            var usernameLabel = new Label(Vector2.zero, "Username", true);
            _displayList.AddChild(usernameLabel);

            var usernameInput = new TextField(Vector2.zero, "usernameInput", 300, 20)
            {
                MaxChars = 20,
                Text = SceneManager.GetArguement<String>("username")
            };
            _displayList.AddChild(usernameInput);

            var passwordLabel = new Label(Vector2.zero, "Password", true);
            _displayList.AddChild(passwordLabel);

            var passwordInput = new TextField(Vector2.zero, "passwordInput", 300, 20)
            {
                PasswordField = true,
                Text = SceneManager.GetArguement<String>("password")
            };
            _displayList.AddChild(passwordInput);

            var registerButton = new Button(Vector2.zero, "Create Account");
            _displayList.AddChild(registerButton);

            var backButton = new Button(Vector2.zero, "Cancel");
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
            _displayList.Y = (Screen.height - _displayList.Height) / 2;
            _displayList.Render();

            var runningY = 0.0f;
            const int spacing = 15;
            for (var i = 0; i < _displayList.NumChildren; i++)
            {
                var child = _displayList.GetChildAt(i);
                child.X = (Screen.width - child.Width) / 2;
                child.Y = runningY;
                runningY += child.Height + spacing;
            }

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
