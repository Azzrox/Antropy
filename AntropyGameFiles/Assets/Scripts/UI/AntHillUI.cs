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
  private GameManager gm = GameManager.Instance;


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
    hatcheryLevelText.text = "Hatchery Level: " + gm.hatcheryLevel;
    
    storageLevelText.text = "Storage Level: " + gm.storageLevel;


    if (gm.hatcheryLevel >= gm.hatcheryMaxLevel)
    {
      costHatcheryText.text = "Cost: " + "Max";
    }
    else
    {
      costHatcheryText.text = "Cost: " + gm.hatcheryCost[gm.hatcheryLevel];
    }


    if (gm.storageLevel >= gm.storageMaxLevel)
    {
      costStorageText.text = "Cost: " + "Max";
    }
    else
    {
      costStorageText.text = "Cost: " + gm.storageCost[gm.storageLevel];
    }
  }

  /// <summary>
  /// Increase the level of the hatchery
  /// </summary>
  void IncreaseLevelHatchery() 
  {
    if(gm.hatcheryLevel < gm.hatcheryMaxLevel) 
    {

      //check resources
      if(gm.resources >= gm.hatcheryCost[gm.hatcheryLevel])
      {
        hatcheryLevelText.text = "Hatchery Level: " + (gm.hatcheryLevel + 1);

        //take resources from the player update the maxSupportedAnts and add +1 to the level and (correct the upkeep "needs to be implemented);
        gm.resources -= gm.hatcheryCost[gm.hatcheryLevel];
        gm.hatcheryLevel += 1;
        gm.currentMaximumPopulationCapacity = gm.populationCapacityAmount[gm.hatcheryLevel];

        if (gm.hatcheryLevel >= gm.hatcheryMaxLevel)
        {
          costHatcheryText.text = "Cost: " + "Max";
        }
        else
        {
          costHatcheryText.text = "Cost: " + gm.hatcheryCost[gm.hatcheryLevel];
          
        }
       
        gm.miniBarInfoInstance.MiniBarInfoUpdate();
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
    if (gm.storageLevel < gm.storageMaxLevel)
    {
      //check cost
      if(gm.resources >= gm.storageCost[gm.storageLevel])
      {
        //(correct the upkeep "needs to be implemented)
        storageLevelText.text = "Storage Level: " + (gm.storageLevel + 1);
        gm.resources -= gm.storageCost[gm.storageLevel];
        gm.storageLevel += 1;
        gm.maxResourceStorage = gm.storageCapacityAmount[gm.storageLevel];

        if (gm.storageLevel >= gm.storageMaxLevel)
        {
          costStorageText.text = "Cost: " + "Max";
        }
        else
        {
          costStorageText.text = "Cost: " + gm.storageCost[gm.storageLevel];
        }
        gm.miniBarInfoInstance.MiniBarInfoUpdate();
      }
      else
      {
        Debug.Log("Not Enough Resources to upgrade Storage");
      }
    }
  }
}
