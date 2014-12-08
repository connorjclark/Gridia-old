using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Gridia
{
    public class ServerSelection : MonoBehaviour
    {
        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

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
            connectButton.OnClick = () =>
            {
                var args = new Hashtable();
                args["ip"] = ipInput.Text;
                SceneManager.LoadScene("Main", args);
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
