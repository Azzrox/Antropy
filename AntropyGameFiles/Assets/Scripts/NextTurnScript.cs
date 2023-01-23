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
      ExploreTurn();
      MapTurn();
      WeatherTurn();
      EventTurn();
      SeasonTurn();
      MessageTurn();
      GameManager.Instance.currentTurnCount++;
      TurnInfoUpdate();
      
      checker = false;

      //Update the infobars
      GameManager.Instance.UpdateIncomeGrowth();
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

    // update resources
    // Storage room check 
    if (GameManager.Instance.resources + GameManager.Instance.income > GameManager.Instance.maxResourceStorage)
    {
      // Trigger for storage overflow
      // effectively stored income (needed for historic data)
      GameManager.Instance.income = GameManager.Instance.maxResourceStorage - GameManager.Instance.resources;
    }

    GameManager.Instance.resources += GameManager.Instance.income;

    // historic data
    GameManager.Instance.totalResources += GameManager.Instance.income;

    // Reset goal check
    //GameManager.Instance.currentGoalProgress = 0;

    // update left resources on tiles
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        
        
        // reduce by harvested amount
        if (GameManager.Instance.Map[i,j].occupiedByPlayer)
        {
          GameManager.Instance.Map[i,j].resourceAmount -= Mathf.Min(GameManager.Instance.Map[i,j].assignedAnts * GameManager.Instance.resourceGatherRate, 
                                                                      GameManager.Instance.Map[i,j].resourceAmount); 

          // check goals
          if (!GameManager.Instance.Map[i,j].dominatedByPlayer && GameManager.Instance.Map[i,j].assignedAnts == GameManager.Instance.Map[i,j].maxAssignedAnts){
            GameManager.Instance.Map[i,j].dominatedByPlayer = true;
            GameManager.Instance.currentGoalProgress += 1;
            GameManager.Instance.mapInstance.mapMatrix[i, j].spawnOwnedFlagOnTile();
          }

        }         
        if (GameManager.Instance.Map[i,j].dominatedByPlayer && GameManager.Instance.Map[i,j].assignedAnts != GameManager.Instance.Map[i,j].maxAssignedAnts){
          GameManager.Instance.Map[i,j].dominatedByPlayer = false;
          GameManager.Instance.currentGoalProgress -= 1;
          GameManager.Instance.mapInstance.mapMatrix[i, j].deleteFlagOnTile();
          Debug.Log("Delete flag on tile: " + i + "|"  + j);
      // update visuals of grass tile
        }                                           
      }
       
    }
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
    // update population
    //Population growth
    int new_pop =  GameManager.Instance.Juniors();
    GameManager.Instance.freeAnts += new_pop;
    GameManager.Instance.totalAnts += new_pop;
    GameManager.Instance.UpdateGrowth();
    
    // ------------------ WINNING / LOSING condition and message triggers --------------------
    //Check if we reached the prototype goal
    GameManager.Instance.prototypeLooseCheck();

    if (GameManager.Instance.resources < GameManager.Instance.maxResourceStorage * 0.1)
    {
      // Trigger for low food warning.
    }
    
 
  }
  void MapTurn() 
  {
    //Insert Map Turn
    //change the tile object
    //GameManager.Instance.income = 0 - GameManager.Instance.Upkeep();
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        GameManager.Instance.Map[i,j].resourceAmount = (int) Mathf.Clamp(GameManager.Instance.Map[i,j].resourceAmount + GameManager.Instance.Map[i,j].regrowResource, 0, GameManager.Instance.Map[i,j].resourceMaxAmount);
      
        // update visuals of grass tile
        if (GameManager.Instance.Map[i, j].type == 1 || GameManager.Instance.Map[i, j].type == 2)
        {
            GameManager.Instance.mapInstance.mapMatrix[i, j].GetComponent<MapTileGeneration>().RecalculateGrassDensity(GameManager.Instance.Map[i, j].resourceAmount);
        }
        
        //check if the growth if we reached a threshhold to update the tile mesh
        GameManager.Instance.mapInstance.TileErosionCheck(i,j); // TODO: think about where to set TileErosion (ExchangeTilePrefab) function!
      }
    }
  }

  void ExploreTurn()
  {
    //TileScript[,] gameMap = gameManager.mapInstance.GameMap;
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
