using System;
using UnityEngine;

namespace Gridia
{
    public class RegistrationScene : MonoBehaviour
    {
        public String UsernameInput { get; set; }
        public String PasswordInput { get; set; }
        private bool LoadGame { get; set; }
        private String ErrorMessage { get; set; }

        public void Start()
        {
            //SceneManager.GetArguement<String>("password")
            Locator.Get<ConnectionToGridiaServerHandler>().GenericEventHandler = (data) =>
            {
                var message = (String) data["obj"];
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

        public void Back()
        {
            SceneManager.LoadScene("ServerTitlescreen");
        }

        public void Register()
        {
            Locator.Get<ConnectionToGridiaServerHandler>().Register(UsernameInput, PasswordInput);
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
