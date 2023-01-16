using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniBarInfoUI : MonoBehaviour
{
  public TextMeshProUGUI resources;
  public TextMeshProUGUI population;
  public TextMeshProUGUI season;
  public TextMeshProUGUI goal;


  private void Awake()
  {
   
  }

  public void MiniBarInfoUpdate()
  {
    int income = GameManager.Instance.income;
    string incomeString = income < 0 ? " (<color=red>" + income + "</color>)/" : " (+" + income + ")/";
    resources.text = GameManager.Instance.resources + incomeString + GameManager.Instance.maxResourceStorage;
    int growth = GameManager.Instance.growth;
    string growthString = growth < 0 ? " (<color=red>" + growth + "</color>)/" : " (+" + growth + ")/";
    population.text = GameManager.Instance.totalAnts + growthString + GameManager.Instance.currentMaximumPopulationCapacity;

    goal.text = "Goal: " + GameManager.Instance.currentGoalProgress + "/" + GameManager.Instance.goal+ " Controlled";

    //Currently Hardgecoded for Prototype
    season.text = "Spring / " + GameManager.Instance.weatherInstance.WeatherName(GameManager.Instance.currentWeather);

    SliderScript populationSlider = GameObject.Find("Population").GetComponent<SliderScript>();
    populationSlider.SetMaxValue(GameManager.Instance.currentMaximumPopulationCapacity);
    populationSlider.SetValue(GameManager.Instance.totalAnts);
    RelativeBoxUI growthIndicator = GameObject.Find("Population").GetComponent<RelativeBoxUI>();
    growthIndicator.SetLeftRight(GameManager.Instance.totalAnts, GameManager.Instance.growth, GameManager.Instance.currentMaximumPopulationCapacity);

    SliderScript resourceSlider = GameObject.Find("Resource").GetComponent<SliderScript>();
    resourceSlider.SetMaxValue(GameManager.Instance.maxResourceStorage);
    resourceSlider.SetValue(GameManager.Instance.resources);
    RelativeBoxUI incomeIndicator = GameObject.Find("Resource").GetComponent<RelativeBoxUI>();
    incomeIndicator.SetLeftRight(GameManager.Instance.resources, GameManager.Instance.income, GameManager.Instance.maxResourceStorage);

    OverCapacityColourChange();
  }

  /// <summary>
  /// Changes the text color of the ui for the player to see
  /// </summary>
  void OverCapacityColourChange() 
  {
    if(GameManager.Instance.totalAnts > GameManager.Instance.currentMaximumPopulationCapacity) 
    {
      population.color = Color.red;

    }
    else 
    {
      population.color = Color.black;
    }

    if (GameManager.Instance.resources >= GameManager.Instance.maxResourceStorage)
    {
      resources.color = Color.red;

    }
    else
    {
      resources.color = Color.black;
    }

  }
}