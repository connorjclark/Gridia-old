using Gridia;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GridiaDriver : MonoBehaviour
{    
    public GridiaGame Game; // :(
    public TextureManager TextureManager; // :(
    public ContentManager ContentManager; // :(
    public TabbedUI TabbedGui; // :(
    public ContainerWindow InvGui;
    public EquipmentWindow EquipmentGui;
    public ChatWindow ChatGui;
    public ItemUsePickWindow ItemUsePickWindow;
    public ItemInstance MouseDownItem = null; // :(
    public InputManager InputManager = new InputManager();
    public List<FloatingText> FloatingTexts = new List<FloatingText>();
    public ContainerWindow SelectedContainer { get; set; }

    void Start()
    {
        Locator.Provide(InputManager);

        Locator.Provide(this);
        Locator.Provide(GridiaConstants.SoundPlayer);
        ResizeCamera();

        TabbedGui = new TabbedUI(new Vector2(Int32.MaxValue, 0));
        Locator.Provide(TabbedGui);
        TabbedGui.ScaleXY = GridiaConstants.GuiScale;

        InvGui = new ContainerWindow(new Vector2(0, Int32.MaxValue));
        Locator.Provide(InvGui);
        InvGui.ScaleXY = GridiaConstants.GuiScale;

        EquipmentGui = new EquipmentWindow(new Vector2(0, 0));
        Locator.Provide(EquipmentGui);
        EquipmentGui.ScaleXY = GridiaConstants.GuiScale;

        ChatGui = new ChatWindow(new Vector2(Int32.MaxValue, Int32.MaxValue));
        Locator.Provide(ChatGui);
        ChatGui.ScaleXY = GridiaConstants.GuiScale;

        ItemUsePickWindow = new ItemUsePickWindow(new Vector2(0, 0));
        Locator.Provide(ItemUsePickWindow);
        ItemUsePickWindow.ScaleXY = GridiaConstants.GuiScale;

        Locator.Provide(ContentManager = new ContentManager(GridiaConstants.WorldName));
        Locator.Provide(TextureManager = new TextureManager(GridiaConstants.WorldName));
    }

    // :(
    private RecipeBookWindow _recipeBook;
    public void OpenRecipeBook(ItemInstance item) 
    {
        if (item.Item.Id == 0) return;
        if (_recipeBook != null) 
        {
            TabbedGui.Remove(_recipeBook);
        }
        _recipeBook = new RecipeBookWindow(Vector2.zero, item) { ScaleXY = GridiaConstants.GuiScale };
        TabbedGui.Add(2008, _recipeBook, true);
    }

    void InitTabbedGui() 
    {
        TabbedGui.Add(1221, InvGui, true); // :(
        TabbedGui.Add(15, EquipmentGui, false); // :(
        TabbedGui.Add(147, ChatGui, true); // :(

        var options = new OptionsWindow(Vector2.zero) {ScaleXY = GridiaConstants.GuiScale};
        options.X = (Screen.width - options.Width) / 2;
        options.Y = (Screen.height - options.Height) / 2;
        TabbedGui.Add(0, options, false);
    }

    public Vector2 GetRelativeScreenPosition(Vector3 playerPosition, Vector3 subjectCoord)
    {
        var tileSize = 32 * Game.View.Scale;
        var relative = subjectCoord - playerPosition + new Vector3(Game.View.Width / 2, Game.View.Height / 2);
        return new Vector2(relative.x * tileSize, Screen.height - relative.y * tileSize - tileSize);
    }

    bool IsGameReady() {
        return ContentManager.DoneLoading && TextureManager.DoneLoading && Game != null;
    }

    // :(
    void OnGUI() {
        if (!IsGameReady())
        {
            return;
        }
        InputManager.Step();
        TabbedGui.Render();
        TabbedGui.HandleEvents();
        if (MouseDownItem != null)
        {
            var rect = new Rect((int)Input.mousePosition.x - 16, Screen.height - (int)Input.mousePosition.y - 16, 32, 32);
            var draggedItem = new ItemRenderable(new Vector2(rect.x, rect.y), MouseDownItem) {ToolTip = null};
            draggedItem.Render();
        }

        if (Game.View.Focus == null)
        {
            return;
        }

        //temp :(
        var playerZ = (int) Game.View.Focus.Position.z;
        var focusPos = Game.View.FocusPosition;
        var tileSize = 32 * Game.View.Scale;
        foreach (var cre in Game.TileMap.Creatures.ValuesToList()) 
        {
            var pos = cre.Position;
            if (playerZ == pos.z)
            {
                // :(
                var dx = Game.TileMap.WrappedDistBetweenX(pos, focusPos);
                var dy = Game.TileMap.WrappedDistBetweenY(pos, focusPos);

                var rect = new Rect(dx * tileSize, Screen.height - dy * tileSize - tileSize, tileSize, tileSize);
                TextureManager.DrawCreature(rect, cre, Game.View.Scale);
                if (cre.Name.Length > 0)
                {
                    var labelRelative = pos - Game.View.FocusPosition; // :(
                    var nameLabel = new Label(new Vector2((labelRelative.x + 0.5f) * tileSize, Screen.height - (labelRelative.y + 1.5f) * tileSize), cre.Name, true, true); // :(
                    nameLabel.Render();
                }
            }
        }

        if (!Game.HideSelector)
        {
            var selectorPos = focusPos + Game.SelectorDelta;
            var selectorRelativePosition = GetRelativeScreenPosition(focusPos, selectorPos);
            var selectorRect = new Rect(selectorRelativePosition.x, selectorRelativePosition.y, tileSize, tileSize);
            GridiaConstants.GUIDrawSelector(selectorRect, new Color32(0, 0, 255, 100));
        }

        foreach (var animation in Game.Animations)
        {
            animation.Rect = new Rect(animation.X, animation.Y, tileSize, tileSize); // :(
            animation.Render();
        }

        Game.StateMachine.OnGUI();

        for (var i = 0; i < FloatingTexts.Count; i++)
        {
            var floatingText = FloatingTexts[i];
            if (floatingText.Coord.z != playerZ) continue;
            floatingText.Reposition(tileSize, focusPos);
            floatingText.Tick();
            floatingText.Render();
            if (floatingText.Life <= 0)
            {
                FloatingTexts.RemoveAt(i);
            }
        }

        // :(
        Game.View.ForEachInView((x, y) =>
        {
            var tile = Game.TileMap.GetTile(x + (int)focusPos.x, y + (int)focusPos.y, (int)focusPos.z);
            if (tile.Item.Quantity > 1)
            {
                var labelRect = new Rect((x - focusPos.x % 1) * tileSize, Screen.height - (y - focusPos.y % 1) * tileSize - tileSize, tileSize, tileSize);
                GUI.Label(labelRect, tile.Item.Quantity.ToString());
            }
        });

        var versionLabelSize = GUI.skin.label.CalcSize(new GUIContent(GridiaConstants.Version));
        GUI.Label(new Rect(0, 0, versionLabelSize.x, versionLabelSize.y), GridiaConstants.Version);

        ToolTipRenderable.Instance.Render();

        GridiaConstants.DrawErrorMessage();

        switch (Event.current.type)
        {
            case EventType.KeyDown:
                switch (Event.current.keyCode)
                {
                    case KeyCode.PageDown:
                        Game.View.Scale += 0.25f;
                        break;
                    case KeyCode.PageUp:
                        Game.View.Scale = Math.Max(0.5f, Game.View.Scale - 0.25f);
                        break;
                }
                break;
            case EventType.KeyUp:
                switch (Event.current.keyCode)
                {
                    case KeyCode.Return:
                        if (GUI.GetNameOfFocusedControl() == "ChatInput")
                        {
                            GUI.FocusControl("");
                        }
                        else
                        {
                            ChatGui.Visible = true;
                            GUI.FocusControl("ChatInput");
                        }
                        break;
                    case KeyCode.Escape:
                        ChatGui.Visible = false;
                        break;
                }
                break;
        }
    }

    public Vector2 GetMouse()
    {
        var pos = Input.mousePosition;
        pos.y = Screen.height - pos.y;
        return pos;
    }

    public bool IsMouseOverGui() 
    {
        return TabbedGui.MouseOverAny() || TabbedGui.ResizingAny();
    }

    void Update()
    {
        if (ContentManager.DoneLoading && TextureManager.DoneLoading && Game == null) {
            Game = Locator.Get<GridiaGame>();
            InitTabbedGui();
            Game.Initialize(GridiaConstants.Size, GridiaConstants.Depth, GridiaConstants.SectorSize); // :(
            ServerSelection.gameInitWaitHandle.Set();
        }
        if (!IsGameReady())
        {
            return;
        }
        if (Game.View.Focus == null)
        {
            return;
        }
        if (Game.StateMachine != null)
        {
            Game.StateMachine.Step(Time.deltaTime);
        }
        Game.View.Render();
        for (int i = 0; i < Game.Animations.Count; i++)
        {
            var animation = Game.Animations[i];
            if (animation.Dead) 
            {
                Game.Animations.RemoveAt(i);
            }
            else
            {
                animation.Step(Time.deltaTime);
            }
        }
        ResizeCamera(); // :( only on resize
    }

    public Vector3 GetTileLocationOfMouse() {
        var x = (int)(Input.mousePosition.x / (GridiaConstants.SpriteSize * Game.View.Scale));
        var y = (int)(Input.mousePosition.y / (GridiaConstants.SpriteSize * Game.View.Scale));
        var z = (int)Game.View.Focus.Position.z;

        x = Game.TileMap.Wrap(x + (int)Game.View.FocusPosition.x);
        y = Game.TileMap.Wrap(y + (int)Game.View.FocusPosition.y);

        return new Vector3(x, y, z);
    }

    public void RemoveAllOpenContainers()
    {
        for (var i = TabbedGui.NumWindows() - 1; i >= 0; i--)
        {
            var window = TabbedGui.GetWindowAt(i);
            if (window is ContainerWindow && window != InvGui && window != EquipmentGui)
            {
                TabbedGui.Remove(window);
            }
        }
        if (SelectedContainer != null)
        {
            MoveSelectedContainerToNext();
        }
    }

    public void AddNewContainer(List<ItemInstance> items, int id, int tabGfxItemId)
    {
        var numOpenContainers = GetNumberOfOpenContainers();
        var containerWindow =
            new ContainerWindow(new Vector2(0, Math.Min(Screen.height - 120, (numOpenContainers - 2)*120)))
            {
                ScaleXY = 1.5f,
                SelectedColor = new Color32(0, 0, 255, 100)
            };
        containerWindow.Set(items, id);
        TabbedGui.Add(ContentManager.GetItem(tabGfxItemId).Animations[0], containerWindow, true);
    }

    public int GetNumberOfOpenContainers()
    {
        var count = 0;
        for (var i = 0; i < TabbedGui.NumWindows(); i++)
        {
            if (TabbedGui.GetWindowAt(i) is ContainerWindow)
            {
                count += 1;
            }
        }
        return count;
    }

    private ContainerWindow GetOpenContainerWith(Predicate<ContainerWindow> predicate)
    {
        for (var i = 0; i < TabbedGui.NumWindows(); i++)
        {
            var window = TabbedGui.GetWindowAt(i);
            if (window is ContainerWindow && predicate(window as ContainerWindow))
            {
                return window as ContainerWindow;
            }
        }
        return null;
    }

    public ContainerWindow GetOpenContainerWithMouseUp()
    {
        return GetOpenContainerWith(containerWindow => containerWindow.MouseUpSlot != -1);
    }

    public ContainerWindow GetOpenContainerWithMouseDown()
    {
        return GetOpenContainerWith(containerWindow => containerWindow.MouseDownSlot != -1);
    }

    public ContainerWindow GetOpenContainerWithId(int id)
    {
        return GetOpenContainerWith(containerWindow => containerWindow.ContainerId == id);
    }

    // :(
    public List<ContainerWindow> GetListOfSelectableContainerWindows()
    {
        var list = new List<ContainerWindow>();
        for (var i = 0; i < TabbedGui.NumWindows(); i++)
        {
            var window = TabbedGui.GetWindowAt(i);
            if (window is ContainerWindow && window != InvGui && window != EquipmentGui)
            {
                list.Add(window as ContainerWindow);
            }
        }
        return list;
    } 

    // :(
    public void MoveSelectedContainerToNext()
    {
        var containers = GetListOfSelectableContainerWindows();
        ContainerWindow nextContainer;
        if (SelectedContainer == null && containers.Count != 0)
        {
            nextContainer = containers[0];
            Game.HideSelector = true;
        }
        else 
        {
            var index = containers.IndexOf(SelectedContainer);
            nextContainer = index != containers.Count - 1 ? containers[index + 1] : null;
        }
        if (SelectedContainer != null)
        {
            SelectedContainer.ShowSelected = false;
        }
        if (nextContainer != null)
        {
            nextContainer.ShowSelected = true;
        }
        else
        {
            Game.HideSelector = false;
        }
        SelectedContainer = nextContainer;
    }

    void ResizeCamera()
    {
        var camera = Camera.main;
        camera.orthographicSize = Screen.height / 2.0f;
        camera.transform.position = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, -100);
    }

    public void OnApplicationQuit() 
    {
        GridiaConstants.OnApplicationQuit();
    }
}
