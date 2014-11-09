using Gridia;
using System.Collections.Generic;
using UnityEngine;

public class GridiaGame
{
    public TileMap tileMap;
    public TileMapView view;
    public StateMachine stateMachine;
    public ConcurrentDictionary<int, Creature> creatures = new ConcurrentDictionary<int, Creature>();
    public bool isConnected = false;

    public void Initialize(int size, int depth, int sectorSize) {
        tileMap = new TileMap(size, depth, sectorSize);
        view = new TileMapView(tileMap, Locator.Get<TextureManager>(), 1.0f);
        Locator.Provide(view);
    }

    public Creature CreateCreature(int id, int image, int x, int y, int z)
    {
        var cre = new Creature(id, image, x, y, z);
        cre.Position = new Vector3(x, y, z);
        tileMap.AddCreature(cre);
        return creatures[id] = cre;
    }

    public void MoveCreature(int id, int x, int y, int z, long time)
    {
        Creature cre;
        creatures.TryGetValue(id, out cre);
        if (cre == null) {
            Debug.LogError("no creature of id: " + id); // :(
            Locator.Get<ConnectionToGridiaServerHandler>().RequestCreature(id);
            return;
        }

        cre.AddPositionSnapshot(new Vector3(x, y, z), time);
    }

    public void RemoveCreature(int id)
    {
        Creature cre;
        creatures.TryGetValue(id, out cre);
        if (cre == null)
        {
            Debug.LogError("no creature of id: " + id); // :(
            Locator.Get<ConnectionToGridiaServerHandler>().RequestCreature(id);
            return;
        }

        Vector3 curLoc = cre.Position;
        tileMap.GetTile((int)curLoc.x, (int)curLoc.y, (int)curLoc.z).Creature = null;
        creatures.Remove(id);
    }
}