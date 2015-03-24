using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Gridia
{
    public class ServerSelection : MonoBehaviour
    {
        public static bool connected; // :(
        public static AutoResetEvent gameInitWaitHandle = new AutoResetEvent(false);

        private RenderableContainer _displayList;
        private GUISkin _gridiaGuiSkin;
        private bool connecting;
        private ConnectionToGridiaServerHandler _conn;
        private Texture _bg;

        public void Start()
        {
            MainThreadQueue.Instantiate();
            GridiaConstants.InitializeGuiStuff();
            #if !UNITY_WEBPLAYER
                GridiaConstants.InitializeSoundPlayer();
            #endif

            Locator.Provide(this);

            _bg = Resources.Load<Texture>("bg");

            _displayList = new RenderableContainer(Vector2.zero) {ScaleXY = GridiaConstants.GuiScale};
            _displayList.AddChild(CreateConnectButton(Vector2.zero, "www.hotengames.com", 1044));
            _displayList.AddChild(CreateConnectButton(Vector2.zero, "localhost", 1044));

            // Connect to specified port

            var connectContainer = new RenderableContainer(Vector2.zero);
            _displayList.AddChild(connectContainer);

            var connectButton = new Button(Vector2.zero, "Connect to other ip");
            connectContainer.AddChild(connectButton);

            var scale = GridiaConstants.GuiScale*1.25f;

            var ipInput = new TextField(new Vector2(90*scale, 3*scale), "ipInput", 150, 30);
            connectContainer.AddChild(ipInput);
            ipInput.Text = "enter ip here";

            var portLabel = new Label(new Vector2(180*scale, 5*scale), "Port:  ");
            connectContainer.AddChild(portLabel);

            var portInput = new TextField(new Vector2(200*scale, 6*scale), "portInput", 50, 20);
            connectContainer.AddChild(portInput);
            portInput.Text = "1044";

            // Host local server
            #if !UNITY_WEBPLAYER

                var localServerButton = new Button(Vector2.zero, "Host local server*");
                _displayList.AddChild(localServerButton);

                var java8WarningLabel = new Label(Vector2.zero, "*To host a server, make sure you have Java 8 installed and in your classpath!");
                _displayList.AddChild(java8WarningLabel);

                localServerButton.OnClick = HostLocal;

            #endif

            connectButton.OnClick = () =>
            {
                Connect(ipInput.Text, int.Parse(portInput.Text));
            };

            // close application

            var closeButton = new Button(Vector2.zero, "Exit Gridia");
            _displayList.AddChild(closeButton);
            closeButton.OnClick = Application.Quit;

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

        #if !UNITY_WEBPLAYER
            public void HostLocal()
            {
                var processInfo = new System.Diagnostics.ProcessStartInfo {FileName = "java"};
                if (Application.isEditor)
                {
                    processInfo.Arguments = "-jar target/server.jar";
                    processInfo.WorkingDirectory = "../Server/GridiaServer/";
                }
                else
                {
                    processInfo.Arguments = "-jar server.jar";
                }

                var proc = new System.Diagnostics.Process {StartInfo = processInfo};
                proc.Start();
            }
        #endif
        
        public void Update() {
            if (GridiaConstants.ErrorMessage == null && connected)
            {
                connected = false;
                SceneManager.LoadScene("ServerTitlescreen");
            }
        }

        public void OnGUI()
        {
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), _bg);

            if (connecting && GridiaConstants.ErrorMessage != null) 
            {
                connecting = false;
            }
            if (connecting)
            {
                var labelMessage = _conn.FileDownloadStatus;
                var centeredTextStyle = new GUIStyle("label") {fontSize = 16};
                var textSize = centeredTextStyle.CalcSize(new GUIContent(labelMessage));
                var x = (Screen.width - textSize.x) / 2;
                var y = (Screen.height - textSize.y) / 2;
                GUI.Label(new Rect(x, y, textSize.x, textSize.y), labelMessage, centeredTextStyle);
            } 
            else
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

                GridiaConstants.DrawErrorMessage();
            }
        }

        private void Connect(String ip, int port)
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
                        Debug.Log(ex);
                        GridiaConstants.ErrorMessage = "Connection to server has been lost.";
                        GridiaConstants.ErrorMessageAction = () => { SceneManager.LoadScene("ServerSelection"); };
                    }
                }).Start();
            }
            catch (SocketException ex)
            {
                GridiaConstants.ErrorMessage = "Could not connect to " + ip + " at port " + port;
                Debug.Log(ex);
            }
        }
    }
}
