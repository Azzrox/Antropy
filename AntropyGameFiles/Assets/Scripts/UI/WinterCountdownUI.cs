using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinterCountdownUI : MonoBehaviour
{
  public TextMeshProUGUI dayDisplay;
  public TextMeshProUGUI resourcesValue;
  public TextMeshProUGUI populationValue;
  public TextMeshProUGUI resourceDescription;
  public TextMeshProUGUI populationDescription;
  public RelativeBoxUI populationFill;
  public RelativeBoxUI resourceFill;
  public RelativeBoxUI incomeIndicator;
  public RelativeBoxUI growthIndicator;
  void Start()
    {
      Debug.Log("add winterCountDownInstance to GameManager");
      GameManager.Instance.winterCountDownInstance = this;
      this.gameObject.SetActive(false);
  }

  public void WinterCountdownUpdate() 
  {
    int income = GameManager.Instance.income;
    string incomeString = income < 0 ? " (<color=red>" + income + "</color>)/" : " (+" + income + ")/";
    resourcesValue.text = GameManager.Instance.resources + incomeString + GameManager.Instance.maxResourceStorage;
    int growth = GameManager.Instance.growth;
    string growthString = growth < 0 ? " (<color=red>" + growth + "</color>)/" : " (+" + growth + ")/";
    populationValue.text = GameManager.Instance.totalAnts + growthString + GameManager.Instance.currentMaximumPopulationCapacity;

    populationFill.SetLeftRight(0, GameManager.Instance.totalAnts, GameManager.Instance.currentMaximumPopulationCapacity, 0);
    growthIndicator.SetLeftRight(GameManager.Instance.totalAnts, GameManager.Instance.growth, GameManager.Instance.currentMaximumPopulationCapacity, 0.17f);

    RelativeBoxUI growthTextBox = populationValue.GetComponent<RelativeBoxUI>();
    if (GameManager.Instance.totalAnts > 0.5 * GameManager.Instance.currentMaximumPopulationCapacity)
    {
      growthTextBox.SetLeftRight(0, 0.5f, 1, 0);
      populationValue.horizontalAlignment = HorizontalAlignmentOptions.Left;
    }
    else
    {
      growthTextBox.SetLeftRight(0, 1, 1, 0);
      populationValue.horizontalAlignment =  HorizontalAlignmentOptions.Right;
    }


    resourceFill.SetLeftRight(0, GameManager.Instance.resources, GameManager.Instance.maxResourceStorage, 0);
    incomeIndicator.SetLeftRight(GameManager.Instance.resources, GameManager.Instance.income, GameManager.Instance.maxResourceStorage, 0.17f);

    RelativeBoxUI resourceTextBox = resourcesValue.GetComponent<RelativeBoxUI>();
    if (GameManager.Instance.resources > 0.5 * GameManager.Instance.maxResourceStorage)
    {
      resourceTextBox.SetLeftRight(0, 0.5f, 1, 0);
      resourcesValue.horizontalAlignment = HorizontalAlignmentOptions.Left;
    }
    else
    {
      resourceTextBox.SetLeftRight(0, 1, 1, 0);
      resourcesValue.horizontalAlignment =  HorizontalAlignmentOptions.Right;
    }
    if (GameManager.Instance.resources + GameManager.Instance.income > GameManager.Instance.maxResourceStorage)
    {
      resourceDescription.color = Color.blue;
    }
    CheckLimits(resourceDescription, GameManager.Instance.resources + GameManager.Instance.income, GameManager.Instance.maxResourceStorage, Color.red, Color.blue);
    CheckLimits(populationDescription, GameManager.Instance.totalAnts + GameManager.Instance.growth, GameManager.Instance.currentMaximumPopulationCapacity, Color.red, Color.red);

    updateDays();
  }

  void CheckLimits(TextMeshProUGUI text, float value, float maximum, Color colMin, Color colMax)
  {
    if (value > maximum)
    { text.color = colMax; }
    else if (value < 0)
    { text.color = colMin; }
    else
    {
      text.color = Color.black;
    }
  }

  void updateDays() 
  {
    dayDisplay.text = (GameManager.Instance.maxTurnCount - GameManager.Instance.currentTurnCount) +  "           Days";
  }
}
