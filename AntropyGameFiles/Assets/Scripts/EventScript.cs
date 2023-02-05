using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventScript : MonoBehaviour
{
  /// <summary>
  /// Current Events that are being queued up
  /// </summary>
  Queue<MessageScript.EventMessages> currentEvents = new Queue<MessageScript.EventMessages>();

  /// <summary>
  /// Flooded Tiles that are getting queued up to change back to their old state
  /// </summary>
  Queue<(GameManager.Tile, (int,int))> floodTiles = new Queue<(GameManager.Tile, (int, int))>();

  /// <summary>
  /// Ants that are lost in the heavy fog event and find home later
  /// </summary>
  Queue<int> fogEventLostAnts = new Queue<int>();

  /// <summary>
  /// Instance of the EventObject
  /// </summary>
  public EventScript EventInstance;

  private void Awake()
  {
    EventInstance = this;
  }

  /// <summary>
  /// Saves events that are getting played after showing the Messages in the Message Turn, (not implemented yet)
  /// </summary>
  /// <param name="eventMessage"></param>
  public void SaveEventMessageForEventTurn(MessageScript.EventMessages eventMessage) 
  {
    Debug.Log("EventTurnMessage added: " + eventMessage.eventName);
    currentEvents.Enqueue(eventMessage);
  }

  /// <summary>
  /// Gets called by the NextTurnScript to see what events are playing
  /// </summary>
  public void PrepareEventTurn() 
  {
    foreach (var item in currentEvents)
    {
      if (item.eventName.Equals("flood"))
      {
        FloodEvent(item);
      }
      else if (item.eventName.Equals("heavyRain"))
      {
        HeavyFogEvent(item);
      }
      else if (item.eventName.Equals("heavyFog"))
      {
        HeavyRainEvent(item);
      }
    }
  }

  /// <summary>
  /// Low Fertility map tiles get flooded for a set period of time
  /// </summary>
  /// <param name="eventMessage"></param>
  void FloodEvent(MessageScript.EventMessages eventMessage) 
  {
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        if (GameManager.Instance.Map[i, j].explored && GameManager.Instance.Map[i, j].fertilityState < 5) 
        {
          floodTiles.Enqueue((GameManager.Instance.Map[i, j],(i,j)));
          GameManager.Instance.mapInstance.UpdatePrefab(i, j, 3);
          GameManager.Instance.mapInstance.UpdatePrefabAppearance(i, j);

          if (GameManager.Instance.Map[i, j].assignedAnts > 0)
          {
            if (eventMessage.antsLost > GameManager.Instance.Map[i, j].assignedAnts)
            {
              GameManager.Instance.Map[i, j].assignedAnts = eventMessage.antsLost - GameManager.Instance.Map[i, j].assignedAnts;
              GameManager.Instance.AntDeath(GameManager.Instance.Map[i, j].assignedAnts);
              Debug.Log("Flood Death [" + i + "]" + "[" + j + "]" + ": " + GameManager.Instance.Map[i, j].assignedAnts + "Ants");
            }
            else
            {
              GameManager.Instance.Map[i, j].assignedAnts = GameManager.Instance.Map[i, j].assignedAnts - eventMessage.antsLost;
              GameManager.Instance.AntDeath(GameManager.Instance.Map[i, j].assignedAnts);
              Debug.Log("Flood Death [" + i + "]" + "[" + j + "]" + ": " + GameManager.Instance.Map[i, j].assignedAnts + "Ants");
            }
          }
        }
      }
    }
  }

  /// <summary>
  /// Undo the flood tiles
  /// </summary>
  void undoFlood() 
  {
    foreach (var item in floodTiles)
    {
      Debug.Log("Flood Restore [" + item.Item2.Item1 + "]" + "[" + item.Item2.Item2 + "]" + ": " + item.Item1.type + " Type");
      GameManager.Instance.mapInstance.UpdatePrefab(item.Item2.Item1, item.Item2.Item2, item.Item1.type);
      GameManager.Instance.mapInstance.UpdatePrefabAppearance(item.Item2.Item1, item.Item2.Item2);
    }
  }

  /// <summary>
  /// Drought event, that decreases the amount of resources and fertility on all tile
  /// </summary>
  /// <param name="eventMessage"></param>
  void DroughtEvent(MessageScript.EventMessages eventMessage)
  {
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        if (GameManager.Instance.Map[i, j].explored)
        {
          GameManager.Instance.Map[i, j].resourceAmount = (int)Mathf.Clamp(GameManager.Instance.Map[i, j].resourceAmount - (GameManager.Instance.Map[i, j].resourceAmount * eventMessage.resourceAffectionRate), 0, GameManager.Instance.Map[i, j].resourceMaxAmount);
          GameManager.Instance.Map[i, j].fertilityState -= 1;

          // update visuals of grass tile
          if (GameManager.Instance.Map[i, j].type == 1 || GameManager.Instance.Map[i, j].type == 2)
          {
            GameManager.Instance.mapInstance.mapMatrix[i, j].GetComponent<MapTileGeneration>().RecalculateGrassDensity(GameManager.Instance.Map[i, j].resourceAmount);
          }

          //check if the growth if we reached a threshhold to update the tile mesh
          GameManager.Instance.mapInstance.TileErosionCheck(i, j);
        }
      }
    }
  }

  /// <summary>
  /// Heavy Fog, lets ants disappear
  /// </summary>
  /// <param name="eventMessage"></param>
  void HeavyFogEvent(MessageScript.EventMessages eventMessage)
  {
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        if (GameManager.Instance.Map[i, j].occupiedByPlayer)
        {
          GameManager.Instance.mapInstance.UpdatePrefab(i, j, 3);
          GameManager.Instance.mapInstance.UpdatePrefabAppearance(i, j);

          if (GameManager.Instance.Map[i, j].assignedAnts > 0 && GameManager.Instance.Map[i, j].constructionState < 4)
          {
            if(eventMessage.antsLost > GameManager.Instance.Map[i, j].assignedAnts) 
            {
              GameManager.Instance.Map[i, j].assignedAnts = eventMessage.antsLost - GameManager.Instance.Map[i, j].assignedAnts;
              fogEventLostAnts.Enqueue(eventMessage.antsLost - GameManager.Instance.Map[i, j].assignedAnts);
              Debug.Log("Fog Lost [" + i + "]" + "[" + j + "]" + ": " + (eventMessage.antsLost - GameManager.Instance.Map[i, j].assignedAnts) + "Ants");
            }
            else 
            {
              GameManager.Instance.Map[i, j].assignedAnts = GameManager.Instance.Map[i, j].assignedAnts - eventMessage.antsLost;
              fogEventLostAnts.Enqueue(eventMessage.antsLost);
              Debug.Log("Fog Lost [" + i + "]" + "[" + j + "]" + ": " + eventMessage.antsLost + "Ants");
            }
          }
        }
      }
    }
  }

  /// <summary>
  /// Ants find back to the anthill
  /// </summary>
  void undFogEvent() 
  {
    int antsThatFoundHome = 0;
    foreach (var item in fogEventLostAnts)
    {
      antsThatFoundHome += item;
    }
    Debug.Log("After dense fog: " + antsThatFoundHome + "Ants found back home");
    GameManager.Instance.totalAnts += antsThatFoundHome;
  }

  /// <summary>
  /// Change of fertility? Maybe change to gentle rain
  /// </summary>
  /// <param name="eventMessage"></param>
  void HeavyRainEvent(MessageScript.EventMessages eventMessage)
  {
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        if (GameManager.Instance.Map[i, j].explored)
        {
         //
        }
      }
    }
  }
}
