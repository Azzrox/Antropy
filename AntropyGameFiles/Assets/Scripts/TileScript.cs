using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class TileScript : MonoBehaviour
{
  /// <summary>
  /// Type of the tile
  /// </summary>
  public int type;

  /// <summary>
  /// Resources on the tile
  /// </summary>
  public int resource_amount;

  /// <summary>
  /// Tile sprite
  /// </summary>
  public MeshRenderer mesh_renderer;

  private void Awake()
  {
    mesh_renderer = GetComponent<MeshRenderer>();
    type = 0;
    type = resource_amount = 0;
  }

  /// <summary>
  /// Tile type, getter and setter
  /// </summary>
  public int TileType 
  { 
    get 
    { 
      return type; 
    }
    set
    {
      type = value;
    }
  }

  /// <summary>
  /// Tile resources, getter and setter
  /// </summary>
  public int ResourceAmount
  {
    get 
    {
      return resource_amount;
    }
    set 
    {
      resource_amount = value;
    }
  }

  /// <summary>
  /// Tile mesh render
  /// </summary>
  public MeshRenderer MeshRendererTile
  {
    get 
    {
      return mesh_renderer;
    }
    set 
    {
      mesh_renderer = value;
    }
  }
  
}
