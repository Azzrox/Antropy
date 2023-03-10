using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreScreenUI : MonoBehaviour
{
  public TextMeshProUGUI Title; 
  public TextMeshProUGUI totalCollectedResources;
  public TextMeshProUGUI finalAnts;
  public TextMeshProUGUI finalResources;
  public TextMeshProUGUI cannibalizedAnts;
  public TextMeshProUGUI score;

  public Button replayButton; 


  private void Start()
  {
    showScore();
    replayButton.onClick.AddListener(ReplayGame);
  }

  void showScore() 
  { 
    if (GameManager.Instance.totalAnts <= 0)
    {
      Title.text = "Your state failed - you didn't survive the winter :(";
    }
    else{
       Title.text = "Congratulations - you survived your first winter!";
    }
    finalResources.text = "Final Resources: " + GameManager.Instance.resources;
    totalCollectedResources.text = "Collected Resources: " + GameManager.Instance.totalResources;
    finalAnts.text = "Final Population: " + GameManager.Instance.totalAnts;
    cannibalizedAnts.text = "Cannibalized Ants: " + GameManager.Instance.totalDeaths;
    score.text = "Final Score: " + (GameManager.Instance.resources + GameManager.Instance.totalResources + GameManager.Instance.totalAnts - GameManager.Instance.totalDeaths).ToString();
  }

  void ReplayGame() 
  {
    GameManager.Instance.GameRunning = false;
    GameManager.Instance.backtogame = 100;
    GameManager.Instance.resetGameVariables();
    SceneManager.LoadScene("Start");
  }
}
