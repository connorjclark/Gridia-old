using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Gridia
{
    public class ServerSelection : MonoBehaviour
    {
        public static AutoResetEvent connectedWaitHandle = new AutoResetEvent(false); // :(
        public static AutoResetEvent gameInitWaitHandle = new AutoResetEvent(false);

        private RenderableContainer _displayList;

        public void Start()
        {
            Locator.Provide(this);

            _displayList = new RenderableContainer(Vector2.zero);

            _displayList.AddChild(CreateConnectButton(Vector2.zero, "localhost"));
            _displayList.AddChild(CreateConnectButton(new Vector2(0, 30), "www.hotengames.com"));

            var connectButton = new Button(new Vector2(0, 60), "Connect to other ip");
            _displayList.AddChild(connectButton);

            var ipInput = new TextField(new Vector2(20, 90), "ipInput", 300, 20);
            _displayList.AddChild(ipInput);
            ipInput.Text = "enter ip here";

            var localServerButton = new Button(new Vector2(0, 120), "Host local server*");
            _displayList.AddChild(localServerButton);

            var java8WarningLabel = new Label(new Vector2(0, 150), "*To host a server, make sure you hava Java 8 installed and in your classpath!");
            _displayList.AddChild(java8WarningLabel);

            connectButton.OnClick = () =>
            {
                Connect(ipInput.Text, 1234);
            };

            localServerButton.OnClick = HostLocal;
        }

        private Button CreateConnectButton(Vector2 location, String ip)
        {
            var connectButton = new Button(location, "Connect to " + ip);
            connectButton.OnClick = () =>
            {
                Connect(ip, 1234);
            };
            return connectButton;
        }

        public void HostLocal()
        {
            var processInfo = new System.Diagnostics.ProcessStartInfo();
            processInfo.FileName = "java";
            if (Application.isEditor)
            {
                processInfo.Arguments = "-jar target/server.jar";
                processInfo.WorkingDirectory = "../Server/GridiaServer/";
            }
            else
            {
                processInfo.Arguments = "-jar server.jar";
            }

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = processInfo;
            proc.Start();
        }
        
        public void OnGUI()
        {
            _displayList.X = (Screen.width - _displayList.Width) / 2;
            _displayList.Y = (Screen.height - _displayList.Height) / 2;
            _displayList.Render();
            GridiaConstants.DrawErrorMessage();
        }

        private void Connect(String ip, int port)
        {
            var game = new GridiaGame();
            try
            {
                var conn = new ConnectionToGridiaServerHandler(ip, port, game);
                Locator.Provide(game);
                Locator.Provide(conn);

                new Thread(() =>
                {
                    try
                    {
                        conn.Start(() => Debug.Log("Connection settled!"), conn);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex);
                        GridiaConstants.ErrorMessage = "Connection to server has been lost.";
                        GridiaConstants.ErrorMessageAction = () => { SceneManager.LoadScene("ServerSelection"); };
                    }
                }).Start();

                connectedWaitHandle.WaitOne();
                if (GridiaConstants.ErrorMessage == null)
                {
                    SceneManager.LoadScene("ServerTitlescreen");
                }
            }
            catch (SocketException ex)
            {
                GridiaConstants.ErrorMessage = "Could not connect to " + ip + " at port " + port;
            }
        }
    }
}
