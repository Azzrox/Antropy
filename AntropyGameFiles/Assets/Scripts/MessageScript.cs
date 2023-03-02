using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using static MessageScript;
using Unity.VisualScripting;
using UnityEditor.Search;

public class MessageScript : MonoBehaviour
{

  //UI
  public Button confirmButton;
  public TextMeshProUGUI title;
  public TextMeshProUGUI speaker;
  public TextMeshProUGUI text;
  public GameObject portrait;
  public List<Image> portraits;

  //Message Queues
  public MessageSystem MessageSystemDataInstance;
  public Queue<TutorialMessages> currentTutorialMessageQueue = new Queue<TutorialMessages>();
  public Queue<Message> currentGeneralMessageQueue = new Queue<Message>();
  public Queue<Message> currentEventMessageQueue = new Queue<Message>();
  public Queue<Message> currentSeasonMessageQueue = new Queue<Message>();
  public Queue<Message> currentWarningMessageQueue = new Queue<Message>();
  public Queue<Message> currentWinterMessageQueue = new Queue<Message>();
  public Queue<Message> currentCriticalMessageQueue = new Queue<Message>();

  public Queue<(GameManager.Tile, (int, int))> resourcesTilesMessage = new Queue<(GameManager.Tile, (int, int))>();
  public Queue<(GameManager.Tile, (int, int))> distanceAntsMessage = new Queue<(GameManager.Tile, (int, int))>();
  public Queue<(GameManager.Tile, (int, int))> fertilityTilesMessage = new Queue<(GameManager.Tile, (int, int))>();


  /// <summary>
  /// old Week (only needed here)
  /// </summary>
  private int oldWeek = 0;

  /// <summary>
  /// old Season (only needed here)
  /// </summary>
  private int oldSeason = 0;

  private void Start()
  {
    GameManager.Instance.messageSystemInstance = this;
    EnableMessageSystem();
    if (GameManager.Instance.tutorialEnabled)
    {
      GetTutorialMessages();
      PlayTutorialMessage();
    }
    else
    {
      this.gameObject.SetActive(false);
    }
  }

  /// <summary>
  /// Enable Message System, creates and loads the json 
  /// </summary>
  public void EnableMessageSystem()
  {
    CreateJsonFromTxt();
    LoadMessagesFromJson();
  }

  /// <summary>
  /// This is the ONLY message function for rounds you need, only call it once each round
  /// </summary>
  public void PrepareRoundMessages()
  {
    //Clear Buttons & Messages
    GameManager.Instance.messageSideBarUIInstance.clearButtons();
    ClearAllMessages();

    //Get Messages
    GetGeneralMessages();
    GetStrategicMessages();
    GetWarningMessages();
    GetEventMessages();
    GetWinterMessages();

    //Check If messages are disabled
    ClearDisabledMessages();

    /*Always Displayed Critical Messages*/
    PlayCriticalMessage();

    //Button Assignment
    GameManager.Instance.messageSideBarUIInstance.spawnMessageButton(PlayGeneralMessage, currentGeneralMessageQueue.Count, Color.blue, "generalMessage");
    GameManager.Instance.messageSideBarUIInstance.spawnMessageButton(PlayStrategicMessage, currentSeasonMessageQueue.Count, Color.red, "strategicMessage");
    GameManager.Instance.messageSideBarUIInstance.spawnMessageButton(PlayWarningMessage, currentWarningMessageQueue.Count, Color.yellow, "warningMessage");
    GameManager.Instance.messageSideBarUIInstance.spawnMessageButton(PlayEventMessage, currentEventMessageQueue.Count, Color.green, "eventMessage");
    GameManager.Instance.messageSideBarUIInstance.spawnMessageButton(PlayWinterMessage, currentWinterMessageQueue.Count, Color.magenta, "winterMessage");
  }

  /// <summary>
  /// Clears all messages and resources
  /// </summary>
  private void ClearAllMessages() 
  {
    currentTutorialMessageQueue.Clear();
    currentGeneralMessageQueue.Clear();
    currentEventMessageQueue.Clear();
    currentSeasonMessageQueue.Clear();
    currentWarningMessageQueue.Clear();
    currentWinterMessageQueue.Clear();
    resourcesTilesMessage.Clear();
    distanceAntsMessage.Clear();
    fertilityTilesMessage.Clear();
  }

  /// <summary>
  /// Plays the critical messages, that will be displayed
  /// </summary>
  private void PlayCriticalMessage()
  {
    confirmButton.onClick.RemoveAllListeners();
    if (currentCriticalMessageQueue.Count > 0)
    {
      portrait.GetComponent<Image>().sprite = Sprite.Create(currentCriticalMessageQueue.Peek().portrait,
                                                            new Rect(0, 0, currentCriticalMessageQueue.Peek().portrait.width,
                                                            currentCriticalMessageQueue.Peek().portrait.height), new Vector2(0.5f, 0.5f), 100);
      title.text = "Critical Message";
      text.text = currentCriticalMessageQueue.Peek().messageText;
      speaker.text = currentCriticalMessageQueue.Peek().speaker;
      this.gameObject.SetActive(true);
      currentCriticalMessageQueue.Dequeue();
      confirmButton.onClick.AddListener(PlayCriticalMessage);
    }
    else
    {
      this.gameObject.SetActive(false);
    }
  }

  /// <summary>
  /// Plays the tutorial messages, that will be spawned on a button
  /// </summary>
  private void PlayTutorialMessage() 
  {
    confirmButton.onClick.RemoveAllListeners();
    if (currentTutorialMessageQueue.Count > 0)
    {
      portrait.GetComponent<Image>().sprite = Sprite.Create(currentTutorialMessageQueue.Peek().portrait,
                                                            new Rect(0, 0, currentTutorialMessageQueue.Peek().portrait.width,
                                                            currentTutorialMessageQueue.Peek().portrait.height), new Vector2(0.5f, 0.5f), 100);
      title.text = "Tutorial Message";
      text.text = currentTutorialMessageQueue.Peek().messageText;
      speaker.text = currentTutorialMessageQueue.Peek().speaker;
      this.gameObject.SetActive(true);
      currentTutorialMessageQueue.Dequeue();
      confirmButton.onClick.AddListener(PlayTutorialMessage);
    }
    else
    {
      this.gameObject.SetActive(false);

      /*Tutorial was already played*/
      GameManager.Instance.tutorialEnabled = false;
    }
  }

  /// <summary>
  /// Plays the general messages, that will be spawned on a button
  /// </summary>
  private void PlayGeneralMessage() 
  {
    confirmButton.onClick.RemoveAllListeners();
    if (currentGeneralMessageQueue.Count > 0)
    {
      portrait.GetComponent<Image>().sprite = Sprite.Create(currentGeneralMessageQueue.Peek().portrait,
                                                            new Rect(0, 0, currentGeneralMessageQueue.Peek().portrait.width,
                                                            currentGeneralMessageQueue.Peek().portrait.height), new Vector2(0.5f, 0.5f), 100);
      title.text = "General Message";
      text.text = currentGeneralMessageQueue.Peek().messageText;
      speaker.text = currentGeneralMessageQueue.Peek().speaker;
      this.gameObject.SetActive(true);
      currentGeneralMessageQueue.Dequeue();
      GameManager.Instance.messageSideBarUIInstance.reduceMessageMessageCountText("generalMessage");
      confirmButton.onClick.AddListener(PlayGeneralMessage);
    }
    else 
    {
      this.gameObject.SetActive(false);
    }
  }

  /// <summary>
  /// Plays the strategic messages, that will be spawned on a button
  /// </summary>
  private void PlayStrategicMessage() 
  {
    confirmButton.onClick.RemoveAllListeners();
    if (currentSeasonMessageQueue.Count > 0)
    {
      portrait.GetComponent<Image>().sprite = Sprite.Create(currentSeasonMessageQueue.Peek().portrait,
                                                           new Rect(0, 0, currentSeasonMessageQueue.Peek().portrait.width,
                                                           currentSeasonMessageQueue.Peek().portrait.height), new Vector2(0.5f, 0.5f), 100);
      title.text = "Season Message";
      text.text = currentSeasonMessageQueue.Peek().messageText;
      speaker.text = currentSeasonMessageQueue.Peek().speaker;
      this.gameObject.SetActive(true);
      currentSeasonMessageQueue.Dequeue();
      GameManager.Instance.messageSideBarUIInstance.reduceMessageMessageCountText("strategicMessage");
      confirmButton.onClick.AddListener(PlayStrategicMessage);
    }
    else
    {
      this.gameObject.SetActive(false);
    }
  }

  /// <summary>
  /// Plays the warning messages, that will be spawned on a button
  /// </summary>
  private void PlayWarningMessage() 
  {
    confirmButton.onClick.RemoveAllListeners();
    if (currentWarningMessageQueue.Count > 0)
    {
      portrait.GetComponent<Image>().sprite = Sprite.Create(currentWarningMessageQueue.Peek().portrait,
                                                           new Rect(0, 0, currentWarningMessageQueue.Peek().portrait.width,
                                                           currentWarningMessageQueue.Peek().portrait.height), new Vector2(0.5f, 0.5f), 100);
      title.text = "Warning Message";
      text.text = currentWarningMessageQueue.Peek().messageText;
      speaker.text = currentWarningMessageQueue.Peek().speaker;
      this.gameObject.SetActive(true);
      currentWarningMessageQueue.Dequeue();
      GameManager.Instance.messageSideBarUIInstance.reduceMessageMessageCountText("warningMessage");
      confirmButton.onClick.AddListener(PlayWarningMessage);
    }
    else
    {
      this.gameObject.SetActive(false);
    }
  }

  /// <summary>
  /// Plays the event messages, that will be spawned on a button
  /// </summary>
  private void PlayEventMessage() 
  {
    confirmButton.onClick.RemoveAllListeners();
    if (currentEventMessageQueue.Count > 0)
    {
      portrait.GetComponent<Image>().sprite = Sprite.Create(currentEventMessageQueue.Peek().portrait,
                                                           new Rect(0, 0, currentEventMessageQueue.Peek().portrait.width,
                                                           currentEventMessageQueue.Peek().portrait.height), new Vector2(0.5f, 0.5f), 100);
      title.text = "Event Message";
      text.text = currentEventMessageQueue.Peek().messageText;
      speaker.text = currentEventMessageQueue.Peek().speaker;
      this.gameObject.SetActive(true);
      currentEventMessageQueue.Dequeue();
      GameManager.Instance.messageSideBarUIInstance.reduceMessageMessageCountText("eventMessage");
      confirmButton.onClick.AddListener(PlayEventMessage);
    }
    else
    {
      this.gameObject.SetActive(false);
    }
  }

  /// <summary>
  /// Plays the winter messages, that will be spawned on a button
  /// </summary>
  private void PlayWinterMessage() 
  {
    confirmButton.onClick.RemoveAllListeners();
    if (currentWinterMessageQueue.Count > 0)
    {
      portrait.GetComponent<Image>().sprite = Sprite.Create(currentWinterMessageQueue.Peek().portrait,
                                                           new Rect(0, 0, currentWinterMessageQueue.Peek().portrait.width,
                                                           currentWinterMessageQueue.Peek().portrait.height), new Vector2(0.5f, 0.5f), 100);
      title.text = "Winter Message";
      text.text = currentWinterMessageQueue.Peek().messageText;
      speaker.text = currentWinterMessageQueue.Peek().speaker;
      this.gameObject.SetActive(true);
      currentWinterMessageQueue.Dequeue();
      GameManager.Instance.messageSideBarUIInstance.reduceMessageMessageCountText("winterMessage");
      confirmButton.onClick.AddListener(PlayWinterMessage);
    }
    else
    {
      this.gameObject.SetActive(false);
    }
  }

  /// <summary>
  /// Clears all Messagequeues
  /// </summary>
  private void ClearDisabledMessages() 
  {
    if (!GameManager.Instance.tutorialEnabled)
    {
      currentTutorialMessageQueue.Clear();
    }
    if (!GameManager.Instance.generalEnabled) 
    {
      currentGeneralMessageQueue.Clear();
    }
    if (!GameManager.Instance.strategicEnabled)
    {
      currentSeasonMessageQueue.Clear();
    }
    if (!GameManager.Instance.warningEnabled)
    {
      currentWarningMessageQueue.Clear();
    }
    if (!GameManager.Instance.eventEnabled)
    {
      currentWarningMessageQueue.Clear();
    }
    if (!GameManager.Instance.winterEnabled)
    {
      currentWinterMessageQueue.Clear();
    }
    if (!GameManager.Instance.criticalEnabled)
    {
      currentWinterMessageQueue.Clear();
    }
  }

  /// <summary>
  /// Disables the Message System, except the Winter Messages
  /// </summary>
  public void disableMessageSystem() 
  {
    ClearDisabledMessages();
    GameManager.Instance.tutorialEnabled = false;
    GameManager.Instance.generalEnabled = false;
    GameManager.Instance.strategicEnabled = false;
    GameManager.Instance.warningEnabled = false;
    GameManager.Instance.eventEnabled = false;
  }

  //TODO Implementation with the rest of the weather
  private string TurnsUntilWinter()
  {
    if(GameManager.Instance.currentTurnCount < (GameManager.Instance.maxTurnCount - GameManager.Instance.seasonLength)) 
    {
       return (GameManager.Instance.maxTurnCount - GameManager.Instance.seasonLength - GameManager.Instance.currentTurnCount).ToString();
    }
   
    return "0";
  }

  //TODO Implementation missing
  private string TurnsUntilStarving()
  {
    return "";
  }

  /// <summary>
  /// Checks if the weekly goal has been reached
  /// </summary>
  /// <returns></returns>
  private bool CheckWeeklyGoal()
  {
    if (GameManager.Instance.goalThreshholds[GameManager.Instance.currentWeek].Item1 <= GameManager.Instance.resources 
      && GameManager.Instance.goalThreshholds[GameManager.Instance.currentWeek].Item2 <= GameManager.Instance.totalAnts)
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  /// <summary>
  /// Currently only checks if there are tiles that have low fertility.
  /// Rework if needed to show each individual tile
  /// </summary>
  /// <returns></returns>
  private bool CheckTilesFertility()
  {
    
    if(fertilityTilesMessage.Count > 0) 
    {
      return true;
    }
    else 
    {
      fertilityTilesMessage.Clear();
      return false;
    }
  }

  /// <summary>
  /// Currently only checks if there are tiles that where Ants are assigned over a distance 10.
  /// Rework if needed to show each individual tile
  /// </summary>
  /// <returns></returns>
  private bool CheckAntTileDistance()
  {
    if(distanceAntsMessage.Count > 0) 
    {
      distanceAntsMessage.Clear();
      return true;
    }
    else 
    {
      distanceAntsMessage.Clear();
      return false;
    }
  }

  /// <summary>
  /// Currently only checks if there are tiles that have <= 20 resources.
  /// Rework if needed to show each individual tile
  /// </summary>
  /// <returns></returns>
  private bool CheckTileDepletion()
  {
    if(resourcesTilesMessage.Count > 0) 
    {
      return true;
    }
    else 
    {
      resourcesTilesMessage.Clear();
      return false;
    }
  }

  //TODO Implementation missing
  private string PredictWeather()
  {
    //(confident | uncertain | indicate nothing good)
    //dependend on difficulty (if we really want to do it)
    return "";
  }

  /// <summary>
  /// Checks if a new season has been reached, updates oldSeason variable
  /// </summary>
  /// <returns></returns>
  private bool CheckSeasonChange()
  {
    if(oldSeason != GameManager.Instance.currentSeason) 
    {
      oldSeason = GameManager.Instance.currentSeason;
      return true;
    }
    else 
    {
      return false;
    }
  }

  /// <summary>
  /// Checks if a new week has been reached, updates oldWeek variable
  /// </summary>
  /// <returns></returns>
  private bool CheckNewWeek()
  {
    if (oldWeek != GameManager.Instance.currentWeek)
    {
      oldWeek = GameManager.Instance.currentWeek;
      return true;
    }
    else
    {
      return false;
    }
  }

  //TODO Implementation missing
  private bool CheckWinterPrediction()
  {
    return false;
  }

  [Serializable]
  public class MessageSystem
  {
    public List<TutorialMessages> tutorialMessages { get; set; }
    public List<Message> generalMessages { get; set; }
    public List<Message> strategicMessages { get; set; }
    public List<Message> warningMessages { get; set; }
    public List<Message> eventMessages { get; set; }
    public List<Message> winterMessages { get; set; }
  }

  [Serializable]
  public class Message
  {
    public string eventName;
    public string messageText;
    public string speaker;
    public Texture2D portrait;
  }

  [Serializable]
  public class TutorialMessages : Message
  {
    public AudioSource voiceRecording;
    //..
  }

  [Serializable]
  public class EventMessages : Message
  {
    //Example Events
    public int fertilityChange;
    public int tilesAffected;
    public int antsLost;
    public float resourceAffectionRate;
    //..
  }

  private List<string> getAllSubStrings(string Message)
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
  private void CreateJsonFromTxt()
  {
    TextAsset textFile = Resources.Load<TextAsset>("Messages/AntropyMessage");
    List<string> TextFileMessages = returnMessageFileString(textFile);
    List<TutorialMessages> tutorialMessages = new List<TutorialMessages>();
    List<Message> generalMessages = new List<Message>();
    List<Message> strategicMessages = new List<Message>();
    List<Message> warningMessages = new List<Message>();
    List<Message> eventMessages = new List<Message>();
    List<Message> winterMessages = new List<Message>();

    foreach (string row in TextFileMessages)
    {
      List<string> indexString = getAllSubStrings(row);
      if (indexString[0] == "0")
      {
        TutorialMessages tutorialMessage = new TutorialMessages();
        tutorialMessage.eventName = indexString[1];
        tutorialMessage.messageText = indexString[2];
        tutorialMessage.speaker = indexString[3];
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
       Message strategicMessage = new Message();
        strategicMessage.eventName = indexString[1];
        strategicMessage.messageText = indexString[2];
        strategicMessage.speaker = indexString[3];
        strategicMessages.Add(strategicMessage);
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
        Message eventMessage = new Message();
        eventMessage.eventName = indexString[1];
        eventMessage.messageText = indexString[2];
        eventMessages.Add(eventMessage);
      }
      else if (indexString[0] == "5")
      {
        Message winterMessage = new Message();
        winterMessage.eventName = indexString[1];
        winterMessage.messageText = indexString[2];
        winterMessage.speaker = indexString[3];
        winterMessages.Add(winterMessage);
      }
    }

    MessageSystem messageStruct = new MessageSystem()
    {
      tutorialMessages = tutorialMessages,
      generalMessages = generalMessages,
      strategicMessages = strategicMessages,
      warningMessages = warningMessages,
      eventMessages = eventMessages,
      winterMessages = winterMessages
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

  private string ApplyVariableDecoding(string MessageString)
  {
    string adaptedString = "";
    for (int i = 0; i < MessageString.Length; i++)
    {
      if (MessageString[i] == '%')
      {
        if (MessageString[i + 2].ToString().Equals(" "))
        {
          adaptedString += ReturnDencodedVariable(Convert.ToInt32(MessageString[i + 1].ToString()));
        }
        else
        {
          adaptedString += ReturnDencodedVariable(Convert.ToInt32(MessageString[i + 1].ToString() + MessageString[i + 2].ToString()));
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
  /// Time until starving         %2 ; 
  /// Collected this turn         %3 ;
  /// Current upkeep              %4 ;
  /// Population grow/decrease    %5 ;
  /// Time until winter           %6 ;
  /// income Food                 %7 ;
  /// season                      %8 ;
  /// prediction Winter           %9 ;
  /// add new Line "\n"           %10;
  /// ants lost during event      %11;
  /// flood fertility condition   %12;
  /// Total Ants                  %13; 
  /// </summary>
  private string ReturnDencodedVariable(int encoding)
  {
    string encodingString = "";
    switch (encoding)
    {
      case 0:
        encodingString = GameManager.Instance.freeAnts.ToString();
        break;

      case 1:
        encodingString = GameManager.Instance.resources.ToString();
        break;

      case 2:
        encodingString = TurnsUntilStarving(); //not implemented
        break;

      case 3:
        encodingString = GameManager.Instance.Harvest().ToString();
        break;

      case 4:
        encodingString = GameManager.Instance.Upkeep().ToString();
        break;

      case 5:
        encodingString = GameManager.Instance.growth.ToString();
        break;

      case 6:
        encodingString = TurnsUntilWinter();
        break;

      case 7:
        encodingString = GameManager.Instance.income.ToString();
        break;

      case 8:
        encodingString = GameManager.Instance.SeasonName(GameManager.Instance.currentSeason);
        break;

      case 9:
        encodingString = PredictWeather();
        break;

      case 10:
        encodingString = "\n";
        break;

      case 11:
        encodingString = GameManager.Instance.eventInstance.antsEventValue.ToString();
        break;
      
      case 12:
        encodingString = GameManager.Instance.FertilityNames[GameManager.Instance.floodFertilityThreshhold].ToString();
        break;

      case 13:
        encodingString = GameManager.Instance.totalAnts.ToString();
        break;

      default:
        encodingString = "Not implemented Encoding";
        break;
    }
    return encodingString;
  }
 
  /// <summary>
  /// Pushes the tutorial messages into the queue
  /// </summary>
  private void GetTutorialMessages()
  {
    foreach (var item in MessageSystemDataInstance.tutorialMessages)
    {
        TutorialMessages message = new TutorialMessages();
        message.messageText = ApplyVariableDecoding(item.messageText);
        message.voiceRecording = item.voiceRecording;
        message.speaker = item.speaker;
        if(item.speaker == "Lyra") 
        {
          message.portrait = Resources.Load<Texture2D>("Images/EngineerAnt");
          //message.voiceRecording = Resources.Load<AudioSource>("Messages/SourceNameHere");
        }
        else 
        {
          message.portrait = Resources.Load<Texture2D>("Images/OfficeAnt");
          //message.voiceRecording = Resources.Load<AudioSource>("Messages/SourceNameHere");
        }
      currentTutorialMessageQueue.Enqueue(message);  
    }
  }

  /// <summary>
  /// Pushes the general messages into the queue
  /// </summary>
  private void GetGeneralMessages()
  {
    foreach (var item in MessageSystemDataInstance.generalMessages)
    {
      Message currentMessage = new Message();
      
      if (item.eventName.Equals("positiveIncome") && GameManager.Instance.income > 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/OfficeAnt");
        currentGeneralMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("negativeIncome") && GameManager.Instance.income < 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/OfficeAnt");
        currentGeneralMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("antGrowth") && GameManager.Instance.growth > 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/OfficeAnt");
        currentGeneralMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("stagnation") && GameManager.Instance.freeAnts == 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/OfficeAnt");
        currentGeneralMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("starvation") && GameManager.Instance.resources == 0 && GameManager.Instance.income < 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/OfficeAnt");
        currentGeneralMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("overpopulation") && GameManager.Instance.totalAnts > GameManager.Instance.currentMaximumPopulationCapacity)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/OfficeAnt");
        currentGeneralMessageQueue.Enqueue(currentMessage);
      }
    }
  }

  /// <summary>
  /// Pushes the warning messages into the queue
  /// </summary>
  private void GetWarningMessages()
  {
    foreach (var item in MessageSystemDataInstance.warningMessages)
    {
      Message currentMessage = new Message();
      if (item.eventName.Equals("depletion") && CheckTileDepletion())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.messageText += "\n\n" + ResourcesTilesMessageString();
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/EngineerAnt2");
        currentWarningMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("negativeIncome") && GameManager.Instance.income < 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/EngineerAnt2");
        currentWarningMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("overpopulation") && GameManager.Instance.totalAnts > GameManager.Instance.currentMaximumPopulationCapacity)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/EngineerAnt2");
        currentWarningMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("stagnation") && GameManager.Instance.freeAnts == 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/EngineerAnt2");
        currentWarningMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("antDistance") && CheckAntTileDistance())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.messageText += "\n\n" + DistanceTilesMessageString();
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/EngineerAnt2");
        currentWarningMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("fullStorage") && GameManager.Instance.maxResourceStorage == GameManager.Instance.resources)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/EngineerAnt2");
        currentWarningMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("lowFertility") && CheckTilesFertility())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText += ApplyVariableDecoding(item.messageText);
        currentMessage.messageText += "\n\n" + FertilityTilesMessageString();
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/EngineerAnt2");
        currentWarningMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("starvation") && GameManager.Instance.resources == 0 && GameManager.Instance.income < 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/EngineerAnt2");
        currentWarningMessageQueue.Enqueue(currentMessage);
        setCrtiticalMessage(currentMessage);
      }
    }
  }

  /// <summary>
  /// Pushes the strategic messages into the queue
  /// </summary>
  private void GetStrategicMessages()
  {
    foreach (var item in MessageSystemDataInstance.strategicMessages)
    {
      Message currentMessage = new Message();
      if (item.eventName.Equals("seasonChange") && CheckSeasonChange())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/OfficeAnt2");
        currentSeasonMessageQueue.Enqueue(currentMessage);
        setCrtiticalMessage(currentMessage);
      }
      else if (item.eventName.Equals("winterPrediction") && CheckWinterPrediction())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/OfficeAnt2");
        currentSeasonMessageQueue.Enqueue(currentMessage);
      }
    }
  }

  /// <summary>
  /// Pushes the event messages into the queue
  /// </summary>
  private void GetEventMessages()
  {
    foreach (var item in MessageSystemDataInstance.eventMessages)
    {
      Message currentMessage = new Message();
      Debug.Log("Current event Name: " + item.eventName + ", GameManager eventmessageTurn: " + GameManager.Instance.eventInstance.GetEventMessageTurn.Peek().Item1);
      
      if (item.eventName.Equals("heavyRain") && GameManager.Instance.eventInstance.GetEventMessageTurn.Peek().Item1.Equals("heavyRain"))
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/EngineerAnt");
        currentEventMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("heavyFog") && GameManager.Instance.eventInstance.GetEventMessageTurn.Peek().Item1.Equals("heavyFog"))
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/EngineerAnt");
        currentEventMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("flood") && GameManager.Instance.eventInstance.GetEventMessageTurn.Peek().Item1.Equals("flood"))
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/EngineerAnt");
        currentEventMessageQueue.Enqueue(currentMessage);
        setCrtiticalMessage(currentMessage);

      }
      else if (item.eventName.Equals("lightRain") && GameManager.Instance.eventInstance.GetEventMessageTurn.Peek().Item1.Equals("lightRain"))
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/EngineerAnt");
        currentEventMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("drought") && GameManager.Instance.eventInstance.GetEventMessageTurn.Peek().Item1.Equals("drought"))
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/EngineerAnt");
        currentEventMessageQueue.Enqueue(currentMessage);
        setCrtiticalMessage(currentMessage);
      }
    }
  }

  /// <summary>
  /// Sets the critical message displayed at the start of the turn
  /// </summary>
  /// <param name="message"></param>
  private void setCrtiticalMessage(Message message) 
  {
    currentCriticalMessageQueue.Enqueue(message);
  }

  private void GetWinterMessages()
  {
    foreach (var item in MessageSystemDataInstance.winterMessages)
    {
      Message currentMessage = new Message();
      if (item.eventName.Equals("winterStart") && (GameManager.Instance.maxTurnCount - GameManager.Instance.currentTurnCount) == GameManager.Instance.seasonLength)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = ApplyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        currentMessage.portrait = Resources.Load<Texture2D>("Images/EngineerAnt2");
        currentWinterMessageQueue.Enqueue(currentMessage);
        setCrtiticalMessage(currentMessage);
      }
    }
  }

  /// <summary>
  /// Saves a tile from the MapTurn in the NextTurnScript in the MessageQueue
  /// </summary>
  /// <param name="tile"></param>
  public void SaveTileForMessage(GameManager.Tile tile, int i, int j) 
  {
    if(tile.fertilityState < GameManager.Instance.fertilityWarningMessageThreshhold) 
    {
      fertilityTilesMessage.Enqueue((tile, (i, j)));
    }
    
    if (tile.resourceAmount <= GameManager.Instance.resourcesWarningMessageThreshhold)
    {
      resourcesTilesMessage.Enqueue((tile, (i, j)));
    }

    if(tile.distanceAntHill > GameManager.Instance.distanceWarningMessageThreshhold && tile.assignedAnts > 0) 
    {
      distanceAntsMessage.Enqueue((tile, (i, j)));
    }
  }

  /// <summary>
  /// Returns a string of all low resource tiles, the player occupies
  /// </summary>
  /// <returns></returns>
  private string ResourcesTilesMessageString() 
  {
    string tile_string = "";
    int message_size = 0;
    foreach (var item in resourcesTilesMessage)
    {
      if(message_size == 10) 
      {
        tile_string += "..." + "\n";
        break;
      }
      tile_string += "[" + item.Item2.Item1 + " , " + item.Item2.Item2 + "]" + "\n";
      message_size++;
    }
    resourcesTilesMessage.Clear();
    return tile_string;
  }

  /// <summary>
  /// Returns a string of all low fertility tiles the player occupies 
  /// </summary>
  /// <returns></returns>
  private string FertilityTilesMessageString()
  {
    string tile_string = "";
    int message_size = 0;
    foreach (var item in fertilityTilesMessage)
    {
      if (message_size == 10)
      {
        tile_string += "..." + "\n";
        break;
      }
      tile_string += "[" + item.Item2.Item1 + " , " + item.Item2.Item2 + "]" + "\n";
      message_size++;
    }
    fertilityTilesMessage.Clear();
    return tile_string;
  }

  /// <summary>
  /// Returns a string of all long distance tiles from a street or the anthill
  /// </summary>
  /// <returns></returns>
  private string DistanceTilesMessageString()
  {
    string tile_string = "";
    int message_size = 0;
    foreach (var item in distanceAntsMessage)
    {
      if (message_size == 10)
      {
        tile_string += "..." + "\n";
        break;
      }
      tile_string += "[" + item.Item2.Item1 + " , " + item.Item2.Item2 + "]" + "\n";
      message_size++;
    }
    distanceAntsMessage.Clear();
    return tile_string;
  }
}
