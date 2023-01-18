using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using System;
//using Unity.VisualScripting;
using UnityEditor;

public class TileScript : MonoBehaviour
{
  /// <summary>
  /// Eucledian Distance to the anthill in steps (no diagonal)
  /// </summary>
  int distanceAnthill;

  /// <summary>
  /// X,Z Pos of the tile
  /// </summary>
  public int xPos;
  public int zPos;

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
    uiListener = GameObject.Find("NextTurnCanvas").GetComponent<MouseListenerUI>();
  }

  /// <summary>
  /// Tile UI Activation
  /// </summary>
  private void OnMouseDown()
  {
    Debug.Log("CLICKED");
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
        antCounter.SetSelectedTile(XPos, ZPos);
        antCounter.UpdateAntText();
        GameObject highlight = GameObject.Find("HighlightTile");
        highlight.transform.position = new Vector3(XPos, 0, ZPos);
        highlight.GetComponent<MeshRenderer>().enabled = true;


        //Why is here an update of MiniBarInfoUpdate necessary?
        GameObject uiMiniBarInfo = GameObject.Find("MiniBarInfo");
        uiMiniBarInfo.GetComponent<MiniBarInfoUI>().MiniBarInfoUpdate();
      }
      Debug.Log("element clicked" + UnityEngine.Random.Range(0, 40) + " pos: " + XPos + "|" + ZPos);
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

}
