using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerUI : MonoBehaviour
{
    public int totalAnts;
    public int freeAnts;
    public int resources;
    public GameObject redTile;
    public GameObject blackTile;

  void Start()
    {
    //SpawnTiles(3,4);
    MapScript.mapInstance.SpawnRandomMap();
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
