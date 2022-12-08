using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextTurnScript : MonoBehaviour
{
  
  public float ant_growth_rate;
  public float tile_resource_rate;
  public float event_rate;
  public float weather_rate;
  GameResourceScript game_resources;


  private void Awake()
  {
    game_resources = GameResourceScript.ResourceInstance;
  }

  void NextTurn() 
  {
    AntTurn();
    MapTurn();
    WeatherTurn();
    EventTurn();
    MessageTurn();
  }

  void AntTurn() 
  { 
  
  }

  void MapTurn() 
  {
    //change the tile object
    TileScript[,] game_map = game_resources.MapInstance.GameMap;

    for (int i = 0; i < game_resources.MapInstance.rows; i++)
    {
      for (int j = 0; j < game_resources.MapInstance.columns; j++)
      {
        game_map[i, j].CalculateNewResourceAmount(tile_resource_rate);
      }
    }
  }

  void WeatherTurn()
  {

  }

  void EventTurn() 
  {
    
  
  }

  void MessageTurn() 
  { 
  
  }
}
