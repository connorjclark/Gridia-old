using Gridia;
using System.IO;
using System.Threading;
using UnityEngine;
using System.Linq;

public class GridiaDriver : MonoBehaviour
{
    public static AutoResetEvent connectedWaitHandle = new AutoResetEvent(false);
    public static AutoResetEvent gameInitWaitHandle = new AutoResetEvent(false);
    private GridiaGame _game;

    void Start()
    {
        _game = new GridiaGame();
        Locator.Provide(_game);
        ResizeCamera();

        MonoBehaviour.print("connecting");
        ConnectionToGridiaServerHandler conn = new ConnectionToGridiaServerHandler(_game, "localhost", 1234);
        Locator.Provide(conn);
        conn.Start();

        connectedWaitHandle.WaitOne();

        Locator.Provide(new ContentManager("TestWorld")); // :(
        Locator.Provide(new TextureManager("TestWorld"));
        _game.Initialize(GridiaConstants.SIZE, GridiaConstants.DEPTH, GridiaConstants.SECTOR_SIZE); // :(

        gameInitWaitHandle.Set();
    }

    void Update()
    {
        if (_game.stateMachine != null) {
            _game.stateMachine.Step(Time.deltaTime);
        }
        MoveCreatures(10.0f * Time.deltaTime);
        _game.view.Render();
    }
    
    void MoveCreatures(float speed)
    {
        _game.creatures.ValuesToList().ForEach(cre => cre.Move(speed));
    }

    void ResizeCamera()
    {
        Camera camera = Camera.main;
        camera.orthographicSize = Screen.height / 2.0f;
        camera.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, -1);
    }
}
