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
    private GridiaGame _game;
    public InventoryGUI invGui;

    void Start()
    {
        float scale = 1.5f;
        invGui = new InventoryGUI(new Vector2(0, Screen.height - 64 * scale - 20), scale);
        Locator.Provide(invGui);

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

    string chatInput = "";
    public static string chatArea = "\n\n\n\n";
    Rect chatInputRect = new Rect(Screen.width / 2, Screen.height - 20, Screen.width / 2, 20);
    Rect chatAreaRect = new Rect(Screen.width / 2, Screen.height - 120, Screen.width / 2, 100);
    public static Vector2 scrollPosition = Vector2.zero;

    void OnGUI() {
        invGui.Render();
        if (mouseDownItem != null)
        {
            var rect = new Rect((int)Input.mousePosition.x - 16, Screen.height - (int)Input.mousePosition.y - 16, 32, 32);
            invGui.RenderSlot(rect, mouseDownItem);
        }

        chatInput = GUI.TextField(chatInputRect, chatInput);

        GUI.BeginGroup(chatAreaRect); 
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(chatAreaRect.width), GUILayout.Height(chatAreaRect.height));
        GUILayout.TextArea(chatArea);
        GUILayout.EndScrollView();
        GUI.EndGroup();

        if (chatInput != "" && Event.current.type == EventType.keyDown && Event.current.character == '\n')
        {
            Locator.Get<ConnectionToGridiaServerHandler>().Chat(chatInput);
            chatInput = "";
        }
    }

    ItemInstance mouseDownItem = null; // :(
    int downSlot;
    int sourceIndex;
    String mouseDownLocation;

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
            else
            {
                mouseDownLocation = "world";
                var downCoord = getTileLocationOfMouse();
                mouseDownItem = _game.tileMap.GetTile((int)downCoord.x, (int)downCoord.y, (int)downCoord.z).Item;
                sourceIndex = _game.tileMap.ToIndex(downCoord);
            }
        }
        else if (Input.GetMouseButtonUp(0))
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
        MoveCreatures(10.0f * Time.deltaTime);
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
    
    void MoveCreatures(float speed)
    {
        _game.creatures.ValuesToList().ForEach(cre => cre.Move(speed));
    }

    void ResizeCamera()
    {
        Camera camera = Camera.main;
        camera.orthographicSize = Screen.height / 2.0f;
        camera.transform.position = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, -100);
    }
}
