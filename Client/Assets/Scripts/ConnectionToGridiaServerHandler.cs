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
        //TODO:
        //Unity doesn't compile this : data["id"].Value<int>()
        //So, I have to use: (int)data["id"]
        //Why?
        switch ((GridiaProtocols.Clientbound)type)
        {
            case GridiaProtocols.Clientbound.AddCreature:
                _game.CreateCreature((int)data["id"], (int)data["loc"]["x"], (int)data["loc"]["y"]);
                break;
            case GridiaProtocols.Clientbound.MoveCreature:
                _game.MoveCreature((int)data["id"], (int)data["loc"]["x"], (int)data["loc"]["y"]);
                break;
        }
    }

    protected override void HandleData(int type, JavaBinaryReader data)
    {
        throw new NotImplementedException();
    }        
}