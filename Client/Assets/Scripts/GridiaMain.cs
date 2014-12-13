using Gridia;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GridiaGame
{
    public TileMap tileMap;
    public TileMapView view;
    public StateMachine stateMachine = new StateMachine();
    public List<AnimationRenderable> animations = new List<AnimationRenderable>();
    public Vector3 selectorDelta = Vector3.zero;
    public bool hideSelector = true;
    public bool isConnected;

    public void Initialize(int size, int depth, int sectorSize) {
        Locator.Provide(stateMachine);
        tileMap = new TileMap(size, depth, sectorSize);
        Locator.Provide(tileMap);
        view = new TileMapView(tileMap, Locator.Get<TextureManager>(), 1.5f);
        Locator.Provide(view);
        stateMachine.SetState(new IdleState());
        Locator.Get<SoundPlayer>().QueueRandomSongs();
    }

    public Vector3 GetSelectorCoord()
    {
        return GetSelectorCoord(selectorDelta);
    }

    public Vector3 GetSelectorCoord(Vector3 sDelta)
    {
        return view.FocusPosition + sDelta + new Vector3(view.width / 2, view.height / 2);
    }

    public Vector3 GetScreenPosition(Vector3 coord) 
    {
        var tileSize = 32 * view.Scale;
        var relative = coord - view.FocusPosition;
        return new Vector2(relative.x * tileSize, Screen.height - relative.y * tileSize - tileSize);
    }

    public void DropItemAtSelection() 
    {
        DropItemAt(view.Focus.Position + selectorDelta);
    }

    private void DropItemAt(Vector3 dropItemLoc) 
    {
        var destIndex = Locator.Get<TileMap>().ToIndex(dropItemLoc);
        var slotSelected = Locator.Get<GridiaDriver>().invGui.SlotSelected;
        Locator.Get<ConnectionToGridiaServerHandler>().MoveItem("inv", "world", slotSelected, destIndex, 1); // :(
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
        UseItemAt("inv", sourceIndex, "world", destIndex);
    }

    private void UseItemAt(String source, int sourceIndex, String dest, int destIndex) 
    {
        Locator.Get<ConnectionToGridiaServerHandler>().UseItem(source, "world", sourceIndex, destIndex);
    }
}