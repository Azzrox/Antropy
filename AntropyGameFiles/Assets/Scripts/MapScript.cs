using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapScript : MonoBehaviour
{

  /// <summary>
  /// Singleton Game Map
  /// </summary>
  public static MapScript MapInstance;

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
  /// TilePrefabs: [0]stone, [1]grass, [2]soil, [3]water
  /// </summary>
  public List<Transform> tile_prefabs = new List<Transform>();

  private void Awake()
  {
    map_matrix = new TileScript[rows, columns];

    //Keep the instance alive
    MapInstance = this;
    DontDestroyOnLoad(transform.gameObject);
  }

  private void Start()
  {
    spawnRandomMap();
    
  }

  void spawnRandomMap()
  {
    for (int i = 0; i < rows; i++)
    {
      for (int j = 0; j < columns; j++)
      {
        int tile_type = Random.Range(0, 3);
        var tile_entry = Instantiate(tile_prefabs[tile_type]) as Transform;
        TileScript new_tile = tile_entry.GetComponent<TileScript>();
        new_tile.TileType = tile_type;

        if (new_tile.TileType == 1 || new_tile.TileType == 3)
        {
          new_tile.ResourceAmount = Random.Range(250, 1000);
        }

        //either the full mesh or just the material, for later use
        new_tile.MeshRendererTile = tile_entry.GetComponent<MeshRenderer>();

        //position of the spawn
        new_tile.transform.position = new Vector3(i, 0, j);

        //save the script in the matrix
        map_matrix[i, j] = new_tile;
      }
    }
  }

  public TileScript[,] GameMap 
  {
    get
    {
      return map_matrix;
    }
  }
}
