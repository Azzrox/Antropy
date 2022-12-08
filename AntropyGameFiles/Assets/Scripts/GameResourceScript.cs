using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Resource Instance of the Project
/// </summary>
public class GameResourceScript: MonoBehaviour
{
  public static GameResourceScript instance;
  public static TileScript[,]  map_matrix;

  void Awake()
  {
    instance = this;                          // linking the self-reference
    DontDestroyOnLoad(transform.gameObject); // set to dont destroy
  }
}
