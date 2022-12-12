using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using System;

public class TileScript : MonoBehaviour
{
  /// <summary>
  /// Eucledian Distance to the anthill in steps (no diagonal)
  /// </summary>
  int distanceAnthill;

  /// <summary>
  /// X,Z Pos of the tile
  /// </summary>
  int xPos;
  int zPos;

  /// <summary>
  /// Type of the tile
  /// </summary>
  int type;

  /// <summary>
  /// Ants on tile
  /// </summary>
  int assignedAnts;


  /// <summary>
  /// Free Ants Global
  /// </summary>
  int freeAnts;

  /// <summary>
  /// Resources on the tile
  /// </summary>
  double resourceAmount;

  /// <summary>
  /// Maximum of resources a tile can hold
  /// </summary>
  int resource_max_amount;

  /// <summary>
  /// Owned by player
  /// </summary>
  bool ownedByPlayer = false;

  private void Awake()
  {
    //type = 0;
    //resourceAmount = 0;
    //resource_max_amount = 0;
    //distanceAnthill = 0;
    assignedAnts = 0;
    freeAnts = 0;
  }

  /// <summary>
  /// Assignment Over
  /// </summary>
  private void OnMouseDown()
  {
    Debug.Log("in click mode");
    GameObject uiAssignAnts = GameObject.Find("AssignAnts");
    uiAssignAnts.GetComponent<Canvas>().enabled = true;

    AntCounter antCounter = uiAssignAnts.GetComponent<AntCounter>();
    antCounter.SetAssignedAnts(XPos, ZPos, assignedAnts, 10, false);
    antCounter.UpdateAntText();

    Debug.Log("element clicked" + UnityEngine.Random.Range(0, 40) + " pos: " + XPos + "|" + ZPos);
  }

  /// <summary>
  /// Calculate new tile resource count
  /// </summary>
  /// <param name="percentage_of_change"> multiplicator </param>
  public void CalculateNewResourceAmount(double percentage_of_change)
  {
    ResourceAmount += Math.Round(((ResourceAmount / 100) * percentage_of_change), 2);

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
  ///  Ants on Tile, getter and setter
  /// </summary>
  public int AssignedAnts
  {
    get
    {
      return assignedAnts;
    }
    set
    {
      assignedAnts = value;
    }
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
  /// Tile distance to anthill, getter and setter
  /// </summary>
  public int TileDistance
  {
    get
    {
      return distanceAnthill;
    }
    set
    {
      distanceAnthill = value;
    }
  }

  /// <summary>
  /// Tile resources, getter and setter
  /// </summary>
  public double ResourceAmount
  {
    get 
    {
      return resourceAmount;
    }
    set 
    {
      resourceAmount = value;
    }
  }

  /// <summary>
  /// Tile resources, getter and setter
  /// </summary>
  public int FreeAnts
  {
    get
    {
      return freeAnts;
    }
    set
    {
      freeAnts = value;
    }
  }

  /// <summary>
  /// XPos tile, getter and setter
  /// </summary>
  public int XPos
  {
    get
    {
      return xPos;
    }
    set
    {
      xPos = value;
    }
  }

  /// <summary>
  /// YPos tile, getter and setter
  /// </summary>
  public int ZPos
  {
    get
    {
      return zPos;
    }
    set
    {
      zPos = value;
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
  /// Owner Status, getter/setter
  /// </summary>
  public bool OwnedByPlayer 
  {
    get
    {
      return ownedByPlayer;
    }
    set
    {
      ownedByPlayer = value;
    }
  }
}
