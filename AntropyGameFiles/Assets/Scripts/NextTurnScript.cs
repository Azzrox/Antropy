using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextTurnScript : MonoBehaviour
{
  
  public float antGrowthRate;
  public float tileResourceRate;
  public float eventRate;
  public float weatherRate;

  private GameManagerUI gameManager;

  private void Awake()
  {
    gameManager = GameObject.Find("Game Manager").GetComponent<GameManagerUI>();
  }

  /// <summary>
  /// Turn Sequence, bind this to a button
  /// </summary>
  public void NextTurn() 
  {
    AntTurn();
    MapTurn();
    WeatherTurn();
    EventTurn();
    SeasonTurn();
    MessageTurn();
  }

  void AntTurn() 
  {
    //Insert Ant Turn
    TileScript[,] gameMap = MapScript.mapInstance.GameMap;//game_resources.map_instance.GameMap;
    for (int i = 0; i < MapScript.mapInstance.rows; i++)
    {
      for (int j = 0; j < MapScript.mapInstance.columns; j++)
      {
        //gameMap[i, j].CalculateNewResourceAmountFlat(50);
        if(gameMap[i, j].OwnedByPlayer) 
        {
          //calculate gathering rate (basic idea)
          int gathering_rate = 20;
          gameManager.resources += gathering_rate;
          gameMap[i, j].CalculateNewResourceAmount(-gathering_rate);
        }
      }
    }
  }

  void MapTurn() 
  {
    //Insert Map Turn
    //change the tile object
    TileScript[,] gameMap = MapScript.mapInstance.GameMap;//game_resources.map_instance.GameMap;

    for (int i = 0; i < MapScript.mapInstance.rows; i++)
    {
      for (int j = 0; j < MapScript.mapInstance.columns; j++)
      {
        //constant growth +
        //gameMap[i, j].CalculateNewResourceAmount(tileResourceRate);
        gameMap[i, j].CalculateNewResourceAmountFlat(20);

        //check if the growth if we reached a threshhold to update the tile mesh
        MapScript.mapInstance.TileErrosionCheck(gameMap[i, j]);

        Debug.Log(name + ": [" + i + "," + j + "]" + gameMap[i, j].ResourceAmount);
      }
    }
  }

  void WeatherTurn()
  {
    //Insert Weather Turn
  }

  void EventTurn() 
  {
    //Insert Event Turn
  }

  void MessageTurn() 
  {
    //Insert Message Turn
  }

  void SeasonTurn() 
  {
    //Insert Season Turn
  }
}
