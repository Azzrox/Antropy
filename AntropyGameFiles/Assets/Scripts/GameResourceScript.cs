using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Resources of the Project
/// </summary>
public class GameResourceScript: MonoBehaviour
{

  /// <summary>
  /// Singleton Game Resources
  /// </summary>
  public static GameResourceScript ResourceInstance;
  public MapScript MapInstance;

  void Awake()
  {
    //Keep the instance alive
    ResourceInstance = this;                          // linking the self-reference
    DontDestroyOnLoad(transform.gameObject); // set to dont destroy

    //get all instances
    MapInstance = MapScript.MapInstance;
  }
}
