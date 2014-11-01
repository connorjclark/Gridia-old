using Newtonsoft.Json.Linq;
using Serving;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class ConnectionToGridiaServerHandler : ConnectionToServerHandler
{
    private GridiaGame _game;

    public ConnectionToGridiaServerHandler(GridiaGame game, String host, int port)
        : base(host, port, new GridiaProtocols(), BoundDest.SERVER)
    {
        _game = game;
    }

    protected override void OnConnectionSettled()
    {
        MonoBehaviour.print("Connection settled!");
        GridiaDriver.connectedWaitHandle.Set();
    }

    protected override void HandleData(int type, JObject data)
    {
        Console.WriteLine(type + " - " + data);
    }

    protected override void HandleData(int type, JavaBinaryReader data)
    {
        throw new NotImplementedException();
    }        
}