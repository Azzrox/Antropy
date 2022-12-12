using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapScript : MonoBehaviour
{

  /// <summary>
  /// Singleton Game Map
  /// </summary>
  public static MapScript mapInstance;

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

  private void Awake()
  {
    mapMatrix = new TileScript[rows, columns];

    //Keep the instance alive
    mapInstance = this;
    DontDestroyOnLoad(transform.gameObject);
  }

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
          RandomResourceTile(i, j, distance_anthill);
          distance_anthill++;
        }
      }
    }
  }

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

    //position of the spawn
    newTile.transform.position = new Vector3(i, 0, j);

    //Assign ants on tile
    newTile.AssignedAnts = 0;

    //save the script in the matrix
    mapMatrix[i, j] = newTile;
  }

  void RandomResourceTile(int i, int j, int distance_anthill) 
  {

    int tileType = Random.Range(0, 4);
    var tileEntry = Instantiate(tilePrefabs[tileType], this.transform) as Transform;
    TileScript newTile = tileEntry.GetComponent<TileScript>();
    newTile.TileType = tileType;
    newTile.TileDistance = distance_anthill;
    newTile.name = (TileName(tileType) + ": [" + i + "," + j + "]");
    newTile.XPos = i;
    newTile.ZPos = j;

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
  /// Game Map, map_matrix getter
  /// </summary>
  public MapScript MapInstance
  {
    get
    {
      return mapInstance;
    }
  }

  public void TileErrosionCheck(TileScript tile)
  {
    //exchange the whole prefab not just the material

    Debug.Log("Type" + tile.TileType);

    if (tile.TileType == 1) 
    {
      Debug.Log("Soil: Test if nullptr" + tile.ResourceAmount);
      if (tile.ResourceAmount < soilThreshold) 
      {
        //update to soil
        //tile.MeshRendererTile.material = tile_material[2];
        //tile.TileType = 2;
        int newType = 2;
        var tileEntry = Instantiate(tilePrefabs[newType], GameObject.Find("MapTiles").transform) as Transform;
        TileScript newTile = tileEntry.GetComponent<TileScript>();
        tileEntry.name = (TileName(newType) + ": [" + tile.XPos + "," + tile.ZPos + "]");
        tileEntry.position = tile.transform.position;

        newTile.TileType = newType;
        newTile.TileDistance = tile.TileDistance;
        newTile.XPos = tile.XPos;
        newTile.ZPos = tile.ZPos;
        newTile.ResourceAmount = tile.ResourceAmount;
        newTile.MaxResourceAmount = tile.MaxResourceAmount;

        mapMatrix[tile.XPos, tile.ZPos] = tileEntry.GetComponent<TileScript>();
        Destroy(tile.gameObject);
        Debug.Log("To Soil: Test if nullptr" + MapScript.mapInstance.GameMap[tile.XPos, tile.ZPos].TileType);
        //return;
      }
    }
    else if (tile.TileType == 2)
    {
      Debug.Log("Grass: Test if nullptr" + tile.ResourceAmount);
      if (tile.ResourceAmount >= grassThreshhold)
      {
        //update to gras
        int newType = 1;
        var tileEntry = Instantiate(tilePrefabs[newType], GameObject.Find("MapTiles").transform) as Transform;
        tileEntry.name = (TileName(newType) + ": [" + tile.XPos + "," + tile.ZPos + "]");
        tileEntry.position = tile.transform.position;

        TileScript newTile = tileEntry.GetComponent<TileScript>();
        newTile.ResourceAmount = tile.ResourceAmount;
        newTile.TileType = newType;
        newTile.TileDistance = tile.TileDistance;
        newTile.XPos = tile.XPos;
        newTile.ZPos = tile.ZPos;
        newTile.MaxResourceAmount = tile.MaxResourceAmount;
        newTile.FreeAnts = tile.FreeAnts;
        newTile.AssignedAnts = tile.AssignedAnts;

        //Debug.Log("Test if nullptr" + newTile.TileType);
        mapMatrix[tile.XPos, tile.ZPos] = tileEntry.GetComponent<TileScript>();
        Destroy(tile.gameObject);
        Debug.Log("To Grass: Test if nullptr" + MapScript.mapInstance.GameMap[tile.XPos, tile.ZPos].TileType);
        //Destroy(tile.gameObject);
        //return;
      }
    }
  }

}
