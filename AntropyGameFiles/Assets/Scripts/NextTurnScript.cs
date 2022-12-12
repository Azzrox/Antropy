using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextTurnScript : MonoBehaviour
{
  
  public float antGrowthRate;
  public float tileResourceRate;
  public float eventRate;
  public float weatherRate;

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
        gameMap[i, j].CalculateNewResourceAmount(tileResourceRate);

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
