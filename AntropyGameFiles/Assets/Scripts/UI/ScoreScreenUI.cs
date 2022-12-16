using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreScreenUI : MonoBehaviour
{
  public TextMeshProUGUI goal;
  public TextMeshProUGUI turns;
  public TextMeshProUGUI totalResources;
  public TextMeshProUGUI totalAnts;
  public TextMeshProUGUI deaths;
  public TextMeshProUGUI score;

  public Button replayButton; 

  private GameManager gameManager;

  private void Awake()
  {
    gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
  }

  private void Start()
  {
    showScore();
    replayButton.onClick.AddListener(ReplayGame);
  }

  void showScore() 
  {
    goal.text = "Goal: " + gameManager.currentGoalProgress  + "/" +  gameManager.goal;
    turns.text = "Turns: " + gameManager.currentTurnCount;
    totalResources.text = "Total Resources: " + gameManager.TotalResources;
    totalAnts.text = "Total Population: " + gameManager.totalAnts;
    //deaths.text = "Deaths: " + gameManager.TotalDeaths;
    score.text = (gameManager.currentTurnCount + gameManager.TotalResources + gameManager.totalAnts - gameManager.TotalDeaths).ToString();
  }

  void ReplayGame() 
  {
    SceneManager.UnloadSceneAsync("Prototype_v3");
    SceneManager.LoadScene("Prototype_v3", LoadSceneMode.Single);
  }
}
