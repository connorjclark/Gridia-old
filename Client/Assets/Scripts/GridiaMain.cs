using Gridia;
using System.Collections.Generic;
using UnityEngine;

public class GridiaGame
{
    public TileMap tileMap;
    public TileMapView view;
    public StateMachine stateMachine = new StateMachine();
    public bool isConnected = false;

    public void Initialize(int size, int depth, int sectorSize) {
        tileMap = new TileMap(size, depth, sectorSize);
        Locator.Provide(tileMap);
        view = new TileMapView(tileMap, Locator.Get<TextureManager>(), 1.0f);
        Locator.Provide(view);
        stateMachine.SetState(new IdleState());
        Locator.Get<SoundPlayer>().QueueRandomSongs();
    }
}