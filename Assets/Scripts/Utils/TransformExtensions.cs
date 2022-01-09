using UnityEngine;
using System.Collections.Generic;

public static class TransformDeepChildExtension
{
  public enum SearchMode
  {
    BreadthFirst,
    DepthFirst
  }

  public static Transform FindDeepChild(this Transform parent, string name, SearchMode mode = SearchMode.BreadthFirst)
  {
    switch (mode)
    {
      case SearchMode.BreadthFirst:
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(parent);
        while (queue.Count > 0)
        {
          Transform c = queue.Dequeue();
          if (c.name == name)
          {
            return c;
          }
          foreach (Transform t in c)
          {
            queue.Enqueue(t);
          }
        }
        break;

      case SearchMode.DepthFirst:
        foreach (Transform child in parent)
        {
          if (child.name == name)
          {
            return child;
          }
          Transform result = child.FindDeepChild(name, mode);
          if (result != null)
          {
            return result;
          }
        }
        break;
    }

    return null;
  }
}