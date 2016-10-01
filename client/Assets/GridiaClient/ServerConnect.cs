using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Gridia
{
    public class ServerConnect
    {
        // TODO remove these bools
        public static bool connected;
        public static bool connecting;
        private static bool loggingIn;
        private static ConnectionToGridiaServerHandler _conn;

        public static void Connect(String ip, int port)
        {
            if (GridiaConstants.ErrorMessage != null || connecting)
            {
                return;
            }
            Debug.Log("Connecting to server ...");
            var game = new GridiaGame();
            try
            {
                _conn = new ConnectionToGridiaServerHandler(ip, port, game);
                Locator.Provide(game);
                Locator.Provide(_conn);

                new Thread(() =>
                {
                    try
                    {
                        Debug.Log("Starting connection ...");
                        connecting = true;
                        _conn.Start(() => Debug.Log("Connection settled!"), _conn);
                        Debug.Log("connection started");
                    }
                    catch (Exception ex)
                    {
                        connecting = false;
                        Debug.LogError(ex);
                        GridiaConstants.ErrorMessage = "Connection to server has been lost.";
                        GridiaConstants.ErrorMessageAction = () =>
                        {
                            GridiaConstants.SoundPlayer.MuteSfx = GridiaConstants.SoundPlayer.MuteMusic = true;
                            SceneManager.LoadScene("ServerSelection");
                        };
                    }
                }).Start();
            }
            catch (SocketException ex)
            {
                GridiaConstants.ErrorMessage = "Could not connect to " + ip + " at port " + port;
                Debug.Log(ex);
            }
        }

        public static void SendLoginRequest(String username, String password, Action<bool, String> callback)
        {
            if (GridiaConstants.ErrorMessage == null && !loggingIn && connected)
            {
                loggingIn = true;
                Locator.Get<ConnectionToGridiaServerHandler>().GenericEventHandler = (data) =>
                {
                    var message = (String) data["obj"];
                    callback(message == "success", message);
                };
                Locator.Get<ConnectionToGridiaServerHandler>().Login(username, password);
            }
            else
            {
                callback(false, "Bad Connection");
            }
        }

        public static void SendRegistrationRequest(String username, String password, Action<bool, String> callback)
        {
            if (GridiaConstants.ErrorMessage == null && !loggingIn && connected)
            {
                Locator.Get<ConnectionToGridiaServerHandler>().GenericEventHandler = (data) =>
                {
                    var message = (String) data["obj"];
                    callback(message == "success", message);
                };
                Locator.Get<ConnectionToGridiaServerHandler>().Register(username, password);
            }
            else
            {
                callback(false, "Bad Connection");
            }
        }
    }
}