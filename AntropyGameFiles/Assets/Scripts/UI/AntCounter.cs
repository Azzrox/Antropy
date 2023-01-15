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
  public Button plusButton;
  public Button plusPlusButton;
  public Button minusButton;
  public Button confirmButton;
  public Button minusMinusButton;

  public TextMeshProUGUI freeAnts;
  public TextMeshProUGUI assignedAntsText;
  public TextMeshProUGUI resources;
  public TextMeshProUGUI tileName;
  
  public GameObject antPrefab;
  

  private void Awake()
  {
    
  }

  // Start is called before the first frame update
  void Start()
  {
    plusButton.onClick.AddListener(IncreaseAnts);
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
          GameManager.Instance.UpdateIncome();
          // update UI
          UpdateAntText();
          GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
          
          //Spawn the Ant sprite (only one due to limitations, "starts to lag at some point with larger maps)
          if (!GameManager.Instance.Map[posX, posZ].partOfAnthill && GameManager.Instance.Map[posX, posZ].assignedAnts == 0) { SpawnAnt(); }

        }
    
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
        GameManager.Instance.UpdateIncome();
        // update UI
        UpdateAntText();
        GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();

        if (!GameManager.Instance.Map[posX, posZ].partOfAnthill) { RemoveAnt(); }
      }
    }

    void Confirm()
    {
        //GetComponentInParent<GameObject>().SetActive(false);
        gameObject.GetComponent<Canvas>().enabled = false;
    }
    public void SetSelectedTile(int ix, int iz)
    {
        // could be replaced by ix, iy to get values from matrix
        posX = ix;
        posZ = iz;
    }

    void AddAllAnts() 
    {
      //This is a very ugly fix for the prototype, rework this
      for (int i = 0; i < GameManager.Instance.maxAntsResourceTile; i++)
      {
        IncreaseAnts();
      }
    }

    void RemoveAllAnts()
    {
      //This is a very ugly fix for the prototype, rework this
      for (int i = 0; i < GameManager.Instance.maxAntsResourceTile; i++)
      {
        DecreaseAnts();
      }
    }


    public void UpdateAntText()
    {
    freeAnts.text = "Free Ants: " + GameManager.Instance.freeAnts;
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
    }

    void SpawnAnt() 
    {
        GameObject ant = Instantiate<GameObject>(antPrefab, new Vector3(posX + 0.5f, 0.2f, posZ + 0.5f), Quaternion.identity, antCollection.transform) ;
    }

    void RemoveAnt() 
    {
        GameObject[] allAnts = GameObject.FindGameObjectsWithTag("Ant");
        
        // some ants might fall outside that area?
        Vector3 where = new Vector3(posX + 0.5f, 0.2f, posZ + 0.5f);
        if (allAnts.Length > 0)
        {
            GameObject squashThis = Array.Find(allAnts, element => element.GetComponent<AntPathing>().spawnpoint == where);
            Destroy(squashThis);
        }
    }
}
