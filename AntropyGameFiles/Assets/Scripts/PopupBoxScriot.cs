using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;



public class PopupBoxScriot : MonoBehaviour
{
   public TMP_Text text;
   public Canvas canvas;
   public static GameObject selectedButton;

   public void Popup() 
   {
      selectedButton = EventSystem.current.currentSelectedGameObject;
      canvas.enabled = true;
      if(selectedButton.tag == "HatcheryL1") 
      {
         text.text = "Hatchery";
         text.enabled = true;
      }
      if (selectedButton.tag == "StorageL1")
      {
         text.text = "Storage";
         text.enabled = true;
      }
   }
   public void Closepopup()
   {
      canvas.enabled = false;
      text.enabled = false;
   }
}
