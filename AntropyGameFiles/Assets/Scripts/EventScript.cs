using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Assertions;

public class EventScript : MonoBehaviour
{
  /// <summary>
  /// Current Events that are being queued up
  /// </summary>
  Queue<(string, int)> currentEvents = new Queue<(string, int)>();

  /// <summary>
  /// Flooded Tiles that are getting queued up to change back to their old state
  /// </summary>
  Queue<(GameManager.Tile, (int,int))> floodTiles = new Queue<(GameManager.Tile, (int, int))>();

  /// <summary>
  /// Ants that are lost in the heavy fog event and find home later
  /// </summary>
  Queue<int> fogEventLostAnts = new Queue<int>();

  /// <summary>
  /// Copy from GameManager for undoing events
  /// </summary>
  float oldGatherRate;

  /// <summary>
  /// Ants that got (lost, drowned, .....) in events
  /// </summary>
  public int antsEventValue;

  private void Start()
  {
    GameManager.Instance.eventInstance = this;
    oldGatherRate = GameManager.Instance.resourceGatherRate;
  }

  /// <summary>
  /// Prepares the EventTurn, this function is called by the NextTurnScript
  /// </summary>
  public void PrepareEventTurn() 
  {
    RemoveEventMessageTurn();
    ChooseEvent();
  }

  /// <summary>
  /// Chooses a random event
  /// </summary>
  private void ChooseEvent() 
  { 
    //check if weatherQueue is still active or an event is still queued
    if(GameManager.Instance.weatherInstance.nextWeatherQueue.Count == 0 && currentEvents.Count == 0) 
    {
      int eventChance = Random.Range(0, 101);
      switch (GameManager.Instance.currentSeason)
      {
        case 0:   
          SpringEvents(eventChance);
          break;

        case 1:
          SummerEvents(eventChance);
          break;

        case 2:
          AutumnEvents(eventChance);
          break;

        case 3:
          WinterEvents(eventChance);
          break;

        default:
          Debug.Log("Error with season assignment in  While Updating");
          break;
      }
    }
  }

  /// <summary>
  /// Possible spring events
  /// </summary>
  /// <param name="eventChance"></param>
  private void SpringEvents(int eventChance) 
  {
    //higher probability of rain < overcast < sun
    if (eventChance <= 50)
    {
      int intensityChance = Random.Range(0, 101);
      if (intensityChance <= 30)
      {
        HeavyRainEvent();
      }
      else if (intensityChance <= 50)
      {
        FloodEvent();
      }
      else
      {
        LightRainEvent();
      }
    }
    else if (eventChance > 50 && eventChance <= 70)
    {
      OvercastDaysEvent();
    }
    else
    {
      ClearDaysEvent();
    }
  }

  /// <summary>
  /// Possible summer events
  /// </summary>
  /// <param name="eventChance"></param>
  private void SummerEvents(int eventChance) 
  {
    //higher probability of sun < overcast < rain
    if (eventChance <= 20)
    {
      int intensityChance = Random.Range(0, 101);
      if (intensityChance <= 30)
      {
        HeavyRainEvent();
      }
      else if (intensityChance <= 50)
      {
        FloodEvent();
      }
      else
      {
        LightRainEvent();
      }
    }
    else if (eventChance > 20 && eventChance <= 40)
    {
      OvercastDaysEvent();
    }
    else
    {
      int intensityChance = Random.Range(0, 101);
      if (intensityChance <= 30)
      {
        DroughtEvent();
      }
      else 
      {
        ClearDaysEvent();
      }
    }
  }

  /// <summary>
  /// Possible autumn events
  /// </summary>
  /// <param name="eventChance"></param>
  private void AutumnEvents(int eventChance) 
  {
    //higher probability of rain < fog < sun
    if (eventChance <= 20)
    {
      ClearDaysEvent();
    }
    else if (eventChance > 20 && eventChance <= 30)
    {
      int intensityChance = Random.Range(0, 101);
      if (intensityChance <= 30)
      {
        HeavyFogEvent();
      }
      else
      {
        LightFogEvent();
      }
    }
    else if (eventChance > 30 && eventChance <= 40)
    {
      OvercastDaysEvent();
    }
    else
    {
      int intensityChance = Random.Range(0, 101);
      if (intensityChance <= 30)
      {
        HeavyRainEvent();
      }
      else if (intensityChance <= 50)
      {
        FloodEvent();
      }
      else
      {
        LightRainEvent();
      }
    }
  }

  /// <summary>
  /// Possible winter events
  /// </summary>
  /// <param name="eventChance"></param>
  private void WinterEvents(int eventChance) 
  {
    //TODO Implement me
    ClearDaysEvent();
  }

  /// <summary>
  /// Saves what event needs to be played at the message turn
  /// </summary>
  /// <param name="eventMessage"></param>
  public void SaveEventMessageTurn(string eventMessage, int turns) 
  {
    Debug.Log("EventTurnMessage added: " + eventMessage);
    currentEvents.Enqueue((eventMessage, turns));
  }

  /// <summary>
  /// Removes an event that gets checked by the message turn
  /// </summary>
  public void RemoveEventMessageTurn()
  {
    if(currentEvents.Count > 0 && currentEvents.Peek().Item2 <= 1) 
    {
      //Pick counter event if needed
      PickCounterEvent();
      currentEvents.Dequeue();
    }
    else if(currentEvents.Count > 0)
    {
      //Remove one Turn from the event
      currentEvents.Enqueue((currentEvents.Peek().Item1, currentEvents.Peek().Item2 - 1));
      currentEvents.Dequeue();
    }
    else 
    {
      //Queue Empty
      return;
    }
  }

  /// <summary>
  /// getter GetEventMessageTurn Message Queue
  /// </summary>
  public Queue<(string, int)> GetEventMessageTurn
  {
    get
    {
      return currentEvents;
    }
  }

  /// <summary>
  /// Adds the weather and an event message to be displayed
  /// </summary>
  /// <param name="evenName"></param>
  /// <param name="weather"></param>
  /// <param name="turns"></param>
  private void AddToQueues(string evenName, int weather, int turns) 
  {
    //Add to weather Queue
    GameManager.Instance.weatherInstance.QueueWeatherWeatherTurn(weather, turns);

    //Add to message Queue
    SaveEventMessageTurn(evenName, turns);
  }

  private void PickCounterEvent() 
  {
    if (currentEvents.Peek().Item1.Equals("flood")) 
    {
      UndoFlood();
    }
    else if (currentEvents.Peek().Item1.Equals("heavyFog") || currentEvents.Peek().Item1.Equals("lightFog")) 
    {
      UndoFogEvent();
    }
    else if (currentEvents.Peek().Item1.Equals("heavyRain"))
    {
      UndoHeavyRainEvent();
    }
  }

  /// <summary>
  /// Low Fertility map tiles get flooded for a set period of time
  /// </summary>
  /// <param name="eventMessage"></param>
  private void FloodEvent() 
  {
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        if (GameManager.Instance.Map[i, j].explored && GameManager.Instance.Map[i, j].fertilityState < GameManager.Instance.floodFertilityThreshhold 
            && GameManager.Instance.Map[i, j].partOfAnthill == false && GameManager.Instance.Map[i, j].type != 3 && GameManager.Instance.Map[i, j].type != 0) 
        {
          floodTiles.Enqueue((GameManager.Instance.Map[i, j],(i,j)));
          if(GameManager.Instance.Map[i, j].assignedAnts > 0)
          {
            if(GameManager.Instance.Map[i, j].assignedAnts > 5)
            {
              int ants_drowned = (int)(GameManager.Instance.Map[i, j].assignedAnts * GameManager.Instance.antsLostFloodpercentage);
              GameManager.Instance.Map[i, j].assignedAnts -= ants_drowned;
              GameManager.Instance.AntDeath(ants_drowned);
              antsEventValue += ants_drowned;


              Debug.Log("Flood Death [" + i + "]" + "[" + j + "]" + ": " + GameManager.Instance.Map[i, j].assignedAnts + "Ants");
            }
            else 
            {
              //don't remove ants, because you could softlock yourself
              Debug.Log("No Flood Death [" + i + "]" + "[" + j + "]" + ": " + GameManager.Instance.Map[i, j].assignedAnts + "Ants");
            }
          }
          GameManager.Instance.Map[i, j].type = 3;
          GameManager.Instance.mapInstance.UpdatePrefab(i, j, 3);
          GameManager.Instance.mapInstance.UpdatePrefabAppearance(i, j);
        }
      }
    }
    AddToQueues("flood", 1, Random.Range(1, 3));
    GameManager.Instance.UpdateIncomeGrowth();
    GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
  }

  /// <summary>
  /// Undo the flood tiles
  /// </summary>
  private void UndoFlood() 
  {
    foreach (var item in floodTiles)
    {
      Debug.Log("Flood Restore [" + item.Item2.Item1 + "]" + "[" + item.Item2.Item2 + "]" + ": " + item.Item1.type + " Type");
      GameManager.Instance.Map[item.Item2.Item1, item.Item2.Item2].type = item.Item1.type;
      GameManager.Instance.mapInstance.UpdatePrefab(item.Item2.Item1, item.Item2.Item2, item.Item1.type);
      GameManager.Instance.mapInstance.UpdatePrefabAppearance(item.Item2.Item1, item.Item2.Item2);
    }
    antsEventValue = 0;
    ClearDaysEvent();
  }

  /// <summary>
  /// Drought event, that decreases the amount of resources and fertility on all tile
  /// </summary>
  /// <param name="eventMessage"></param>
  private void DroughtEvent()
  {

    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
          GameManager.Instance.Map[i, j].resourceAmount = (int)Mathf.Clamp(GameManager.Instance.Map[i, j].resourceAmount - (GameManager.Instance.Map[i, j].resourceAmount * GameManager.Instance.droughtResourceAffectionRate), 0, GameManager.Instance.Map[i, j].resourceMaxAmount);
        
          GameManager.Instance.Map[i, j].fertilityState = (int)Mathf.Clamp(GameManager.Instance.Map[i, j].fertilityState - 1, 0, 6);
          GameManager.Instance.Map[i, j].regrowResource = GameManager.Instance.regrowRateVector[GameManager.Instance.Map[i, j].fertilityState];
          

          // update visuals of grass tile
          if (GameManager.Instance.Map[i, j].type == 1 || GameManager.Instance.Map[i, j].type == 2)
          {
            GameManager.Instance.mapInstance.mapMatrix[i, j].GetComponent<MapTileGeneration>().RecalculateGrassDensity(GameManager.Instance.Map[i, j].resourceAmount);
            GameManager.Instance.mapInstance.UpgradeFertilityColor(i, j, GameManager.Instance.Map[i, j].fertilityState);
          }
        GameManager.Instance.mapInstance.TileErosionCheck(i, j);
        GameManager.Instance.mapInstance.UpdatePrefabAppearance(i, j);
        //UpdateAntText

      }
    }
    GameManager.Instance.UpdateIncomeGrowth();
    GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
    AddToQueues("drought", 0, Random.Range(1, 3));
  }

  /// <summary>
  /// Heavy Fog, lets loads of ants disappear
  /// </summary>
  /// <param name="eventMessage"></param>
  private void HeavyFogEvent()
  {
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        if (GameManager.Instance.Map[i, j].occupiedByPlayer)
        {
          if (GameManager.Instance.Map[i, j].assignedAnts > 0 && GameManager.Instance.Map[i, j].constructionState < 4)
          {
            if (GameManager.Instance.heavyFogAntsLostAmount > GameManager.Instance.Map[i, j].assignedAnts)
            {
              int lost_ants  = GameManager.Instance.heavyFogAntsLostAmount - GameManager.Instance.Map[i, j].assignedAnts;
              
              GameManager.Instance.Map[i, j].assignedAnts -= lost_ants;
              GameManager.Instance.totalAnts -= lost_ants;
              fogEventLostAnts.Enqueue(lost_ants);
              antsEventValue += lost_ants;
              Debug.Log("Fog Lost [" + i + "]" + "[" + j + "]" + ": " + (GameManager.Instance.heavyFogAntsLostAmount - GameManager.Instance.Map[i, j].assignedAnts) + "Ants");
            }
            else
            {
              int lost_ants = GameManager.Instance.Map[i, j].assignedAnts - GameManager.Instance.heavyFogAntsLostAmount;
              GameManager.Instance.Map[i, j].assignedAnts -= lost_ants;
              GameManager.Instance.totalAnts -= lost_ants;
              fogEventLostAnts.Enqueue(lost_ants);
              antsEventValue += lost_ants;
              Debug.Log("Fog Lost [" + i + "]" + "[" + j + "]" + ": " + GameManager.Instance.heavyFogAntsLostAmount + "Ants");
            }
          }
        }
        //Fog effect?
        //GameManager.Instance.mapInstance.UpdatePrefab(i, j, 3);
        //GameManager.Instance.mapInstance.UpdatePrefabAppearance(i, j);
      }
    }
    AddToQueues("heavyFog", 3, Random.Range(1, 3));
    GameManager.Instance.UpdateIncomeGrowth();
    GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
  }

  /// <summary>
  /// light Fog, lets minimal ants disappear
  /// </summary>
  /// <param name="eventMessage"></param>
  private void LightFogEvent()
  {
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        if (GameManager.Instance.Map[i, j].occupiedByPlayer)
        {
          if (GameManager.Instance.Map[i, j].assignedAnts > 0 && GameManager.Instance.Map[i, j].constructionState < 4)
          {
            if (GameManager.Instance.lightFogAntsLostAmount > GameManager.Instance.Map[i, j].assignedAnts)
            {
              int lost_ants = GameManager.Instance.lightFogAntsLostAmount - GameManager.Instance.Map[i, j].assignedAnts;
              GameManager.Instance.Map[i, j].assignedAnts -= lost_ants;
              GameManager.Instance.totalAnts -= lost_ants;
              fogEventLostAnts.Enqueue(lost_ants);
              antsEventValue += lost_ants;
              Debug.Log("Fog Lost [" + i + "]" + "[" + j + "]" + ": " + (GameManager.Instance.lightFogAntsLostAmount - GameManager.Instance.Map[i, j].assignedAnts) + "Ants");
            }
            else
            {
              int lost_ants = GameManager.Instance.Map[i, j].assignedAnts - GameManager.Instance.lightFogAntsLostAmount;
              GameManager.Instance.Map[i, j].assignedAnts -= lost_ants;
              GameManager.Instance.totalAnts -= lost_ants;
              fogEventLostAnts.Enqueue(lost_ants);
              antsEventValue += lost_ants;
              Debug.Log("Fog Lost [" + i + "]" + "[" + j + "]" + ": " + GameManager.Instance.lightFogAntsLostAmount + "Ants");
            }
          }
        }
        //Fog effect?
        //GameManager.Instance.mapInstance.UpdatePrefab(i, j, 3);
        //GameManager.Instance.mapInstance.UpdatePrefabAppearance(i, j);
      }
    }
    AddToQueues("lightFog", 3, Random.Range(1, 3));
    GameManager.Instance.UpdateIncomeGrowth();
    GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
  }

  /// <summary>
  /// Ants find back to the anthill
  /// </summary>
  private void UndoFogEvent() 
  {
    int antsThatFoundHome = 0;
    foreach (var item in fogEventLostAnts)
    {
      antsThatFoundHome += item;
    }
    Debug.Log("After dense fog: " + antsThatFoundHome + "Ants found back home");
    GameManager.Instance.totalAnts += antsThatFoundHome;
    GameManager.Instance.freeAnts += antsThatFoundHome;
    GameManager.Instance.UpdateIncomeGrowth();
    GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();

    //Assert.IsTrue(antsThatFoundHome == antsEventValue);
    antsEventValue = 0;
    //Default back to sun
    ClearDaysEvent();
  }

  /// <summary>
  /// Change of fertility? Maybe change to gentle rain
  /// </summary>
  /// <param name="eventMessage"></param>
  private void HeavyRainEvent()
  {
    GameManager.Instance.resourceGatherRate -= 2;
    AddToQueues("heavyRain", 1, Random.Range(1, 4));
    GameManager.Instance.UpdateIncomeGrowth();
    GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
  }

  /// <summary>
  /// Change GatherRate back to old
  /// </summary>
  /// <param name="eventMessage"></param>
  private void UndoHeavyRainEvent()
  {
    GameManager.Instance.resourceGatherRate = oldGatherRate;
    //default to clear days
    ClearDaysEvent();
  }

  /// <summary>
  /// Light Rain, adds +1 fertility to each tile (except, Anthill, Stone, Water)
  /// </summary>
  private void LightRainEvent() 
  {
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        if(GameManager.Instance.Map[i, j].type != 3 && GameManager.Instance.Map[i, j].type != 4 && GameManager.Instance.Map[i, j].type != 0) 
        {
          //Increase Fertility for all
          if(GameManager.Instance.Map[i, j].fertilityState <= 6) 
          {
            GameManager.Instance.Map[i, j].fertilityState += 1;
            GameManager.Instance.Map[i, j].regrowResource = GameManager.Instance.regrowRateVector[GameManager.Instance.Map[i, j].fertilityState];
            GameManager.Instance.mapInstance.UpgradeFertilityColor(i, j, GameManager.Instance.Map[i, j].fertilityState);
          }
        }
      }
    }
    AddToQueues("lightRain", 1, Random.Range(1, 3));
    GameManager.Instance.UpdateIncomeGrowth();
    GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
  }

  /// <summary>
  /// clear Days
  /// </summary>
  private void ClearDaysEvent() 
  {
    AddToQueues("clearDays", 0, Random.Range(1, 4));
  }

  /// <summary>
  /// overcast Days 
  /// </summary>
  private void OvercastDaysEvent() 
  {
    AddToQueues("overcastDays", 2, Random.Range(1, 4));
  }
}
