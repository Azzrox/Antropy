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

  public void MiniBarInfoUpdate()
  {
    resources.text = "Resources:      " + gameManagerInstance.resources + "/" + gameManagerInstance.maxResourceStorage;
    population.text = "Population:      " + gameManagerInstance.freeAnts + "/" + gameManagerInstance.totalAnts;
    income.text = "Income:           " + gameManagerInstance.income;

    //Currently Hardgecoded for Prototype
    season.text = "Spring / " + gameManagerInstance.weatherInstance.WeatherName(gameManagerInstance.currentWeather);
  }


}