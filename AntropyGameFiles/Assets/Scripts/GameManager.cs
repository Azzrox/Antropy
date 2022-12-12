using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{

    //general player properties
    public string playerName;
    public int totalAnts = 100;
    public int freeAnts = 100;
    //map specific properties
    public int rows;
    public int columns;
    public int[,] type;
    public int[,] assignedMapAnts;
    public int[,] maxAssignedMapAnts;
    public float[,] resourceAmount;
    public float[,] resouceMaxAmount;
    public bool[,] partOfAnthill;
    //anthill specific properties (assuming a fixed list of chambers)
    public int[] assignedHillAnts;


    public int resources;
    public int currentSeason;
    public int currentWeather;

    public int hatcheryLevel;
    public int storageLevel;
    public int hatcheryMaxLevel;
    public int storageMaxLevel;

    public int[] hatcheryCost;
    public int[] storageCost;
    
    public int maxAntsResourceTile;
    public int maxAntsAnthillTile;

    /// <summary>
    /// Current Turn Number
    /// </summary>
    public int MaxTurnCount;

    /// <summary>
    /// Max allowed turn number
    /// </summary>
    public int currentTurnCount;

    /// <summary>
    /// Weight for the grass creating closer to less = more grass closer
    /// </summary>
    public float grassWeight = 1.5f;
    /// <summary>
    /// Weight for the max resources and current resources on tile, more = more resources further away
    /// </summary>
    public float resourceWeight = 0.05f;

    /// <summary>
    /// Weight for the max resources and current resources on tile, less = less resources on soil tiles
    /// </summary>
    public float soilWeight = 0.3f;


  public MapScript mapInstance;
    public MapCameraScript cameraInstance;




  // Creates an instance that is present in all other classes
  public static GameManager Instance;
    //takes care that there is only one instance of GameManager
    private void Awake()
    {
        // start of new code
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        Instance = this;
        DontDestroyOnLoad(gameObject);
        // optional: load last gameplay (if saved)
        //LoadLastGame();

        mapInstance = GameObject.Find("MapTiles").GetComponent<MapScript>();
        cameraInstance = GameObject.Find("MapControls").GetComponent<MapCameraScript>();
  }

  [System.Serializable]
    class SaveData
    {
        // data to be saved (copy from above)
        // general player data
        public string playerName;
        public int totalAnts;
        public int freeAnts;
        //map specific properties
        public int[,] type;
        public int[,] assignedMapAnts;
        public int[,] maxAssignedMapAnts;
        public float[,] resourceAmount;
        public float[,] resouceMaxAmount;
        public bool[,] partOfAnthill;
        //anthill specific properties (assuming a fixed list of chambers)
        public int[] assignedHillAnts;

    }

    

    // optional: add filename from textInput
    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.playerName = playerName;
        data.totalAnts = totalAnts;
        //....
        string json = JsonUtility.ToJson(data);
    
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    // optional: add filename
    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            playerName = data.playerName;
            totalAnts = data.totalAnts;
            //....
        }   
    }
    // Start is called before the first frame update
    void Start()
    {
      //Anthill values
      hatcheryLevel = 0;
      storageLevel = 0;
      hatcheryMaxLevel = 3;
      storageMaxLevel = 3;
      hatcheryCost = new int[] { 200, 400, 600, 800 };
      storageCost = new int[] { 100, 200, 400, 600 };

      //TODO DELETE THIS, after we have an actual game
      //Current Default in case someone forgets
      MaxTurnCount = 1000;
  }

    // Update is called once per frame
    void Update()
    {
        
    }


    float DistanceToHill(int pos_x, int pos_y)
    {
        int nx = type.GetLength(0);
        int ny = type.GetLength(1);
        List<float> distances = new List<float>();
        for (int i = 0; i < nx; i++){
            for (int j = 0; j < ny; j++){
                if (partOfAnthill[i,j])
                {
                    distances.Add( Mathf.Sqrt( (pos_x - i)^2 + (pos_y - j)^2 ));
                }
            }
        }
        return distances.Min();
    }
}
