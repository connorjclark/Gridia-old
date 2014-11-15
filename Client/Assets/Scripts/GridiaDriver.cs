using Gridia;
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
    public GridiaGame _game; // :(
    public TabbedUI tabbedGui;
    public InventoryGUI invGui;
    public ChatGUI chatGui;
    public ItemInstance mouseDownItem = null; // :(

    void Start()
    {
        Locator.Provide(this);
        ResizeCamera();

        float guiScale = 1.75f;

        tabbedGui = new TabbedUI(new Rect(Int32.MaxValue, 0, 300, 300), guiScale);
        Locator.Provide(tabbedGui);

        invGui = new InventoryGUI(new Rect(0, Int32.MaxValue, 300, 300), guiScale);
        Locator.Provide(invGui);
        tabbedGui.Add(1397, invGui); // :(

        chatGui = new ChatGUI(new Rect(Int32.MaxValue, Int32.MaxValue, 300, 300));
        Locator.Provide(chatGui);
        tabbedGui.Add(351, chatGui); // :(

        _game = new GridiaGame();
        Locator.Provide(_game);

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
        tabbedGui.Render();
        if (mouseDownItem != null)
        {
            var rect = new Rect((int)Input.mousePosition.x - 16, Screen.height - (int)Input.mousePosition.y - 16, 32, 32);
            GridiaWindow.RenderSlot(rect, mouseDownItem);
        }

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

    public Vector2 getMouse()
    {
        var pos = Input.mousePosition;
        pos.y = Screen.height - pos.y;
        return pos;
    }

    public bool isMouseOverGUI() 
    {
        var mouse = getMouse();
        return tabbedGui.MouseOverAny() || tabbedGui.ResizingAny();
    }

    void Update()
    {
        if (_game.stateMachine != null) {
            _game.stateMachine.Step(Time.deltaTime);
        }
        _game.view.Render();
        ResizeCamera(); // :( only on resize
    }

    public Vector3 getTileLocationOfMouse() {
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
