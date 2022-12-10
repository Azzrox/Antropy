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

   /// <summary>
   /// Canvas for menu buttons
   /// </summary>
   public GameObject canvas;

   private void Awake()
   {
      type = 0;
      assignedAnts = 0;
      freeAnts = 0;
      canvas.SetActive(false);
   }

   /// <summary>
   /// Assignment Over
   /// </summary>
   private void OnMouseDown()
   {
      Debug.Log("in click mode");
      canvas.SetActive(true);
      Debug.Log(canvas.name);
      AntCounterAnthill antCounter = canvas.GetComponent<AntCounterAnthill>();
      antCounter.room = this;
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
   ///  Canvas for menu buttons, getter and setter
   /// </summary>
   public GameObject CanvasAssign
   {
      get
      {
         return canvas;
      }
      set
      {
         canvas = value;
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

