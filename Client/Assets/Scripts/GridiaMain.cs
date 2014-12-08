using Gridia;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GridiaGame
{
    public TileMap tileMap;
    public TileMapView view;
    public StateMachine stateMachine = new StateMachine();
    public Vector3 selectorDelta = Vector3.zero;
    public bool hideSelector = true;
    public bool isConnected;

    public void Initialize(int size, int depth, int sectorSize) {
        Locator.Provide(stateMachine);
        tileMap = new TileMap(size, depth, sectorSize);
        Locator.Provide(tileMap);
        view = new TileMapView(tileMap, Locator.Get<TextureManager>(), 1.25f);
        Locator.Provide(view);
        stateMachine.SetState(new IdleState());
        Locator.Get<SoundPlayer>().QueueRandomSongs();
    }

    public Vector3 GetSelectorCoord()
    {
        return view.FocusPosition + selectorDelta + new Vector3(view.width / 2, view.height / 2);
    }

    public void PickUpItemAtSelection() 
    {
        PickUpItemAt(view.Focus.Position + selectorDelta);
    }

    private void PickUpItemAt(Vector3 pickupItemLoc)
    {
        pickupItemLoc = tileMap.Wrap(pickupItemLoc);
        var pickupItemIndex = tileMap.ToIndex(pickupItemLoc);
        Locator.Get<ConnectionToGridiaServerHandler>().MoveItem("world", "inv", pickupItemIndex, -1); // :(
    }

    public void UseItemAtSelection(int sourceIndex)
    {
        var destIndex = tileMap.ToIndex(GetSelectorCoord());
        Debug.Log(GetSelectorCoord());
        UseItemAt("inv", sourceIndex, "world", destIndex);
    }

    private void UseItemAt(String source, int sourceIndex, String dest, int destIndex) 
    {
        Locator.Get<ConnectionToGridiaServerHandler>().UseItem(source, "world", sourceIndex, destIndex);
    }
}