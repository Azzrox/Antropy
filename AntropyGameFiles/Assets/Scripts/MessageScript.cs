using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class MessageScript : MonoBehaviour
{
  private GameManager gameManager;
  public Button confirmButton;
  public TextMeshProUGUI title;
  public TextMeshProUGUI text;
  public GameObject portrait;
  public MessageSystem MessageSystemDataInstance;
  public List<Image>portraits;
  private int tutorialIndex = 0;
  public Queue<Message> currentGeneralMessageQueue;
  public Queue<EventMessages> currentEventMessageQueue;
  public Queue<Message> currentSeasonMessageQueue;
  public Queue<Message> currentWarningMessageQueue;

  private void Awake()
  {
    gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    confirmButton.onClick.AddListener(PlayNextTutorialMessage);
  }

  private void Start()
  {
    if (gameManager.tutorialEnabled) 
    {
      PlayNextTutorialMessage();
    }
    else 
    {
      this.gameObject.SetActive(false);
    }
  }

  /// <summary>
  /// Enable Message System 
  /// </summary>
  public void EnableMessageSystem()
  {
    CreateJsonFromTxt();
    LoadMessagesFromJson();
  }

  /// <summary>
  /// Needs to be called after each turn, to update the message queues
  /// </summary>
  public void PrepareRoundMessages() 
  {
    currentGeneralMessageQueue =  getGeneralMessages();
    currentWarningMessageQueue = getWarningMessages();
    currentSeasonMessageQueue = getWeatherSeasonMessages();
    confirmButton.onClick.AddListener(PlayNextMessage);
    PlayNextMessage();
    this.gameObject.SetActive(true);
  }

  /// <summary>
  /// Plays the next tutorial message
  /// </summary>
  public void PlayNextTutorialMessage() 
  {
    Debug.Log("Count List Tutorial: " + MessageSystemDataInstance.tutorialMessages.Count);
    if(tutorialIndex < MessageSystemDataInstance.tutorialMessages.Count) 
    {
      this.gameObject.SetActive(true);
      TutorialMessages currentMessage = startTutorialSequence(tutorialIndex);
      //Prototype, until we have portraits
      portrait.GetComponent<Image>().color = (currentMessage.speaker.Equals("Lyra")) ? Color.blue : Color.yellow;
      //portrait.GetComponent<Image>().sprite = (currentMessage.speaker.Equals("Lyra"))? portraits[0].sprite : portraits[1].sprite;
      title.text = "Tutorial Message";
      text.text = currentMessage.messageText;
      Debug.Log(currentMessage.messageText);
      //..
      //play voice recording
      //message.voiceRecording.Play();
      tutorialIndex++;
    }
    else 
    {
      this.gameObject.SetActive(false);
    }
  }

  /// <summary>
  /// Function that Plays the next Round Message
  /// </summary>
  private void PlayNextMessage()
  {
    if (currentGeneralMessageQueue.Count > 0)
    {
      portrait.GetComponent<Image>().color = Color.green;
      //portrait.GetComponent<Image>().sprite = portraits[0].sprite;
      title.text = "General Message";
      text.text = currentGeneralMessageQueue.Peek().messageText;
      this.gameObject.SetActive(true);
      currentGeneralMessageQueue.Dequeue();
    }
    else if(currentSeasonMessageQueue.Count > 0) 
    {
      portrait.GetComponent<Image>().color = Color.green;
      //portrait.GetComponent<Image>().sprite = portraits[0].sprite;
      title.text = "Season Message";
      text.text = currentSeasonMessageQueue.Peek().messageText;
      this.gameObject.SetActive(true);
      currentSeasonMessageQueue.Dequeue();
    }
    else if (currentWarningMessageQueue.Count > 0)
    {
      portrait.GetComponent<Image>().color = Color.green;
      //portrait.GetComponent<Image>().sprite = portraits[0].sprite;
      title.text = "Warning Message";
      text.text = currentWarningMessageQueue.Peek().messageText;
      this.gameObject.SetActive(true);
      currentWarningMessageQueue.Dequeue();
    }
    else if (currentEventMessageQueue.Count > 0)
    {
      portrait.GetComponent<Image>().color = Color.green;
      //portrait.GetComponent<Image>().sprite = portraits[0].sprite;
      title.text = "Event Message";
      text.text = currentEventMessageQueue.Peek().messageText;
      this.gameObject.SetActive(true);
      currentEventMessageQueue.Dequeue();
    }
    else
    {
      this.gameObject.SetActive(false);
    }
  }

  //TODO Implementation with the rest of the weather
  private string TurnsUntilWinter()
  {
    return "";
  }

  //TODO Implementation missing
  private string TurnsUntilStarving()
  {
    return "";
  }

  //TODO Implementation missing
  private bool checkWeeklyGoal()
  {
    return false;
  }

  //TODO Implementation missing
  private bool checkTilesFertility()
  {
    return false;
  }

  private bool checkAntTileDistance()
  {
    return false;
  }

  //TODO Implementation missing
  private bool checkTileDepletion()
  {
    return false;
  }

  //TODO Implementation missing
  private string predictWeather()
  {
    //(confident | uncertain | indicate nothing good)
    return "";
  }

  //TODO Implementation missing
  private bool checkSeasonChange()
  {
    return false;
  }

  //TODO Implementation missing
  private bool checkWinterPrediction()
  {
    return false;
  }

  [Serializable]
  public class MessageSystem
  {
    public List<TutorialMessages> tutorialMessages { get; set; }
    public List<Message> generalMessages { get; set; }
    public List<Message> seasonMessages { get; set; }
    public List<Message> warningMessages { get; set; }
    public List<EventMessages> eventMessages { get; set; }
  }

  [Serializable]
  public class Message
  {
    public string eventName;
    public string messageText;
    public string speaker;
    public Image portrait;
  }

  [Serializable]
  public class TutorialMessages : Message
  {

    public AudioSource voiceRecording;
    //..
  }

  //Not implemented yet
  [Serializable]
  public class EventMessages : Message
  {
    int fertilityChange;
    int tilesAffected;
    int antsLost;
    //..
  }

  List<string> getAllSubStrings(string Message)
  {
    string currentValue = "";
    List<string> returnArray = new List<string>();

    for (int i = 0; i < Message.Length; i++)
    {
      if (Message[i] == ';')
      {
        returnArray.Add(currentValue);
        currentValue = "";
      }
      else
      {
        currentValue += Message[i];
      }
    }
    returnArray.Add(currentValue);
    return returnArray;
  }

  private List<String> returnMessageFileString(TextAsset textAsset)
  {
    return new List<String>(textAsset.text.Split('\n'));
  }
  public void CreateJsonFromTxt()
  {
    TextAsset textFile = Resources.Load<TextAsset>("Messages/AntropyMessage");
    List<string> TextFileMessages = returnMessageFileString(textFile);
    List<TutorialMessages> tutorialMessages = new List<TutorialMessages>();
    List<Message> generalMessages = new List<Message>();
    List<Message> seasonMessages = new List<Message>();
    List<Message> warningMessages = new List<Message>();
    List<EventMessages> eventMessages = new List<EventMessages>();

    foreach (string row in TextFileMessages)
    {
      List<string> indexString = getAllSubStrings(row);
      if (indexString[0] == "0")
      {
        TutorialMessages tutorialMessage = new TutorialMessages();
        tutorialMessage.eventName = indexString[1];
        tutorialMessage.messageText = indexString[2];
        tutorialMessage.speaker = indexString[3];
        ////tutorialMessage.picture = Resources.Load<Image>("Messages/SourceNameHere"); ;
        //tutorialMessage.voiceRecording = Resources.Load<AudioSource>("Messages/SourceNameHere"); ;
        //..
        tutorialMessages.Add(tutorialMessage);
      }
      else if (indexString[0] == "1")
      {
        Message generalMessage = new Message();
        generalMessage.eventName = indexString[1];
        generalMessage.messageText = indexString[2];
        generalMessages.Add(generalMessage);
      }
      else if (indexString[0] == "2")
      {
        Message weatherSeasonMessage = new Message();
        weatherSeasonMessage.eventName = indexString[1];
        weatherSeasonMessage.messageText = indexString[2];
        generalMessages.Add(weatherSeasonMessage);
      }
      else if (indexString[0] == "3")
      {
        Message warningMessage = new Message();
        warningMessage.eventName = indexString[1];
        warningMessage.messageText = indexString[2];
        warningMessage.speaker = indexString[3];
        warningMessages.Add(warningMessage);
      }
      else if (indexString[0] == "4")
      {
        EventMessages eventMessage = new EventMessages();
        eventMessage.eventName = indexString[1];
        eventMessage.messageText = indexString[2];
        //..
        //..
        //..
        eventMessages.Add(eventMessage);
      }
    }

    MessageSystem messageStruct = new MessageSystem()
    {
      tutorialMessages = tutorialMessages,
      generalMessages = generalMessages,
      seasonMessages = seasonMessages,
      warningMessages = warningMessages,
      eventMessages = eventMessages
    };

    var json = JsonConvert.SerializeObject(messageStruct);
    File.WriteAllText(Application.persistentDataPath + "/messagefile.json", json);
  }

  private void LoadMessagesFromJson()
  {
    string path = Application.persistentDataPath + "/messagefile.json";
    if (File.Exists(path))
    {
      MessageSystemDataInstance = JsonConvert.DeserializeObject<MessageSystem>(File.ReadAllText(path));
    }
  }

  private string manipulateMessageString(string MessageString)
  {
    return applyColourEncoding(applyVariableDecoding(MessageString));
  }

  private string applyVariableDecoding(string MessageString)
  {
    string adaptedString = "";
    for (int i = 0; i < MessageString.Length; i++)
    {
      if (MessageString[i] == '%')
      {
        if(MessageString[i + 2].ToString().Equals(" "))
        {
          adaptedString += returnDencodedVariable(Convert.ToInt32(MessageString[i + 1].ToString()));
        }
        else 
        {
          adaptedString += returnDencodedVariable(Convert.ToInt32(MessageString[i + 1].ToString() + MessageString[i + 2].ToString()));
          i++;
        }
        i++;
      }
      else
      {
        adaptedString += MessageString[i];
      }
    }
    return adaptedString;
  }

  /// <summary>
  ///Message Encoding
  /// Free Ants                   %0 ;
  /// Current resources           %1 ;
  /// Time until starving(atm)    %2 ; 
  /// Collected this turn         %3 ;
  /// Current upkeep              %4 ;
  /// Population grow/decrease    %5 ;
  /// Time until winter           %6 ;
  /// income Food                 %7 ;
  /// season                      %8 ;
  /// prediction Winter           %9 ;
  /// add new Line "\n"           %10 ;
  /// </summary>
  private string returnDencodedVariable(int encoding)
  {
    string encodingString = "";
    switch (encoding)
    {
      case 0:
        encodingString = gameManager.freeAnts.ToString();
        break;

      case 1:
        encodingString = gameManager.resources.ToString();
        break;

      case 2:
        encodingString = TurnsUntilStarving();
        break;

      case 3:
        encodingString = gameManager.Harvest().ToString();
        break;

      case 4:
        encodingString = gameManager.Upkeep().ToString();
        break;

      case 5:
        encodingString = gameManager.growth.ToString();
        break;

      case 6:
        encodingString = TurnsUntilWinter();
        break;

      case 7:
        encodingString = gameManager.income.ToString();
        break;

      case 8:
        encodingString = gameManager.currentSeason.ToString();
        break;

      case 9:
        encodingString = predictWeather();
        break;

      case 10:
        encodingString = "\n";
        break;

      default:
        encodingString = "Not implemented Encoding";
        break;
    }
    return encodingString;
  }

  /// <summary>
  /// Colour Encoding        S  E
  /// Blue                   |b | ;
  /// Green                  |g | ;
  /// Red                    |r | ;
  /// </summary>
  /// <param name="MessageString"></param>
  /// <returns></returns>
  private string applyColourEncoding(string MessageString)
  {
    string adaptedString = "";
    for (int i = 0, j = 0; i < MessageString.Length; i++)
    {
      if (MessageString[i] == '|')
      {
        adaptedString += (j == 0) ? pickDecodecColourStart(MessageString[i + 1].ToString()) : "</color>";
        i++;
        j = (j > 0) ? 0 : 1;
      }
      else
      {
        adaptedString += MessageString[i];
      }
    }
    return adaptedString;
  }

  private string pickDecodecColourStart(string colour)
  {
    string colourPick = "";
    if (colour.Equals("b"))
    {
      colourPick = "<color=blue>";
    }
    else if (colour.Equals("g"))
    {
      colourPick = "<color=green>";
    }
    else if (colour.Equals("r"))
    {
      colourPick = "<color=red>";
    }
    else if (colour.Equals("p"))
    {
      colourPick = "<color=yellow>";
    }
    return colourPick;
  }

  private List<string> getTutorialMessages(string tutorialIndex)
  {
    List<string> formatedMessages = new List<string>();
    foreach (var item in MessageSystemDataInstance.tutorialMessages)
    {
      if (item.Equals(tutorialIndex)) 
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
    }
    return formatedMessages;
  }
  private TutorialMessages getTutorialMessage(string tutorialIndex)
  {
    TutorialMessages message = new TutorialMessages();
    foreach (var item in MessageSystemDataInstance.tutorialMessages)
    {
      if (item.eventName.Equals(tutorialIndex))
      {
        message.messageText = manipulateMessageString(item.messageText);
        message.voiceRecording = item.voiceRecording;
        message.speaker = item.speaker;
        return message;
      }
    }
    return message;
  }

  public TutorialMessages startTutorialSequence(int sequenceIndex) 
  {
    TutorialMessages message = new TutorialMessages();
    switch (sequenceIndex)
    {
      case 0:
        message = getTutorialMessage("L0");
        break;

      case 1:
        message = getTutorialMessage("L1");
        break;

      case 2:
        message = getTutorialMessage("L2");
        break;

      case 3:
        message = getTutorialMessage("C0");
        break;

      case 4:
        message = getTutorialMessage("C1");
        break;

      case 5:
        message = getTutorialMessage("C2");
        break;

      default:
        Debug.Log("Not implemented Tutorial Message");
        break;
    }
    return message;
  }

  private Queue<Message> getGeneralMessages()
  {
    Queue<Message> messages = new Queue<Message>();
    
    foreach (var item in MessageSystemDataInstance.generalMessages)
    {
      Message currentMessage = new Message();
      if (item.eventName.Equals("positiveIncome") && gameManager.income > 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("negativeIncome") && gameManager.income < 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("antGrowth") && gameManager.growth > 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("stagnation") && gameManager.freeAnts == 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("starvation") && gameManager.resources == 0 && gameManager.income < 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("overpopulation") && gameManager.totalAnts > gameManager.currentMaximumPopulationCapacity)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
      Debug.Log("Message Content: " + currentMessage.messageText);
    }
    Debug.Log("Message list: " + messages.Count);
    return messages;
  }

  private Queue<Message> getWarningMessages()
  {
    Queue<Message> messages = new Queue<Message>();
    foreach (var item in MessageSystemDataInstance.warningMessages)
    {
      Message currentMessage = new Message();
      if (item.eventName.Equals("depletion") && checkTileDepletion())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("negativeIncome") && gameManager.income < 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("overpopulation") && gameManager.totalAnts > gameManager.currentMaximumPopulationCapacity)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("stagnation") && gameManager.freeAnts == 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("antDistance") && checkAntTileDistance())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("fullStorage") && gameManager.maxResourceStorage == gameManager.resources)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("lowFertility") && checkTilesFertility())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("starvation") && gameManager.resources == 0 && gameManager.income < 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
    }
    return messages;
  }

  private Queue<Message> getWeatherSeasonMessages()
  {
    Queue<Message> messages = new Queue<Message>();
    
    foreach (var item in MessageSystemDataInstance.seasonMessages)
    {
      Message currentMessage = new Message();
      if (item.eventName.Equals("seasonChange") && checkSeasonChange())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("winterPrediction") && checkWinterPrediction())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("goal_reached") && checkWeeklyGoal())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("goal_missed") && !checkWeeklyGoal())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = manipulateMessageString(item.messageText);
        //currentMessage.portrait = insertPortraitHere;
        messages.Enqueue(currentMessage);
      }
    }
    return messages;
  }
}
