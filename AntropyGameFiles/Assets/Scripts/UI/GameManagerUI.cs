using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerUI : MonoBehaviour
{
    public int totalAnts;
    public int freeAnts;
    public int resources;
    public int currentSeason;
    public int currentWeather;

    public int hatcheryLevel;
    public int storageLevel;
    public int hatcheryMaxLevel;
    public int storageMaxLevel;

    public int[] hatcheryCost;
    public int[] storageCost;

  public GameObject redTile;
    public GameObject blackTile;

    public MapScript mapInstance;

    private void Awake()
    {
      mapInstance = GameObject.Find("MapTiles").GetComponent<MapScript>();
    }

    void Start()
    {
    //SpawnTiles(3,4);
      mapInstance.SpawnRandomMap();
      mapInstance.SpawnTerrainMap();
      hatcheryLevel = 0;
      storageLevel = 0;
      hatcheryMaxLevel = 3;
      storageMaxLevel = 3;
      hatcheryCost = new int[] { 200, 400, 600, 800 };
      storageCost = new int[] { 100, 200, 400, 600 };
  }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SpawnTiles(int nx, int nz) {

        GameObject tile;
        for (int i = 0; i < nx; i++){
            for (int j = 0; j < nz; j++){
                if ((i+j) % 2 == 0)
                {
                    tile = Instantiate(redTile, new Vector3(i*100, 0, j *100), redTile.transform.rotation);}
                else
                {
                    tile = Instantiate(blackTile, new Vector3(i*100, 0, j *100), redTile.transform.rotation);}

                ClickMenu clickMenu = tile.GetComponent<ClickMenu>();
                clickMenu.xPos = i;
                clickMenu.yPos = j;
                clickMenu.assignedAnts = 0;
                clickMenu.canvas = GameObject.Find("AssignAnts");
            }
        }
        GameObject.Find("AssignAnts").SetActive(false);
    }
}
