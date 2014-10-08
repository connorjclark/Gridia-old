using UnityEngine;
using System.Collections;
using Gridia;
using System.Collections.Generic;

public class GridiaMain : MonoBehaviour
{
    public TileMap tileMap;
    public TileMapView view;
    public StateMachine stateMachine;
    public List<Creature> creatures;

    void Start ()
    {
        if (GridiaConstants.IsServer)
        {
            ServerConnection.CreateServer(101);
        }
        else 
        {
            ServerConnection.ConnectAsClient("127.0.0.1", 101);
        }
        ServerConnection.instance.Gridia = this;

        ResizeCamera();
        creatures = new List<Creature>();
        ContentManager contentManager = new ContentManager();
        TextureManager textureManager = new TextureManager();

        tileMap = new TileMap(30);
        Player player = CreatePlayer(0, 0);

        view = new TileMapView (tileMap, textureManager, 2.0f);
        view.Focus = player;

        stateMachine = new StateMachine(ServerConnection.instance);
        stateMachine.SetState(new PlayerMovementState(player, 4f));

        Spawn(10);
    }

    void Spawn(int amount) {
        for (int i = 0; i < amount; i++)
        {
            int x = Random.Range(0, tileMap.Size);
            int y = Random.Range(0, tileMap.Size);
            CreateCreature(x, y);
        }
    }

    Player CreatePlayer(int x, int y) {
        Player cre = new Player();
        cre.Position = new Vector2(x, y);
        tileMap.AddCreature(cre);
        creatures.Add(cre);
        return cre;
    }

    void CreateCreature(int x, int y) {
        Creature cre = new Creature();
        cre.Position = new Vector2(x, y);
        tileMap.AddCreature(cre);
        creatures.Add(cre);
    }

    void MoveCreatures(float speed) {
        float stepSpeed = speed * Time.deltaTime;
        creatures.ForEach(cre =>
        {
            if (cre.MovementDirection == Direction.None)
            {
                if (Random.Range(1, 50) <= 1)
                {
                    Vector2 direction = Direction.RandomDirection();
                    Vector2 target = cre.Position + direction;
                    if (tileMap.Walkable((int)target.x, (int)target.y))
                    {
                        cre.MovementDirection = direction;
                    }
                }
            }
            else
            {
                cre.Offset = cre.Offset + cre.MovementDirection * stepSpeed;
                if (Utilities.Vector2IsAbsoluteGreaterThanOne(cre.Offset))
                {
                    tileMap.UpdateCreature(cre, cre.Position + cre.MovementDirection);
                    cre.Offset = Vector2.zero;
                    cre.MovementDirection = Direction.None;
                }
            }
        });
    }

    void Update ()
    {
        stateMachine.Step (Time.deltaTime);
        //MoveCreatures(10.0f);
        view.Render ();
    }

    void ResizeCamera ()
    {
        Camera camera = Camera.main;
        camera.orthographicSize = Screen.height / 2.0f;
        camera.transform.position = new Vector3 (Screen.width / 2, Screen.height / 2, -1);
    }
}