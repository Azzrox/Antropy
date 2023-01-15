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
  bool checker = false;

  private void Awake()
  {
    gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    uiAssignAnts = GameObject.Find("AssignAnts");
    antCounter = uiAssignAnts.GetComponent<AntCounter>();
  }

  private void Start()
  {
    //Calculate the first upkeep
    GameManager.Instance.currentUpkeep = (int)Mathf.Ceil(GameManager.Instance.totalAnts * GameManager.Instance.foodPerAnt);
    TurnInfoUpdate();
    antCounter.UpdateAntText();
  }

  /// <summary>
  /// Turn Sequence, bind this to a button
  /// </summary>
  public void NextTurn() 
  {
    if(GameManager.Instance.currentTurnCount < GameManager.Instance.maxTurnCount) 
    {
      Debug.Log("Turn: " + GameManager.Instance.currentTurnCount);
      AntTurn();
      MapTurn();
      WeatherTurn();
      EventTurn();
      SeasonTurn();
      MessageTurn();
      ExploreTurn();
      GameManager.Instance.currentTurnCount++;
      TurnInfoUpdate();
      checker = false;

      //Update the infobars
      GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
      antCounter.UpdateAntText();
    }
    else 
    {
      Debug.Log("Hit Max Turn Count, turn denied");
    }
    GameManager.Instance.prototypeGoalCheck();
  }

  void AntTurn() 
  {

    //Reset GoalCheck
    GameManager.Instance.currentGoalProgress = 0;

    //Insert Ant Turn
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        if(GameManager.Instance.Map[i, j].ownedByPlayer) 
        {
          //Tile Distance Degradation + Current Weather influence
          float gatheringBase = GameManager.Instance.Map[i, j].assignedAnts * GameManager.Instance.resourceGatherRate;// * gameManager.weatherAcessMultiplier);
          for (int k = 0; k < GameManager.Instance.Map[i, j].distanceAntHill; k++)
          {
            //gatheringBase = Mathf.Ceil(gatheringBase * gameManager.distanceGatheringReductionRate);
          }

          if(GameManager.Instance.Map[i, j].type == 1 || GameManager.Instance.Map[i, j].type == 2) // tile is grass or soil
          {
            GameManager.Instance.resources += GameManager.Instance.Map[i, j].reservedResources;
            //Debug.Log("NewResources: " + GameManager.Instance.resources);

            //End Score
            GameManager.Instance.totalResources += GameManager.Instance.Map[i, j].reservedResources;
            CalculateNewResourceAmountFlat((int)-GameManager.Instance.Map[i, j].reservedResources, i, j);
          
          }
          
          //add the flag because it's owned (Prototype) only if 10 are on a tile you get a flag
          if(GameManager.Instance.Map[i, j].assignedAnts == 10) 
          {
            GameManager.Instance.mapInstance.mapMatrix[i, j].spawnOwnedFlagOnTile();
            GameManager.Instance.currentGoalProgress += 1;
          }
          else 
          {
            GameManager.Instance.mapInstance.mapMatrix[i, j].deleteFlagOnTile();
            GameManager.Instance.currentGoalProgress -= 1;
          }    
        }
        else if (GameManager.Instance.Map[i, j].assignedAnts != 10) 
        {
          //if tile not owned by player remove flag
          GameManager.Instance.mapInstance.mapMatrix[i, j].deleteFlagOnTile();
        }
      }
    }

    //Current Upkeep Calculation
    GameManager.Instance.currentUpkeep = (int)Mathf.Ceil(GameManager.Instance.totalAnts * GameManager.Instance.foodPerAnt);

    //Storage Room Check
    //resources - currentUpkeet => leftover resources
    int netto_resource = GameManager.Instance.resources - GameManager.Instance.currentUpkeep;
    GameManager.Instance.resources = Mathf.Min(GameManager.Instance.maxResourceStorage, Mathf.Max(0, netto_resource));
    /*
    if ((GameManager.Instance.resources - GameManager.Instance.currentUpkeep) > 0) 
    {
      GameManager.Instance.resources -= GameManager.Instance.currentUpkeep;
    }
    else 
    {
      GameManager.Instance.resources = 0;
    }
   
    if (GameManager.Instance.resources > GameManager.Instance.maxResourceStorage)
    {
      GameManager.Instance.resources = GameManager.Instance.maxResourceStorage;
    }
    */
    //Check if we reached the prototype goal
    GameManager.Instance.prototypeLooseCheck();

    //Population growth
    int new_pop = (int)Mathf.Ceil((float)GameManager.Instance.totalAnts * GameManager.Instance.antPopGrowthPerTurn);
    GameManager.Instance.freeAnts += new_pop;
    GameManager.Instance.totalAnts += new_pop;
    
    //Calculate new upkeep for the next turn
    GameManager.Instance.currentUpkeep = (int)Mathf.Ceil(GameManager.Instance.totalAnts * GameManager.Instance.foodPerAnt);
  }
  void MapTurn() 
  {
    //Insert Map Turn
    //change the tile object
    GameManager.Instance.income = 0 - GameManager.Instance.currentUpkeep;
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        int regrowAmount = (int)Mathf.Ceil(GameManager.Instance.tileRegrowAmount);
        CalculateNewResourceAmountFlat(regrowAmount,i ,j);
        
        if(GameManager.Instance.Map[i, j].assignedAnts * (int)GameManager.Instance.resourceGatherRate > (int)GameManager.Instance.Map[i,j].resourceAmount)
        {
          GameManager.Instance.Map[i, j].reservedResources = (int) GameManager.Instance.Map[i, j].resourceAmount;
        }
        else
        {
          GameManager.Instance.Map[i, j].reservedResources = GameManager.Instance.Map[i, j].assignedAnts * (int)GameManager.Instance.resourceGatherRate;
        }
        GameManager.Instance.income += GameManager.Instance.Map[i, j].reservedResources;
        

        //check if the growth if we reached a threshhold to update the tile mesh
        GameManager.Instance.mapInstance.TileErosionCheck(i,j); // TODO: think about where to set TileErosion (ExchangeTilePrefab) function!
      }
    }
  }

  void ExploreTurn()
  {
    TileScript[,] gameMap = gameManager.mapInstance.GameMap;
    int[,] adder = new int[,] { { -1, 0 }, { -1, -1 }, { -1, 1 }, { 1, 0 }, { 1, -1 }, { 1, 1 }, { 0, -1 }, { 0, 1 } };
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        if(GameManager.Instance.Map[i, j].assignedAnts > 0)
        {
          for (int k = 0; k < adder.Length / 2; k++)
          {
            if (i + adder[k, 0] < GameManager.Instance.rows && i + adder[k, 0] >= 0 && j + adder[k, 1] < GameManager.Instance.columns && j + adder[k, 1] >= 0)
              if (GameManager.Instance.Map[i + adder[k, 0], j + adder[k, 1]].explored == false)
              {
                GameManager.Instance.mapInstance.SetExplored(i + adder[k, 0], j + adder[k, 1], true); // TODO: think about where to set SetExplored (ExchangeTilePrefab) function!
              }
          }
        }
        //check if the growth if we reached a threshhold to update the tile mesh
        GameManager.Instance.mapInstance.TileErosionCheck(i,j); // TODO: think about where to set TileErosion (ExchangeTilePrefab) function!
      }
    }
  }

  void WeatherTurn()
  {
    //Insert Weather Turn
    GameManager.Instance.weatherInstance.UpdateWeather(GameManager.Instance.currentSeason);
    GameManager.Instance.weatherInstance.WeatherMultiplierUpdate(GameManager.Instance.currentWeather);
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
    TurnText.text = GameManager.Instance.currentTurnCount + "/" + GameManager.Instance.maxTurnCount;
  }



  /// <summary>
  /// Adds an integer number to the current resource amount on the tile
  /// </summary>
  /// <param name="amount_of_change"></param>
  public void CalculateNewResourceAmountFlat(int amount_of_change, int i, int j)
  {
    float resourceAmount = GameManager.Instance.Map[i,j].resourceAmount + amount_of_change;
    float resourceMaxAmount = GameManager.Instance.Map[i,j].resourceMaxAmount;

    if (resourceAmount < 0)
    {
      resourceAmount = 0;
    }
    else if (resourceAmount > resourceMaxAmount)
    {
      resourceAmount = resourceMaxAmount;
    }
    GameManager.Instance.Map[i,j].resourceAmount = resourceAmount;
  }

  int CalculationCollectedResources()
  {
    float resource = 0;
    for(int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        resource += Mathf.Max(0,GameManager.Instance.Map[i,j].assignedAnts * GameManager.Instance.resourceGatherRate);

      }

    }
    return (int)resource;
  }


}
