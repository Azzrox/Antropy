using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class TileScript : MonoBehaviour
{
  /// <summary>
  /// Eucledian Distance to the anthill in steps (no diagonal)
  /// </summary>
  int distance_anthill;

  /// <summary>
  /// Type of the tile
  /// </summary>
  int type;

  /// <summary>
  /// Ants on tile
  /// </summary>
  int ants_present;

  /// <summary>
  /// Resources on the tile
  /// </summary>
  double resource_amount;

  /// <summary>
  /// Maximum of resources a tile can hold
  /// </summary>
  int resource_max_amount;

  /// <summary>
  /// Tile sprite
  /// </summary>
  MeshRenderer mesh_renderer;

  /// <summary>
  /// Owned by player
  /// </summary>
  bool owned_by_player = false;

  private void Awake()
  {
    mesh_renderer = GetComponent<MeshRenderer>();
    type = 0;
    resource_amount = 0;
    resource_max_amount = 0;
    distance_anthill = 0;
    ants_present = 0;
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
  /// Ants on Tile, getter and setter
  /// </summary>
  public int AntsPresent
  {
    get
    {
      return ants_present;
    }
    set
    {
      ants_present = value;
    }
  }

  /// <summary>
  /// Tile distance to anthill, getter and setter
  /// </summary>
  public int TileDistance
  {
    get
    {
      return distance_anthill;
    }
    set
    {
      distance_anthill = value;
    }
  }

  /// <summary>
  /// Tile resources, getter and setter
  /// </summary>
  public double ResourceAmount
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
  /// Max tile resources, getter and setter
  /// </summary>
  public int  MaxResourceAmount
  {
    get
    {
      return resource_max_amount;
    }
    set
    {
      resource_max_amount = value;
    }
  }

  /// <summary>
  /// Calculate new tile resource count
  /// </summary>
  /// <param name="percentage_of_change"> multiplicator </param>
  public void CalculateNewResourceAmount(double percentage_of_change) 
  {
    ResourceAmount += (ResourceAmount / 100) * percentage_of_change;

    if(ResourceAmount < 0) 
    {
      ResourceAmount = 0;
    }

    if (ResourceAmount > MaxResourceAmount)
    {
      ResourceAmount = MaxResourceAmount;
    }
  }

  /// <summary>
  /// Adds an integer number to the current resource amount on the tile
  /// </summary>
  /// <param name="amount_of_change"></param>
  public void CalculateNewResourceAmountFlat(int amount_of_change) 
  {
    ResourceAmount += amount_of_change;

    if (ResourceAmount < 0)
    {
      ResourceAmount = 0;
    }

    if (ResourceAmount > MaxResourceAmount)
    {
      ResourceAmount = MaxResourceAmount;
    }
  }

  /// <summary>
  /// Tile mesh render, getter and setter
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

  /// <summary>
  /// Owner Status, getter/setter
  /// </summary>
  public bool OwnedByPlayer 
  {
    get
    {
      return owned_by_player;
    }
    set
    {
      owned_by_player = value;
    }
  }
  
}
