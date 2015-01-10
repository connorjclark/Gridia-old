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
        private GUISkin _gridiaGuiSkin;

        public void Start()
        {
            GridiaConstants.LoadGUISkins();

            Locator.Provide(this);

            _displayList = new RenderableContainer(Vector2.zero);

            _displayList.AddChild(CreateConnectButton(Vector2.zero, "localhost", 1044));
            _displayList.AddChild(CreateConnectButton(Vector2.zero, "www.hotengames.com", 1044));

            // Connect to specified port

            var connectContainer = new RenderableContainer(Vector2.zero);
            _displayList.AddChild(connectContainer);

            var connectButton = new Button(Vector2.zero, "Connect to other ip");
            connectContainer.AddChild(connectButton);

            var ipInput = new TextField(new Vector2(160, 6), "ipInput", 150, 30);
            connectContainer.AddChild(ipInput);
            ipInput.Text = "enter ip here";

            var portLabel = new Label(new Vector2(320, 10), "Port:  ");
            connectContainer.AddChild(portLabel);

            var portInput = new TextField(new Vector2(360, 12), "portInput", 50, 20);
            connectContainer.AddChild(portInput);
            portInput.Text = "1044";

            // Host local server

            var localServerButton = new Button(Vector2.zero, "Host local server*");
            _displayList.AddChild(localServerButton);

            var java8WarningLabel = new Label(Vector2.zero, "*To host a server, make sure you have Java 8 installed and in your classpath!");
            _displayList.AddChild(java8WarningLabel);

            connectButton.OnClick = () =>
            {
                Connect(ipInput.Text, int.Parse(portInput.Text));
            };

            localServerButton.OnClick = HostLocal;

            var cursorTexture = Resources.Load<Texture2D>("GUI Components/cursorHand_grey");
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }

        private Button CreateConnectButton(Vector2 location, String hostname, int port)
        {
            var text = String.Format("Connect to {0}:{1}", hostname, port);
            var connectButton = new Button(location, text);
            connectButton.OnClick = () =>
            {
                Connect(hostname, port);
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
            _displayList.Y = (Screen.height - _displayList.Height) / 2;
            _displayList.Render();

            var runningY = 0.0f;
            var spacing = 15;
            for (int i = 0; i < _displayList.NumChildren; i++)
            {
                var child = _displayList.GetChildAt(i);
                child.X = (Screen.width - child.Width) / 2;
                child.Y = runningY;
                runningY += child.Height + spacing;
            }

            GridiaConstants.DrawErrorMessage();
        }

        private void Connect(String ip, int port)
        {
            if (GridiaConstants.ErrorMessage != null)
            {
                return;
            }
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
