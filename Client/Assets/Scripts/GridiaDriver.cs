﻿using Gridia;
using System.IO;
using System.Threading;
using UnityEngine;
using System.Linq;
using Serving;
using System.Collections.Generic;
using System;

public class GridiaDriver : MonoBehaviour
{
    public static AutoResetEvent connectedWaitHandle = new AutoResetEvent(false);
    public static AutoResetEvent gameInitWaitHandle = new AutoResetEvent(false);
    private GridiaGame _game;
    public InventoryGUI invGui;
    public ChatGUI chatGui;

    void Start()
    {
        invGui = new InventoryGUI(new Vector2(0, Int32.MaxValue), 1.5f);
        Locator.Provide(invGui);

        chatGui = new ChatGUI(new Vector2(Int32.MaxValue, Int32.MaxValue));
        Locator.Provide(chatGui);

        _game = new GridiaGame();
        Locator.Provide(_game);
        ResizeCamera();

        MonoBehaviour.print("connecting");
        ConnectionToGridiaServerHandler conn = new ConnectionToGridiaServerHandler(_game, "localhost", 1234);
        //ConnectionToGridiaServerHandler conn = new ConnectionToGridiaServerHandler(_game, "23.102.24.247", 1234);
        Locator.Provide(conn);
        conn.Start();

        connectedWaitHandle.WaitOne();

        Locator.Provide(new ContentManager("TestWorld")); // :(
        Locator.Provide(new TextureManager("TestWorld"));
        _game.Initialize(GridiaConstants.SIZE, GridiaConstants.DEPTH, GridiaConstants.SECTOR_SIZE); // :(

        gameInitWaitHandle.Set();
    }

    void OnGUI() {
        invGui.Render();
        if (mouseDownItem != null)
        {
            var rect = new Rect((int)Input.mousePosition.x - 16, Screen.height - (int)Input.mousePosition.y - 16, 32, 32);
            invGui.RenderSlot(rect, mouseDownItem);
        }

        chatGui.Render();

        //temp :(
        var cm = Locator.Get<ContentManager>();
        foreach (var cre in _game.tileMap.creatures.ValuesToList()) 
        {
            var pos = cre.Position;
            var focusPos = _game.view.FocusPosition;
            var relative = pos - focusPos;
            var rect = new Rect(relative.x * 32, Screen.height - relative.y * 32 - 32, 32, 32);

            var textures = Locator.Get<TextureManager>();

            int spriteId = cre.Image;
            int textureX = (spriteId % GridiaConstants.SPRITES_IN_SHEET) % GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
            int textureY = 9 - (spriteId % GridiaConstants.SPRITES_IN_SHEET) / GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
            var texCoords = new Rect(textureX / 10.0f, textureY / 10.0f, 1 / 10.0f, 1 / 10.0f); // :( don't hardcode 10
            GUI.DrawTextureWithTexCoords(rect, textures.GetCreaturesTexture(spriteId / GridiaConstants.SPRITES_IN_SHEET), texCoords);
        }
    }

    ItemInstance mouseDownItem = null; // :(
    int downSlot;
    int sourceIndex;
    String mouseDownLocation;

    Vector2 getMouse() {
        var pos = Input.mousePosition;
        pos.y = Screen.height - pos.y;
        return pos;
    }

    bool isMouseOverGUI() 
    {
        var mouse = getMouse();
        return invGui.ResizingWindow || invGui.MouseOver || chatGui.MouseOver || chatGui.ResizingWindow;
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (invGui.MouseDownSlot != -1)
            {
                mouseDownLocation = "inv";
                sourceIndex = invGui.MouseDownSlot;
                mouseDownItem = invGui.Inventory[invGui.MouseDownSlot];
            }
            else if (!isMouseOverGUI())
            {
                mouseDownLocation = "world";
                var downCoord = getTileLocationOfMouse();
                mouseDownItem = _game.tileMap.GetTile((int)downCoord.x, (int)downCoord.y, (int)downCoord.z).Item;
                sourceIndex = _game.tileMap.ToIndex(downCoord);
            }
        }
        else if (Input.GetMouseButtonUp(0) && mouseDownItem != null)
        {
            String dest;
            int destIndex;
            if (invGui.MouseUpSlot != -1)
            {
                dest = "inv";
                destIndex = invGui.MouseUpSlot;
            }
            else
            {
                dest = "world";
                destIndex = _game.tileMap.ToIndex(getTileLocationOfMouse());
            }
            Locator.Get<ConnectionToGridiaServerHandler>().MoveItem(mouseDownLocation, dest, sourceIndex, destIndex);
            mouseDownItem = null;
        }

        if (_game.stateMachine != null) {
            _game.stateMachine.Step(Time.deltaTime);
        }
        _game.view.Render();
        ResizeCamera(); // :( only on resize
    }

    Vector3 getTileLocationOfMouse() {
        int x = (int)(Input.mousePosition.x / (GridiaConstants.SPRITE_SIZE * _game.view.Scale));
        int y = (int)(Input.mousePosition.y / (GridiaConstants.SPRITE_SIZE * _game.view.Scale));

        x = _game.tileMap.Wrap(x + (int)_game.view.FocusPosition.x);
        y = _game.tileMap.Wrap(y + (int)_game.view.FocusPosition.y);

        return new Vector3(x, y);
    }

    void ResizeCamera()
    {
        Camera camera = Camera.main;
        camera.orthographicSize = Screen.height / 2.0f;
        camera.transform.position = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, -100);
    }
}
