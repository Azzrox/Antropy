using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MapScript : MonoBehaviour
{
  /// <summary>
  /// rows map
  /// </summary>
  public int rows;

  /// <summary>
  /// columns map
  /// </summary>
  public int columns;

  /// <summary>
  /// Matrix of the Map
  /// </summary>
  TileScript[,] mapMatrix;

  /// <summary>
  /// TilePrefabs: [0]stone, [1]grass, [2]soil, [3]water, [4]anthill
  /// </summary>
  public List<Transform> tilePrefabs = new List<Transform>();

  /// <summary>
  /// threshhold to update to grass
  /// </summary>
  public int grassThreshhold;

  /// <summary>
  /// threshhold to update to soil
  /// </summary>
  public int soilThreshold;

  /// <summary>
  /// Game Manager Scene Instance
  /// </summary>
  GameManager gameManagerInstance;
  

  private void Awake()
  {
    gameManagerInstance = GameObject.Find("Game Manager").GetComponent<GameManager>();
    rows = gameManagerInstance.rows;
    columns = gameManagerInstance.columns;
    mapMatrix = new TileScript[gameManagerInstance.rows, gameManagerInstance.columns];
  }

  /// <summary>
  /// Random Map creation with no seed
  /// </summary>
  public void SpawnRandomMap()
  {
    for (int i = 0; i < rows; i++)
    {
      int distance_anthill = 0;
      for (int j = 0; j < columns; j++)
      {
        if(i == 0 && j == 0) 
        {
          CreateAnthillTile(i, j);
        }
        else 
        {
          WeightResourceTile(i, j, distance_anthill, rows + columns);
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
    for (int i = 0; i < rows; i++)
    {
      for (int j = 0; j < columns; j++)
      {
        
        int sameFound = 0;
        for (int k = 0; k < adder.Length/2; k++)
        {
          if (i + adder[k, 0] < rows && i + adder[k, 0] >= 0 && j + adder[k, 1] < columns && j + adder[k, 1] >= 0)
            if (mapMatrix[i, j].TileType == mapMatrix[i + adder[k, 0], j + adder[k, 1]].TileType)
            {
              sameFound = 1;
            }
        }
        if(sameFound == 0 && (j != 0 || i != 0)) 
        { 
          if(j + 1 > columns - 1) 
          {
            ExchangeTilePrefab(mapMatrix[i, j], mapMatrix[i, j - 1].TileType);
          }
          else 
          {
            ExchangeTilePrefab(mapMatrix[i, j], mapMatrix[i, j + 1].TileType);
          }
        }
        if (i < 3 && j < 3 && (j != 0 || i != 0))
        {
          ExchangeTilePrefab(mapMatrix[i, j], 1);
          SetExplored(mapMatrix[i, j], true);
          SetVisible(mapMatrix[i, j], true);
        }
      }
    }
  }

  public void SetExplored(TileScript tile, bool explored)
  {
    tile.Explored = explored;
    if (tile.Explored == true)
    {
      if(tile.TileType >= 5)
      {
        ExchangeTilePrefab(mapMatrix[tile.XPos, tile.ZPos], tile.TileType - 5);
      }
      else
      {
        ExchangeTilePrefab(mapMatrix[tile.XPos, tile.ZPos], tile.TileType);
      }
    }
    //tile.GetComponentInChildren<MeshRenderer>().enabled = false;

  }
  public void SetVisible(TileScript tile, bool visible)
  {
    tile.Visible = visible;
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
    newTile.TileType = tileType;
    newTile.TileDistance = -1;
    newTile.XPos = i;
    newTile.ZPos = j;
    newTile.name = (TileName(tileType) + ": [" + i + "," + j + "]");
    newTile.IsAntHill = true;
    newTile.Explored = false;
    newTile.Visible = false;

    //position of the spawn
    newTile.transform.position = new Vector3(i, 0, j);

    //Assign ants on tile
    newTile.AssignedAnts = 0;

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
    newTile.TileType = tileType;
    newTile.TileDistance = distance_anthill;
    newTile.name = (TileName(tileType) + ": [" + i + "," + j + "]");
    newTile.XPos = i;
    newTile.ZPos = j;
    newTile.IsAntHill = false;

    //Random Amount of resources on the tile
    if (newTile.TileType == 1 || newTile.TileType == 2)
    {
      newTile.ResourceAmount = Random.Range(250, 500);
      newTile.MaxResourceAmount = 650;
    }

    //position of the spawn
    newTile.transform.position = new Vector3(i, 0, j);

    //Assign ants on tile
    newTile.AssignedAnts = 0;

    //save the script in the matrix
    mapMatrix[i, j] = newTile;
  }

  void WeightResourceTile(int i, int j, int distance_anthill, int maxdist)
  {

    //weights
    int basemax = 500;
    int tileType = Random.Range(5, 9); //For normal map : int tileType = Random.Range(0, 4);
    //int tileType = Random.Range(0, 4);
    if (Random.Range(0, maxdist) > i+j + (maxdist) * GameManager.Instance.grassWeight)
    {
      tileType = 6;
      //tileType = 1; //Also needs to be in for normal map
    }

    var tileEntry = Instantiate(tilePrefabs[tileType], this.transform) as Transform;
    TileScript newTile = tileEntry.GetComponent<TileScript>();
    newTile.TileType = tileType;
    newTile.TileDistance = distance_anthill;
    newTile.name = (TileName(tileType) + ": [" + i + "," + j + "]");
    newTile.XPos = i;
    newTile.ZPos = j;
    newTile.IsAntHill = false;
    newTile.Explored = false;
    newTile.Visible = false;

    //Random Amount of resources on the tile
    if (newTile.TileType == 6 || newTile.TileType == 7)
    {
      newTile.MaxResourceAmount = 500;
      for (int k = 0; k < i+j; k++)
      {
        newTile.MaxResourceAmount = (int)(newTile.MaxResourceAmount + (newTile.MaxResourceAmount * GameManager.Instance.resourceWeight));
      }
      newTile.ResourceAmount = Random.Range(250, newTile.MaxResourceAmount);
      if (newTile.TileType == 7)
      {
        newTile.ResourceAmount = (int)(GameManager.Instance.soilWeight * newTile.ResourceAmount);
      }
    }

    //position of the spawn
    newTile.transform.position = new Vector3(i, 0, j);

    //Assign ants on tile
    newTile.AssignedAnts = 0;

    //save the script in the matrix
    mapMatrix[i, j] = newTile;
  }

  /// <summary>
  /// Returns tile name string from prefab type
  /// </summary>
  /// <param name="type"> [0]stone, [1]grass, [2]soil, [3]water, [4] anthill</param>
  /// <returns></returns>
  string TileName(int type) 
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
  public void TileErosionCheck(TileScript tile)
  {
    if (tile.TileType == 1) 
    {
      if (tile.ResourceAmount < soilThreshold) 
      {
        //update to soil
        int newType = 2;
        ExchangeTilePrefab(tile, newType);
      }
    }
    else if (tile.TileType == 2)
    {
      if (tile.ResourceAmount >= grassThreshhold)
      {
        //update to gras
        int newType = 1;
        ExchangeTilePrefab(tile, newType);
      }
    }
  }

  /// <summary>
  /// Exchanges a tiles prefab to the set type
  /// </summary>
  /// <param name="tile"></param>
  /// <param name="newTileType"></param>
  public void ExchangeTilePrefab(TileScript tile, int newTileType) 
  {
    Debug.Log(newTileType);
    Debug.Log(tilePrefabs[newTileType]);
    var tileEntry = Instantiate(tilePrefabs[newTileType], GameObject.Find("MapTiles").transform) as Transform;
    tileEntry.name = (TileName(newTileType) + ": [" + tile.XPos + "," + tile.ZPos + "]");
    tileEntry.position = tile.transform.position;

    TileScript newTile = tileEntry.GetComponent<TileScript>();
    newTile.TileType = newTileType;
    newTile.TileDistance = tile.TileDistance;
    newTile.XPos = tile.XPos;
    newTile.ZPos = tile.ZPos;
    newTile.Explored = tile.Explored;
    newTile.Visible = tile.Visible;

    newTile.ResourceAmount = tile.ResourceAmount;
    
    newTile.FreeAnts = tile.FreeAnts;
    newTile.AssignedAnts = tile.AssignedAnts;
    
    //old
    //newTile.canvas = tile.canvas;

    //Resources on the Tile only soil and grass can have resources
    if(newTileType == 1 || newTileType == 2) 
    {
      newTile.MaxResourceAmount = 500;
      for (int k = 0; k < newTile.XPos + newTile.ZPos; k++)
      {
        newTile.MaxResourceAmount = (int)(newTile.MaxResourceAmount + (newTile.MaxResourceAmount * GameManager.Instance.resourceWeight));
      }
      newTile.ResourceAmount = Random.Range(250, newTile.MaxResourceAmount);
      if (newTile.TileType == 2)
      {
        newTile.ResourceAmount = (int)(GameManager.Instance.soilWeight * newTile.ResourceAmount);
      }
    }
    else 
    {
      newTile.MaxResourceAmount = 0;
      newTile.ResourceAmount = 0;
    }
    
    mapMatrix[tile.XPos, tile.ZPos] = tileEntry.GetComponent<TileScript>();
    Destroy(tile.gameObject);
  }
}
