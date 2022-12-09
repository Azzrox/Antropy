using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMenu : MonoBehaviour
{
    public int assignedAnts = 0;
    public int xPos;
    public int yPos;
    public GameObject canvas;


    // Start is called before the first frame update
    void Start()
    {
        assignedAnts = 0;
        
        Debug.Log("canvas found");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown(){
        //if (gameManager.isGameActive){
            Debug.Log("in click mode");
            canvas.SetActive(true);
            
            AntCounter antCounter = canvas.GetComponent<AntCounter>();
            antCounter.tile = this;
            antCounter.UpdateAntText();

            
            Debug.Log("element clicked" + Random.Range(0,40) + " pos: " + xPos + "|" + yPos);
           
            //antCounter.a
        //}
    }
}
