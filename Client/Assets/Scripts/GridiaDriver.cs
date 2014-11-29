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
    public TabbedUI tabbedGui; // :(
    public InventoryWindow invGui;
    public ChatWindow chatGui;
    public ItemUsePickWindow itemUsePickWindow;
    public ItemInstance mouseDownItem = null; // :(
    public InputManager inputManager = new InputManager();

    void Start()
    {
        Locator.Provide(inputManager);

        Locator.Provide(this);
        Locator.Provide<SoundPlayer>(GetComponent<SoundPlayer>());
        ResizeCamera();

        float guiScale = 1.75f;

        tabbedGui = new TabbedUI(new Vector2(Int32.MaxValue, 0));
        Locator.Provide(tabbedGui);
        tabbedGui.ScaleXY = guiScale;

        invGui = new InventoryWindow(new Vector2(0, Int32.MaxValue));
        Locator.Provide(invGui);
        invGui.ScaleXY = guiScale;

        chatGui = new ChatWindow(new Vector2(Int32.MaxValue, Int32.MaxValue));
        Locator.Provide(chatGui);
        chatGui.ScaleXY = guiScale;

        itemUsePickWindow = new ItemUsePickWindow(new Vector2(0, 0));
        Locator.Provide(itemUsePickWindow);
        itemUsePickWindow.ScaleXY = guiScale;

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

        InitTabbedGui();
    }

    private RecipeBookWindow _recipeBook;
    public void OpenRecipeBook(ItemInstance item) 
    {
        if (item.Item.Id != 0) 
        {
            var rect = new Rect(0, 0, 300, 300);
            if (_recipeBook != null) 
            {
                rect = _recipeBook.Rect;
                tabbedGui.Remove(_recipeBook);
            }
            _recipeBook = new RecipeBookWindow(Vector2.zero, item);
            _recipeBook.ScaleXY = 1.75f;
            tabbedGui.Add(3126, _recipeBook);
        }
    }

    void InitTabbedGui() 
    {
        tabbedGui.Add(1397, invGui); // :(
        tabbedGui.Add(351, chatGui); // :(
        var options = new OptionsWindow(Vector2.zero);
        options.Visible = false;
        tabbedGui.Add(1, options);
    }

    void OnGUI() {
        inputManager.Step();
        tabbedGui.Render();
        if (mouseDownItem != null)
        {
            var rect = new Rect((int)Input.mousePosition.x - 16, Screen.height - (int)Input.mousePosition.y - 16, 32, 32);
            var draggedItem = new ItemRenderable(new Vector2(rect.x, rect.y), mouseDownItem);
            draggedItem.ToolTip = null;
            draggedItem.Render();
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

        ToolTipRenderable.instance.Render();
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

    public void OnApplicationQuit() 
    {
        Application.CancelQuit();
        //Locator.Get<ConnectionToServerHandler>().End();
        if (!Application.isEditor)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
