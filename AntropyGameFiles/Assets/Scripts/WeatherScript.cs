using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherScript : MonoBehaviour
{
  /// [0]Spring, [1]Summer, [2]Autumn, [3]Winter
  /// [0]sun, [1]rain, [2]overcast, [3]fog, [4] snow


  private void Awake()
  {
  }

  private void Start(){
    GameManager.Instance.weatherInstance = this;
  }
    /// <summary>
    /// Updates the current Weather
    /// </summary>
    /// <param name="currentSeason"> [0]Spring, [1]Summer, [2]Autumn, [3]Winter</param>
   public void UpdateWeather(int currentSeason) 
   {
    int weatherChance = Random.Range(0, 101);
    switch (currentSeason)
     {
        case 0:
          //higher probability of rain and sun
          //Basic Implementation for the Prototype
          //higher probability of rain < overcast < sun
          if (weatherChance <= 50)
          {
            GameManager.Instance.currentWeather = 1;
          }
          else if (weatherChance > 50 && weatherChance <= 70)
          {
            GameManager.Instance.currentWeather = 2;
          }
          else
          {
            GameManager.Instance.currentWeather = 0;
          }
          WeatherEffectsUpdate(currentSeason, GameManager.Instance.currentWeather);
          break;

        case 1:
          //higher probability of sun < overcast < rain
          if (weatherChance <= 20)
          {
            GameManager.Instance.currentWeather = 1;
          }
          else if (weatherChance > 20 && weatherChance <= 40)
          {
            GameManager.Instance.currentWeather = 2;
          }
          else
          {
            GameManager.Instance.currentWeather = 0;
          }
          WeatherEffectsUpdate(currentSeason, GameManager.Instance.currentWeather);
          break;

        case 2:
          //higher probability of rain < fog < sun
          if (weatherChance <= 20)
          {
            GameManager.Instance.currentWeather = 0;
          }
          else if (weatherChance > 20 && weatherChance <= 30)
          {
            GameManager.Instance.currentWeather = 3;
          }
          else if (weatherChance > 30 && weatherChance <= 40)
          {
            GameManager.Instance.currentWeather = 3;
          }
          else
          {
          GameManager.Instance.currentWeather = 3;
          }
          WeatherEffectsUpdate(currentSeason, GameManager.Instance.currentWeather);
          break;

        case 3:
          //DEATH
          GameManager.Instance.currentWeather = 4;
          WeatherEffectsUpdate(currentSeason, GameManager.Instance.currentWeather);
          break;

        default:
          Debug.Log("Error with season assignment in  While Updating");
          break;
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
