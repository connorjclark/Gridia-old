using Gridia;
using Newtonsoft.Json;
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

    protected override void Run()
    {
        try
        {
            base.Run();
        }
        catch (Exception ex) 
        {
            Debug.LogError(ex);
        }
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
                _game.CreateCreature((int)data["id"], (int)data["loc"]["x"], (int)data["loc"]["y"], 0);
                break;
            case GridiaProtocols.Clientbound.MoveCreature:
                _game.MoveCreature((int)data["id"], (int)data["loc"]["x"], (int)data["loc"]["y"], 0);
                break;
        }
    }

    protected override void HandleData(int type, JavaBinaryReader data)
    {
        switch ((GridiaProtocols.Clientbound)type)
        {
            case GridiaProtocols.Clientbound.SectorData:
                var sx = data.ReadInt32();
                var sy = data.ReadInt32();
                var sz = data.ReadInt32();
                var sectorSize = _game.tileMap.SectorSize;
                var tiles = new Tile[sectorSize, sectorSize];
                var cm = Locator.Get<ContentManager>();
                for (int x = 0; x < sectorSize; x++)
                {
                    for (int y = 0; y < sectorSize; y++)
                    {
                        var floor = data.ReadInt16();
                        var itemType = data.ReadInt16();
                        var tile = new Tile();
                        tile.Floor = floor;
                        tile.Item = new ItemInstance(cm.GetItem(itemType));
                        tiles[x, y] = tile;
                    }
                }
                _game.tileMap.SetSector(new Sector(tiles), sx, sy, sz);
                break;
        }
    }

    public void RequestSector(int x, int y, int z) {
        Message message = new JsonMessageBuilder()
            .Protocol(Outbound(GridiaProtocols.Serverbound.RequestSector))
            .Set("x", x)
            .Set("y", y)
            .Set("z", z)
            .Build();
        Send(message);
    }
}