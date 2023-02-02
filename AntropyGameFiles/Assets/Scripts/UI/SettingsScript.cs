using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{

    public Slider sound;
    public Slider music;
    public Toggle antsMovement;
    public Toggle weatherEffects;
    public Toggle grassMovement;
    // Start is called before the first frame update
    void Start()
    {
        sound.onValueChanged.AddListener(delegate {UpdateSound(); });
        music.onValueChanged.AddListener(delegate {UpdateMusic(); });
        antsMovement.onValueChanged.AddListener(delegate {UpdateAntsMovement(); });
        weatherEffects.onValueChanged.AddListener(delegate {UpdateWeatherEffects(); });
        grassMovement.onValueChanged.AddListener(delegate {UpdateGrassMovementt(); });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateSound(){
        GameManager.Instance.soundVolume = sound.value;

    }

    void UpdateMusic(){
        GameManager.Instance.musicVolume = music.value;
    }

    void UpdateWeatherEffects(){
        GameManager.Instance.showWeatherEffects = weatherEffects.isOn;
        Debug.Log("weather Effects: " + weatherEffects.isOn);
    }
    void UpdateGrassMovementt(){
        GameManager.Instance.showGrassMovement = grassMovement.isOn;
    }
    void UpdateAntsMovement(){
        GameManager.Instance.showAntsMovement = antsMovement.isOn;
    }

}
