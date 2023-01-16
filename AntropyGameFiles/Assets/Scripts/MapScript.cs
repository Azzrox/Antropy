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


  private void Awake()
  {
    Debug.Log("starts mapScript");
    mapMatrix = new TileScript[GameManager.Instance.rows, GameManager.Instance.columns];
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
        if(i == 0 && j == 0) 
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
    var tileEntry = Instantiate(tilePrefabs[tileType], this.transform) as Transform;
    TileScript newTile = tileEntry.GetComponent<TileScript>();
    
    GameManager.Instance.Map[i,j].type = tileType;
    GameManager.Instance.Map[i,j].tileName = (TileName(tileType) + ": [" + i + "," + j + "]");
    GameManager.Instance.Map[i,j].distanceAntHill = -1;
    GameManager.Instance.Map[i,j].explored = false;
    GameManager.Instance.Map[i,j].visible = false;
    GameManager.Instance.Map[i,j].explored = false;
    GameManager.Instance.Map[i,j].assignedAnts = 0;
    GameManager.Instance.Map[i,j].maxAssignedAnts = GameManager.Instance.maxAntsAnthillTile;
    
    newTile.XPos = i; 
    newTile.ZPos = j;
    
    //position of the spawn
    newTile.transform.position = new Vector3(i, 0, j);

    //save the script in the matrix
    mapMatrix[i, j] = newTile;
  }

  /// <summary>
  /// Creates a Random Resource Tile
  /// </summary>
  /// <param name="i"></param>
  /// <param name="j"></param>
  /// <param name="distance_anthill">Eucledian Distance</param>
  void RandomResourceTile(int i, int j, int distance_anthill) 
  {

    //weights
    int tileType = Random.Range(0, 3);
    if(Random.Range(0,11) > 7) 
    {
      tileType = 3;
    }

    var tileEntry = Instantiate(tilePrefabs[tileType], this.transform) as Transform;
    TileScript newTile = tileEntry.GetComponent<TileScript>();

    GameManager.Instance.Map[i,j].type = tileType;
    GameManager.Instance.Map[i,j].tileName = (TileName(tileType) + ": [" + i + "," + j + "]");
    GameManager.Instance.Map[i,j].distanceAntHill = distance_anthill;
    GameManager.Instance.Map[i,j].explored = false;
    GameManager.Instance.Map[i,j].visible = false;
    GameManager.Instance.Map[i,j].explored = false;
    GameManager.Instance.Map[i,j].assignedAnts = 0;

    newTile.XPos = i;
    newTile.ZPos = j;

    //Random Amount of resources on the tile
    if (tileType == 1 || tileType == 2)
    {
      GameManager.Instance.Map[i,j].resourceAmount = Random.Range(50,300);
      GameManager.Instance.Map[i,j].resourceMaxAmount = 300;


    }

    //position of the spawn
    newTile.transform.position = new Vector3(i, 0, j);

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

    var tileEntry = Instantiate(tilePrefabs[tileType], this.transform) as Transform;
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
      for (int k = 0; k < i+j; k++)
      {
        GameManager.Instance.Map[i,j].resourceMaxAmount = (int)( GameManager.Instance.Map[i,j].resourceMaxAmount * (1 + GameManager.Instance.resourceWeight));
      }      
      GameManager.Instance.Map[i,j].resourceAmount = Random.Range(25,GameManager.Instance.Map[i,j].resourceMaxAmount);
      if (tileType == 7)
      {
        GameManager.Instance.Map[i,j].resourceAmount = (int)(GameManager.Instance.soilWeight *  GameManager.Instance.Map[i,j].resourceAmount);
      }
    }

    //position of the spawn
    newTile.transform.position = new Vector3(i, 0, j);

    //save the script in the matrix
    mapMatrix[i, j] = newTile;
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
        ExchangeTilePrefab(posX, posZ, newType);
      }
    }
    else if (GameManager.Instance.Map[posX, posZ].type == 2)
    {
      if (GameManager.Instance.Map[posX, posZ].resourceAmount >= GameManager.Instance.grassThreshhold)
      {
        //update to grass
        int newType = 1;
        ExchangeTilePrefab(posX, posZ, newType);
      }
    }
  }


  /// <summary>
  /// Exchanges a tiles prefab to the set type
  /// </summary>
  /// <param name="tile"></param>
  /// <param name="newTileType"></param>
  public void ExchangeTilePrefab(int posX, int posZ, int newTileType) 
  {
    GameManager.Instance.Map[posX, posZ].type = newTileType;
    GameManager.Instance.Map[posX,posZ].tileName = (TileName(newTileType) + ": [" + posX + "," + posZ + "]");

    // flag should be already there

    // exchange prefab at tile!!!
    // TODO!
    UpdatePrefab(posX, posZ, newTileType);
    
    

    //Resources on the Tile only soil and grass can have resources
    if (newTileType == 1 || newTileType == 2) 
    {
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
    }
    
    
  }

  public void UpdatePrefab(int posX, int posZ, int tileType)
  {

    var tileEntry = Instantiate(tilePrefabs[tileType], GameObject.Find("MapTiles").transform) as Transform;
    
    tileEntry.position = mapMatrix[posX, posZ].transform.position; // all tiles are stored in mapMatrix
    

    TileScript newTile = tileEntry.GetComponent<TileScript>();
    newTile.xPos = posX;
    newTile.zPos = posZ;
    Destroy(mapMatrix[posX, posZ].gameObject);

    mapMatrix[posX, posZ] = newTile;
    if (GameManager.Instance.Map[posX, posZ].occupiedByPlayer)
    {
      mapMatrix[posX, posZ].spawnOwnedFlagOnTile();
    }
  }


}
