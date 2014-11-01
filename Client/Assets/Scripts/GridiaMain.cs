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
        tileMap = new TileMap(30);
        var player = CreateCreature(0, 0, 0);
        view = new TileMapView(tileMap, Locator.Get<TextureManager>(), 1.5f);
        view.Focus = player;
        stateMachine = new StateMachine();
        stateMachine.SetState(new PlayerMovementState(player, 4f));
    }

    public Creature CreateCreature(int id, int x, int y)
    {
        var cre = new Creature(id, x, y);
        cre.Position = new Vector2(x, y);
        tileMap.AddCreature(cre);
        return creatures[id] = cre;
    }

    public void MoveCreature(int id, int x, int y)
    {
        var cre = creatures[id];
        if (tileMap.Walkable(x, y))
        {
            cre.MovementDirection = Utilities.GetRelativeDirection(cre.Position, new Vector2(x, y));
        }
    }
}