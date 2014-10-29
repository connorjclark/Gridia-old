﻿using Gridia;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Serving;

public class GridiaDriver : MonoBehaviour
{
    private GridiaGame _game;

    void Start()
    {
        _game = new GridiaGame();
        Locator.Provide(_game);
        ResizeCamera();
        Spawn(10);

        ConnectionToGridiaServerHandler conn = new ConnectionToGridiaServerHandler(_game, "localhost", 1234);
        Locator.Provide(conn);
    }

    void Update()
    {
        _game.stateMachine.Step(Time.deltaTime);
        MoveCreatures(10.0f * Time.deltaTime);
        _game.view.Render();
    }

    void Spawn(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int x = Random.Range(0, _game.tileMap.Size);
            int y = Random.Range(0, _game.tileMap.Size);
            _game.CreateCreature(x, y);
        }
    }
    
    void MoveCreatures(float speed)
    {
        _game.creatures.ForEach(cre =>
        {
            if (cre is Player) return;
            if (cre.MovementDirection == Direction.None)
            {
                if (Random.Range(1, 50) <= 1)
                {
                    Vector2 direction = Direction.RandomDirection();
                    Vector2 target = cre.Position + direction;
                    if (_game.tileMap.Walkable((int)target.x, (int)target.y))
                    {
                        cre.MovementDirection = direction;
                    }
                }
            }
            else
            {
                cre.Move(speed);
            }
        });
    }

    void ResizeCamera()
    {
        Camera camera = Camera.main;
        camera.orthographicSize = Screen.height / 2.0f;
        camera.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, -1);
    }
}