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
    public TextureManager _textureManager; // :(
    public TabbedUI tabbedGui; // :(
    public InventoryWindow invGui;
    public EquipmentWindow equipmentGui;
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

        equipmentGui = new EquipmentWindow(new Vector2(0, 0));
        Locator.Provide(equipmentGui);
        equipmentGui.ScaleXY = guiScale;

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
        Locator.Provide(_textureManager = new TextureManager("TestWorld"));
        _game.Initialize(GridiaConstants.SIZE, GridiaConstants.DEPTH, GridiaConstants.SECTOR_SIZE); // :(

        gameInitWaitHandle.Set();

        if (GridiaConstants.IS_ADMIN)
        {
            var adminWindow = new AdminWindow(Vector2.zero);
            tabbedGui.Add(10, adminWindow, false);
        }

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
            tabbedGui.Add(2008, _recipeBook, true);
        }
    }

    void InitTabbedGui() 
    {
        tabbedGui.Add(1221, invGui, false); // :(
        tabbedGui.Add(15, equipmentGui, false); // :(
        tabbedGui.Add(147, chatGui, false); // :(

        var options = new OptionsWindow(Vector2.zero);
        options.X = (Screen.width - options.Width) / 2;
        options.Y = (Screen.height - options.Height) / 2;
        tabbedGui.Add(0, options, false);
    }

    void OnGUI() {
        inputManager.Step();
        tabbedGui.Render();
        tabbedGui.HandleEvents();
        if (mouseDownItem != null)
        {
            var rect = new Rect((int)Input.mousePosition.x - 16, Screen.height - (int)Input.mousePosition.y - 16, 32, 32);
            var draggedItem = new ItemRenderable(new Vector2(rect.x, rect.y), mouseDownItem);
            draggedItem.ToolTip = null;
            draggedItem.Render();
        }

        //temp :(
        var cm = Locator.Get<ContentManager>();
        var playerZ = (int) _game.view.Focus.Position.z;
        foreach (var cre in _game.tileMap.creatures.ValuesToList()) 
        {
            var pos = cre.Position;
            if (playerZ == pos.z)
            {
                var focusPos = _game.view.FocusPosition;
                var relative = pos - focusPos;

                var tileSize = 32 * _game.view.Scale;
                var rect = new Rect(relative.x * tileSize, Screen.height - relative.y * tileSize - tileSize, tileSize, tileSize);

                DrawCreature(rect, cre.Image);
            }
        }

        ToolTipRenderable.instance.Render();

        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.PageDown)
            {
                _game.view.Scale += 0.25f;
            }
            else if (Event.current.keyCode == KeyCode.PageUp)
            {
                _game.view.Scale -= 0.25f;
            }
        }
    }

    // :(
    private void DrawCreature(Rect rect, CreatureImage image)
    {
        if (image is DefaultCreatureImage)
        {
            var defaultImage = image as DefaultCreatureImage;
            int spriteId = defaultImage.SpriteIndex;
            int textureX = (spriteId % GridiaConstants.SPRITES_IN_SHEET) % GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
            int textureY = 10 - (spriteId % GridiaConstants.SPRITES_IN_SHEET) / GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW - defaultImage.Height; // ?
            var texCoords = new Rect(textureX / 10.0f, textureY / 10.0f, defaultImage.Width / 10.0f, defaultImage.Height / 10.0f); // :( don't hardcode 10
            rect.width *= defaultImage.Width;
            rect.height *= defaultImage.Height;
            rect.y -= (defaultImage.Height - 1) * GridiaConstants.SPRITE_SIZE * _game.view.Scale;
            GUI.DrawTextureWithTexCoords(rect, _textureManager.Creatures[spriteId / GridiaConstants.SPRITES_IN_SHEET], texCoords);
        }
        else if (image is CustomPlayerImage)
        {
            var customImage = image as CustomPlayerImage;
            DrawCreaturePart(rect, _textureManager.Heads, customImage.Head);
            DrawCreaturePart(rect, _textureManager.Chests, customImage.Chest);
            DrawCreaturePart(rect, _textureManager.Legs, customImage.Legs);
            DrawCreaturePart(rect, _textureManager.Arms, customImage.Arms);
            DrawCreaturePart(rect, _textureManager.Weapons, customImage.Weapon);
            DrawCreaturePart(rect, _textureManager.Shields, customImage.Shield);
        }
    }

    private void DrawCreaturePart(Rect rect, List<Texture> textures, int spriteIndex)
    {
        var texture = textures[spriteIndex / GridiaConstants.SPRITES_IN_SHEET];
        int textureX = (spriteIndex % GridiaConstants.SPRITES_IN_SHEET) % GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
        int textureY = 9 - (spriteIndex % GridiaConstants.SPRITES_IN_SHEET) / GridiaConstants.NUM_TILES_IN_SPRITESHEET_ROW;
        var texCoords = new Rect(textureX / 10.0f, textureY / 10.0f, 1 / 10.0f, 1 / 10.0f); // :( don't hardcode 10
        GUI.DrawTextureWithTexCoords(rect, texture, texCoords);
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
        int z = (int)_game.view.Focus.Position.z;

        x = _game.tileMap.Wrap(x + (int)_game.view.FocusPosition.x);
        y = _game.tileMap.Wrap(y + (int)_game.view.FocusPosition.y);

        return new Vector3(x, y, z);
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
