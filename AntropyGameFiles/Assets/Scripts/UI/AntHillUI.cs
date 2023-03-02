using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AntHillUI : MonoBehaviour
{
  public Button storageUpgradeplusButton;
  public Button hatcheryUpgradeplusButton;
  public Button confirmButton;

  public TextMeshProUGUI freeAnts;
  public TextMeshProUGUI assignedAntsText;
  public TextMeshProUGUI storageLevelText;
  public TextMeshProUGUI hatcheryLevelText;
  public TextMeshProUGUI costStorageText;
  public TextMeshProUGUI costHatcheryText;
  public TextMeshProUGUI popPerTurnText;

  public TileScript tile;


  private void Awake()
  {
  }
  private void Start()
  {
    storageUpgradeplusButton.onClick.AddListener(IncreaseLevelStorage);
    hatcheryUpgradeplusButton.onClick.AddListener(IncreaseLevelHatchery);
    confirmButton.onClick.AddListener(Confirm);
  }

  void Confirm()
  {
    gameObject.GetComponent<Canvas>().enabled = false;
  }

  /// <summary>
  /// Update Anthill UI Text with current Data
  /// </summary>
  public void UpdateAntText()
  {
    hatcheryLevelText.text = "Hatchery Level: " + GameManager.Instance.hatcheryLevel;
    
    storageLevelText.text = "Storage Level: " + GameManager.Instance.storageLevel;


    if (GameManager.Instance.hatcheryLevel >= GameManager.Instance.hatcheryMaxLevel)
    {
      costHatcheryText.text = "Cost: " + "Max";
    }
    else
    {
      costHatcheryText.text = "Cost: " + GameManager.Instance.hatcheryCost[GameManager.Instance.hatcheryLevel];
    }


    if (GameManager.Instance.storageLevel >= GameManager.Instance.storageMaxLevel)
    {
      costStorageText.text = "Cost: " + "Max";
    }
    else
    {
      costStorageText.text = "Cost: " + GameManager.Instance.storageCost[GameManager.Instance.storageLevel];
    }
  }

  /// <summary>
  /// Increase the level of the hatchery
  /// </summary>
  void IncreaseLevelHatchery() 
  {
    if(GameManager.Instance.hatcheryLevel < GameManager.Instance.hatcheryMaxLevel) 
    {

      //check resources
      if(GameManager.Instance.resources >= GameManager.Instance.hatcheryCost[GameManager.Instance.hatcheryLevel])
      {
        hatcheryLevelText.text = "Hatchery Level: " + (GameManager.Instance.hatcheryLevel + 1);
        GameManager.Instance.resources -= GameManager.Instance.hatcheryCost[GameManager.Instance.hatcheryLevel];
        GameManager.Instance.hatcheryLevel += 1;
        GameManager.Instance.currentMaximumPopulationCapacity = GameManager.Instance.populationCapacityAmount[GameManager.Instance.hatcheryLevel];

        if (GameManager.Instance.hatcheryLevel >= GameManager.Instance.hatcheryMaxLevel)
        {
          costHatcheryText.text = "Cost: " + "Max";
        }
        else
        {
          costHatcheryText.text = "Cost: " + GameManager.Instance.hatcheryCost[GameManager.Instance.hatcheryLevel];
          
        }
        GameManager.Instance.UpdateIncomeGrowth();
       
        GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
      }
      else 
      {
        Debug.Log("Not Enough Resources to upgrade Hatchery");
      }
    }
  }

  /// <summary>
  /// Increase the Level of the Storage
  /// </summary>
  void IncreaseLevelStorage()
  {
    if (GameManager.Instance.storageLevel < GameManager.Instance.storageMaxLevel)
    {
      //check cost
      if(GameManager.Instance.resources >= GameManager.Instance.storageCost[GameManager.Instance.storageLevel])
      {
        storageLevelText.text = "Storage Level: " + (GameManager.Instance.storageLevel + 1);
        GameManager.Instance.resources -= GameManager.Instance.storageCost[GameManager.Instance.storageLevel];
        GameManager.Instance.storageLevel += 1;
        GameManager.Instance.maxResourceStorage = GameManager.Instance.storageCapacityAmount[GameManager.Instance.storageLevel];

        if (GameManager.Instance.storageLevel >= GameManager.Instance.storageMaxLevel)
        {
          costStorageText.text = "Cost: " + "Max";
        }
        else
        {
          costStorageText.text = "Cost: " + GameManager.Instance.storageCost[GameManager.Instance.storageLevel];
        }
        GameManager.Instance.UpdateIncomeGrowth();
        GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
      }
      else
      {
        Debug.Log("Not Enough Resources to upgrade Storage " + GameManager.Instance.resources + "/" + GameManager.Instance.storageCost[GameManager.Instance.storageLevel]);
      }
    }
  }
}
