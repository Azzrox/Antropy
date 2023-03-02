using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NextTurnScript : MonoBehaviour
{
  public TextMeshProUGUI TurnText;

  //UI Update
  GameObject uiAssignAnts; //= GameObject.Find("AssignAnts");
  AntCounter antCounter;

  private static int previousSeason;

    private void Awake()
  {
    uiAssignAnts = GameObject.Find("AssignAnts");
    antCounter = uiAssignAnts.GetComponent<AntCounter>();
  }

  private void Start()
  {
    GameManager.Instance.nextTurnInstance = this;

    TurnInfoUpdate();
    antCounter.UpdateAntText();
    previousSeason = GameManager.Instance.currentSeason;
  }

  /// <summary>
  /// Turn Sequence, bind this to a button
  /// </summary>
  public void NextTurn() 
  {
    if(GameManager.Instance.currentTurnCount < GameManager.Instance.maxTurnCount) 
    {
      Debug.Log("Turn: " + GameManager.Instance.currentTurnCount);
      if(GameManager.Instance.currentSeason != 3) 
      {
        AntTurn();
        ExploreTurn();
        MapTurn();
        SeasonTurn();
        EventTurn();
        WeatherTurn();
        MessageTurn();
        GameManager.Instance.currentTurnCount++;
      }
      else 
      {
        winterTurnSequence();
        GameManager.Instance.prototypeGoalCheck();
      }
     
      //Update the infobars
      GameManager.Instance.adjustWeek();
      TurnInfoUpdate();
      GameManager.Instance.UpdateIncomeGrowth();
      GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
      antCounter.UpdateAntText();
      antCounter.UpdateSlider();

        if (previousSeason == GameManager.Instance.currentSeason)
        {
            GameManager.Instance.prototypeGoalCheck();
            return;
        }
            if (GameManager.Instance.currentSeason == ((int)t_season.SPRING))
            {
                Debug.Log("Playing spring music");
                GameManager.Instance.playMusic(GameManager.Instance.springMusic);
                previousSeason = (int)t_season.SPRING;
            }
            else if (GameManager.Instance.currentSeason == ((int)t_season.FALL))
            {
                Debug.Log("Playing autmn music");
                GameManager.Instance.playMusic(GameManager.Instance.autmnMusic);
                previousSeason = (int)t_season.FALL;
            }
            else if (GameManager.Instance.currentSeason == ((int)t_season.SUMMER))
            {
                Debug.Log("Playing summer music");
                GameManager.Instance.playMusic(GameManager.Instance.summerMusic);
                previousSeason = (int)t_season.SUMMER;
            }
            else
            {
                Debug.Log("Playing winter music");
                GameManager.Instance.playMusic(GameManager.Instance.winterMusic);
                previousSeason = (int)t_season.WINTER;
            }
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

    if (GameManager.Instance.resources < 0)
    {
      // Trigger for storage underflow
      // effectively stored income (needed for historic data)
      GameManager.Instance.resources = 0;
    }

    // historic data
    GameManager.Instance.totalResources += GameManager.Instance.income;

    // update left resources on tiles
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        // reduce by harvested amount
        if (GameManager.Instance.Map[i,j].occupiedByPlayer)
        {
          GameManager.Instance.messageSystemInstance.SaveTileForMessage(GameManager.Instance.Map[i, j], i, j);

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
        }  
      }
    }
  
    // update population
    //Population growth
    int new_pop =  GameManager.Instance.growth;
    
    if ( new_pop < 0)
    {
      int toBeDeleted = 0;
      for (int k = 0; k < Mathf.Abs(new_pop); k++)
      {
        toBeDeleted = 0;
        if (GameManager.Instance.freeAnts > 0)
        {
          GameManager.Instance.freeAnts--;
        }
        else
        {
          for (int i = 0; i < GameManager.Instance.columns; i++)
          {
            for (int j = 0; j < GameManager.Instance.rows; j++)
            {
             
              if ((GameManager.Instance.Map[i,j].assignedAnts > 0) && (toBeDeleted == 0))
              {
                GameManager.Instance.Map[i,j].assignedAnts--;
                GameManager.Instance.mapInstance.UpdatePrefabAppearance(i,j);
                toBeDeleted = 1;
                

              }
            }
          }
        }
      }
    }
    else{
      GameManager.Instance.freeAnts += new_pop;
    }
    GameManager.Instance.totalAnts += new_pop;


    if(GameManager.Instance.totalAnts + new_pop > GameManager.Instance.currentMaximumPopulationCapacity)
    {
      new_pop = GameManager.Instance.currentMaximumPopulationCapacity - GameManager.Instance.totalAnts;
    }
    
    GameManager.Instance.UpdateGrowth();
    
    // ------------------ WINNING / LOSING condition and message triggers --------------------
    //Check if we reached the prototype goal
    GameManager.Instance.prototypeLooseCheck();
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
        //GameManager.Instance.mapInstance.TileErosionCheck(i,j); // TODO: think about where to set TileErosion (ExchangeTilePrefab) function!
      }
    }
  }

  void WeatherTurn()
  {
    GameManager.Instance.weatherInstance.UpdateWeather(GameManager.Instance.currentSeason);
  }

  void EventTurn() 
  {
    GameManager.Instance.eventInstance.PrepareEventTurn();
  }

  void MessageTurn() 
  {
    GameManager.Instance.messageSystemInstance.PrepareRoundMessages(); 
  }

  void SeasonTurn() 
  {
    GameManager.Instance.checkSeasonChange();
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

  /// <summary>
  /// Starts the Winter Season
  /// </summary>
  void winterTurnSequence() 
  {
    //Disable systems and antgrowth
    GameManager.Instance.messageSystemInstance.disableMessageSystem();
    GameManager.Instance.growth = 0;
    GameManager.Instance.freeAnts = 0;

    //Activate the interface
    GameManager.Instance.winterCountDownInstance.gameObject.SetActive(true);

    //Disable the Interface
    GameManager.Instance.miniBarInfoInstance.gameObject.SetActive(false);
    GameObject.Find("NextTurnButton").SetActive(false);
    GameObject.Find("TurnText").SetActive(false);

    //Start the automated WinterTurn
    StartCoroutine(winterTurn(1));
    
  }

  /// <summary>
  /// Automated Winter Turn Sequence (Stops for X seconds each turn)
  /// </summary>
  /// <returns></returns>
  public IEnumerator winterTurn(float time)
  {
    WaitForSeconds wait = new WaitForSeconds(time);
    for (int i = GameManager.Instance.currentTurnCount; i <= GameManager.Instance.maxTurnCount && !GameManager.Instance.gameOverWinter; i++) 
    {
      GameManager.Instance.winterCountDownInstance.WinterCountdownUpdate();
      GameManager.Instance.prototypeLooseCheck();
      GameManager.Instance.prototypeGoalCheck();
      GameManager.Instance.adjustWeek();
      GameManager.Instance.WinterAntTurn();
      EventTurn();
      WeatherTurn();
      MessageTurn();
      GameManager.Instance.currentTurnCount++;
      yield return wait;
    }
  }
}
