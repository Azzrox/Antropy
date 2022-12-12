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
    public Button minusButton;
    public Button confirmButton;
    public TextMeshProUGUI freeAnts;
    public TextMeshProUGUI assignedAntsText;
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
        antCollection = new GameObject();
       // UpdateAntText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IncreaseAnts()
    {
        int freeAnts = gameManager.freeAnts;
        if (freeAnts > 0 && assignedAnts < maxAssignedAnts)
        {
            assignedAnts += 1;
            gameManager.freeAnts -= 1;
            SetAssignedAnts_remote();
            UpdateAntText();

            if (!isAntHill) { SpawnAnt(); }
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
    }
    public void SetAssignedAnts_remote(){
        if (isAntHill)
        {
            gameManager.mapInstance.GameMap[posX,posZ].AssignedAnts = assignedAnts;
        } else{
            gameManager.mapInstance.GameMap[posX,posZ].AssignedAnts = assignedAnts;
        }
        
    }

    public void UpdateAntText(){
        if (isAntHill)
        {

        } else 
        {   freeAnts.text = "Free ants: " + gameManager.freeAnts + "/" + gameManager.totalAnts;
        }
        if (assignedAnts == 1)
        {
            assignedAntsText.text = "assign " + assignedAnts + " ant";
        } else
        {
            assignedAntsText.text = "assign " + assignedAnts + " ants";
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
