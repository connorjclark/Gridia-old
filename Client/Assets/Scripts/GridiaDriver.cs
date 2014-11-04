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
        invGui = new InventoryGUI(0, Screen.height - 32 * 2);
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
            invGui.RenderSlot((int)Input.mousePosition.x - 16, Screen.height - (int)Input.mousePosition.y - 16, mouseDownItem);
        }
        chatInput = GUI.TextField(chatInputRect, chatInput);

        GUI.BeginGroup(chatAreaRect); 
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(chatAreaRect.width), GUILayout.Height(chatAreaRect.height));
        GUILayout.TextArea(chatArea);
        GUILayout.EndScrollView();
        GUI.EndGroup();

        if (invGui.tooltip != null) GUI.Box(invGui.toolRect, invGui.tooltip);

        if (chatInput != "" && Event.current.type == EventType.keyDown && Event.current.character == '\n')
        {
            Locator.Get<ConnectionToGridiaServerHandler>().Chat(chatInput);
            chatInput = "";
        }
    }

    ItemInstance mouseDownItem = null; // :(
    Vector3 downCoord;
    int downSlot;
    String mouseDownLocation;

    Vector2 GetMouse() {
        var mouse = Input.mousePosition;
        mouse.y = Screen.height - mouse.y;
        return mouse;
    }

    void Update()
    {
        Vector2 mouse = GetMouse();
        // no no no no :(

        if (Input.GetMouseButtonDown(0) && !chatInputRect.Contains(mouse) && !chatAreaRect.Contains(mouse)) 
        {
            downSlot = invGui.getSlotIndexUnderPoint(mouse);
            if (downSlot != -1)
            {
                mouseDownLocation = "inv";
            }
            else 
            {
                mouseDownLocation = "world";
                downCoord = getTileLocationOfMouse();
                mouseDownItem = _game.tileMap.GetTile((int)downCoord.x, (int)downCoord.y, (int)downCoord.z).Item;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (mouseDownLocation == "inv")
            {
                Debug.Log(downSlot);
            }
            else 
            {
                Locator.Get<ConnectionToGridiaServerHandler>().MoveItem(downCoord, getTileLocationOfMouse());
            }
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
