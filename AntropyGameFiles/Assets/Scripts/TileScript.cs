using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using System;
using Unity.VisualScripting;

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
  /// Amount of resources reserved to take from tile
  /// </summary>
  public int reservedResources;

  /// <summary>
  /// Ants on tile
  /// </summary>
  public int assignedAnts;

  /// <summary>
  /// Max Counter of assigned ants on tile
  /// </summary>
  public int maxAssignedAnts;

  /// <summary>
  /// Free Ants Global
  /// </summary>
  int freeAnts;

  /// <summary>
  /// Resources on the tile
  /// </summary>
  public double resourceAmount;

  /// <summary>
  /// Maximum of resources a tile can hold
  /// </summary>
  public int resourceMaxAmount;

  /// <summary>
  /// Owned by player
  /// </summary>
  public bool ownedByPlayer = false;

  /// <summary>
  /// Fog of war Unexplored/Explored
  /// </summary>
  public bool explored = false;

  /// <summary>
  /// Fog of war Visible
  /// </summary>
  public bool visible = false;

  /// <summary>
  /// Anthill or not
  /// </summary>
  bool isAnthill = false;

  /// <summary>
  /// Flag currently displayed on the tile
  /// </summary>
  GameObject flag;

  /// <summary>
  /// Flag Prefab for Owned Tiles
  /// </summary>
  public GameObject redFlagPrefab;

  GameManager gameManagerInstance;
  private MouseListenerUI uiListener;

  private void Awake()
  {
    gameManagerInstance = GameObject.Find("Game Manager").GetComponent<GameManager>();
  }
  private void Start()
  {
    maxAssignedAnts = gameManagerInstance.maxAntsResourceTile;
    uiListener = GameObject.Find("NextTurnCanvas").GetComponent<MouseListenerUI>();
  }

  /// <summary>
  /// Tile UI Activation
  /// </summary>
  private void OnMouseDown()
  {
    if (uiListener.isUIOverride)
    {
      Debug.Log("Cancelled OnMouseDown! A UI element has override this object!");
    }
    else
    {
      Debug.Log("in click mode");
      if (xPos == 0 && zPos == 0)
      {
        GameObject anthillUI = GameObject.Find("Anthill");
        anthillUI.GetComponent<Canvas>().enabled = true;
        AntHillUI antHill = anthillUI.GetComponent<AntHillUI>();
        antHill.tile = this;
        antHill.UpdateAntText();

        GameObject uiMiniBarInfo = GameObject.Find("MiniBarInfo");
        uiMiniBarInfo.GetComponent<MiniBarInfoUI>().MiniBarInfoUpdate();
      }
      else
      {
        GameObject uiAssignAnts = GameObject.Find("AssignAnts");
        uiAssignAnts.GetComponent<Canvas>().enabled = true;
      
        AntCounter antCounter = uiAssignAnts.GetComponent<AntCounter>();
        Debug.Log(assignedAnts);
        antCounter.SetAssignedAnts(XPos, ZPos, AssignedAnts, MaxAssignedAnts, false);
        antCounter.UpdateAntText();

        GameObject uiMiniBarInfo = GameObject.Find("MiniBarInfo");
        uiMiniBarInfo.GetComponent<MiniBarInfoUI>().MiniBarInfoUpdate();
      }

      Debug.Log("element clicked" + UnityEngine.Random.Range(0, 40) + " pos: " + XPos + "|" + ZPos);
    }
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
    else if (ResourceAmount > MaxResourceAmount)
    {
      ResourceAmount = MaxResourceAmount;
    }
  }

  /// <summary>
  /// Spawns the owned flag prefab on the tile
  /// </summary>
  public void spawnOwnedFlagOnTile()
  {
    flag = Instantiate<GameObject>(redFlagPrefab, new Vector3(xPos + 0.5f, 0.2f, zPos + 0.5f), Quaternion.identity, transform);
  }

  /// <summary>
  /// Destroys any flag object on the tile
  /// </summary>
  public void deleteFlagOnTile() 
  {
    if(flag != null) 
    {
      Destroy(flag);
    }
  }

  /// <summary>
  ///  Ants on tile, getter and setter
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
  ///  Max ants on tile, getter and setter
  /// </summary>
  public int MaxAssignedAnts
  {
    get
    {
      return maxAssignedAnts;
    }
    set
    {
      maxAssignedAnts = value;
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
      return resourceMaxAmount;
    }
    set
    {
      resourceMaxAmount = value;
    }
  }

  /// <summary>
  /// Anthill?, getter/setter
  /// </summary>
  public bool IsAntHill
  {
    get
    {
      return isAnthill;
    }
    set
    {
      isAnthill = value;
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

  /// <summary>
  /// Owner Status, getter/setter
  /// </summary>
  public bool Explored
  {
    get
    {
      return explored;
    }
    set
    {
      explored = value;
    }
  }
  /// <summary>
  /// Owner Status, getter/setter
  /// </summary>
  public bool Visible
  {
    get
    {
      return visible;
    }
    set
    {
      visible = value;
    }
  }
  /// <summary>
  /// Reserved resources per turn for tile, getter/setter
  /// </summary>
  public int ReservedResources
  {
    get
    {
      return reservedResources;
    }
    set
    {
      reservedResources = value;
    }
  }
}
