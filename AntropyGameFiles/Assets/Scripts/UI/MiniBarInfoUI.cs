using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniBarInfoUI : MonoBehaviour
{
  public TextMeshProUGUI resources;
  public TextMeshProUGUI population;
  public TextMeshProUGUI season;
  public TextMeshProUGUI income;

  private GameManager gameManagerInstance;
  private void Awake()
  {
    gameManagerInstance = GameObject.Find("Game Manager").GetComponent<GameManager>();
  }

  // YOU NEED TO ADD A INCOME FROM THE TILE TO CURRENTLY GATHERING, INCOME DEPENDS ON IT

  public void MiniBarInfoUpdate()
  {
    resources.text = "Resources:      " + gameManagerInstance.resources;
    population.text = "Population:      X/X" + gameManagerInstance.freeAnts + "/" + gameManagerInstance.totalAnts;
    income.text = "Income:           " + gameManagerInstance.currentlyGathering;

    //Currently Hardgecoded for Prototype
    season.text = "Spring";
  }


}