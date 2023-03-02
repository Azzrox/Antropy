using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class SceneScript : MonoBehaviour
{
    
    public Button newGame; 
    public Button quitGame; 
    public Button saveGame; 
    public Button loadGame; 
    public Button openSettings;
    public Button SettingsOK;
    public Button backToGame;

    public Canvas Menu;
    public Canvas Settings;
    public Canvas backToMenu;

    private ExitToMenu exitToMenu;
    private InputAction menuExit;

    void Awake(){
        exitToMenu = new ExitToMenu();
    }
    // Start is called before the first frame update
    void Start()
    {
        newGame.onClick.AddListener(NewGame);
        quitGame.onClick.AddListener(QuitGame);
        saveGame.onClick.AddListener(SaveGame);
        loadGame.onClick.AddListener(LoadGame);
        SettingsOK.onClick.AddListener(BackToMenuOrGame);
        openSettings.onClick.AddListener(OpenSettings);
        backToGame.onClick.AddListener(BackToGame);
        if (GameManager.Instance.GameRunning)
        {
            backToMenu.enabled = true;
        }

        GameManager.Instance.playMusic(GameManager.Instance.mainMenuMusic);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGame(){
        // generate map if no map was loaded
        GameManager.Instance.GameRunning = false;
        GameManager.Instance.backtogame = 100;
        SceneManager.LoadScene("Prototype_v3 1");
        GameManager.Instance.playMusic(GameManager.Instance.springMusic);

    }

    public void QuitGame(){
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit(); // original code to quit Unity player
        #endif 
    }

    public void BackToMenuOrGame(){
        if (GameManager.Instance.GameRunning)
        {
            BackToGame();
        }
        else
        {
            Settings.enabled = false;
        Menu.enabled = true;
        }        
    }

    public void OpenMap(){
        SceneManager.LoadScene("Prototype_v3 1");
    }

    public void OpenMenu(){
        SceneManager.LoadScene("Start");
    }

    public void OpenSettings(){
        Settings.enabled = true;
        Menu.enabled = false;
        backToMenu.enabled = false;
    }

    public void SaveGame(){
        //SceneManager.LoadScene("Menu");
        SceneManager.LoadScene("Credits");
    }

    public void LoadGame(){
        //SceneManager.LoadScene("Menu");
    }
    public void BackToGame(){
        if(GameManager.Instance.backtogame > 0)
        {
          SceneManager.LoadScene("Prototype_v3 1");
        }
    }

}
