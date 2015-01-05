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

    public Vector3 _selectorDelta = Vector3.zero;
    public Vector3 SelectorDelta
    {
        get
        {
            return _selectorDelta;
        }
        set
        {
            _selectorDelta = value;
            _selectorDelta.x = Mathf.Clamp(_selectorDelta.x, -2, 2);
            _selectorDelta.y = Mathf.Clamp(_selectorDelta.y, -2, 2);
        }
    }

    public bool hideSelector = true;
    public bool isConnected;

    public void Initialize(int size, int depth, int sectorSize) {
        Locator.Provide(stateMachine);
        tileMap = new TileMap(size, depth, sectorSize);
        Locator.Provide(tileMap);
        view = new TileMapView(tileMap, Locator.Get<TextureManager>(), 1.75f);
        Locator.Provide(view);
        stateMachine.SetState(new IdleState());
        Locator.Get<SoundPlayer>().QueueRandomSongs();
    }

    // :(
    public void InitAdminWindowTab()
    {
        var adminWindow = new AdminWindow(Vector2.zero);
        Locator.Get<GridiaDriver>().tabbedGui.Add(10, adminWindow, false);
    }

    public Vector3 GetSelectorCoord()
    {
        return GetSelectorCoord(SelectorDelta);
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
        DropItemAt(view.Focus.Position + SelectorDelta);
    }

    private void DropItemAt(Vector3 dropItemLoc) 
    {
        var destIndex = Locator.Get<TileMap>().ToIndex(dropItemLoc);
        var slotSelected = Locator.Get<GridiaDriver>().invGui.SlotSelected;
        Locator.Get<ConnectionToGridiaServerHandler>().MoveItem("inv", "world", slotSelected, destIndex, 1); // :(
    }

    public void PickUpItemAtSelection() 
    {
        PickUpItemAt(view.Focus.Position + SelectorDelta);
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