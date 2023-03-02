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


  private void Start()
  {
    showScore();
    replayButton.onClick.AddListener(ReplayGame);
  }

  void showScore() 
  {
    goal.text = "Goal: " + GameManager.Instance.currentGoalProgress  + "/" + GameManager.Instance.goal;
    turns.text = "Turns: " + GameManager.Instance.currentTurnCount;
    totalResources.text = "Total Resources: " + GameManager.Instance.TotalResources;
    totalAnts.text = "Total Population: " + GameManager.Instance.totalAnts;
    //deaths.text = "Deaths: " + gameManager.TotalDeaths;
    score.text = (GameManager.Instance.currentTurnCount + GameManager.Instance.TotalResources + GameManager.Instance.totalAnts - GameManager.Instance.TotalDeaths).ToString();
  }

  void ReplayGame() 
  {
    SceneManager.UnloadSceneAsync("Prototype_v3");
    SceneManager.LoadScene("Prototype_v3", LoadSceneMode.Single);
  }
}
