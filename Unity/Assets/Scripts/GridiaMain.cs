using UnityEngine;
using System.Collections;
using Gridia;

public class GridiaMain : MonoBehaviour
{
    private TileMapView view;
    private StateMachine stateMachine;

    void Start ()
    {
        ContentManager contentManager = new ContentManager ();

        stateMachine = new StateMachine ();

        ResizeCamera ();
        TextureManager textureManager = new TextureManager ();
        TileMap tileMap = new TileMap (100);
        view = new TileMapView (tileMap, textureManager, 2.0f);

        stateMachine.SetState (new PlayerMovementState (view, 4f));
    }
	
    void Update ()
    {
        stateMachine.Step (Time.deltaTime);
        view.Render ();
    }

    void ResizeCamera ()
    {
        Camera camera = Camera.main;
        camera.orthographicSize = Screen.height / 2.0f;
        camera.transform.position = new Vector3 (Screen.width / 2, Screen.height / 2, -1);
    }
}