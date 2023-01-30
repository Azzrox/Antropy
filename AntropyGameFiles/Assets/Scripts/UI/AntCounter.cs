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
  public Slider antSlider;

  public TextMeshProUGUI freeAnts;
  public TextMeshProUGUI assignedAntsText;
  public TextMeshProUGUI resources;
  public TextMeshProUGUI tileName;
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
          if (!GameManager.Instance.Map[posX, posZ].partOfAnthill && GameManager.Instance.Map[posX, posZ].assignedAnts == 1) { SpawnAnt(); }
          else if (!GameManager.Instance.Map[posX, posZ].partOfAnthill && GameManager.Instance.Map[posX, posZ].assignedAnts > 1) { AdjustAntSize((float)GameManager.Instance.Map[posX, posZ].assignedAnts); }

          

            //Debug.Log("partOfAnthill: " + GameManager.Instance.Map[posX, posZ].partOfAnthill + "assignedAnts: " + GameManager.Instance.Map[posX, posZ].assignedAnts);
        }
    
  } 

    void AdjustAntSize(float size)
    {
        GameObject targetAnt = antlist.Find(element => (element.GetComponent<AntPathing>().coordinates[0] == posX) && (element.GetComponent<AntPathing>().coordinates[1] == posZ));

        if (targetAnt != null) {
            var scale = targetAnt.transform.localScale;
            size = Mathf.Sqrt(size);
            scale.Set(size, size, size);
            targetAnt.transform.localScale = scale;
            //Debug.Log("new Size: " + size);
        }
        else { Debug.Log("Could not find the ant!"); }
        
    }
    void DecreaseAnts()
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

        if (!GameManager.Instance.Map[posX, posZ].partOfAnthill && GameManager.Instance.Map[posX, posZ].assignedAnts == 0) { RemoveAnt(); }
        else if (!GameManager.Instance.Map[posX, posZ].partOfAnthill && GameManager.Instance.Map[posX, posZ].assignedAnts > 0) { AdjustAntSize((float)GameManager.Instance.Map[posX, posZ].assignedAnts); }
      }
    }

    void UpdateAnts()
    {
      Debug.Log("Slider moved!, x|y: " + posX + "|" + posZ);
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
      
      GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
    }

    void Confirm()
    {
        //GetComponentInParent<GameObject>().SetActive(false);
        gameObject.GetComponent<Canvas>().enabled = false;
        GameObject highlight = GameObject.Find("HighlightTile");
        highlight.GetComponent<MeshRenderer>().enabled = false;
        GameManager.Instance.WeightedDistanceToHill();
        Debug.Log("Anthill is dominated: " + GameManager.Instance.Map[posX, posZ].dominatedByPlayer + ", occupied: " + GameManager.Instance.Map[posX, posZ].occupiedByPlayer);
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
    freeAnts.text = "Nursing Ants: " + GameManager.Instance.freeAnts;
    if (GameManager.Instance.Map[posX, posZ].assignedAnts == 1)
      { 
          assignedAntsText.text = GameManager.Instance.Map[posX, posZ].assignedAnts + "/"  + GameManager.Instance.Map[posX, posZ].maxAssignedAnts + " ants";
          resources.text = "Resources: " + GameManager.Instance.Map[posX, posZ].resourceAmount;
          tileName.text = GameManager.Instance.TileName(GameManager.Instance.Map[posX, posZ].type) + "[" + posX + "," + posZ + "]"; 
      } 
      else
      {
          assignedAntsText.text = GameManager.Instance.Map[posX, posZ].assignedAnts + "/" + GameManager.Instance.Map[posX, posZ].maxAssignedAnts + " ants";
          resources.text = "Resources: " + GameManager.Instance.Map[posX, posZ].resourceAmount;
          tileName.text = GameManager.Instance.TileName(GameManager.Instance.Map[posX, posZ].type) + "[" + posX + "," + posZ + "]";
      }

    regrowText.text = "Expected regrow: " + GameManager.Instance.Map[posX, posZ].regrowResource;
    transportCostText.text = "Transport cost: " + GameManager.Instance.Map[posX, posZ].foodTransportCost + ", dis: " + GameManager.Instance.Map[posX, posZ].distanceAntHill;
    }

    void SpawnAnt() 
    {
        GameObject ant = Instantiate<GameObject>(antPrefab, new Vector3(posX + 0.5f, 0.2f, posZ + 0.5f), Quaternion.identity, antCollection.transform) ;
        ant.GetComponent<AntPathing>().coordinates = new int[] {posX, posZ};
        antlist.Add(ant);

    }

    void RemoveAnt() 
    {
        // some ants might fall outside that area?
        if (antlist.Count > 0)
        {
            GameObject squashThis = antlist.Find(element => element.GetComponent<AntPathing>().coordinates[0] == posX && element.GetComponent<AntPathing>().coordinates[1] == posZ);
            antlist.Remove(squashThis);
            Destroy(squashThis);
        }
    }
}
