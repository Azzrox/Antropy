using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AntHillUI : MonoBehaviour
{

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
  private GameManagerUI gameManager;


  private void Awake()
  {
    gameManager = GameObject.Find("Game Manager").GetComponent<GameManagerUI>();
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
    if (freeAnts > 0)
    {
      tile.AssignedAnts += 1;
      gameManager.freeAnts -= 1;
      UpdateAntText();
      PopulationPerTurnTextUpdate();
    }
  }

  void DecreaseAnts()
  {
    if (tile.AssignedAnts > 0)
    {
      tile.AssignedAnts -= 1;
      gameManager.freeAnts += 1;
      UpdateAntText();
      PopulationPerTurnTextUpdate();

    }
  }

  void Confirm()
  {
    gameObject.SetActive(false);
  }

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

  void PopulationPerTurnTextUpdate() 
  {
    int basicGrowth = 1;
    int newGrowth = basicGrowth * tile.AssignedAnts;
    popPerTurnText.text = "Pop per Turn: " + newGrowth;
  }

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
