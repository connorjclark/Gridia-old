using System;
using System.Collections;
using UnityEngine;

namespace Gridia
{
    public class ServerTitlescreen : MonoBehaviour
    {
        private RenderableContainer _displayList;
        private bool LoadGame { get; set; }
        private Texture _bg;

        public void Start()
        {
            #if UNITY_WEBPLAYER
                GridiaConstants.InitializeSoundPlayer();
            #endif

            _bg = Resources.Load<Texture>("bg");

            _displayList = new RenderableContainer(Vector2.zero) {ScaleXY = GridiaConstants.GuiScale};

            var usernameLabel = new Label(Vector2.zero, "Username", true);
            _displayList.AddChild(usernameLabel);

            var usernameInput = new TextField(Vector2.zero, "usernameInput", 300, 20);
            _displayList.AddChild(usernameInput);

            var passwordLabel = new Label(Vector2.zero, "Password", true);
            _displayList.AddChild(passwordLabel);

            var passwordInput = new TextField(Vector2.zero, "passwordInput", 300, 20);
            passwordInput.PasswordField = true;

            _displayList.AddChild(passwordInput);

            var loginButton = new Button(Vector2.zero, "Login");
            _displayList.AddChild(loginButton);

            var registerButton = new Button(Vector2.zero, "Register");
            _displayList.AddChild(registerButton);

            var disconnectButton = new Button(Vector2.zero, "Disconnect from server");
            _displayList.AddChild(disconnectButton);
            disconnectButton.OnClick = () => {
                Locator.Get<ConnectionToGridiaServerHandler>().Close();
                SceneManager.LoadScene("ServerSelection");
            };

            loginButton.OnClick = usernameInput.OnEnter = passwordInput.OnEnter = () => {
                SendLoginRequest(usernameInput.Text, passwordInput.Text);
            };
            
            registerButton.OnClick = () =>
            {
                if (GridiaConstants.ErrorMessage == null)
                {
                    var args = new Hashtable();
                    args["username"] = usernameInput.Text;
                    args["password"] = passwordInput.Text;
                    SceneManager.LoadScene("RegistrationScene", args);
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
                    loggingIn = false;
                }
            };
        }

        public void OnGUI()
        {
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), _bg);

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

        private bool loggingIn = false; // :(

        private void SendLoginRequest(string username, string password)
        {
            if (GridiaConstants.ErrorMessage == null && !loggingIn)
            {
                loggingIn = true;
                Locator.Get<ConnectionToGridiaServerHandler>().Login(username, password);
            }
        }
    }
}
