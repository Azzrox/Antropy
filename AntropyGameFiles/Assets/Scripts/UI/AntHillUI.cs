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
  private GameManager gameManager;


  private void Awake()
  {
    gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
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
    hatcheryLevelText.text = "Hatchery Level: " + gameManager.hatcheryLevel;
    
    storageLevelText.text = "Storage Level: " + gameManager.storageLevel;


    if (gameManager.hatcheryLevel >= gameManager.hatcheryMaxLevel)
    {
      costHatcheryText.text = "Cost: " + "Max";
    }
    else
    {
      costHatcheryText.text = "Cost: " + gameManager.hatcheryCost[gameManager.hatcheryLevel];
    }


    if (gameManager.storageLevel >= gameManager.storageMaxLevel)
    {
      costStorageText.text = "Cost: " + "Max";
    }
    else
    {
      costStorageText.text = "Cost: " + gameManager.storageCost[gameManager.storageLevel];
    }
  }

  /// <summary>
  /// Increase the level of the hatchery
  /// </summary>
  void IncreaseLevelHatchery() 
  {
    if(gameManager.hatcheryLevel < gameManager.hatcheryMaxLevel) 
    {

      //check resources
      if(gameManager.resources >= gameManager.hatcheryCost[gameManager.hatcheryLevel])
      {
        hatcheryLevelText.text = "Hatchery Level: " + (gameManager.hatcheryLevel + 1);

        //take resources from the player update the maxSupportedAnts and add +1 to the level;
        gameManager.resources -= gameManager.hatcheryCost[gameManager.hatcheryLevel];
        gameManager.hatcheryLevel += 1;
        gameManager.currentMaximumPopulationCapacity = gameManager.populationCapacityAmount[gameManager.hatcheryLevel];

        if (gameManager.hatcheryLevel >= gameManager.hatcheryMaxLevel)
        {
          costHatcheryText.text = "Cost: " + "Max";
        }
        else
        {
          costHatcheryText.text = "Cost: " + gameManager.hatcheryCost[gameManager.hatcheryLevel];
        }
        gameManager.miniBarInfoInstance.MiniBarInfoUpdate();
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
    if (gameManager.storageLevel < gameManager.storageMaxLevel)
    {
      //check cost
      if(gameManager.resources >= gameManager.storageCost[gameManager.storageLevel])
      {
        storageLevelText.text = "Storage Level: " + (gameManager.storageLevel + 1);
        gameManager.resources -= gameManager.storageCost[gameManager.storageLevel];
        gameManager.storageLevel += 1;
        gameManager.maxResourceStorage = gameManager.storageCapacityAmount[gameManager.storageLevel];

        if (gameManager.storageLevel >= gameManager.storageMaxLevel)
        {
          costStorageText.text = "Cost: " + "Max";
        }
        else
        {
          costStorageText.text = "Cost: " + gameManager.storageCost[gameManager.storageLevel];
        }
        gameManager.miniBarInfoInstance.MiniBarInfoUpdate();
      }
      else
      {
        Debug.Log("Not Enough Resources to upgrade Storage");
      }
    }
  }
}
