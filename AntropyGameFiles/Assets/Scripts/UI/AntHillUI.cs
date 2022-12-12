using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AntHillUI : MonoBehaviour
{

  private int posX;
  private int posZ;
  private int assignedAnts;
  private int maxAssignedAnts;

  public Button storageUpgradeplusButton;
  public Button hatcheryUpgradeplusButton;
  public Button assignAntsToHatcherplusButton;
  public Button assignAntsToHatcherminusButton;
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
    assignAntsToHatcherplusButton.onClick.AddListener(IncreaseAnts);
    assignAntsToHatcherminusButton.onClick.AddListener(DecreaseAnts);
    storageUpgradeplusButton.onClick.AddListener(IncreaseLevelStorage);
    hatcheryUpgradeplusButton.onClick.AddListener(IncreaseLevelHatchery);
    confirmButton.onClick.AddListener(Confirm);
  }

  void IncreaseAnts()
  {
    int freeAnts = gameManager.freeAnts;

    if (freeAnts > 0 && tile.AssignedAnts < tile.MaxAssignedAnts)
    {
      assignedAnts += 1;
      gameManager.freeAnts -= 1;
      SetAssignedAnts_remote();
      UpdateAntText();
      PopulationPerTurnTextUpdate();
    }
  }

  void DecreaseAnts()
  {
    if (assignedAnts > 0)
    {
      assignedAnts -= 1;
      gameManager.freeAnts += 1;
      SetAssignedAnts_remote();
      UpdateAntText();
      PopulationPerTurnTextUpdate();
    }
  }

  void Confirm()
  {
    gameObject.GetComponent<Canvas>().enabled = false;
  }

  public void SetAssignedAnts_remote()
  {
    if (tile.IsAntHill)
    {
      gameManager.mapInstance.GameMap[tile.XPos, tile.ZPos].AssignedAnts = assignedAnts;
    }
    else
    {
      gameManager.mapInstance.GameMap[tile.XPos, tile.ZPos].AssignedAnts = assignedAnts;
    }

  }

  public void SetAssignedAnts(int ix, int iz, int asAnts, int maxAnts, bool isHill)
  {
    // could be replaced by ix, iy to get values from matrix
    posX = ix;
    posZ = iz;
    assignedAnts = asAnts;
    maxAssignedAnts = maxAnts;
  }

  /// <summary>
  /// Update Anthill UI Text with current Data
  /// </summary>
  public void UpdateAntText()
  {

    freeAnts.text = "Free ants: " + gameManager.freeAnts + "/" + gameManager.totalAnts;
    if (tile.AssignedAnts == 1)
    {
      assignedAntsText.text = "Nursery: " + tile.AssignedAnts + " ant";
    }
    else
    {
      assignedAntsText.text = "Nursery: " + tile.AssignedAnts + " ants";
    }

    hatcheryLevelText.text = "Hatchery Level: " + gameManager.hatcheryLevel;
    costHatcheryText.text = "Cost: " + gameManager.storageCost[gameManager.hatcheryLevel];
    storageLevelText.text = "Storage Level: " + gameManager.storageLevel;
    costStorageText.text = "Cost: " + gameManager.storageCost[gameManager.storageLevel];
    PopulationPerTurnTextUpdate();
  }

  /// <summary>
  /// Update the population count per turn
  /// </summary>
  void PopulationPerTurnTextUpdate() 
  {
    int basicGrowth = 1;
    int newGrowth = basicGrowth * tile.AssignedAnts;
    popPerTurnText.text = "Pop per Turn: " + newGrowth;
  }

  /// <summary>
  /// Increase the level of the hatchery
  /// </summary>
  void IncreaseLevelHatchery() 
  {
    if(gameManager.hatcheryLevel < gameManager.hatcheryMaxLevel) 
    {
      hatcheryLevelText.text = "Hatchery Level: " + (gameManager.hatcheryLevel + 1);
      gameManager.hatcheryLevel += 1;
     
      if (gameManager.hatcheryLevel >= gameManager.hatcheryMaxLevel)
      {
        costHatcheryText.text = "Cost: " + "Max";
      }
      else
      {
        costHatcheryText.text = "Cost: " + gameManager.storageCost[gameManager.hatcheryLevel];
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
      storageLevelText.text = "Storage Level: " + (gameManager.storageLevel + 1);
      gameManager.storageLevel += 1;

      if(gameManager.storageLevel >= gameManager.storageMaxLevel) 
      {
        costStorageText.text = "Cost: " + "Max";
      }
      else 
      {
        costStorageText.text = "Cost: " + gameManager.storageCost[gameManager.storageLevel];
      } 
    }
  }
}
