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
    plusPlusButton.onClick.AddListener(AllAnts);
    antCollection = new GameObject();
    antCollection.name = "MovingAnts";
     //UpdateAntText();
  }

    void IncreaseAnts()
    {
        int freeAnts = gameManager.freeAnts;
        if (freeAnts > 0 && assignedAnts < maxAssignedAnts)
        {
          assignedAnts += 1;

          //Spawn the Ant sprite (only one due to limitations, "starts to lag at some point with larger maps)
          if (!isAntHill && gameManager.mapInstance.GameMap[posX, posZ].AssignedAnts == 0) { SpawnAnt(); }

          gameManager.freeAnts -= 1;
          SetAssignedAnts_remote();
          UpdateAntText();

          if (gameManager.mapInstance.GameMap[posX, posZ].TileType != 0 && 
              gameManager.mapInstance.GameMap[posX, posZ].TileType != 3 &&
              gameManager.mapInstance.GameMap[posX, posZ].ResourceAmount > gameManager.resourceGatherRate &&
              gameManager.mapInstance.GameMap[posX, posZ].ResourceAmount - gameManager.mapInstance.GameMap[posX, posZ].ReservedResources >= gameManager.resourceGatherRate) 
              {
                gameManager.income += (int)gameManager.resourceGatherRate;
                gameManager.mapInstance.GameMap[posX, posZ].ReservedResources += (int)gameManager.resourceGatherRate;
              }
                gameManager.miniBarInfoInstance.MiniBarInfoUpdate();
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

        //Correcting the income and resource calculation in real time
        if (gameManager.mapInstance.GameMap[posX, posZ].TileType != 0 &&
            gameManager.mapInstance.GameMap[posX, posZ].TileType != 3 &&
            gameManager.mapInstance.GameMap[posX, posZ].ReservedResources > 0)
        {
          gameManager.income -= (int)gameManager.resourceGatherRate;
          gameManager.mapInstance.GameMap[posX, posZ].ReservedResources -= (int)gameManager.resourceGatherRate;
        }

        gameManager.miniBarInfoInstance.MiniBarInfoUpdate();
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
        assignedAnts = asAnts;
        maxAssignedAnts = maxAnts;
        isAntHill = isHill;
        SetAssignedAnts_remote();
    }

    void AllAnts() 
    {
      //This is a very ugly fix for the prototype, rework this
      for (int i = 0; i < gameManager.maxAntsResourceTile; i++)
      {
        IncreaseAnts();
      }
    }


    public void SetAssignedAnts_remote()
    {
      if (isAntHill)
      {
          gameManager.mapInstance.GameMap[posX,posZ].AssignedAnts = assignedAnts;
      } 
      else
      {
        gameManager.mapInstance.GameMap[posX,posZ].AssignedAnts = assignedAnts;
        if(assignedAnts > 0)
        {
          gameManager.mapInstance.GameMap[posX, posZ].OwnedByPlayer = true;
        }
        else
        {
          gameManager.mapInstance.GameMap[posX, posZ].OwnedByPlayer = false;
        }
      }
    }

    public void UpdateAntText()
    {
    freeAnts.text = "Free Ants: " + gameManager.freeAnts;
    if (assignedAnts == 1)
      {
          
          assignedAntsText.text = assignedAnts + "/"  + gameManager.mapInstance.GameMap[posX, posZ].MaxAssignedAnts + " ants";
          resources.text = "Resources: " + gameManager.mapInstance.GameMap[posX, posZ].resourceAmount;
          tileName.text = gameManager.mapInstance.TileName(gameManager.mapInstance.GameMap[posX, posZ].TileType) + "[" + posX + "," + posZ + "]"; 
      } 
      else
      {
          assignedAntsText.text = assignedAnts + "/" + gameManager.mapInstance.GameMap[posX, posZ].MaxAssignedAnts + " ants";
          resources.text = "Resources: " + gameManager.mapInstance.GameMap[posX, posZ].resourceAmount;
          tileName.text = gameManager.mapInstance.TileName(gameManager.mapInstance.GameMap[posX, posZ].TileType) + "[" + posX + "," + posZ + "]";
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
