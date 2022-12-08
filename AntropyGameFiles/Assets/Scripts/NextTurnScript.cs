using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextTurnScript : MonoBehaviour
{
  
  public float ant_growth_rate;
  public float tile_resource_rate;
  public float event_rate;
  public float weather_rate;
  MapScript map_script;



  private void Awake()
  {
    map_script = GetComponent<MapScript>();
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
