using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine;

enum t_season { 
    SPRING = 0, 
    SUMMER = 1,
    FALL = 2,
    WINTER = 3
};

public class GameManager : MonoBehaviour
{

    //general player properties

    public struct Tile
    {
      public int type;
      public string tileName;
      public int assignedAnts;
      public int maxAssignedAnts;
      public int reservedResources;
      public float resourceAmount;
      public int resourceMaxAmount;
      public bool dominatedByPlayer;
      public bool occupiedByPlayer; 
      public bool partOfAnthill;
      public float distanceAntHill;
      public bool explored; 
      public bool visible;
      // regrowResource can also be negative
      public int regrowResource;
    /// <summary>
    /// fertility (0 - desert, 1 - steppe, 2 -  lean soil, 3 - normal soil, 4 - humus-rich soil, 5 - ant garden, 6 - ant paradise)
    /// </summary>
    public int fertilityState;
      public int constructionState;
      // (0 - not passable (water), 1 - hard-to-cross (rock), 2 - rough, 3 - normal plain land, 4 - ant path, 5 - ant street, 6 - ant highway )
      public float foodTransportCost;
    }

    public Tile[,] Map;
    [Header("Map properties")]
    public int rows;
    public int columns;

    public int anthillX;
    public int anthillY;
   
    public string playerName;
    [Header("Current population data")]
    public int totalAnts;
    public int freeAnts; 
    //anthill specific properties (assuming a fixed list of chambers)
    public int[] assignedHillAnts;
    //map specific properties

    /// <summary>
    /// Supported population cap
    /// </summary>
    public int currentMaximumPopulationCapacity;

    [Header("Current economic data")]
    /// <summary>
    /// Current resources of the player
    /// </summary>
    public int resources;

    /// <summary>
    /// Max storage the player can fill up
    /// </summary>
    public int maxResourceStorage;

    /// <summary>
    /// Income rate of food, tileIncome - Upkeep()
    /// </summary>
    public int income;

    /// <summary>
    /// new ants
    /// </summary>
    public int growth;
    
    [Header("Current Turn and Goal state")]
    /// <summary>
    /// Turn number
    /// </summary>
    public int currentTurnCount;

    public int currentGoalProgress;
    
    /// <summary>
    /// Goal threshold (resources, ants) for each week
    /// </summary>
    public (int,int)[] goalThreshholds;

    [Header("Difficulty Settings")]
    public int winterFoodUse;
    //public int weatherPrediction;


    [Header("Grow / degrow rates")]

    /// <summary>
    /// Food need per Ant
    /// </summary>
    public float foodPerAnt;

     /// <summary>
    /// ant growth per turn
    /// </summary>
    public float antPopGrowthPerTurn;

    /// <summary>
    /// Death of ants per turn due to lack of resources;
    /// </summary>
    public float antDeathLackofResourcesRate;

    /// <summary>
    /// Death of Ants per turn due to winter attrition
    /// </summary>
    public float antWinterDeathRate;

    /// <summary>
    /// Gathering distance deduction
    /// </summary>
    public float distanceGatheringReductionRate;
    
    /// <summary>
    /// Gather rate per ant
    /// </summary>
    public float resourceGatherRate;

    /// <summary>
    /// Regrow rate of tiles
    /// </summary>
    public int tileRegrowAmount;

    /// <summary>
    /// [0]Spring, [1]Summer, [2]Autumn, [3]Winter
    /// </summary>
    public int currentSeason;

    /// <summary>
    /// current audio source that is playing the current audio clip
    /// </summary>
    public static AudioSource currentAudioSource = new AudioSource();

    /// <summary>
    /// [0]sun, [1]rain, [2]overcast, [3]fog, [4] snow
    /// </summary>
    public int currentWeather;
     
    [Header("Fertility and transportation")]
    public int[] regrowRateVector;
    public float[] transportCostVector;

    public int[] fertilityUpgradeCost;
    public int[] transportUpgradeCost;


    [Header("Hatchery and Anthill")]
    /// <summary>
    /// Current player hatchery level
    /// </summary>
    public int hatcheryLevel;

    /// <summary>
    /// Current player storage level
    /// </summary>
    public int storageLevel;

    /// <summary>
    /// Hatchery max level bound
    /// </summary>
    public int hatcheryMaxLevel;

    /// <summary>
    /// Storage max level bound
    /// </summary>
    public int storageMaxLevel;

    /// <summary>
    /// Hatchery cost per level
    /// </summary>
    public int[] hatcheryCost;
    
    /// <summary>
    /// Storage cost per level
    /// </summary>
    public int[] storageCost;
    
    /// <summary>
    /// Capacity the player unlocks with each upgrade
    /// </summary>
    public int[] storageCapacityAmount;

    /// <summary>
    /// Population cap. player unlocks with each upgrade
    /// </summary>
    public int[] populationCapacityAmount;

    /// <summary>
    /// ResourceTile, max boundry of an tile
    /// </summary>
    public int maxAntsResourceTile;
    
    /// <summary>
    /// Anthill, max boundry of ants
    /// </summary>
    public int maxAntsAnthillTile;

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

    
    /// <summary>
    /// threshhold to update to grass
    /// </summary>
    public int grassThreshhold = 100;

    /// <summary>
    /// threshhold to update to soil
    /// </summary>
    public int soilThreshold = 80;

    [Header("Time and Scope limit")]
    //Turns
    /// <summary>
    /// Current Turn Number
    /// </summary>
    public int maxTurnCount = 120;

    /// <summary>
    /// Week Length
    /// </summary>
    public int weekLength = 10;

    /// <summary>
    /// Season Length for each season
    /// </summary>
    public int seasonLength = 30;

    /// <summary>
    /// Ingame Week, adjusted based on weekLength automatically
    /// </summary>
    public int currentWeek;

    //PrototypeGoal
    public int goal;

    /// <summary>
    /// Stops the Game from Updating values in Winter
    /// </summary>
    public bool gameOverWinter = false;

    [Header("EventSystem")]
    public int floodFertilityThreshhold = 3;
    public float antsLostFloodpercentage = 0.5f;
    public float droughtResourceAffectionRate = 0.5f;
    public int heavyFogAntsLostAmount = 5;
    public int lightFogAntsLostAmount = 2;

    [Header("MessageSystem Messages")]
    //Enables Messages
    public bool tutorialEnabled = false;
    public bool generalEnabled = false;
    public bool strategicEnabled = false;
    public bool warningEnabled = false;
    public bool eventEnabled = false;
    public bool winterEnabled = true;

  [Header("Statistics (cummulated data)")]

    public int totalResources;
    public int totalDeaths;

    [Header("Settings")]
    public float musicVolume;
    public float soundVolume;
    public bool showAntsMovement;
    public bool showWeatherEffects;
    public bool showGrassMovement;

    public bool GameRunning;


    public AudioClip summerMusic;
    public AudioClip autmnMusic;
    public AudioClip springMusic;
    public AudioClip winterMusic;
    public AudioClip mainMenuMusic;


    public MapScript mapInstance;
    public MapCameraScript cameraInstance;
    public WeatherScript weatherInstance;
    public MiniBarInfoUI miniBarInfoInstance;
    public NextTurnScript nextTurnInstance;
    public MessageScript messageSystemInstance;
    public WinterCountdownUI winterCountDownInstance;
    public EventScript eventInstance;
    public MessageSideBarUI messageSideBarUIInstance;




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

        // Fertility and transportation setups
        // fertility (0 - desert, 1 - steppe, 2 -  sandy soil, 3 - drained soil, 4 - modest soil, 5 - normal soil, 6 - humus-rich soil, 7 - ant garden, 8 - ant paradise)
        regrowRateVector = new int[] {-10, -5, 0, 5, 10, 20, 35, 55, 80};
        fertilityUpgradeCost = new int[] {10, 15, 20, 25, 50, 75, 100, 125};
        // construction states: (0 - not passable (water), 1 - hard-to-cross (rock), 2 - rough, 3 - normal plain land, 4 - ant path, 5 - ant street, 6 - ant highway )
        transportCostVector = new float[] {99, 10, 5, 2, 1, 0.5f, 0.1f};
        transportUpgradeCost = new int[] {1000, 100, 50, 50 , 50, 50};
        
        //adjust them accordingly, just a test
        goalThreshholds = new (int, int)[] {(3000,100), (3500, 250), (4000,300), (5000,400), (7000, 700), 
                                            (7000, 700) , (7000, 700) , (7000, 700)  , (7000, 700) , (7000, 700), (7000, 700), (7000, 700), (0, 0) };


        Map = new Tile[rows, columns];

        anthillX = (int) Mathf.Round(columns / 2);
        anthillY = (int) Mathf.Round(rows / 2);

        Debug.Log("Map created: " + Map[1,0].type + " before mapinstance is initialized");

        //mapInstance = GameObject.Find("MapTiles").GetComponent<MapScript>();
        //cameraInstance = GameObject.Find("MapControls").GetComponent<MapCameraScript>();
        //weatherInstance = GameObject.Find("Weather").GetComponent<WeatherScript>();
        //miniBarInfoInstance = GameObject.Find("MiniBarInfo").GetComponent<MiniBarInfoUI>();
        //nextTurnInstance = GameObject.Find("NextTurnCanvas").GetComponent<NextTurnScript>();

        currentAudioSource = GetComponent<AudioSource>();
        currentAudioSource.clip = mainMenuMusic;
        currentAudioSource.Play();
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
        public Tile[,] map;

    }

    // optional: add filename from textInput
    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.playerName = playerName;
        data.totalAnts = totalAnts;
        data.freeAnts = freeAnts;
        data.map = Map;
        string json = JsonUtility.ToJson(data);

        Debug.Log("Array_length: " + data.map.Length);
    
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

    public string TileName(int type) 
    {
      string type_name;

      /// TilePrefabs: [0]stone, [1]grass, [2]soil, [3]water, [4] anthill
      switch (type)
      {
        case 0:
          type_name = "Stone";
          break;
        case 1:
          type_name = "Grass";
          break;
        case 2:
          type_name = "Soil";
          break;
        case 3:
          type_name = "Water";
          break;
        case 4:
          type_name = "Anthill";
            break;
        case 5:
          type_name = "StoneBlack";
          break;
        case 6:
          type_name = "GrassBlack";
          break;
        case 7:
          type_name = "SoilBlack";
          break;
        case 8:
          type_name = "WaterBlack";
          break;
        default:
          type_name = "notSet";
          break;
      }
      return type_name;
    }

  /// <summary>
  /// Returns the season string
  /// </summary>
  /// <param name="season"></param>
  /// <returns></returns>
  public string SeasonName(int season)
  {
    switch (season)
    {
      case 0:
        return "Spring";
      case 1:
        return "Summer";
      case 2:
        return "Autumn";
      case 3:
        return "Winter";
      default:
        return "InvalidSeason";
    }
  }

  public Dictionary<int, string> FertilityNames = new Dictionary<int, string>()
    {{0, "Desert"},
     {1, "Steppe"},
     {2, "Sandy Soil"},
     {3, "Drained Soil"},
     {4, "Modest Soil"},
     {5, "Normal Soil"},
     {6, "Humus-rich Soil"},
     {7, "Ant Garden"},
     {8, "Ant Paradise"}
    };

    public Dictionary<int, string> StreetNames = new Dictionary<int, string>()
    {{0, "not passable"},
     {1, "hard to cross"},
     {2, "rough land"},
     {3, "normal plain land"},
     {4, "ant path"},
     {5, "ant street"},
     {6, "ant highway"}
    };
    


    // Start is called before the first frame update
    void Start()
    {
      //Anthill values
      hatcheryLevel = 0;
      storageLevel = 0;
      
      UpdateIncomeGrowth();
      hatcheryCost =             new int[] {200,  400, 600,   800,  1600, 3200, 4800, 5400, 5800, 6500, 7000 };
      storageCapacityAmount =    new int[] {350,  500, 1000,  1500, 2000, 2500, 3000, 3500, 4000, 5000, 7000 };
      storageCost =              new int[] {100,  200, 400,   600,  1200, 1800, 2400, 3000, 3600, 4200, 4800 };
      populationCapacityAmount = new int[] {250,  400, 550,   700,  1000, 1200, 1400, 2000, 2500, 3000, 6000};

      hatcheryMaxLevel = populationCapacityAmount.Length;
      storageMaxLevel = storageCapacityAmount.Length;
      //miniBarInfoInstance.MiniBarInfoUpdate();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetInitialValues(){

      totalAnts = 20;
      freeAnts = 20;
      currentMaximumPopulationCapacity = 70;
      resources = 0;
      maxResourceStorage = 150;
      income = 0;
      growth = 0;

      currentTurnCount = 0;
      currentGoalProgress = 0;
      

    }

    float DistanceToHill(int pos_x, int pos_y)
    {
        
        List<float> distances = new List<float>();
        for (int i = 0; i < rows; i++){
            for (int j = 0; j < columns; j++){
                if (Map[i,j].partOfAnthill)
                {
                    distances.Add( Mathf.Sqrt( (pos_x - i)^2 + (pos_y - j)^2 ));
                }
            }
        }
        return distances.Min();
    }

    public void WeightedDistanceToHill()
    {
      // start from anthill and reach out to all tiles
      // intialize weights
      float[,] weights = new float[rows, columns];
      for(int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          weights[i,j] = 100000f;
        }
      }
      int X = anthillX;
      int Y = anthillY;
      weights[X,Y] = 0;
      // go left
      for (int j = Y; j>= 0; j--)
      {
        int i = X;
        // check neighbors
        if (i < rows - 1)
        {weights[i + 1, j] = Mathf.Min(weights[i, j] +  GameManager.Instance.Map[i + 1, j].foodTransportCost,  weights[i + 1, j]);}
        if (i >0)
        {weights[i - 1, j]  = Mathf.Min(weights[i, j] +  GameManager.Instance.Map[i - 1, j].foodTransportCost,  weights[i - 1, j]);}
        if (j < columns - 1)
        {weights[i, j + 1] = Mathf.Min(weights[i, j] +  GameManager.Instance.Map[i, j + 1].foodTransportCost,  weights[i, j + 1]);}
        if (j > 0)
        {weights[i, j - 1] = Mathf.Min(weights[i, j] +  GameManager.Instance.Map[i, j - 1].foodTransportCost,  weights[i, j - 1]);}

      }
      Debug.Log("neighborhood: " + weights[X,Y] + " up: " + weights[X+1,Y] + ", down: " + weights[X-1, Y]);
      Debug.Log("neighborhood: " + weights[X,Y] + " right: " + weights[X,Y+1] + ", left: " + weights[X, Y-1]);
      for (int j = Y + 1; j < columns; j++)
      {
        int i = X;
        if (i < rows - 1)
        {weights[i + 1, j] = Mathf.Min(weights[i, j] +  GameManager.Instance.Map[i + 1, j].foodTransportCost,  weights[i + 1, j]);}
        if (i >0)
        {weights[i - 1, j]  = Mathf.Min(weights[i, j] +  GameManager.Instance.Map[i - 1, j].foodTransportCost,  weights[i - 1, j]);}
        if (j < columns - 1)
        {weights[i, j + 1] = Mathf.Min(weights[i, j] +  GameManager.Instance.Map[i, j + 1].foodTransportCost,  weights[i, j + 1]);}
        if (j > 0)
        {weights[i, j - 1] = Mathf.Min(weights[i, j] +  GameManager.Instance.Map[i, j - 1].foodTransportCost,  weights[i, j - 1]);}
      }
      for (int i = X + 1; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          if (i < rows - 1)
          {weights[i + 1, j] = Mathf.Min(weights[i, j] +  GameManager.Instance.Map[i + 1, j].foodTransportCost,  weights[i + 1, j]);}
          if (i >0)
          {weights[i - 1, j]  = Mathf.Min(weights[i, j] +  GameManager.Instance.Map[i - 1, j].foodTransportCost,  weights[i - 1, j]);}
          if (j < columns - 1)
          {weights[i, j + 1] = Mathf.Min(weights[i, j] +  GameManager.Instance.Map[i, j + 1].foodTransportCost,  weights[i, j + 1]);}
          if (j > 0)
          {weights[i, j - 1] = Mathf.Min(weights[i, j] +  GameManager.Instance.Map[i, j - 1].foodTransportCost,  weights[i, j - 1]);}
        }
      }
      for (int i = X - 1; i >= 0; i--)
      {
        for (int j = 0; j < columns; j++)
        {
          if (i < rows - 1)
          {weights[i + 1, j] = Mathf.Min(weights[i, j] +  GameManager.Instance.Map[i + 1, j].foodTransportCost,  weights[i + 1, j]);}
          if (i >0)
          {weights[i - 1, j]  = Mathf.Min(weights[i, j] +  GameManager.Instance.Map[i - 1, j].foodTransportCost,  weights[i - 1, j]);}
          if (j < columns - 1)
          {weights[i, j + 1] = Mathf.Min(weights[i, j] +  GameManager.Instance.Map[i, j + 1].foodTransportCost,  weights[i, j + 1]);}
          if (j > 0)
          {weights[i, j - 1] = Mathf.Min(weights[i, j] +  GameManager.Instance.Map[i, j - 1].foodTransportCost,  weights[i, j - 1]);}
        }
      }

      for(int i = 0; i < rows; i++)
      {
        for (int j = 0; j < columns; j++)
        {
          float weight =  100000f;
          if (i > 0){weight = Mathf.Min(weight, weights[i-1,j]);}
          if (i < rows - 1){weight = Mathf.Min(weight, weights[i+1,j]);}
          if (j > 0){weight = Mathf.Min(weight, weights[i,j-1]);}
          if (j < columns - 1){weight = Mathf.Min(weight, weights[i,j+1]);}
          GameManager.Instance.Map[i,j].distanceAntHill = weight;
        }
      }
    }

    public int Harvest()
    {
      int income = 0;
      for(int i = 0; i < rows; i++){
        for(int j = 0; j < columns; j++){
          // calculates the total amount of harvested food by the ants taking into account to projected regrow
          // NOTE: regrow rates may be dependent on the tile fertility (to be discussed)
          // SECOND NOTE: income may be reduced by the distance to the anthill via the formula
          // exp(- distance * distanceGatheringReductionRate)
          if (Map[i,j].type == 1 || Map[i,j].type == 2)
          {
            income += (int) Mathf.Round(Mathf.Min(Map[i,j].assignedAnts * resourceGatherRate, Map[i,j].resourceAmount) * Mathf.Exp(-Map[i,j].distanceAntHill * distanceGatheringReductionRate));
          }
        }
      }
      return income;
    }
    
    public int Upkeep()
    {
      int upkeep =  (int) Mathf.Ceil(totalAnts * foodPerAnt);
      if (totalAnts  > currentMaximumPopulationCapacity)
      {
        // To be be discussed
          upkeep += (int) Mathf.Ceil((totalAnts -  currentMaximumPopulationCapacity) * (foodPerAnt * 5));
      }
      return upkeep;
    }

    public void UpdateIncome()
    {
      // calculate income
      income = Harvest() - Upkeep();
    }

    public void UpdateGrowth()
    {
      growth = Juniors();
    }

    public void UpdateIncomeGrowth()
    {
      UpdateIncome();
      UpdateGrowth();
    }

    public int Juniors()
    {
    // use nurse ants == free ants
    if (resources <= 0 && income < 0)
    {
      if(currentSeason == 3) 
      {
        //Starving in Winter
        return -(int)(Mathf.Ceil(antDeathLackofResourcesRate) + (Mathf.Ceil(antWinterDeathRate)));
      }
      else
      {
        return -(int)Mathf.Ceil(antDeathLackofResourcesRate);
      }
    }
    else if(currentSeason == 3) 
    {
      return -(int)Mathf.Ceil(antWinterDeathRate);
    }
    else 
    {
      return (int)Mathf.Ceil(freeAnts * 3 * antPopGrowthPerTurn);
    } 
      // old: 
      //return (int)Mathf.Ceil((float)totalAnts * antPopGrowthPerTurn);
    }
    
  /// <summary>
  /// Ant Turn calculation for the winter season
  /// </summary>
  public void WinterAntTurn() 
  {
    //Resources
    income = -Upkeep();
    if(income > resources) 
    {
      resources = 0;
    }
    else 
    {
      resources += income;//Mathf.Clamp(resources- income, 0, 1);
    }
    totalResources += income;

    //Population
    if (resources <= 0) 
    {
      AntDeath((int)Mathf.Ceil(antDeathLackofResourcesRate));
      AntDeath((int)Mathf.Ceil(antWinterDeathRate));
    }
    else 
    {
      AntDeath((int)Mathf.Ceil(antWinterDeathRate));
    }
  }

  public void AntDeath(int amount) 
  {
    totalAnts -= amount;
    UpdateGrowth();
  }

    /// <summary>
    /// Prototype end screen (Remove Later)
    /// </summary>
    public void prototypeGoalCheck() 
    { 
      if(currentGoalProgress >= goal || currentTurnCount >= maxTurnCount) 
      {
        SceneManager.LoadScene("PrototypeEndScreen", LoadSceneMode.Additive);
      }
      
    }
  public void prototypeLooseCheck() 
  {
    if (totalAnts < 1)
    {
      if(currentSeason == 3) 
      {
        gameOverWinter = true;
        GameObject.Find("WinterCountdownSystem").SetActive(false);
      }
      SceneManager.LoadScene("PrototypeEndScreen", LoadSceneMode.Additive);
    }
  }

  /// <summary>
  /// check if a new week has
  /// </summary>
  public void adjustWeek() 
  { 
    if(currentTurnCount % weekLength == 0) 
    {
      currentWeek++;
    } 
  }

  public void checkSeasonChange() 
  {
    currentSeason = Mathf.CeilToInt(currentTurnCount / seasonLength);
  }

    /// <summary>
    /// Endscore resource counter
    /// </summary>
    public int TotalResources
    {
      get
      {
        return totalResources;
      }
      set
      {
        totalResources = value;
      }
    }

    /// <summary>
    /// Endscore death counter
    /// </summary>
    public int TotalDeaths
    {
      get
      {
        return totalDeaths;
      }
      set
      {
        totalDeaths = value;
      }
    }
}
