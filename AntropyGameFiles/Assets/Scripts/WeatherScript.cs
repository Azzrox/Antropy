using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherScript : MonoBehaviour
{
  /// <summary>
  /// EventScript fills the next weather into the queue
  /// </summary>
  public Queue<int> nextWeatherQueue = new Queue<int>();


  /// [0]Spring, [1]Summer, [2]Autumn, [3]Winter
  /// [0]sun, [1]rain, [2]overcast, [3]fog, [4] snow
  
  private void Start(){
    GameManager.Instance.weatherInstance = this;
  }

  /// <summary>
  /// Queues the weather for the weatherTurn
  /// </summary>
  /// <param name="eventWeather"></param>
  /// <param name="turns"></param>
  public void QueueWeatherWeatherTurn(int eventWeather, int turns)
  {
    Debug.Log("Weather added: " +  eventWeather + "Turns: " + turns);
    for (int i = 0; i < turns; i++)
    {
      nextWeatherQueue.Enqueue(eventWeather);
    }
  }

  /// <summary>
  /// Updates the current Weather
  /// </summary>
  /// <param name="currentSeason"> [0]Spring, [1]Summer, [2]Autumn, [3]Winter</param>
  public void UpdateWeather(int currentSeason) 
   {
    if(nextWeatherQueue.Count == 0) 
    {
      Debug.Log("Event Script has not added new weather! This message should only show if the EventSystem is disabled!");
      WeatherEffectsUpdate(currentSeason, GameManager.Instance.currentWeather);
    }
    else 
    {
      GameManager.Instance.currentWeather = nextWeatherQueue.Peek();
      WeatherEffectsUpdate(currentSeason, GameManager.Instance.currentWeather);
      nextWeatherQueue.Dequeue();
    }
  }

    /// <summary>
    /// Updates the particle systems in the world
    /// </summary>
    /// <param name="weather"></param>
    private void WeatherEffectsUpdate(int currentSeason, int weather)
    {
        bool rain = false;
        bool snow = false;
        bool leaves = false;
        bool lightning = false;

        if (GameManager.Instance.showWeatherEffects)
        {
            switch (weather)
            {
                case 0: //Sun
                    if (currentSeason != 3) //no leaves in the winter
                    {
                        leaves = true;
                    }
                    break;
                case 1: //Rain
                    rain = true;
                    lightning = true;
                    break;
                case 2: //Overcast
                    break;
                case 3: //Fog
                    break;
                case 4: //Snow
                    snow = true;
                    break;
                default:
                    Debug.Log("Error with updating weather particle systems");
                    break;
            }
        }

        foreach (GameObject wEffect in GameObject.FindGameObjectsWithTag("WeatherEffect"))
        {
            var em = wEffect.GetComponent<ParticleSystem>().emission;
                switch (wEffect.name)
                {
                    case "Autumn Leaves":
                        em.enabled = leaves && (currentSeason == 2);
                        break;
                    case "Lush Leaves":
                        em.enabled = leaves && (currentSeason <= 1);
                        break;
                    case "Rain":
                        em.enabled = rain;
                        break;
                    case "Snow":
                        em.enabled = snow;
                        break;
                    case "Lightning":
                        em.enabled = lightning;
                        break;
            }
        }
    }

  /// <summary>
  /// Updates the current active multiplier
  /// </summary>
  /// <param name="weather"></param>
  public void WeatherMultiplierUpdate(int weather)
  {
    if (weather == 0)
    {
      GameManager.Instance.weatherAcessMultiplier = GameManager.Instance.sunAccess;
      GameManager.Instance.weatherRegrowMultiplier = GameManager.Instance.sunRegrow;
    }
    else if (weather == 1)
    {
      GameManager.Instance.weatherAcessMultiplier = GameManager.Instance.rainAccess;
      GameManager.Instance.weatherRegrowMultiplier = GameManager.Instance.rainRegrow;
    }
    else if (weather == 2)
    {
      GameManager.Instance.weatherAcessMultiplier = GameManager.Instance.overcastAccess;
      GameManager.Instance.weatherRegrowMultiplier = GameManager.Instance.overcastRegrow;
    }
    else if (weather == 3)
    {
      GameManager.Instance.weatherAcessMultiplier = GameManager.Instance.fogAccess;
      GameManager.Instance.weatherRegrowMultiplier = GameManager.Instance.fogRegrow;
    }
    else if (weather == 4)
    {
      GameManager.Instance.weatherAcessMultiplier = GameManager.Instance.snowAccess;
      GameManager.Instance.weatherRegrowMultiplier = GameManager.Instance.snowRegrow;
    }
  }


}
