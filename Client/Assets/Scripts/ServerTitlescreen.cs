using System.Collections;
using UnityEngine;

namespace Gridia
{
    public class ServerTitlescreen : MonoBehaviour
    {
        private RenderableContainer _displayList;

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
                SceneManager.LoadScene("Main");
            };
            registerButton.OnClick = () =>
            {
                var args = new Hashtable();
                args["username"] = usernameInput.Text;
                args["password"] = passwordInput.Text;
                SceneManager.LoadScene("Register", args);
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
