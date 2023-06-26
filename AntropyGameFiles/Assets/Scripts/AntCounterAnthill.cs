using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AntCounterAnthill : MonoBehaviour
{
   public int antsAssigned = 0;
   public RoomSelectScript room;
   public Button plusButton;
   public Button minusButton;
   public Button confirmButton;
   public TextMeshProUGUI freeAnts;
   public TextMeshProUGUI assignedAntsText;
   //private GameManagerAnthillScript gameManager;
   GameManager gameManager;

   private void Awake()
   {
      gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
   }

   // Start is called before the first frame update
   void Start()
   {
      plusButton.onClick.AddListener(IncreaseAnts);
      minusButton.onClick.AddListener(DecreaseAnts);
      confirmButton.onClick.AddListener(Confirm);
      // UpdateAntText();
   }

   // Update is called once per frame
   void Update()
   {

   }

   void IncreaseAnts()
   {
      int freeAnts = gameManager.freeAnts;
      if (freeAnts > 0)
      {
         room.AssignedAnts += 1;
         gameManager.freeAnts -= 1;
         UpdateAntText();
      }
   }

   void DecreaseAnts()
   {
      if (room.AssignedAnts > 0)
      {
         room.AssignedAnts -= 1;
         gameManager.freeAnts += 1;
         UpdateAntText();

      }
   }
   
   void Confirm()
   {
      //GetComponentInParent<GameObject>().SetActive(false);

      foreach (Transform child in room.transform)
      {
         Debug.Log(child.name);
         if (child.gameObject.active == true)
         {
            child.gameObject.SetActive(false);
            Debug.Log(room.AssignedAnts);
            //NO time factor that is why the room gets built instantly
            gameManager.freeAnts += room.AssignedAnts;
            room.AssignedAnts = 0;
         }
      }

      gameObject.SetActive(false);
   }

   public void UpdateAntText()
   {
      freeAnts.text = "Free ants: " + gameManager.freeAnts + "/" + gameManager.totalAnts;
      if (room.AssignedAnts == 1)
      {
         assignedAntsText.text = "assign " + room.AssignedAnts + " ant";
      }
      else
      {
         assignedAntsText.text = "assign " + room.AssignedAnts + " ants";
      }
   }
}
