using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class GameManager : MonoBehaviour
{

    //general player properties
    public string playerName;
    public int totalAnts = 100;
    public int freeAnts;
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
