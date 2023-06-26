using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageSideBarUI : MonoBehaviour
{
  public Button ButtonPrefab;
  public List<Button> MessageButtons = new List<Button>();
  public Vector3 basePosition = new Vector3(0.6f, -70, 0);
  public int offset = 35;

  private void Start()
  {
    GameManager.Instance.messageSideBarUIInstance = this;
  }

  /// <summary>
  /// Spawns the message buttons for the message system, instantiates at the parent position
  /// </summary>
  /// <param name="functionForButton"></param>
  /// <param name="messagesAmount"></param>
  /// <param name="messageColour"></param>
  /// <param name="messageType"></param>
  public void spawnMessageButton(UnityAction functionForButton, int messagesAmount, Color messageColour, string messageType) 
  {
    if(messagesAmount > 0) 
    {
      Button newButton = Instantiate(ButtonPrefab, new Vector3(GetComponentInParent<Transform>().position.x, GetComponentInParent<Transform>().position.y + offset * MessageButtons.Count, 
                                                               basePosition.z), Quaternion.identity, GetComponentInParent<Transform>().transform) as Button;
      newButton.onClick.AddListener(functionForButton);
      newButton.name = messageType;
      newButton.GetComponentInChildren<TextMeshProUGUI>().text = messagesAmount.ToString();
      ColorBlock cb = newButton.colors;
      cb.normalColor = messageColour;
      
      newButton.colors = cb;

      MessageButtons.Add(newButton);
      newButton.gameObject.SetActive(true);
    }
  }
  /// <summary>
  /// Destroys all message buttons
  /// </summary>
  public void clearButtons() 
  {
    foreach (var item in MessageButtons)
    {
      Destroy(item.gameObject);
    }
    MessageButtons.Clear();
  }

  /// <summary>
  /// Reduces the number on the buttons
  /// </summary>
  /// <param name="messageType"></param>
  public void reduceMessageMessageCountText(string messageType) 
  {
    foreach (var item in MessageButtons)
    {
      if(item.name == messageType) 
      {
        string currentNumber = item.GetComponentInChildren<TextMeshProUGUI>().text;
        item.GetComponentInChildren<TextMeshProUGUI>().text = (Mathf.Clamp(int.Parse(currentNumber) - 1, 0, 5)).ToString();
        break;
      }
    }
  }
}
