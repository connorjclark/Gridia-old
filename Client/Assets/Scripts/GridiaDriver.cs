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
    public List<FloatingText> floatingTexts = new List<FloatingText>();

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
        ConnectionToGridiaServerHandler conn = new ConnectionToGridiaServerHandler(_game, SceneManager.GetArguement<String>("ip"), 1234);
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
        tabbedGui.Add(1221, invGui, true); // :(
        tabbedGui.Add(15, equipmentGui, false); // :(
        tabbedGui.Add(147, chatGui, true); // :(

        var options = new OptionsWindow(Vector2.zero);
        options.X = (Screen.width - options.Width) / 2;
        options.Y = (Screen.height - options.Height) / 2;
        tabbedGui.Add(0, options, false);
    }

    public Vector2 GetRelativeScreenPosition(Vector3 playerPosition, Vector3 subjectCoord)
    {
        var tileSize = 32 * _game.view.Scale;
        var relative = subjectCoord - playerPosition + new Vector3(_game.view.width / 2, _game.view.height / 2);
        return new Vector2(relative.x * tileSize, Screen.height - relative.y * tileSize - tileSize);
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
        var focusPos = _game.view.FocusPosition;
        var tileSize = 32 * _game.view.Scale;
        foreach (var cre in _game.tileMap.creatures.ValuesToList()) 
        {
            var pos = cre.Position;
            if (playerZ == pos.z)
            {
                var relative = pos - focusPos; // :(
                var rect = new Rect(relative.x * tileSize, Screen.height - relative.y * tileSize - tileSize, tileSize, tileSize);
                DrawCreature(rect, cre);

                var labelRelative = pos - _game.view.FocusPosition; // :(
                var nameLabel = new Label(new Vector2((labelRelative.x + 0.5f) * tileSize, Screen.height - (labelRelative.y + 1.5f) * tileSize), cre.Name, true, true); // :(
                nameLabel.Render();
            }
        }

        if (!_game.hideSelector)
        {
            var selectorPos = focusPos + _game.selectorDelta;
            var selectorRelativePosition = GetRelativeScreenPosition(focusPos, selectorPos);
            var selectorRect = new Rect(selectorRelativePosition.x, selectorRelativePosition.y, tileSize, tileSize);
            GUIDrawSelector(selectorRect, new Color32(0, 0, 255, 100));
        }

        foreach (var animation in _game.animations)
        {
            animation.Rect = new Rect(animation.X, animation.Y, tileSize, tileSize); // :(
            animation.Render();
        }

        _game.stateMachine.OnGUI();

        for (int i = 0; i < floatingTexts.Count; i++)
        {
            var floatingText = floatingTexts[i];
            if (floatingText.Coord.z == playerZ)
            {
                floatingText.Reposition(tileSize, focusPos);
                floatingText.Tick();
                floatingText.Render();
                if (floatingText.Life <= 0)
                {
                    floatingTexts.RemoveAt(i);
                }
            }
        }

        // :(
        _game.view.ForEachInView((x, y) =>
        {
            var tile = _game.tileMap.GetTile(x + (int)focusPos.x, y + (int)focusPos.y, (int)focusPos.z);
            if (tile.Item.Quantity > 1)
            {
                var labelRect = new Rect((x - focusPos.x % 1) * tileSize, Screen.height - (y - focusPos.y % 1) * tileSize - tileSize, tileSize, tileSize);
                GUI.Label(labelRect, tile.Item.Quantity.ToString());
            }
        });

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
        if (Event.current.type == EventType.KeyUp)
        {
            if (Event.current.keyCode == KeyCode.Tab)
            {
                chatGui.Visible = true;
                GUI.FocusControl("ChatInput");
            }
            else if (Event.current.keyCode == KeyCode.Escape)
            {
                if (GUI.GetNameOfFocusedControl() == "ChatInput")
                {
                    GUI.FocusControl("");
                }
                else
                {
                    chatGui.Visible = false;
                }
            }
        }
    }

    // :(
    private Dictionary<Color, Texture2D> _staticRectTexture = new Dictionary<Color,Texture2D>();
    private Dictionary<Color, GUIStyle> _staticRectStyle = new Dictionary<Color,GUIStyle>();

    public void GUIDrawSelector(Rect rect, Color color)
    {
        if (!_staticRectTexture.ContainsKey(color))
        {
            _staticRectTexture[color] = new Texture2D(1, 1);
            _staticRectStyle[color] = new GUIStyle();
            _staticRectStyle[color].normal.background = _staticRectTexture[color];
        }
        _staticRectTexture[color].SetPixel(0, 0, color);
        _staticRectTexture[color].Apply();
        GUI.Box(rect, GUIContent.none, _staticRectStyle[color]);
    }

    // :(
    private void DrawCreature(Rect rect, Creature creature)
    {
        var image = creature.Image;
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
        for (int i = 0; i < _game.animations.Count; i++)
        {
            var animation = _game.animations[i];
            if (animation.Dead) 
            {
                _game.animations.RemoveAt(i);
            }
            else
            {
                animation.Step(Time.deltaTime);
            }
        }
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
