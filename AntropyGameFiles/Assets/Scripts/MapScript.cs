using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapScript : MonoBehaviour
{

  /// <summary>
  /// Singleton Game Map
  /// </summary>
  public static MapScript map_instance;

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
  TileScript[,] map_matrix;

  /// <summary>
  /// TilePrefabs: [0]stone, [1]grass, [2]soil, [3]water, [4]anthill
  /// </summary>
  public List<Transform> tile_prefabs = new List<Transform>();


  /// <summary>
  /// threshhold to update to grass
  /// </summary>
  public int grass_threshhold;

  /// <summary>
  /// threshhold to update to soil
  /// </summary>
  public int soil_threshold;

  private void Awake()
  {
    map_matrix = new TileScript[rows, columns];

    //Keep the instance alive
    map_instance = this;
    DontDestroyOnLoad(transform.gameObject);
  }

  /*
  private void Start()
  {
    SpawnRandomMap();
  }
  */

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
    GameObject.Find("AssignAnts").SetActive(false);
  }

  void CreateAnthillTile(int i, int j) 
  {
    int tile_type = 4;
    var tile_entry = Instantiate(tile_prefabs[tile_type], this.transform) as Transform;
    TileScript new_tile = tile_entry.GetComponent<TileScript>();
    new_tile.TileType = tile_type;
    new_tile.TileDistance = -1;
    new_tile.XPos = i;
    new_tile.ZPos = j;
    new_tile.name = (TileName(tile_type) + ": [" + i + "," + j + "]");

    //position of the spawn
    new_tile.transform.position = new Vector3(i, 0, j);

    //Assign ants on tile
    new_tile.AssignedAnts = 0;

    //Assign button canvas
    new_tile.CanvasAssign = GameObject.Find("AssignAnts");

    //save the script in the matrix
    map_matrix[i, j] = new_tile;
  }

  void RandomResourceTile(int i, int j, int distance_anthill) 
  {

    int tile_type = Random.Range(0, 4);
    var tile_entry = Instantiate(tile_prefabs[tile_type], this.transform) as Transform;
    TileScript new_tile = tile_entry.GetComponent<TileScript>();
    new_tile.TileType = tile_type;
    new_tile.TileDistance = distance_anthill;
    new_tile.name = (TileName(tile_type) + ": [" + i + "," + j + "]");
    new_tile.XPos = i;
    new_tile.ZPos = j;

    //Random Amount of resources on the tile
    if (new_tile.TileType == 1 || new_tile.TileType == 2)
    {
      new_tile.ResourceAmount = Random.Range(250, 500);
      new_tile.MaxResourceAmount = 650;
    }

    ////either the full mesh or just the material, for later use
    //new_tile.MeshRendererTile = tile_entry.GetComponent<MeshRenderer>();

    //position of the spawn
    new_tile.transform.position = new Vector3(i, 0, j);

    //Assign ants on tile
    new_tile.AssignedAnts = 0;

    //Assign button canvas
    new_tile.CanvasAssign = GameObject.Find("AssignAnts");

    //save the script in the matrix
    map_matrix[i, j] = new_tile;
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
      return map_matrix;
    }
  }

  /// <summary>
  /// Game Map, map_matrix getter
  /// </summary>
  public MapScript MapInstance
  {
    get
    {
      return map_instance;
    }
  }

  public void TileErrosionCheck(TileScript tile)
  {
    //exchange the whole prefab not just the material

    /*
    if(tile.TileType == 1) 
    { 
      if(tile.ResourceAmount < soil_threshold) 
      {
        //update to soil
        tile.MeshRendererTile.material = tile_material[2];
        tile.TileType = 2;
      }
    }
    else if (tile.TileType == 2)
    {
      if (tile.ResourceAmount >= grass_threshhold)
      {
        //update to gras
        tile.MeshRendererTile.material = tile_material[1];
        tile.TileType = 1;
      }
    }
     */
  }

}
