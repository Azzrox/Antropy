using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class RoomSelectScript : MonoBehaviour
{
   /// <summary>
   /// Type of the Room
   /// </summary>
   int type;

   /// <summary>
   /// Ants on tile
   /// </summary>
   int assignedAnts;

   /// <summary>
   /// Free Ants Global
   /// </summary>
   int freeAnts;

   public int roomId = 0;

   private void Awake()
   {
      type = 0;
      assignedAnts = 0;
      freeAnts = 0;
      
   }

   /// <summary>
   /// Assignment Over
   /// </summary>
   private void OnMouseDown()
   {
      Debug.Log("in click mode");
      GameObject uiAssignAnts = GameObject.Find("AssignAnts");
      uiAssignAnts.GetComponent<Canvas>().enabled = true;

      AntCounter antCounter = uiAssignAnts.GetComponent<AntCounter>();
      antCounter.SetAssignedAnts(roomId, 0, assignedAnts, 10, true);
      
      antCounter.UpdateAntText();
   }


   /// <summary>
   ///  Ants on Tile, getter and setter
   /// </summary>
   public int AssignedAnts
   {
      get
      {
         return assignedAnts;
      }
      set
      {
         assignedAnts = value;
      }
   }

   /// <summary>
   /// Room type, getter and setter
   /// </summary>
   public int RoomType
   {
      get
      {
         return type;
      }
      set
      {
         type = value;
      }
   }
}

