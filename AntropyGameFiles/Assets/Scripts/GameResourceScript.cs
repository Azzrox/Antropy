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
  public static GameResourceScript resource_instance;
  public MapScript map_instance;

  void Awake()
  {
    //Keep the instance alive
    resource_instance = this;                      
    DontDestroyOnLoad(transform.gameObject);

    //get all instance
    //map_instance = MapScript.map_instance;
  }
}
