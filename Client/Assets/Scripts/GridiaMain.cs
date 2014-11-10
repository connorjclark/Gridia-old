using Gridia;
using System.Collections.Generic;
using UnityEngine;

public class GridiaGame
{
    public TileMap tileMap;
    public TileMapView view;
    public StateMachine stateMachine;
    public bool isConnected = false;

    public void Initialize(int size, int depth, int sectorSize) {
        tileMap = new TileMap(size, depth, sectorSize);
        view = new TileMapView(tileMap, Locator.Get<TextureManager>(), 1.0f);
        Locator.Provide(view);
        //Locator.Provide(tileMap);
    }
}