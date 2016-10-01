using Gridia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class GridiaDriver : MonoBehaviour
{
    public static AutoResetEvent GameInitWaitHandle = new AutoResetEvent(false);

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
    public ActionWindow ActionWindow { get; set; }
    private Creature _selectedCreature;

    public Creature SelectedCreature
    {
        get { return _selectedCreature; }
        set
        {
            _selectedCreature = value;
            Locator.Get<ConnectionToGridiaServerHandler>().SelectTarget(_selectedCreature);
        }
    }

    public Vector3 focusPos; // cached
    public float tileSize;

    void Start()
    {
        GridiaConstants.SoundPlayer.MuteSfx = GridiaConstants.SoundPlayer.MuteMusic = Application.isEditor;

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

        var helpMenu = new HelpMenu(new Vector2(0, 0));
        Locator.Provide(helpMenu);
        helpMenu.ScaleXY = GridiaConstants.GuiScale;

        Locator.Provide(ContentManager = new ContentManager(GridiaConstants.WorldName));
        Locator.Provide(TextureManager = new TextureManager(GridiaConstants.WorldName));

        ActionWindow = new ActionWindow(new Vector2(Int32.MaxValue, Int32.MaxValue));
        Locator.Provide(ActionWindow);
        helpMenu.ScaleXY = GridiaConstants.GuiScale;
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

    // TODO delete
    /*void InitTabbedGui() 
    {
        TabbedGui.Add(1221, InvGui, false); // :(
        TabbedGui.Add(15, EquipmentGui, false); // :(
        TabbedGui.Add(147, ChatGui, false); // :(
        TabbedGui.Add(0, Locator.Get<HelpMenu>(), true); // :(
        ActionWindow.TempAddActions();
        TabbedGui.Add(32, ActionWindow, true);

        var options = new OptionsWindow(Vector2.zero) {ScaleXY = GridiaConstants.GuiScale};
        options.X = (Screen.width - options.Width) / 2;
        options.Y = (Screen.height - options.Height) / 2;
        TabbedGui.Add(132, options, false);
    }*/

    public Vector2 GetRelativeScreenPosition(Vector3 playerPosition, Vector3 subjectCoord)
    {
        var relative = subjectCoord - playerPosition + new Vector3(Game.View.Width / 2, Game.View.Height / 2);
        return new Vector2(relative.x * tileSize, Screen.height - relative.y * tileSize - tileSize);
    }

    // :(
    public Vector2 GetRelativeScreenPositionForCreature(Vector3 playerPosition, Vector3 subjectCoord)
    {
        var relative = subjectCoord - playerPosition + new Vector3(Game.View.Width/2 + 0.5f, Game.View.Height/2 + 0.5f);
        return relative*GridiaConstants.SpriteSize;
    }

    public bool IsGameReady() {
        return Game != null;
    }

    // :(
    void OnGUI() {
        if (Game == null)
        {
            return;
        }
        var mouseTileCoord = GetTileFloatLocationOfMouse();

        // :( temp set Action window in middle of bottom part of screen
        // really need to use unity GUI...
        ActionWindow.X = (Screen.width - ActionWindow.Width) / 2;
        ActionWindow.Y = Screen.height - ActionWindow.Height - 20;

        InputManager.Step();
        TabbedGui.Render();
        TabbedGui.HandleEvents();
        if (MouseDownItem != null && MouseDownItem.Item.Id != 0)
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
        focusPos = Game.View.FocusPosition;
        tileSize = 32 * Game.View.Scale;
        GameObject.Find("Game").transform.localScale = new Vector3(Game.View.Scale, Game.View.Scale, 1); // :(
//        foreach (var cre in Game.TileMap.Creatures.ValuesToList()) 
//        {
//            var pos = cre.Position;
//            if (playerZ != pos.z) continue;
//
//            var rect = GetScreenRectOfLocation(pos);
//            TextureManager.DrawCreature(rect, cre, Game.View.Scale);
//
//            var mouseDx = mouseTileCoord.x - pos.x;
//            var mouseDy = mouseTileCoord.y - pos.y;
//            if (mouseDx >= 0 && mouseDy >= 0 && mouseDx < 1 && mouseDy < 1 && cre != Game.View.Focus && SelectedCreature != cre)
//            {
//                if (Event.current.type == EventType.MouseUp)
//                {
//                    SelectedCreature = cre;
//                    Locator.Get<ConnectionToGridiaServerHandler>().SelectTarget(SelectedCreature);
//                }
//                else
//                {
//                    GridiaConstants.GUIDrawSelector(rect, new Color32(255, 255, 0, 100));
//                }
//            }
//
//            if (SelectedCreature == cre)
//            {
//                GridiaConstants.GUIDrawSelector(rect, new Color32(255, 0, 0, 100));
//            }
//
//            if (cre.Name.Length <= 0) continue;
//            var labelRelative = pos - Game.View.FocusPosition; // :(
//            var nameLabel = new Label(new Vector2((labelRelative.x + 0.5f) * tileSize, Screen.height - (labelRelative.y + 1.5f) * tileSize), cre.Name, true, true); // :(
//            nameLabel.TextWidth = (int) GUI.skin.label.CalcSize(new GUIContent(nameLabel.Text)).x;
//            nameLabel.Render();
//        }

        if (SelectedCreature != null)
        {
            var rect = GetScreenRectOfLocation(SelectedCreature.Position);
            GridiaConstants.GUIDrawSelector(rect, new Color32(255, 255, 0, 50));
            if (SelectedCreature.Position.z != focusPos.z)
            {
                SelectedCreature = null;
            }
        }

        if (!Game.HideSelector)
        {
            var selectorPos = focusPos + Game.SelectorDelta;
            var selectorRelativePosition = GetRelativeScreenPosition(focusPos, selectorPos);
            var selectorRect = new Rect(selectorRelativePosition.x, selectorRelativePosition.y, tileSize, tileSize);
            GridiaConstants.GUIDrawSelector(selectorRect, new Color32(0, 0, 255, 100));
        }

        foreach (var animation in Game.Animations.Where(a => a.IsInWorld))
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

    public void AddCreature(Creature creature)
    {
        var go = Instantiate(Resources.Load("Creature")) as GameObject;
        go.name = "Creature " + creature.Id;
        go.transform.parent = GameObject.Find("Creatures").transform;
        go.transform.localScale = Vector3.one; // :(
        var script = go.GetComponent<CreatureScript>();
        script.Creature = creature;
    }

    public Vector3 GetScreenPositionOfLocation(Vector3 loc)
    {
        var dx = Game.TileMap.WrappedDistBetweenX(loc, focusPos);
        var dy = Game.TileMap.WrappedDistBetweenY(loc, focusPos);
        return new Vector3(dx * tileSize, Screen.height - dy * tileSize - tileSize, 0);
    }

    public Rect GetScreenRectOfLocation(Vector3 loc)
    {
        var dx = Game.TileMap.WrappedDistBetweenX(loc, focusPos);
        var dy = Game.TileMap.WrappedDistBetweenY(loc, focusPos);
        return new Rect(dx * tileSize, Screen.height - dy * tileSize - tileSize, tileSize, tileSize);
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
        if (Game == null)
        {
            Game = Locator.Get<GridiaGame>();
            //InitTabbedGui();
            Game.Initialize(GridiaConstants.Size, GridiaConstants.Depth, GridiaConstants.SectorSize); // :(
            GameInitWaitHandle.Set();
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

    public Vector3 GetTileFloatLocationOfMouse()
    {
        var x = Input.mousePosition.x / (GridiaConstants.SpriteSize * Game.View.Scale);
        var y = Input.mousePosition.y / (GridiaConstants.SpriteSize * Game.View.Scale);
        var intX = (int) x;
        var floatX = x - intX;
        var intY = (int) y;
        var floatY = y - intY;
        var z = (int) Game.View.FocusPosition.z;

        x = Game.TileMap.Wrap(intX + (int)Game.View.FocusPosition.x) + floatX;
        y = Game.TileMap.Wrap(intY + (int)Game.View.FocusPosition.y) + floatY;

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
        camera.orthographicSize = Screen.height/2f;
        camera.transform.position = new Vector3(Screen.width/2f, Screen.height/2f, -100);
    }

    public void OnApplicationQuit() 
    {
        GridiaConstants.OnApplicationQuit();
    }
}
