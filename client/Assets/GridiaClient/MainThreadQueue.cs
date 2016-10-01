using System.Collections.Generic;

using UnityEngine;

public delegate void Action();

public class MainThreadQueue : MonoBehaviour
{
    #region Fields

    private static Queue<Action> queue = new Queue<Action>();
    private static MainThreadQueue _instance;

    #endregion Fields

    #region Methods

    public static void Add(Action action)
    {
        queue.Enqueue(action);
    }

    public static void Instantiate()
    {
        if (_instance == null)
        {
          var gameObject = new GameObject();
          gameObject.name = "MainThreadQueue";
          _instance = gameObject.AddComponent<MainThreadQueue>();
          DontDestroyOnLoad(gameObject);
          DontDestroyOnLoad(_instance);
        }
    }

    public void OnDestroy()
    {
        _instance = null;
    }

    public void Update()
    {
        while (queue.Count != 0)
        {
          var action = queue.Dequeue();
          action();
        }
    }

    #endregion Methods
}