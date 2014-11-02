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

    public Creature CreateCreature(int id, int x, int y, int z)
    {
        var cre = new Creature(id, x, y, z);
        cre.Position = new Vector3(x, y, z);
        tileMap.AddCreature(cre);
        return creatures[id] = cre;
    }

    public void MoveCreature(int id, int x, int y, int z)
    {
        Creature cre;
        creatures.TryGetValue(id, out cre);
        if (cre == null) {
            Debug.LogError("no creature of id: " + id); // :(
            Locator.Get<ConnectionToGridiaServerHandler>().RequestCreature(id);
            return;
        }
        if (tileMap.Walkable(x, y, z))
        {
            cre.MovementDirection = Utilities.GetRelativeDirection(cre.Position, new Vector3(x, y, z));
        }
    }

    public void RemoveCreature(int id)
    {
        creatures.Remove(id);
    }
}