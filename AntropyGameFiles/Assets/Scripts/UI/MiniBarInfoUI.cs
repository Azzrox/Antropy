using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniBarInfoUI : MonoBehaviour
{
  public TextMeshProUGUI resourcesValue;
  public TextMeshProUGUI populationValue;
  public TextMeshProUGUI season;
  public TextMeshProUGUI goal;
  public TextMeshProUGUI resourceDescription;
  public TextMeshProUGUI populationDescription;
  public GameObject resourceObject;
  public GameObject populationObject;
  public RelativeBoxUI populationFill;
  public RelativeBoxUI resourceFill;
  public RelativeBoxUI incomeIndicator;
  public RelativeBoxUI growthIndicator;


  private void Awake()
  {
   
  }

  public void MiniBarInfoUpdate()
  {
    int income = GameManager.Instance.income;
    string incomeString = income < 0 ? " (<color=red>" + income + "</color>)/" : " (+" + income + ")/";
    resourcesValue.text = GameManager.Instance.resources + incomeString + GameManager.Instance.maxResourceStorage;
    int growth = GameManager.Instance.growth;
    string growthString = growth < 0 ? " (<color=red>" + growth + "</color>)/" : " (+" + growth + ")/";
    populationValue.text = GameManager.Instance.totalAnts + growthString + GameManager.Instance.currentMaximumPopulationCapacity;

    goal.text = "Goal: " + GameManager.Instance.currentGoalProgress + "/" + GameManager.Instance.goal+ " Controlled";

    //Currently Hardgecoded for Prototype
    season.text = "Spring / " + GameManager.Instance.weatherInstance.WeatherName(GameManager.Instance.currentWeather);

    populationFill.SetLeftRight(0, GameManager.Instance.totalAnts, GameManager.Instance.currentMaximumPopulationCapacity,0);
    growthIndicator.SetLeftRight(GameManager.Instance.totalAnts, GameManager.Instance.growth, GameManager.Instance.currentMaximumPopulationCapacity, 0.17f);

    RelativeBoxUI growthTextBox = populationValue.GetComponent<RelativeBoxUI>();
    if (GameManager.Instance.totalAnts > 0.5 * GameManager.Instance.currentMaximumPopulationCapacity)
    {
      growthTextBox.SetLeftRight(0, 0.5f, 1, 0);
    }
    else{
      growthTextBox.SetLeftRight(0, 1, 1, 0);
    }


    resourceFill.SetLeftRight(0, GameManager.Instance.resources, GameManager.Instance.maxResourceStorage,0);
    incomeIndicator.SetLeftRight(GameManager.Instance.resources, GameManager.Instance.income, GameManager.Instance.maxResourceStorage, 0.17f);

    RelativeBoxUI resourceTextBox = resourcesValue.GetComponent<RelativeBoxUI>();
    if (GameManager.Instance.resources > 0.5 * GameManager.Instance.maxResourceStorage)
    {
      resourceTextBox.SetLeftRight(0, 0.5f, 1, 0);
    }
    else{
      resourceTextBox.SetLeftRight(0, 1, 1, 0);
    }
    if (GameManager.Instance.resources + GameManager.Instance.income > GameManager.Instance.maxResourceStorage)
    {
      resourceDescription.color = Color.blue;
    }
    CheckLimits(resourceDescription, GameManager.Instance.resources + GameManager.Instance.income,  GameManager.Instance.maxResourceStorage, Color.red, Color.blue);
    CheckLimits(populationDescription, GameManager.Instance.totalAnts + GameManager.Instance.growth,  GameManager.Instance.currentMaximumPopulationCapacity, Color.red, Color.red);
    //OverCapacityColourChange();
  }


  void CheckLimits(TextMeshProUGUI text, float value, float maximum, Color colMin, Color colMax)
  {
    if (value > maximum)
    {text.color = colMax;}
    else if (value < 0)
    {text.color = colMin;}
    else
    { text.color = Color.black;
    }
  }
  /// <summary>
  /// Changes the text color of the ui for the player to see
  /// </summary>
  void OverCapacityColourChange() 
  {
    if(GameManager.Instance.totalAnts  > GameManager.Instance.currentMaximumPopulationCapacity) 
    {
      populationValue.color = Color.red;

    }
    else 
    {
      populationValue.color = Color.black;
    }

    if (GameManager.Instance.resources >= GameManager.Instance.maxResourceStorage)
    {
      resourcesValue.color = Color.red;

    }
    else
    {
      resourcesValue.color = Color.black;
    }

  }
}