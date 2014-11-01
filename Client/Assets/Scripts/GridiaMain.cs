using Gridia;
using System.Collections.Generic;
using UnityEngine;

public class GridiaGame
{
    public TileMap tileMap;
    public TileMapView view;
    public StateMachine stateMachine;
    public Dictionary<int, Creature> creatures = new Dictionary<int, Creature>();
    public bool isConnected = false;

    public GridiaGame() {
    }

    public void Initialize() {
        tileMap = new TileMap(100, 1, 20);/* do not hardcode */
        var player = CreateCreature(0, 0, 0, 0);
        view = new TileMapView(tileMap, Locator.Get<TextureManager>(), 1.5f);
        view.Focus = player;
        stateMachine = new StateMachine();
        stateMachine.SetState(new PlayerMovementState(player, 4f));
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
        if (cre == null) return;
        if (tileMap.Walkable(x, y, z))
        {
            cre.MovementDirection = Utilities.GetRelativeDirection(cre.Position, new Vector3(x, y, z));
        }
    }
}