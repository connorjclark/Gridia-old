using UnityEngine;
using System.Collections;
using Gridia;
using System.Collections.Generic;

public class GridiaMain : MonoBehaviour
{
    private TileMap tileMap;
    private TileMapView view;
    private StateMachine stateMachine;
    private List<Creature> creatures;

    void Start ()
    {
        ResizeCamera();
        creatures = new List<Creature>();
        ContentManager contentManager = new ContentManager();
        TextureManager textureManager = new TextureManager();

        tileMap = new TileMap(30);
        Player player = new Player();
        tileMap.AddCreature(player);

        view = new TileMapView (tileMap, textureManager, 2.0f);
        view.Focus = player;

        stateMachine = new StateMachine();
        stateMachine.SetState(new PlayerMovementState(player, 4f));

        Spawn(10);
    }

    void Spawn(int amount) {
        for (int i = 0; i < amount; i++)
        {
            Creature cre = new Creature();
            int x = Random.Range(0, tileMap.Size);
            int y = Random.Range(0, tileMap.Size);
            cre.Position = new Vector2(x, y);
            tileMap.AddCreature(cre);
            creatures.Add(cre);
        }
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
        MoveCreatures(10.0f);
        view.Render ();
    }

    void ResizeCamera ()
    {
        Camera camera = Camera.main;
        camera.orthographicSize = Screen.height / 2.0f;
        camera.transform.position = new Vector3 (Screen.width / 2, Screen.height / 2, -1);
    }
}