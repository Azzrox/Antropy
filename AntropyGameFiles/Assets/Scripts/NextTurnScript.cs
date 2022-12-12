using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextTurnScript : MonoBehaviour
{
  /// <summary>
  /// Current Turn Number
  /// </summary>
  int currentTurnCount;

  /// <summary>
  /// Max allowed turn number
  /// </summary>
  public int MaxTurnCount;

  private GameManagerUI gameManager;

  private void Awake()
  {
    gameManager = GameObject.Find("Game Manager").GetComponent<GameManagerUI>();

    //TODO DELETE THIS, after we have an actual game
    //Current Default in case someone forgets
    MaxTurnCount = 1000;
  }

  /// <summary>
  /// Turn Sequence, bind this to a button
  /// </summary>
  public void NextTurn() 
  {
    if(currentTurnCount < MaxTurnCount) 
    {
      Debug.Log("Turn: " + currentTurnCount);
      AntTurn();
      MapTurn();
      WeatherTurn();
      EventTurn();
      SeasonTurn();
      MessageTurn();
      currentTurnCount++;
    }
    else 
    {
      Debug.Log("Hit Max Turn Count, turn denied");
    }
  }

  void AntTurn() 
  {
    //Insert Ant Turn
    TileScript[,] gameMap = gameManager.mapInstance.GameMap;//game_resources.map_instance.GameMap;
    for (int i = 0; i < gameManager.mapInstance.rows; i++)
    {
      for (int j = 0; j < gameManager.mapInstance.columns; j++)
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
    TileScript[,] gameMap = gameManager.mapInstance.GameMap;//game_resources.map_instance.GameMap;

    for (int i = 0; i < gameManager.mapInstance.rows; i++)
    {
      for (int j = 0; j < gameManager.mapInstance.columns; j++)
      {
        //constant growth +
        gameMap[i, j].CalculateNewResourceAmountFlat(500);

        //check if the growth if we reached a threshhold to update the tile mesh
        gameManager.mapInstance.TileErosionCheck(gameMap[i, j]);
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
