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
  private int assignedAnts;
  private int maxAssignedAnts;
  private bool isAntHill;
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
  private GameManager gameManager;

  private void Awake()
  {
    gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
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
        int freeAnts = GameManager.Instance.freeAnts;
        if (freeAnts > 0 && assignedAnts < maxAssignedAnts)
        {
          assignedAnts += 1;

          //Spawn the Ant sprite (only one due to limitations, "starts to lag at some point with larger maps)
          if (!isAntHill && GameManager.Instance.Map[posX, posZ].assignedAnts == 0) { SpawnAnt(); }

          GameManager.Instance.freeAnts -= 1;
          SetAssignedAnts_remote();
          UpdateAntText();

          if (GameManager.Instance.Map[posX, posZ].type != 0 && 
              GameManager.Instance.Map[posX, posZ].type != 3 &&
              GameManager.Instance.Map[posX, posZ].resourceAmount > GameManager.Instance.resourceGatherRate &&
              GameManager.Instance.Map[posX, posZ].resourceAmount - GameManager.Instance.Map[posX, posZ].reservedResources >=  GameManager.Instance.resourceGatherRate) 
              {
                GameManager.Instance.income += (int)GameManager.Instance.resourceGatherRate;
                GameManager.Instance.Map[posX, posZ].reservedResources += (int) GameManager.Instance.resourceGatherRate;
              }
                
        }
    GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
  } 

    void DecreaseAnts()
    {
      if (assignedAnts > 0)
      {
        assignedAnts -= 1;
        GameManager.Instance.freeAnts += 1;
        SetAssignedAnts_remote();
        UpdateAntText();

        //Correcting the income and resource calculation in real time
        if (GameManager.Instance.Map[posX, posZ].type != 0 && 
            GameManager.Instance.Map[posX, posZ].type != 3 &&
            GameManager.Instance.Map[posX, posZ].reservedResources > 0)
        {
           GameManager.Instance.income -= (int)GameManager.Instance.resourceGatherRate;
           GameManager.Instance.Map[posX, posZ].reservedResources -= (int)GameManager.Instance.resourceGatherRate;
        }

        GameManager.Instance.miniBarInfoInstance.MiniBarInfoUpdate();
        if (!isAntHill) { RemoveAnt(); }
      }
    }

    void Confirm()
    {
        //GetComponentInParent<GameObject>().SetActive(false);
        gameObject.GetComponent<Canvas>().enabled = false;
    }
    public void SetAssignedAnts(int ix, int iz, int asAnts, int maxAnts, bool isHill)
    {
        // could be replaced by ix, iy to get values from matrix
        posX = ix;
        posZ = iz;
        assignedAnts = GameManager.Instance.Map[ix,iz].assignedAnts;
        maxAssignedAnts = GameManager.Instance.Map[ix,iz].maxAssignedAnts;
        isAntHill = GameManager.Instance.Map[ix,iz].partOfAnthill;
        SetAssignedAnts_remote();
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

  public void SetAssignedAnts_remote()
    {
      if (isAntHill)
      {
          GameManager.Instance.Map[posX,posZ].assignedAnts = assignedAnts;
      } 
      else
      {
        GameManager.Instance.Map[posX,posZ].assignedAnts = assignedAnts;
        if(assignedAnts > 0)
        {
          GameManager.Instance.Map[posX, posZ].ownedByPlayer = true;
        }
        else
        {
          GameManager.Instance.Map[posX, posZ].ownedByPlayer = false;
        }
      }
    }

    public void UpdateAntText()
    {
    freeAnts.text = "Free Ants: " + GameManager.Instance.freeAnts;
    if (assignedAnts == 1)
      { 
          assignedAntsText.text = assignedAnts + "/"  + GameManager.Instance.Map[posX, posZ].maxAssignedAnts + " ants";
          resources.text = "Resources: " + GameManager.Instance.Map[posX, posZ].resourceAmount;
          tileName.text = GameManager.Instance.TileName(GameManager.Instance.Map[posX, posZ].type) + "[" + posX + "," + posZ + "]"; 
      } 
      else
      {
          assignedAntsText.text = assignedAnts + "/" + GameManager.Instance.Map[posX, posZ].maxAssignedAnts + " ants";
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

        Vector3 where = new Vector3(posX + 0.5f, 0.2f, posZ + 0.5f);
        if (allAnts.Length > 0)
        {
            GameObject squashThis = Array.Find(allAnts, element => element.GetComponent<AntPathing>().spawnpoint == where);
            Destroy(squashThis);
        }
    }
}
