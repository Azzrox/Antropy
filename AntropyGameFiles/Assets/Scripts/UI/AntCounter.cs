using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AntCounter : MonoBehaviour
{
    public int antsAssigned = 0;
    public TileScript tile;
    public Button plusButton;
    public Button minusButton;
    public Button confirmButton;
    public TextMeshProUGUI freeAnts;
    public TextMeshProUGUI assignedAntsText; 
    private GameManagerUI gameManager;

  private void Awake()
  {
    gameManager = GameObject.Find("Game Manager").GetComponent<GameManagerUI>();
  }

  // Start is called before the first frame update
  void Start()
    {
        plusButton.onClick.AddListener(IncreaseAnts);
        minusButton.onClick.AddListener(DecreaseAnts);
        confirmButton.onClick.AddListener(Confirm);
       // UpdateAntText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IncreaseAnts()
    {
        int freeAnts = gameManager.freeAnts;
        if (freeAnts > 0)
        {
            tile.AssignedAnts += 1;
            gameManager.freeAnts -= 1;
            UpdateAntText();
        }
    } 

    void DecreaseAnts()
    {
        if (tile.AssignedAnts > 0)
        {
            tile.AssignedAnts -= 1;
            gameManager.freeAnts += 1;
            UpdateAntText();

        }
    }

    void Confirm()
    {
        //GetComponentInParent<GameObject>().SetActive(false);
        gameObject.SetActive(false);
    }

    public void UpdateAntText(){
        freeAnts.text = "Free ants: " + gameManager.freeAnts + "/" + gameManager.totalAnts;
        if (tile.AssignedAnts == 1)
        {
            assignedAntsText.text = "assign " + tile.AssignedAnts + " ant";
        } else
        {
            assignedAntsText.text = "assign " + tile.AssignedAnts + " ants";
        }
    }
}
