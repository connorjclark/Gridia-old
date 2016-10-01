using System;
using System.Collections.Generic;

using Gridia;

using MarkLight.UnityProject;

using UnityEngine;

public class GridiaGame
{
    #region Fields

    public List<AnimationRenderable> Animations = new List<AnimationRenderable>();
    public bool HideSelector = true;
    public bool IsConnected;
    public StateMachine StateMachine = new StateMachine();
    public TileMap TileMap;
    public TileMapView View;

    private GridiaDriver _driver;
    private Vector3 _selectorDelta = Vector3.zero;

    #endregion Fields

    #region Properties

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

    #endregion Properties

    #region Methods

    public void CreateCreature(int id, String name, CreatureImage image, int x, int y, int z)
    {
        var cre = TileMap.CreateCreature(id, name, image, x, y, z);
        if (cre != null) MainThreadQueue.Add(() => _driver.AddCreature(cre));
    }

    public void DropItemAtSelection()
    {
        if (_driver.SelectedContainer == null)
        {
            DropItemAt(View.Focus.Position + SelectorDelta);
        }
        else
        {
            var destIndex = _driver.SelectedContainer.SlotSelected;
            var slotSelected = _driver.InvGui.SlotSelected;
            Locator.Get<ConnectionToGridiaServerHandler>().MoveItem(Main.Instance.InventoryContainerId, _driver.SelectedContainer.ContainerId, slotSelected, destIndex, 1); // :(
        }
    }

    public List<Creature> GetCreaturesNearPlayer(int rangex, int rangey, int limit)
    {
        var loc = View.Focus.Position;
        var list = new List<Creature>();
        var pos = new Vector3(loc.x, loc.y, loc.z);
        var dir = Vector2.right;
        var segmentLength = 1;
        var segmentPassed = 0;
        while (list.Count < limit)
        {
            var inXRange = Math.Abs(loc.x - pos.x) <= rangex;
            var inYRange = Math.Abs(loc.y - pos.y) <= rangey;
            if (!inXRange && !inYRange) break;
            if (inXRange && inYRange)
            {
                var cre = TileMap.GetCreatureAt(pos);
                if (cre != null && cre != View.Focus)
                {
                    list.Add(cre);
                }
            }
            IncreaseInSpriral(ref pos, ref dir, ref segmentLength, ref segmentPassed);
        }
        return list;
    }

    public Vector3 GetScreenPosition(Vector3 coord)
    {
        var tileSize = 32 * View.Scale;
        var relative = coord - View.FocusPosition;
        return new Vector2(relative.x * tileSize, Screen.height - relative.y * tileSize - tileSize);
    }

    public Vector3 GetSelectorCoord()
    {
        return GetSelectorCoord(SelectorDelta);
    }

    // :(
    public Vector3 GetSelectorCoord(Vector3 sDelta)
    {
        return View.FocusPosition + sDelta + new Vector3(View.Width / 2, View.Height / 2);
    }

    // :(
    public void InitAdminWindowTab()
    {
        var adminWindow = new AdminWindow(Vector2.zero);
        _driver.TabbedGui.Add(10, adminWindow, false);
    }

    public void Initialize(int size, int depth, int sectorSize)
    {
        Locator.Provide(StateMachine);
        TileMap = new TileMap(size, depth, sectorSize);
        Locator.Provide(TileMap);
        View = new TileMapView(TileMap, Locator.Get<TextureManager>(), 1.0f);
        Locator.Provide(View);
        StateMachine.SetState(new IdleState());
        _driver = Locator.Get<GridiaDriver>();
    }

    public void PickUpItemAtSelection()
    {
        if (_driver.SelectedContainer == null)
        {
            PickUpItemAt(View.Focus.Position + SelectorDelta);
        }
        else
        {
            var pickupItemIndex = _driver.SelectedContainer.SlotSelected;
            Locator.Get<ConnectionToGridiaServerHandler>().MoveItem(_driver.SelectedContainer.ContainerId, Main.Instance.InventoryContainerId, pickupItemIndex, -1); // :(
        }
    }

    public void RemoveCreature(int id)
    {
        TileMap.RemoveCreature(id);
        MainThreadQueue.Add(() => GameObject.Destroy(GameObject.Find("Creature " + id)));
        if (_driver.SelectedCreature != null && _driver.SelectedCreature.Id == id) _driver.SelectedCreature = null;
    }

    public void UseItemAtSelection(int sourceIndex)
    {
        if (_driver.SelectedContainer == null)
        {
            var destIndex = TileMap.ToIndex(GetSelectorCoord());
            UseItemAt(Main.Instance.InventoryContainerId, sourceIndex, 0, destIndex);
        }
        else
        {
            UseItemAt(Main.Instance.InventoryContainerId, sourceIndex, _driver.SelectedContainer.ContainerId, _driver.SelectedContainer.SlotSelected);
        }
    }

    private void DropItemAt(Vector3 dropItemLoc)
    {
        var destIndex = Locator.Get<TileMap>().ToIndex(dropItemLoc);
        var slotSelected = _driver.InvGui.SlotSelected;
        Locator.Get<ConnectionToGridiaServerHandler>().MoveItem(Main.Instance.InventoryContainerId, 0, slotSelected, destIndex, 1); // :(
    }

    private void IncreaseInSpriral(ref Vector3 pos, ref Vector2 dir, ref int segmentLength, ref int segmentPassed)
    {
        pos.x += dir.x;
        pos.y += dir.y;
        ++segmentPassed;

        if (segmentPassed == segmentLength)
        {
            segmentPassed = 0;
            var buffer = dir.x;
            dir.x = -dir.y;
            dir.y = buffer;
            if (dir.y == 0)
            {
                ++segmentLength;
            }
        }
    }

    private void PickUpItemAt(Vector3 pickupItemLoc)
    {
        pickupItemLoc = TileMap.Wrap(pickupItemLoc);
        var pickupItemIndex = TileMap.ToIndex(pickupItemLoc);
        Locator.Get<ConnectionToGridiaServerHandler>().MoveItem(0, Main.Instance.InventoryContainerId, pickupItemIndex, -1); // :(
    }

    private void UseItemAt(int source, int sourceIndex, int dest, int destIndex)
    {
        Locator.Get<ConnectionToGridiaServerHandler>().UseItem(source, dest, sourceIndex, destIndex);
    }

    #endregion Methods
}