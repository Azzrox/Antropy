using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherScript : MonoBehaviour
{
  /// <summary>
  /// Sun, easy tile access, no bonus
  /// </summary>
  bool sun;

  /// <summary>
  /// Rain, slower tile access, major regrow bonus
  /// </summary>
  bool rain;

  /// <summary>
  /// Overcast,  normal tile access, no bonus 
  /// </summary>
  bool overcast;

  /// <summary>
  /// Fog, slower tile access,  minor regrow bonus
  /// </summary>
  bool fog;

  /// <summary>
  /// Snow, no tile access, no bonus
  /// </summary>
  bool snow;

  private GameManagerUI gameManager;

  private void Awake()
  {
    gameManager = GameObject.Find("Game Manager").GetComponent<GameManagerUI>();
    sun = false;
    rain = false;
    overcast = false;
    fog = false;
    snow = false;
  }
  /// <summary>
  /// Updates the current Weather
  /// </summary>
  /// <param name="currentSeason"> [0]Spring, [1]Summer, [2]Autumn, [3]Winter</param>
  void UpdateWeather(int currentSeason) 
  {
    /*
    switch (currentSeason)
    {
      case 0:
        break;
      default:
        break;
    }
    */
  }

}
