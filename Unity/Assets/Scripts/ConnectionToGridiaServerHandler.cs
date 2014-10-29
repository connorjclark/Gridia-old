using Serving;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        throw new NotImplementedException();
    }

    protected override void HandleData(int type, Dictionary<string, string> data)
    {
        throw new NotImplementedException();
    }

    protected override void HandleData(int type, BinaryReader data)
    {
        throw new NotImplementedException();
    }        
}