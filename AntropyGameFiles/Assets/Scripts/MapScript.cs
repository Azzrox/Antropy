using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

// Sets the script to be executed later than all default scripts
// This is helpful for the map / a UI, since other things may need to be initialized before setting the map / UI
// e.g. GameManager for initilizing rows and columns when directly tested in the map scene
[DefaultExecutionOrder(1000)]
public class MapScript : MonoBehaviour
{

  /// <summary>
  /// Matrix of the Map
  /// </summary>
  public TileScript[,] mapMatrix;

  /// <summary>
  /// TilePrefabs: [0]stone, [1]grass, [2]soil, [3]water, [4]anthill
  /// </summary>
  public List<Transform> tilePrefabs = new List<Transform>();

  public List<GameObject> roadPrefabs = new List<GameObject>();


  private void Awake()
  {
    Debug.Log("starts mapScript");
    mapMatrix = new TileScript[GameManager.Instance.rows, GameManager.Instance.columns];
  }

  void Start()
    {
      Debug.Log("Random Map created");
      GameManager.Instance.mapInstance = this;
      SpawnRandomMap();
      SpawnTerrainMap();    
      UpdateGrass();
      GameManager.Instance.WeightedDistanceToHill();

      

    }

  /// <summary>
  /// Random Map creation with no seed
  /// </summary>
  public void SpawnRandomMap()
  {
    
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      int distance_anthill = 0;
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        
        if(i == GameManager.Instance.anthillX && j == GameManager.Instance.anthillY) 
        {
          CreateAnthillTile(i, j);
        }
        else
        {
          WeightResourceTile(i, j, distance_anthill, GameManager.Instance.rows + GameManager.Instance.columns);
          //RandomResourceTile(i, j, distance_anthill);
          distance_anthill++;
        }
      }
    }
    GameObject.Find("AssignAnts").GetComponent<Canvas>().enabled = false;
    GameObject.Find("Anthill").GetComponent<Canvas>().enabled = false;
    
  }

    /// <summary>
    /// Map creation with added terrains
    /// </summary>
    /*
      public void SpawnTerrainMap()
      {
        int[,] adder = new int[,] {{-1, 0}, {0, -1}, {0, 1}, {1, 0}};
        for (int i = 0; i < GameManager.Instance.rows; i++)
        {
          for (int j = 0; j < GameManager.Instance.columns; j++)
          {

            int sameFound = 0;
            for (int k = 0; k < adder.Length/2; k++)
            {
              if (i + adder[k, 0] < GameManager.Instance.rows && i + adder[k, 0] >= 0 && j + adder[k, 1] < GameManager.Instance.columns && j + adder[k, 1] >= 0)
                if (GameManager.Instance.Map[i, j].type == GameManager.Instance.Map[i + adder[k, 0], j + adder[k, 1]].type)
                {
                  sameFound = 1;
                }
            }
            if(sameFound == 0 && (j != 0 || i != 0)) 
            { 
              if(j + 1 > GameManager.Instance.columns - 1) 
              {
                //ExchangeTilePrefab(mapMatrix[i, j], mapMatrix[i, j - 1].TileType);
                ExchangeTilePrefab(i,j,GameManager.Instance.Map[i, j - 1].type);
              }
              else 
              {
                //ExchangeTilePrefab(mapMatrix[i, j], mapMatrix[i, j + 1].TileType);
                ExchangeTilePrefab(i,j,GameManager.Instance.Map[i, j + 1].type);
              }
            }
            if (i < 3 && j < 3 && (j != 0 || i != 0))
            {
              //ExchangeTilePrefab(mapMatrix[i, j], 1);
              ExchangeTilePrefab(i,j,1);
              //SetExplored(mapMatrix[i, j], true);
              SetExplored(i,j, true);
              //SetVisible(mapMatrix[i, j], true);
              SetVisible(i,j, true);
            }
          }
        }
      }
     */
    public void SpawnTerrainMap()
    {
        int[,] adder = new int[,] { { -1, 0 }, { 0, -1 }, { 0, 1 }, { 1, 0 } };

        List<Vector2> anthillRadius = new List<Vector2>();
        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                
                anthillRadius.Add(new Vector2(i, j));
                
            }
        }

        anthillRadius.Add(new Vector2(2, 0));
        anthillRadius.Add(new Vector2(0, 2));
        anthillRadius.Add(new Vector2(0, -2));
        anthillRadius.Add(new Vector2(-2, 0));

        for (int i = 0; i < GameManager.Instance.rows; i++)
    {
        for (int j = 0; j < GameManager.Instance.columns; j++)
        {

            int sameFound = 0;
            for (int k = 0; k < adder.Length / 2; k++)
            {
                if (i + adder[k, 0] < GameManager.Instance.rows && i + adder[k, 0] >= 0 && j + adder[k, 1] < GameManager.Instance.columns && j + adder[k, 1] >= 0)
                    if (GameManager.Instance.Map[i, j].type == GameManager.Instance.Map[i + adder[k, 0], j + adder[k, 1]].type)
                    {
                        sameFound = 1;
                    }
            }
            if (sameFound == 0 && (j != GameManager.Instance.anthillY || i != GameManager.Instance.anthillX))
            {
                if (j + 1 > GameManager.Instance.columns - 1)
                {
                    //ExchangeTilePrefab(mapMatrix[i, j], mapMatrix[i, j - 1].TileType);
                    ExchangeTilePrefab(i, j, GameManager.Instance.Map[i, j - 1].type);
                }
                else
                {
                    //ExchangeTilePrefab(mapMatrix[i, j], mapMatrix[i, j + 1].TileType);
                    ExchangeTilePrefab(i, j, GameManager.Instance.Map[i, j + 1].type);
                }
            }
            
            /*if (j != GameManager.Instance.anthillX || i != GameManager.Instance.anthillY)
            {
                //ExchangeTilePrefab(mapMatrix[i, j], 1);
                ExchangeTilePrefab(i, j, 1);
                //SetExplored(mapMatrix[i, j], true);
                SetExplored(i, j, true);
                //SetVisible(mapMatrix[i, j], true);
                SetVisible(i, j, true);
            }*/
        }
    }

        initAnthillRadius(anthillRadius);
}

void initAnthillRadius(List<Vector2> anthillRadius)
{
        foreach(Vector2 point in anthillRadius)
        {
            //ExchangeTilePrefab(mapMatrix[i, j], 1);
            ExchangeTilePrefab((int)System.Math.Round(point.x + GameManager.Instance.anthillX), (int)System.Math.Round(point.y + GameManager.Instance.anthillY), 1);
            //SetExplored(mapMatrix[i, j], true);
            SetExplored((int)System.Math.Round(point.x + GameManager.Instance.anthillX), (int)System.Math.Round(point.y + GameManager.Instance.anthillY), true);
            //SetVisible(mapMatrix[i, j], true);
            SetVisible((int)System.Math.Round(point.x + GameManager.Instance.anthillX), (int)System.Math.Round(point.y + GameManager.Instance.anthillY), true);
        }
}

public void SetExplored(int posX, int posZ, bool explored)
  {
    GameManager.Instance.Map[posX, posZ].explored = explored;
    if (explored == true)
    {
      if(GameManager.Instance.Map[posX, posZ].type >= 5)
      {
        ExchangeTilePrefab(posX, posZ, GameManager.Instance.Map[posX, posZ].type - 5);
      }
      else
      {
        ExchangeTilePrefab(posX, posZ, GameManager.Instance.Map[posX, posZ].type); // TODO: delete since it's doing nothing
      }
    }
    //tile.GetComponentInChildren<MeshRenderer>().enabled = false;

  }

  public void SetVisible(int posX, int posZ, bool visible) {
    GameManager.Instance.Map[posX, posZ].visible = visible;
  }

  /// <summary>
  /// Creates the anthill tile
  /// </summary>
  /// <param name="i"></param>
  /// <param name="j"></param>
  void CreateAnthillTile(int i, int j) 
  {
    int tileType = 4;
    var tileEntry = Instantiate(tilePrefabs[tileType],
        new Vector3(i, 0, j), //position of the spawn
        Quaternion.identity, 
        this.transform) as Transform;
    TileScript newTile = tileEntry.GetComponent<TileScript>();
    
    GameManager.Instance.Map[i,j].type = tileType;
    GameManager.Instance.Map[i,j].tileName = (TileName(tileType) + ": [" + i + "," + j + "]");
    GameManager.Instance.Map[i,j].distanceAntHill = -1;
    GameManager.Instance.Map[i,j].explored = false;
    GameManager.Instance.Map[i,j].visible = false;
    GameManager.Instance.Map[i,j].explored = true;
    GameManager.Instance.Map[i,j].assignedAnts = 0;
    GameManager.Instance.Map[i,j].maxAssignedAnts = GameManager.Instance.maxAntsAnthillTile;
    GameManager.Instance.Map[i,j].constructionState = 6; // highway
    GameManager.Instance.Map[i,j].foodTransportCost = GameManager.Instance.transportCostVector[GameManager.Instance.Map[i,j].constructionState];
    GameManager.Instance.Map[i,j].fertilityState = 2; // no grow
    GameManager.Instance.Map[i,j].regrowResource = GameManager.Instance.regrowRateVector[GameManager.Instance.Map[i,j].fertilityState];
    
    newTile.XPos = i; 
    newTile.ZPos = j;

    //save the script in the matrix
    mapMatrix[i, j] = newTile;
  }

  void WeightResourceTile(int i, int j, int distance_anthill, int maxdist)
  {

    //weights
    int tileType = Random.Range(5, 9); //For normal map : int tileType = Random.Range(0, 4);
    //int tileType = Random.Range(0, 4);
    if (Random.Range(0, maxdist) > i+j + (maxdist) * GameManager.Instance.grassWeight)
    {
      tileType = 6;
      //tileType = 1; //Also needs to be in for normal map
    }

    var tileEntry = Instantiate(tilePrefabs[tileType],
        new Vector3(i, 0, j), //position of the spawn
        Quaternion.identity,
        this.transform) as Transform;
    TileScript newTile = tileEntry.GetComponent<TileScript>();

    GameManager.Instance.Map[i,j].type = tileType;
    GameManager.Instance.Map[i,j].tileName = (TileName(tileType) + ": [" + i + "," + j + "]");
    GameManager.Instance.Map[i,j].distanceAntHill = distance_anthill;
    GameManager.Instance.Map[i,j].explored = false;
    GameManager.Instance.Map[i,j].visible = false;
    GameManager.Instance.Map[i,j].explored = false;
    GameManager.Instance.Map[i,j].occupiedByPlayer = false;
    GameManager.Instance.Map[i,j].dominatedByPlayer = false;
    GameManager.Instance.Map[i,j].assignedAnts = 0;
    GameManager.Instance.Map[i,j].maxAssignedAnts = GameManager.Instance.maxAntsResourceTile;

    
    

    newTile.XPos = i;
    newTile.ZPos = j;

    //Random Amount of resources on the tile
    if (tileType == 6 || tileType == 7)
    {
      GameManager.Instance.Map[i,j].resourceMaxAmount = 300;
      SetGrassLand(i,j);
      
      for (int k = 0; k < i+j; k++)
      {
        GameManager.Instance.Map[i,j].resourceMaxAmount = (int)( GameManager.Instance.Map[i,j].resourceMaxAmount * (1 + GameManager.Instance.resourceWeight));
      }      
      GameManager.Instance.Map[i,j].resourceAmount = Random.Range(25,GameManager.Instance.Map[i,j].resourceMaxAmount);
      if (tileType == 7)
      {
        GameManager.Instance.Map[i,j].resourceAmount = (int)(GameManager.Instance.soilWeight *  GameManager.Instance.Map[i,j].resourceAmount);
      }
    } else if (tileType == 5) // rock
    {
      SetRock(i,j);
    }
    else if (tileType == 8) // water
    { 
      SetWater(i,j);
    }

    AssignFertilityRoad(i, j);

    //save the script in the matrix
    mapMatrix[i, j] = newTile;
  }
  void AssignFertilityRoad(int i, int j)
  {
    GameManager.Instance.Map[i,j].foodTransportCost = GameManager.Instance.transportCostVector[GameManager.Instance.Map[i,j].constructionState];
    GameManager.Instance.Map[i,j].regrowResource = GameManager.Instance.regrowRateVector[GameManager.Instance.Map[i,j].fertilityState];
    if(GameManager.Instance.Map[i,j].type == 1 || GameManager.Instance.Map[i,j].type == 2)
    {
      UpgradeFertilityColor(i,j,GameManager.Instance.Map[i,j].fertilityState);
    }
  }

  void SetRock(int i, int j)
  {
    GameManager.Instance.Map[i,j].constructionState = 1; // hard to cross (10 foodTransportCost)
    GameManager.Instance.Map[i,j].fertilityState = 2; // no regrow
  }
  void SetWater(int i, int j)
  {
    GameManager.Instance.Map[i,j].constructionState = 0; // not passable (99 foodTransportCost)
    GameManager.Instance.Map[i,j].fertilityState = 2; // no regrow
  }
  void SetGrassLand(int i, int j)
  {
    GameManager.Instance.Map[i,j].constructionState = 3; // normal (2 foodTransportCost)
    GameManager.Instance.Map[i,j].fertilityState = 5; // normal soil (20)
  }

  /// <summary>
  /// Returns tile name string from prefab type
  /// </summary>
  /// <param name="type"> [0]stone, [1]grass, [2]soil, [3]water, [4] anthill</param>
  /// <returns></returns>
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
  /// Game Map, map_matrix getter
  /// </summary>
  public TileScript[,] GameMap 
  {
    get
    {
      return mapMatrix;
    }
  }

  /// <summary>
  /// Tile Erosion 
  /// </summary>
  /// <param name="tile"></param>
  public void TileErosionCheck(int posX, int posZ)
  {
    if (GameManager.Instance.Map[posX, posZ].type == 1) 
    {
      if (GameManager.Instance.Map[posX, posZ].resourceAmount < GameManager.Instance.soilThreshold) 
      {
        //update to soil
        int newType = 2;

        


        //ExchangeTilePrefab(posX, posZ, newType);
        GameManager.Instance.Map[posX, posZ].type = newType;
        GameManager.Instance.Map[posX,posZ].tileName = (TileName(newType) + ": [" + posX + "," + posZ + "]");
        UpdatePrefab(posX, posZ, newType);
        UpdatePrefabAppearance(posX, posZ);

      }
    }
    else if (GameManager.Instance.Map[posX, posZ].type == 2)
    {
      if (GameManager.Instance.Map[posX, posZ].resourceAmount >= GameManager.Instance.grassThreshhold)
      {
        //update to grass
        int newType = 1;
        //ExchangeTilePrefab(posX, posZ, newType);
        GameManager.Instance.Map[posX, posZ].type = newType;
        GameManager.Instance.Map[posX,posZ].tileName = (TileName(newType) + ": [" + posX + "," + posZ + "]");
        UpdatePrefab(posX, posZ, newType);
        UpdatePrefabAppearance(posX, posZ);

      }
    }

    if ((GameManager.Instance.Map[posX, posZ].type == 1 || GameManager.Instance.Map[posX, posZ].type == 2) && GameManager.Instance.Map[posX, posZ].resourceAmount < 100)
    {
      // decrease fertility
      float pThreshold = (0.2f + GameManager.Instance.Map[posX, posZ].fertilityState * 0.15f) * (100 - GameManager.Instance.Map[posX, posZ].resourceAmount)/100;
      float randNumber = Random.value;
      Debug.Log("rand Number: " + randNumber + ", prob: " + pThreshold);
      if (Random.value < pThreshold)
      {
        GameManager.Instance.Map[posX, posZ].fertilityState = Mathf.Max(0, GameManager.Instance.Map[posX, posZ].fertilityState - 1);

        AssignFertilityRoad(posX, posZ);
        UpdatePrefabAppearance(posX, posZ);
      }
    }  
  }


  /// <summary>
  /// Exchanges a tiles prefab to the set type
  /// </summary>
  /// <param name="tile"></param>
  /// <param name="newTileType"></param>
  // NOTE: Only used at map creation!
  public void ExchangeTilePrefab(int posX, int posZ, int newTileType) 
  {
    GameManager.Instance.Map[posX, posZ].type = newTileType;
    GameManager.Instance.Map[posX,posZ].tileName = (TileName(newTileType) + ": [" + posX + "," + posZ + "]");
 
 
    UpdatePrefab(posX, posZ, newTileType);

    //Resources on the Tile only soil and grass can have resources
    if (newTileType == 1 || newTileType == 2) 
    {
      SetGrassLand(posX,posZ);
      GameManager.Instance.Map[posX, posZ].resourceMaxAmount = 250;
      for (int k = 0; k < posX + posZ; k++)
      {
        GameManager.Instance.Map[posX, posZ].resourceMaxAmount = (int)(GameManager.Instance.Map[posX, posZ].resourceMaxAmount * (1 + GameManager.Instance.resourceWeight));
      }
      GameManager.Instance.Map[posX, posZ].resourceAmount = Random.Range(100, GameManager.Instance.Map[posX, posZ].resourceMaxAmount);
      if (newTileType == 2)
      {
        GameManager.Instance.Map[posX, posZ].resourceAmount = (int)(GameManager.Instance.soilWeight * GameManager.Instance.Map[posX, posZ].resourceAmount);
      }
    }
    else 
    {
      GameManager.Instance.Map[posX, posZ].resourceMaxAmount = 0;
      GameManager.Instance.Map[posX, posZ].resourceAmount = 0;
      if(newTileType == 0 || newTileType == 5) // Rock
      {
        SetRock(posX,posZ);
      }
      else if(newTileType == 3 || newTileType == 8)
      {
        SetWater(posX,posZ);
      }
      
    }
    AssignFertilityRoad(posX, posZ);
    UpdatePrefabAppearance(posX, posZ);
    
   

  }

  public void UpdatePrefab(int posX, int posZ, int tileType)
  {
    var position = mapMatrix[posX, posZ].transform.position; // all tiles are stored in mapMatrix

    var tileEntry = Instantiate(tilePrefabs[tileType], position, Quaternion.identity, GameObject.Find("MapTiles").transform) as Transform;
    
    TileScript newTile = tileEntry.GetComponent<TileScript>();
    newTile.xPos = posX;
    newTile.zPos = posZ;
    Destroy(mapMatrix[posX, posZ].gameObject);

    mapMatrix[posX, posZ] = newTile;
    

  }

  public void UpdatePrefabAppearance(int posX, int posZ)
  {
    if(GameManager.Instance.Map[posX, posZ].type == 1 || GameManager.Instance.Map[posX, posZ].type == 2)
    { 
        if (GameManager.Instance.Map[posX, posZ].dominatedByPlayer)
      {
        mapMatrix[posX, posZ].spawnOwnedFlagOnTile();
      }
      if (GameManager.Instance.Map[posX, posZ].constructionState > 3)
      {
        mapMatrix[posX, posZ].spawnRoadOnTile(GameManager.Instance.Map[posX, posZ].constructionState - 4);
      }
       
      GameManager.Instance.mapInstance.mapMatrix[posX, posZ].GetComponent<MapTileGeneration>().RecalculateGrassDensity(GameManager.Instance.Map[posX, posZ].resourceAmount);

      UpgradeFertilityColor(posX, posZ, GameManager.Instance.Map[posX, posZ].fertilityState);
    }
  }


  public void UpdateGrass()
  {
    //yield return new WaitForSeconds(0.01f);
    //Insert Map Turn
    //change the tile object
    //GameManager.Instance.income = 0 - GameManager.Instance.Upkeep();
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
       
        // update visuals of grass tile
        if (GameManager.Instance.Map[i, j].type == 1 || GameManager.Instance.Map[i, j].type == 2)
        {
            GameManager.Instance.mapInstance.mapMatrix[i, j].GetComponent<MapTileGeneration>().RecalculateGrassDensity(GameManager.Instance.Map[i, j].resourceAmount);
        }
      
      }
    }
  }


    public void UpgradeRoadPrefab(int posX, int posZ, int level)
    {
        var mapTile = GameManager.Instance.mapInstance.mapMatrix[posX, posZ].gameObject;
        //Remove Grass from the middle
        if (level == 0)
        {
            GameManager.Instance.mapInstance.mapMatrix[posX, posZ].GetComponent<MapTileGeneration>().RemoveDecorationForRoads();
        }
        else if (level > 0) 
        {
            //Delete old road
            foreach (Transform child in mapTile.transform)
            {
                if (child.CompareTag("road"))
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
        }

        //instantiate new road
        var position = mapTile.transform.position;
        position += Vector3.up * 0.05f;

        var road = GameObject.Instantiate(roadPrefabs[level], position, Quaternion.identity, mapTile.transform);
    }

    public void UpgradeFertilityColor(int posX, int posZ, int level)
    {
        GameManager.Instance.mapInstance.mapMatrix[posX, posZ].GetComponent<MapTileGeneration>().UpdateFertilityColor(level);
    }

}
