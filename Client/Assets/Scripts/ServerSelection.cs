using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
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
            _displayList = new RenderableContainer(Vector2.zero);

            var ipInput = new TextField(Vector2.zero, "ipInput", 300, 20);
            _displayList.AddChild(ipInput);
            ipInput.Text = "localhost";

            var connectButton = new Button(Vector2.zero, "Connect");
            _displayList.AddChild(connectButton);
            connectButton.Y = 30;

            var localServerButton = new Button(Vector2.zero, "Host local server");
            _displayList.AddChild(localServerButton);
            localServerButton.Y = 60;

            connectButton.OnClick = () =>
            {
                Connect(ipInput.Text, 1234);
                SceneManager.LoadScene("ServerTitlescreen");
            };

            localServerButton.OnClick = HostLocal;
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
                processInfo.WorkingDirectory = "Server/GridiaServer/";
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
        }

        private void Connect(String ip, int port)
        {
            Debug.Log("connecting");
            var game = new GridiaGame();
            Locator.Provide(game);
            ConnectionToGridiaServerHandler conn = new ConnectionToGridiaServerHandler(game, ip, port);
            Locator.Provide(conn);
            conn.Start();
            connectedWaitHandle.WaitOne();
        }
    }
}
