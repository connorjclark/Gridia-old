using UnityEngine;
using System.Collections.Generic;

public delegate void Action();

public class MainThreadQueue : MonoBehaviour
{
  private static MainThreadQueue _instance;
  private static Queue<Action> queue = new Queue<Action>();

  public static void Instantiate()
  {
    if (_instance == null)
    {
      var gameObject = new GameObject();
      gameObject.name = "MainThreadQueue";
      _instance = gameObject.AddComponent<MainThreadQueue>();
    }
  }

  public static void Add(Action action)
  {
    queue.Enqueue(action);
  }

  public void Update()
  {
    while (queue.Count != 0)
    {
      var action = queue.Dequeue();
      action();
    }
  }

  public void OnDestroy()
  {
    _instance = null;
  }
}
