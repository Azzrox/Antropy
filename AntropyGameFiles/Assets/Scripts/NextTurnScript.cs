using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NextTurnScript : MonoBehaviour
{
  private GameManager gameManager;
  public TextMeshProUGUI TurnText;


  private void Awake()
  {
    gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
  }

  private void Start()
  {
    //Calculate new upkeep
    gameManager.currentUpkeep = (int)Mathf.Ceil(gameManager.totalAnts * gameManager.foodPerAnt);
    //Income
    gameManager.income -= gameManager.currentUpkeep;

    TurnInfoUpdate();
  }

  /// <summary>
  /// Turn Sequence, bind this to a button
  /// </summary>
  public void NextTurn() 
  {
    if(gameManager.currentTurnCount < gameManager.maxTurnCount) 
    {
      Debug.Log("Turn: " + gameManager.currentTurnCount);
      AntTurn();
      MapTurn();
      WeatherTurn();
      EventTurn();
      SeasonTurn();
      MessageTurn();
      ExploreTurn();
      gameManager.currentTurnCount++;
      TurnInfoUpdate();

      //Update the infobar
      gameManager.miniBarInfoInstance.MiniBarInfoUpdate();
    }
    else 
    {
      Debug.Log("Hit Max Turn Count, turn denied");
    }
  }

  void AntTurn() 
  {
    //Insert Ant Turn
    //TileScript[,] gameManager.mapInstance.GameMap = gameManager.mapInstance.gameManager.mapInstance.GameMap;//game_resources.map_instance.gameManager.mapInstance.GameMap;
    for (int i = 0; i < gameManager.mapInstance.rows; i++)
    {
      for (int j = 0; j < gameManager.mapInstance.columns; j++)
      {
        if(gameManager.mapInstance.GameMap[i, j].OwnedByPlayer) 
        {
          //Tile Distance Degridation + Current Weather influence
          float gatheringBase = gameManager.mapInstance.GameMap[i, j].AssignedAnts * gameManager.resourceGatherRate;// * gameManager.weatherAcessMultiplier);
          for (int k = 0; k < gameManager.mapInstance.GameMap[i, j].TileDistance; k++)
          {
            gatheringBase = Mathf.Ceil(gatheringBase * gameManager.distanceGatheringReductionRate);
          }

          //Storage Room Check
          if(gameManager.maxResourceStorage <= (gameManager.resources + (int)gatheringBase)) 
          {
            gameManager.resources = gameManager.maxResourceStorage;
            gameManager.income += (int)gatheringBase;
          }
          else 
          {
            gameManager.resources += (int)gatheringBase;
            gameManager.income += (int)gatheringBase;
          }
          
          Debug.Log("NewResources: " + gameManager.resources);
          gameManager.mapInstance.GameMap[i, j].CalculateNewResourceAmountFlat((int)-gatheringBase);
          
        }
      }
    }

    //Population growth
    gameManager.freeAnts += (int)Mathf.Ceil((float)gameManager.freeAnts * gameManager.antGrowth);

    //Calculate new upkeep
    gameManager.currentUpkeep = (int) Mathf.Ceil(gameManager.totalAnts * gameManager.foodPerAnt);

    //Income
    gameManager.income -= gameManager.currentUpkeep;
  }

  void MapTurn() 
  {
    //Insert Map Turn
    //change the tile object
    //TileScript[,] gameManager.mapInstance.GameMap = gameManager.mapInstance.gameManager.mapInstance.GameMap;//game_resources.map_instance.gameManager.mapInstance.GameMap;

    for (int i = 0; i < gameManager.mapInstance.rows; i++)
    {
      for (int j = 0; j < gameManager.mapInstance.columns; j++)
      {
        //constant growth + current weather influence
        int regrowAmount = (int)Mathf.Ceil(gameManager.tileRegrowAmount * gameManager.weatherRegrowMultiplier);
        gameManager.mapInstance.GameMap[i, j].CalculateNewResourceAmountFlat(regrowAmount);

        //check if the growth if we reached a threshhold to update the tile mesh
        gameManager.mapInstance.TileErosionCheck(gameManager.mapInstance.GameMap[i, j]);
      }
    }
  }

  void ExploreTurn()
  {
   // TileScript[,] gameManager.mapInstance.GameMap = gameManager.mapInstance.gameManager.mapInstance.GameMap;
    int[,] adder = new int[,] { { -1, 0 }, { -1, -1 }, { -1, 1 }, { 1, 0 }, { 1, -1 }, { 1, 1 }, { 0, -1 }, { 0, 1 } };
    for (int i = 0; i < gameManager.mapInstance.rows; i++)
    {
      for (int j = 0; j < gameManager.mapInstance.columns; j++)
      {
        //constant growth +
        if(gameManager.mapInstance.GameMap[i, j].AssignedAnts > 0)
        {
          for (int k = 0; k < adder.Length / 2; k++)
          {
            if (i + adder[k, 0] < gameManager.mapInstance.rows && i + adder[k, 0] >= 0 && j + adder[k, 1] < gameManager.mapInstance.columns && j + adder[k, 1] >= 0)
              if (gameManager.mapInstance.GameMap[i + adder[k, 0], j + adder[k, 1]].Explored == false)
              {
                gameManager.mapInstance.SetExplored(gameManager.mapInstance.GameMap[i + adder[k, 0], j + adder[k, 1]], true);
              }
          }
        }
        //check if the growth if we reached a threshhold to update the tile mesh
        gameManager.mapInstance.TileErosionCheck(gameManager.mapInstance.GameMap[i, j]);
      }
    }
  }

  void WeatherTurn()
  {
    //Insert Weather Turn
    gameManager.weatherInstance.UpdateWeather(gameManager.currentSeason);
    gameManager.weatherInstance.WeatherMultiplierUpdate(gameManager.currentWeather);
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

  public void TurnInfoUpdate()
  {
    TurnText.text = gameManager.currentTurnCount + "/" + gameManager.maxTurnCount;
  }


}
