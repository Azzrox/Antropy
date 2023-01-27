using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreenUI : MonoBehaviour
{
  public Button playButton;
  void Start()
    {
    playButton.onClick.AddListener(StartNewGame);
  }

  void StartNewGame() 
  {
    SceneManager.LoadScene("Prototype_v3 1", LoadSceneMode.Single);
  }
}
