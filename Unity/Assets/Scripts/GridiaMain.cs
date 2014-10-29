using Gridia;
using System.Collections.Generic;
using UnityEngine;

public class GridiaGame
{
    public TileMap tileMap;
    public TileMapView view;
    public StateMachine stateMachine;
    public List<Creature> creatures;

    public GridiaGame() {
        creatures = new List<Creature>();
        ContentManager contentManager = new ContentManager();
        TextureManager textureManager = new TextureManager();

        tileMap = new TileMap(30);
        Player player = CreatePlayer(0, 0);

        view = new TileMapView(tileMap, textureManager, 2.0f);
        view.Focus = player;

        stateMachine = new StateMachine();
        stateMachine.SetState(new PlayerMovementState(player, 4f));
    }

    public Player CreatePlayer(int x, int y)
    {
        Player cre = new Player();
        cre.Position = new Vector2(x, y);
        tileMap.AddCreature(cre);
        creatures.Add(cre);
        return cre;
    }

    public void CreateCreature(int x, int y)
    {
        Creature cre = new Creature();
        cre.Position = new Vector2(x, y);
        tileMap.AddCreature(cre);
        creatures.Add(cre);
    }
}