using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NextTurnScript : MonoBehaviour
{
  private GameManager gameManager;
  public TextMeshProUGUI TurnText;

  //UI Update
  GameObject uiAssignAnts; //= GameObject.Find("AssignAnts");
  AntCounter antCounter;

  private void Awake()
  {
    gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    uiAssignAnts = GameObject.Find("AssignAnts");
    antCounter = uiAssignAnts.GetComponent<AntCounter>();
  }

  private void Start()
  {
    //Calculate the first upkeep
    gameManager.currentUpkeep = (int)Mathf.Ceil(gameManager.totalAnts * gameManager.foodPerAnt);
    TurnInfoUpdate();
    antCounter.UpdateAntText();
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
      //WeatherTurn();
      EventTurn();
      SeasonTurn();
      MessageTurn();
      ExploreTurn();
      gameManager.currentTurnCount++;
      TurnInfoUpdate();

      //Update the infobars
      gameManager.miniBarInfoInstance.MiniBarInfoUpdate();
      antCounter.UpdateAntText();
    }
    else 
    {
      Debug.Log("Hit Max Turn Count, turn denied");
    }

    //Check if we reached the prototype goal
    gameManager.prototypeGoalCheck();
  }

  void AntTurn() 
  {

    //Reset GoalCheck
    gameManager.currentGoalProgress = 0;


    //Insert Ant Turn
    TileScript[,] gameMap = gameManager.mapInstance.GameMap;//game_resources.map_instance.gameManager.mapInstance.GameMap;
    for (int i = 0; i < gameManager.mapInstance.rows; i++)
    {
      for (int j = 0; j < gameManager.mapInstance.columns; j++)
      {
        if(gameMap[i, j].OwnedByPlayer) 
        {
          //Tile Distance Degridation + Current Weather influence
          float gatheringBase = gameMap[i, j].AssignedAnts * gameManager.resourceGatherRate;// * gameManager.weatherAcessMultiplier);
          for (int k = 0; k < gameMap[i, j].TileDistance; k++)
          {
            //gatheringBase = Mathf.Ceil(gatheringBase * gameManager.distanceGatheringReductionRate);
          }
          gameManager.resources += (int)gatheringBase;
          //gameManager.income += (int)gatheringBase;
 
          Debug.Log("NewResources: " + gameManager.resources);

          //End Score
          gameManager.TotalResources += (int)gatheringBase;

          gameMap[i, j].CalculateNewResourceAmountFlat((int)-gatheringBase);

          //add the flag because it's owned
          if(gameMap[i, j].assignedAnts == 10) 
          {
            gameMap[i, j].spawnOwnedFlagOnTile();
            gameManager.currentGoalProgress += 1;
          }
          
        }
        else 
        {
          //if tile not owned by player remove flag
          gameMap[i, j].deleteFlagOnTile();
        }
      }
    }
    //Calculate new upkeep
    gameManager.currentUpkeep = (int)Mathf.Ceil(gameManager.totalAnts * gameManager.foodPerAnt);
    
    //Storage Room Check
    if((gameManager.resources - gameManager.currentUpkeep) >= 0) 
    {
      gameManager.resources -= gameManager.currentUpkeep;
    }
    else 
    {
      gameManager.resources = 0;
    }
   
    if (gameManager.maxResourceStorage <= gameManager.resources) // needs fixing not good
    {
      gameManager.resources = gameManager.maxResourceStorage;
    }

    //Population growth
    int new_pop = (int)Mathf.Ceil((float)gameManager.totalAnts * gameManager.antPopGrowthPerTurn);
    gameManager.freeAnts += new_pop;
    gameManager.totalAnts += new_pop;//(int)Mathf.Ceil((float)gameManager.totalAnts * gameManager.antPopGrowthPerTurn);

    //Population no resources death checks
    if (gameManager.resources == 0 && gameManager.income < 0) 
    {
      gameManager.freeAnts -= gameManager.antDeathLackofResourcesAmount;

      //Score
      gameManager.TotalDeaths += gameManager.antDeathLackofResourcesAmount;

      //TODO Later
      //Remove them from a random tile if there are no free ants
      //gameManager.totalAnts -= gameManager.antDeathLackofResourcesAmount;
    }

    //Over Population penalty/death checks
    if (gameManager.currentMaximumPopulationCapacity < gameManager.totalAnts)
    {
      gameManager.freeAnts -= gameManager.antDeathLackofResourcesAmount;

      //Score
      gameManager.TotalDeaths += gameManager.antDeathLackofResourcesAmount;

      //TODO Later
      //Remove them from a random tile if there are no free ants
      //gameManager.totalAnts -= gameManager.antOverPopulationDeathAmount;
    }

    //Calculate new upkeep for the next turn
    gameManager.currentUpkeep = (int)Mathf.Ceil(gameManager.totalAnts * gameManager.foodPerAnt);
  }

  void MapTurn() 
  {
    //Insert Map Turn
    //change the tile object
    TileScript[,] gameMap = gameManager.mapInstance.GameMap;//game_resources.map_instance.GameMap;
    gameManager.income = 0 - gameManager.currentUpkeep;
    for (int i = 0; i < gameManager.mapInstance.rows; i++)
    {
      for (int j = 0; j < gameManager.mapInstance.columns; j++)
      {
        //constant growth + current weather influence (currently disabled)
        //int regrowAmount = (int)Mathf.Ceil(gameManager.tileRegrowAmount * gameManager.weatherRegrowMultiplier);
        int regrowAmount = (int)Mathf.Ceil(gameManager.tileRegrowAmount);
        gameMap[i, j].CalculateNewResourceAmountFlat(regrowAmount);
        if(gameMap[i, j].AssignedAnts * (int)gameManager.resourceGatherRate > (int)gameMap[i,j].ResourceAmount)
        {
          gameMap[i, j].ReservedResources = (int) gameMap[i, j].ResourceAmount;
        }
        else
        {
          gameMap[i, j].ReservedResources = gameMap[i, j].AssignedAnts * (int)gameManager.resourceGatherRate;
        }
        gameManager.income += gameMap[i, j].ReservedResources;

        //check if the growth if we reached a threshhold to update the tile mesh
        gameManager.mapInstance.TileErosionCheck(gameMap[i, j]);
      }
    }
  }

  void ExploreTurn()
  {
    TileScript[,] gameMap = gameManager.mapInstance.GameMap;
    int[,] adder = new int[,] { { -1, 0 }, { -1, -1 }, { -1, 1 }, { 1, 0 }, { 1, -1 }, { 1, 1 }, { 0, -1 }, { 0, 1 } };
    for (int i = 0; i < gameManager.mapInstance.rows; i++)
    {
      for (int j = 0; j < gameManager.mapInstance.columns; j++)
      {
        if(gameMap[i, j].AssignedAnts > 0)
        {
          for (int k = 0; k < adder.Length / 2; k++)
          {
            if (i + adder[k, 0] < gameManager.mapInstance.rows && i + adder[k, 0] >= 0 && j + adder[k, 1] < gameManager.mapInstance.columns && j + adder[k, 1] >= 0)
              if (gameMap[i + adder[k, 0], j + adder[k, 1]].Explored == false)
              {
                gameManager.mapInstance.SetExplored(gameMap[i + adder[k, 0], j + adder[k, 1]], true);
              }
          }
        }
        //check if the growth if we reached a threshhold to update the tile mesh
        gameManager.mapInstance.TileErosionCheck(gameMap[i, j]);
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
