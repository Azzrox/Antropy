using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using System;
//using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour, IPointerClickHandler
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

  GameObject road;

  GameObject ant;

  /// <summary>
  /// Flag Prefab for Owned Tiles
  /// </summary>
  public GameObject redFlagPrefab;
  public GameObject[] roadPrefab;
  public GameObject antPrefab;

  private MouseListenerUI uiListener;


  private void Start()
  {
    uiListener = GameObject.Find("NextTurnCanvas").GetComponent<MouseListenerUI>();
  }
 

  
  /// <summary>
  /// Tile UI Activation
  /// </summary>
  //private void OnMouseDown()
  public void OnPointerClick (PointerEventData eventData)
  {
    
    //if (uiListener.isUIOverride)
    //{
    //  Debug.Log("Cancelled OnMouseDown! A UI element has override this object!");
    //}
    //else
    //{
      
      if (xPos == GameManager.Instance.anthillX && zPos == GameManager.Instance.anthillY)
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
      //Debug.Log("element clicked" + UnityEngine.Random.Range(0, 40) + " pos: " + XPos + "|" + ZPos);
    //}
  }

  

  /// <summary>
  /// Spawns the owned flag prefab on the tile
  /// </summary>
  public void spawnOwnedFlagOnTile()
  {
    if(flag != null) 
    {
      Destroy(flag);
    }
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

  public void spawnRoadOnTile(int level)
  {
    if (road != null)
    {Destroy(road);}
    if (level >= 0 && level < roadPrefab.Length){
      GameManager.Instance.mapInstance.mapMatrix[xPos, zPos].GetComponent<MapTileGeneration>().RemoveDecorationForRoads();
      road = Instantiate<GameObject>(roadPrefab[level], new Vector3(xPos,0.02f - level/100, zPos), Quaternion.identity, transform);
    }
  }

  public void deleteRoad()
  {if (road != null)
    {Destroy(road);}
  }

  public void SpawnAnt() 
  {
      ant = Instantiate<GameObject>(antPrefab, new Vector3(xPos + 0.5f, 0.2f, zPos + 0.5f), Quaternion.identity, transform) ;
      ant.GetComponent<AntPathing>().coordinates = new int[] {xPos, zPos};

  }

  public void RemoveAnt() 
  {
      // some ants might fall outside that area?
      if (ant != null)
      {
          Destroy(ant);
      }
  }

  public void AdjustAntSize(int assignedAnts)
  {
    if (ant == null)
    {
      SpawnAnt();
    }

    var scale = ant.transform.localScale;
    float size = Mathf.Sqrt(assignedAnts);
    scale.Set(size, size, size);
    ant.transform.localScale = scale;
      
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
