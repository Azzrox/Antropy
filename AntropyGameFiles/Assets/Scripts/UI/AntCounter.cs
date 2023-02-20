using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using Unity.VisualScripting;

public class AntCounter : MonoBehaviour
{
  private int posX;
  private int posZ;

  private GameObject antCollection;
  private List<GameObject> antlist = new List<GameObject>();
  
  public Button plusButton;
  public Button plusPlusButton;
  public Button minusButton;
  public Button confirmButton;
  public Button minusMinusButton;
  public Button buildRoad;
  public Button UpdateFertilityButton;
  public Slider antSlider;

  public TextMeshProUGUI freeAnts;
  public TextMeshProUGUI assignedAntsText;
  public TextMeshProUGUI resources;
  public TextMeshProUGUI tileName;
  public TextMeshProUGUI fertilityText;
  public TextMeshProUGUI fertilityUpgradeCost;
  public TextMeshProUGUI streetText;
  public TextMeshProUGUI streetUpgradeCost;
  public TextMeshProUGUI regrowText;
  public TextMeshProUGUI transportCostText;
  
  public GameObject antPrefab;

  private void Awake()
  {
    
  }

  // Start is called before the first frame update
  void Start()
  {
    plusButton.onClick.AddListener(IncreaseAnts);
    antSlider.onValueChanged.AddListener(delegate {UpdateAnts(); });

    minusButton.onClick.AddListener(DecreaseAnts);
    confirmButton.onClick.AddListener(Confirm);
    buildRoad.onClick.AddListener(UpdateRoad);
    UpdateFertilityButton.onClick.AddListener(UpdateFertility);
    plusPlusButton.onClick.AddListener(AddAllAnts);
    minusMinusButton.onClick.AddListener(RemoveAllAnts);
    antCollection = new GameObject();
    antCollection.name = "MovingAnts";
  }

    void IncreaseAnts()
    {
        
        //int freeAnts = GameManager.Instance.freeAnts;
        if (GameManager.Instance.freeAnts > 0 && GameManager.Instance.Map[posX, posZ].assignedAnts < GameManager.Instance.Map[posX, posZ].maxAssignedAnts)
        {
          // update data
          GameManager.Instance.Map[posX, posZ].assignedAnts += 1;
          GameManager.Instance.Map[posX, posZ].occupiedByPlayer = true;
          GameManager.Instance.freeAnts -= 1;
          // update economic data
          GameManager.Instance.UpdateIncomeGrowth();
          // update UI
          UpdateAntText();
          UpdateSlider();
          GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
          
          
          //Spawn the Ant sprite (only one due to limitations, "starts to lag at some point with larger maps)
          UpdateAntTile();
          

            //Debug.Log("partOfAnthill: " + GameManager.Instance.Map[posX, posZ].partOfAnthill + "assignedAnts: " + GameManager.Instance.Map[posX, posZ].assignedAnts);
        }
    
    }

   
    public void DecreaseAnts()
    {
      if (GameManager.Instance.Map[posX, posZ].assignedAnts > 0)
      {
        // update data
        GameManager.Instance.Map[posX, posZ].assignedAnts -= 1;
        GameManager.Instance.freeAnts += 1;
        if (GameManager.Instance.Map[posX, posZ].assignedAnts == 0)
        {
          GameManager.Instance.Map[posX, posZ].occupiedByPlayer = false;
        }
        // update economic data
        GameManager.Instance.UpdateIncomeGrowth();
        // update UI
        UpdateAntText();
        UpdateSlider();
        GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();

        UpdateAntTile();

      }
    }

// Triggered by Slider Action
    void UpdateAnts()
    {
      int oldAnt = GameManager.Instance.Map[posX,posZ].assignedAnts;
      int antDelta = oldAnt - (int)antSlider.value;
      GameManager.Instance.freeAnts += antDelta;
      GameManager.Instance.Map[posX, posZ].assignedAnts = (int) antSlider.value;
      if (GameManager.Instance.Map[posX, posZ].assignedAnts == 0)
      {
        GameManager.Instance.Map[posX, posZ].occupiedByPlayer = false;
      } else
      {
        GameManager.Instance.Map[posX, posZ].occupiedByPlayer = true;
      }
      // update economic data
      GameManager.Instance.UpdateIncomeGrowth();
      // update UI
      UpdateAntText();
      UpdateAntTile();

      GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
    }

    void Confirm()
    {
        //GetComponentInParent<GameObject>().SetActive(false);
        gameObject.GetComponent<Canvas>().enabled = false;
        GameObject highlight = GameObject.Find("HighlightTile");
        highlight.GetComponent<MeshRenderer>().enabled = false;
    }

    void UpdateRoad()
    {
      if (GameManager.Instance.Map[posX, posZ].constructionState < GameManager.Instance.transportCostVector.Length - 1)
      {
        if (GameManager.Instance.transportUpgradeCost[GameManager.Instance.Map[posX,posZ].constructionState] <= GameManager.Instance.resources)
        {
          
          GameManager.Instance.resources -= GameManager.Instance.transportUpgradeCost[GameManager.Instance.Map[posX,posZ].constructionState];
          GameManager.Instance.Map[posX,posZ].constructionState += 1;
          GameManager.Instance.Map[posX, posZ].foodTransportCost = GameManager.Instance.transportCostVector[GameManager.Instance.Map[posX, posZ].constructionState];
          
          GameManager.Instance.WeightedDistanceToHill();
          GameManager.Instance.UpdateIncomeGrowth();
          GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
          UpdateAntText();

          UpdateRoadTile();
          


     
        }
        else
        {
           Debug.Log("Not Enough Resources to upgrade Transportation " + GameManager.Instance.resources + "/" + GameManager.Instance.transportUpgradeCost[GameManager.Instance.Map[posX,posZ].constructionState]);
        }
      }
    }

    void UpdateFertility()
    { 
      if (GameManager.Instance.Map[posX, posZ].fertilityState < GameManager.Instance.fertilityUpgradeCost.Length)
      {
        if (GameManager.Instance.fertilityUpgradeCost[GameManager.Instance.Map[posX,posZ].fertilityState] <= GameManager.Instance.resources)
        {
          
          GameManager.Instance.resources -= GameManager.Instance.fertilityUpgradeCost[GameManager.Instance.Map[posX,posZ].fertilityState];
          GameManager.Instance.Map[posX,posZ].fertilityState += 1;
          GameManager.Instance.Map[posX, posZ].regrowResource = GameManager.Instance.regrowRateVector[GameManager.Instance.Map[posX, posZ].fertilityState];
          
          GameManager.Instance.UpdateIncomeGrowth();
          GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
          UpdateAntText();

          UpdateRoadTile();
          GameManager.Instance.mapInstance.UpgradeFertilityColor(posX, posZ, GameManager.Instance.Map[posX, posZ].fertilityState);
        
        }
        else
        {
           Debug.Log("Not Enough Resources to upgrade Fertility " + GameManager.Instance.resources + "/" + GameManager.Instance.fertilityUpgradeCost[GameManager.Instance.Map[posX,posZ].fertilityState]);
        }
      }
    }

    public void SetSelectedTile(int ix, int iz)
    {
        // could be replaced by ix, iy to get values from matrix
        posX = ix;
        posZ = iz;
        UpdateSlider();

    }

    public void UpdateSlider()
    {
      // set slider values
        // BUG: setting maxValue of slider triggers the UpdateAnts function 
        //Debug.Log("BEFORE: " +  antSlider.value + "assigned ants: " +  GameManager.Instance.Map[posX, posZ].assignedAnts +  ", x: "  + posX + ", y: " + posZ );

        antSlider.SetValueWithoutNotify(GameManager.Instance.Map[posX, posZ].assignedAnts);
        antSlider.maxValue = Mathf.Min(GameManager.Instance.Map[posX, posZ].maxAssignedAnts, GameManager.Instance.freeAnts + GameManager.Instance.Map[posX, posZ].assignedAnts);
        antSlider.SetValueWithoutNotify(GameManager.Instance.Map[posX, posZ].assignedAnts);        
        //Debug.Log("AFTER: " +  antSlider.value + "assigned ants: " +  GameManager.Instance.Map[posX, posZ].assignedAnts +  ", x: "  + posX + ", y: " + posZ );

    }

    void AddAllAnts() 
    {
      //This is a very ugly fix for the prototype, rework this
      for (int i = 0; i < GameManager.Instance.Map[posX, posZ].maxAssignedAnts; i++)
      {
        IncreaseAnts();
      }
    }

    void RemoveAllAnts()
    {
      //This is a very ugly fix for the prototype, rework this
      for (int i = 0; i < GameManager.Instance.Map[posX, posZ].maxAssignedAnts; i++)
      {
        DecreaseAnts();
      }
    }


    public void UpdateAntText()
    { 
    resources.text = "Resources: " + GameManager.Instance.Map[posX, posZ].resourceAmount;
    tileName.text = GameManager.Instance.TileName(GameManager.Instance.Map[posX, posZ].type) + "[" + posX + "," + posZ + "]";
    fertilityText.text = GameManager.Instance.FertilityNames[GameManager.Instance.Map[posX, posZ].fertilityState];
    if (GameManager.Instance.Map[posX, posZ].fertilityState < GameManager.Instance.fertilityUpgradeCost.Length){
      fertilityUpgradeCost.text = "Cost: " + GameManager.Instance.fertilityUpgradeCost[GameManager.Instance.Map[posX, posZ].fertilityState];
    } else{
    fertilityUpgradeCost.text = "Max";}

    streetText.text = GameManager.Instance.StreetNames[GameManager.Instance.Map[posX, posZ].constructionState] + " | " + GameManager.Instance.Map[posX, posZ].constructionState;
    if (GameManager.Instance.Map[posX, posZ].constructionState < GameManager.Instance.transportUpgradeCost.Length) {
      streetUpgradeCost.text = "Cost: " + GameManager.Instance.transportUpgradeCost[GameManager.Instance.Map[posX, posZ].constructionState];
    } else
    {streetUpgradeCost.text = "Max";}
    
    freeAnts.text = "Nursing Ants: " + GameManager.Instance.freeAnts;  
    assignedAntsText.text = GameManager.Instance.Map[posX, posZ].assignedAnts + "/" + GameManager.Instance.Map[posX, posZ].maxAssignedAnts + " ants";
    
    regrowText.text = "Expected regrow: " + GameManager.Instance.Map[posX, posZ].regrowResource;
    transportCostText.text = "Transport cost: " + GameManager.Instance.Map[posX, posZ].foodTransportCost + ", dis: " + GameManager.Instance.Map[posX, posZ].distanceAntHill;
    }

    void UpdateRoadTile()
    {
      if (GameManager.Instance.Map[posX, posZ].constructionState > 3){
        GameManager.Instance.mapInstance.mapMatrix[posX,posZ].spawnRoadOnTile(GameManager.Instance.Map[posX, posZ].constructionState - 4);
      }
    }

    void UpdateAntTile()
    {
      if(GameManager.Instance.Map[posX, posZ].assignedAnts > 0)
      {
        GameManager.Instance.mapInstance.mapMatrix[posX, posZ].AdjustAntSize(GameManager.Instance.Map[posX, posZ].assignedAnts);
      }
      else
      {
        GameManager.Instance.mapInstance.mapMatrix[posX, posZ].RemoveAnt();
      }
    }
}
