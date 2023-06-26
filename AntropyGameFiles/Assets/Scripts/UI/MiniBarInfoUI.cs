using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(100)]
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
  public RelativeBoxUI finalPopulationIndicator;
  public RelativeBoxUI finalResourceIndicator;
  public TextMeshProUGUI eventPlaying;


  private void Awake()
  {
   
  }
  private void Start() {
    MiniBarInfoUpdate();
    Debug.Log("add miniBarInfoInstance to GameManager");
    GameManager.Instance.miniBarInfoInstance = this;
  }

  
  /// <summary>
  /// Returns the type integer to string name
  /// </summary>
  /// <param name="weatherType">[0]sun, [1]rain, [2]overcast, [3]fog, [4] snow</param>
  /// <returns></returns>
  public string WeatherName(int weatherType) 
  {
    switch (weatherType)
    {
      case 0:
        return "Sun";
      case 1:
        return "Rain";
      case 2:
        return "Overcast";
      case 3:
        return "Fog";
      case 4:
        return "Snow";

      default:
        return "InvalidWeather";
    }
  }

  public void MiniBarInfoUpdate()
  {
    //Debug.Log("Function: update of minibar Info: " + GameManager.Instance.income);
    int income = GameManager.Instance.income;
    string incomeString = income < 0 ? " (<color=red>" + income + "</color>)/" : " (+" + income + ")/";
    resourcesValue.text = GameManager.Instance.resources + incomeString + GameManager.Instance.maxResourceStorage;
    int growth = GameManager.Instance.growth;
    string growthString = growth < 0 ? " (<color=red>" + growth + "</color>)/" : " (+" + growth + ")/";
    populationValue.text = GameManager.Instance.totalAnts + growthString + GameManager.Instance.currentMaximumPopulationCapacity;

    goal.text = "Goal: " + GameManager.Instance.currentGoalProgress + "/" + GameManager.Instance.goal+ " Controlled";

    season.text = GameManager.Instance.SeasonName(GameManager.Instance.currentSeason) + " / " + WeatherName(GameManager.Instance.currentWeather);

    int intermediate_growth = GameManager.Instance.growth;
    if(intermediate_growth < 0)
    {
      growthIndicator.indicator.GetComponent<Image>().color = new Color32(255,0,0,170); 
    }
    else
    {
      growthIndicator.indicator.GetComponent<Image>().color = new Color32(0,74,255,255); 
    }
    //else if (intermediate_growth + GameManager.Instance.totalAnts > GameManager.Instance.currentMaximumPopulationCapacity)
    //{
    //  intermediate_growth = GameManager.Instance.currentMaximumPopulationCapacity - GameManager.Instance.totalAnts;
    //}
    populationFill.SetLeftRight(0, GameManager.Instance.totalAnts, GameManager.Instance.currentMaximumPopulationCapacity,0);
    growthIndicator.SetLeftRight(GameManager.Instance.totalAnts, intermediate_growth, GameManager.Instance.currentMaximumPopulationCapacity, 0.17f);

    RelativeBoxUI growthTextBox = populationValue.GetComponent<RelativeBoxUI>();
    if (GameManager.Instance.totalAnts > 0.5 * GameManager.Instance.currentMaximumPopulationCapacity)
    {
      growthTextBox.SetLeftRight(0, 0.5f, 1, 0);
      populationValue.horizontalAlignment = HorizontalAlignmentOptions.Left;
    }
    else{
      growthTextBox.SetLeftRight(0, 1, 1, 0);
      populationValue.horizontalAlignment =  HorizontalAlignmentOptions.Right;

    }


    resourceFill.SetLeftRight(0, GameManager.Instance.resources, GameManager.Instance.maxResourceStorage,0);
    int intermediate_income = GameManager.Instance.income;
    if(intermediate_income < 0)
    {
      incomeIndicator.indicator.GetComponent<Image>().color = new Color32(89,0,19,190); 
    }
    else{
      incomeIndicator.indicator.GetComponent<Image>().color = new Color32(0,89,6,255); 
    }
    //else if (intermediate_income + GameManager.Instance.resources > GameManager.Instance.maxResourceStorage)
    //{
    //  intermediate_income = GameManager.Instance.maxResourceStorage - GameManager.Instance.resources;
    //}
    incomeIndicator.SetLeftRight(GameManager.Instance.resources, intermediate_income, GameManager.Instance.maxResourceStorage, 0.17f);

    RelativeBoxUI resourceTextBox = resourcesValue.GetComponent<RelativeBoxUI>();
    if (GameManager.Instance.resources > 0.5 * GameManager.Instance.maxResourceStorage)
    {
      resourceTextBox.SetLeftRight(0, 0.5f, 1, 0);
      resourcesValue.horizontalAlignment = HorizontalAlignmentOptions.Left;
    }
    else{
      resourceTextBox.SetLeftRight(0, 1, 1, 0);
      resourcesValue.horizontalAlignment =  HorizontalAlignmentOptions.Right;
    }
    if (GameManager.Instance.resources + GameManager.Instance.income > GameManager.Instance.maxResourceStorage)
    {
      resourceDescription.color = Color.blue;
    }

    // final population and resource indicators
    float popEstimate = 30 * GameManager.Instance.antWinterDeathRate;
    finalPopulationIndicator.SetLeftRight(0.0f, Mathf.Min((float)GameManager.Instance.currentMaximumPopulationCapacity, popEstimate), (float) GameManager.Instance.currentMaximumPopulationCapacity, 0.0f);
    float resEstimate =GameManager.Instance.totalAnts * 25 * GameManager.Instance.winterFoodUse;
    finalResourceIndicator.SetLeftRight(0.0f, Mathf.Min(GameManager.Instance.maxResourceStorage, resEstimate), GameManager.Instance.maxResourceStorage, 0);


    CheckLimits(resourceDescription, GameManager.Instance.resources + GameManager.Instance.income,  GameManager.Instance.maxResourceStorage, Color.red, Color.blue);
    CheckLimits(populationDescription, GameManager.Instance.totalAnts + GameManager.Instance.growth,  GameManager.Instance.currentMaximumPopulationCapacity, Color.red, Color.red);
    //OverCapacityColourChange();
    getEventData();
  }

  void CheckLimits(TextMeshProUGUI text, float value, float maximum, Color colMin, Color colMax)
  {
    if (value > maximum)
    {text.color = colMax;}
    else if (value <= 0)
    { text.color = colMin;}
    else
    { text.color = Color.black;
    }
  }

  void getEventData() 
  {
    if(GameManager.Instance.messageSystemInstance.currentEventMessageQueue.Count > 0) 
    {
      eventPlaying.text = "Event: " + GameManager.Instance.messageSystemInstance.currentEventMessageQueue.Peek().eventName;
    }
    else 
    {
      eventPlaying.text = "Event: " + "none";
    } 
  }
 
}