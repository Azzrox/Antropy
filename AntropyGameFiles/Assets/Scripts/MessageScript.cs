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
  public Queue<EventMessages> currentEventMessageQueue = new Queue<EventMessages>();
  public Queue<Message> currentSeasonMessageQueue = new Queue<Message>();
  public Queue<Message> currentWarningMessageQueue = new Queue<Message>();
  public Queue<Message> currentWinterMessageQueue = new Queue<Message>();
  public Queue<GameManager.Tile> fertilityTilesMessage = new Queue<GameManager.Tile>();
  public Queue<GameManager.Tile> resourcesTilesMessage = new Queue<GameManager.Tile>();
  public Queue<GameManager.Tile> distanceAntsMessage = new Queue<GameManager.Tile>();

  /// <summary>
  /// old Week (only needed here)
  /// </summary>
  private int oldWeek = 0;

  /// <summary>
  /// old Season (only needed here)
  /// </summary>
  private int oldSeason = 0;

  private void Awake()
  {
    confirmButton.onClick.AddListener(PlayNextMessage);
  }

  private void Start()
  {
    GameManager.Instance.messageSystemInstance = this;
    EnableMessageSystem();
    if (GameManager.Instance.tutorialEnabled)
    {
      getTutorialMessages();
      PlayNextMessage();
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
    getGeneralMessages();
    getStrategicMessages();
    getWarningMessages();
    getEventMessages();
    getWinterMessages();
    PlayNextMessage();
  }

  /// <summary>
  /// Function that Plays the next Round Message and displays it in the UI
  /// </summary>
  private void PlayNextMessage()
  {
    //Clears Queues
    clearDisabledMessages();

    if (currentTutorialMessageQueue.Count > 0) 
    {
      portrait.GetComponent<Image>().color = (currentTutorialMessageQueue.Peek().speaker.Equals("Lyra")) ? Color.blue : Color.yellow;
      //portrait.GetComponent<Image>().sprite = portraits[0].sprite;
      title.text = "Tutorial Message";
      text.text = currentTutorialMessageQueue.Peek().messageText;
      speaker.text = currentTutorialMessageQueue.Peek().speaker;
      this.gameObject.SetActive(true);
      currentTutorialMessageQueue.Dequeue();
    }
    else if (currentGeneralMessageQueue.Count > 0)
    {
      portrait.GetComponent<Image>().color = Color.green;
      //portrait.GetComponent<Image>().sprite = portraits[0].sprite;
      title.text = "General Message";
      text.text = currentGeneralMessageQueue.Peek().messageText;
      speaker.text = currentGeneralMessageQueue.Peek().speaker;
      this.gameObject.SetActive(true);
      currentGeneralMessageQueue.Dequeue();
    }
    else if (currentSeasonMessageQueue.Count > 0)
    {
      portrait.GetComponent<Image>().color = Color.yellow;
      //portrait.GetComponent<Image>().sprite = portraits[0].sprite;
      title.text = "Season Message";
      text.text = currentSeasonMessageQueue.Peek().messageText;
      speaker.text = currentSeasonMessageQueue.Peek().speaker;
      this.gameObject.SetActive(true);
      currentSeasonMessageQueue.Dequeue();
    }
    else if (currentWarningMessageQueue.Count > 0)
    {
      portrait.GetComponent<Image>().color = Color.green;
      //portrait.GetComponent<Image>().sprite = portraits[0].sprite;
      title.text = "Warning Message";
      text.text = currentWarningMessageQueue.Peek().messageText;
      speaker.text = currentWarningMessageQueue.Peek().speaker;
      this.gameObject.SetActive(true);
      currentWarningMessageQueue.Dequeue();
    }
    else if (currentEventMessageQueue.Count > 0)
    {
      portrait.GetComponent<Image>().color = Color.green;
      //portrait.GetComponent<Image>().sprite = portraits[0].sprite;
      title.text = "Event Message";
      text.text = currentEventMessageQueue.Peek().messageText;
      speaker.text = currentEventMessageQueue.Peek().speaker;
      this.gameObject.SetActive(true);
      currentEventMessageQueue.Dequeue();
    }
    else if (currentWinterMessageQueue.Count > 0)
    {
      portrait.GetComponent<Image>().color = Color.red;
      //portrait.GetComponent<Image>().sprite = portraits[0].sprite;
      title.text = "Winter Message";
      text.text = currentWinterMessageQueue.Peek().messageText;
      speaker.text = currentWinterMessageQueue.Peek().speaker;
      this.gameObject.SetActive(true);
      currentWinterMessageQueue.Dequeue();
    }
    else 
    {
      this.gameObject.SetActive(false);
    }
  }

  /// <summary>
  /// Clears all Messagequeues
  /// </summary>
  private void clearDisabledMessages() 
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
  }

  public void ShowWinterMessages() 
  { 
    
  }

  /// <summary>
  /// Disables the Message System, except the Winter Messages
  /// </summary>
  public void disableMessageSystem() 
  {
    clearDisabledMessages();
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
  private bool checkWeeklyGoal()
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
  private bool checkTilesFertility()
  {
    
    if(fertilityTilesMessage.Count > 0) 
    {
      fertilityTilesMessage.Clear();
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
  private bool checkAntTileDistance()
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
  private bool checkTileDepletion()
  {
    if(resourcesTilesMessage.Count > 0) 
    {
      resourcesTilesMessage.Clear();
      return true;
    }
    else 
    {
      resourcesTilesMessage.Clear();
      return false;
    }
  }

  //TODO Implementation missing
  private string predictWeather()
  {
    //(confident | uncertain | indicate nothing good)
    //dependend on difficulty (if we really want to do it)
    return "";
  }

  /// <summary>
  /// Checks if a new season has been reached, updates oldSeason variable
  /// </summary>
  /// <returns></returns>
  private bool checkSeasonChange()
  {
    if(oldSeason != GameManager.Instance.currentSeason) 
    {
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
  private bool checkNewWeek()
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
  private bool checkWinterPrediction()
  {
    return false;
  }

  //TODO Implementation missing
  private bool heavyRainCheck() 
  {
    return false;
  }

  //TODO Implementation missing
  private bool heavyFogCheck() 
  {
    return false;
  }

  //TODO Implementation missing
  private bool floodCheck() 
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
    public List<EventMessages> eventMessages { get; set; }
    public List<Message> winterMessages { get; set; }
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

  [Serializable]
  public class EventMessages : Message
  {
    //Example Events
    public int fertilityChange;
    public int tilesAffected;
    public int antsLost;
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
    List<EventMessages> eventMessages = new List<EventMessages>();
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
        //tutorialMessage.picture = Resources.Load<Image>("Messages/SourceNameHere");
        //tutorialMessage.voiceRecording = Resources.Load<AudioSource>("Messages/SourceNameHere"); ;
        //..
        tutorialMessages.Add(tutorialMessage);
      }
      else if (indexString[0] == "1")
      {
        Message generalMessage = new Message();
        generalMessage.eventName = indexString[1];
        generalMessage.messageText = indexString[2];
        //generalMessage.picture = Resources.Load<Image>("Messages/SourceNameHere");
        generalMessages.Add(generalMessage);
      }
      else if (indexString[0] == "2")
      {
       Message weatherSeasonMessage = new Message();
        weatherSeasonMessage.eventName = indexString[1];
        weatherSeasonMessage.messageText = indexString[2];
        weatherSeasonMessage.speaker = indexString[3];
        //weatherSeasonMessage.picture = Resources.Load<Image>("Messages/SourceNameHere");
        strategicMessages.Add(weatherSeasonMessage);
      }
      else if (indexString[0] == "3")
      {
        Message warningMessage = new Message();
        warningMessage.eventName = indexString[1];
        warningMessage.messageText = indexString[2];
        warningMessage.speaker = indexString[3];
        //warningMessage.picture = Resources.Load<Image>("Messages/SourceNameHere");
        warningMessages.Add(warningMessage);
      }
      else if (indexString[0] == "4")
      {
        EventMessages eventMessage = new EventMessages();
        eventMessage.eventName = indexString[1];
        eventMessage.messageText = indexString[2];
        //..
        //eventMessage.picture = Resources.Load<Image>("Messages/SourceNameHere");
        eventMessage.fertilityChange = Convert.ToInt32(indexString[3]);
        eventMessage.tilesAffected = Convert.ToInt32(indexString[4]);
        eventMessage.antsLost = Convert.ToInt32(indexString[5]);
        eventMessage.speaker = indexString[6];
        eventMessages.Add(eventMessage);
      }
      else if (indexString[0] == "5")
      {
        Message winterMessage = new Message();
        winterMessage.eventName = indexString[1];
        winterMessage.messageText = indexString[2];
        //..
        //eventMessage.picture = Resources.Load<Image>("Messages/SourceNameHere");
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

  private string applyVariableDecoding(string MessageString)
  {
    string adaptedString = "";
    for (int i = 0; i < MessageString.Length; i++)
    {
      if (MessageString[i] == '%')
      {
        if (MessageString[i + 2].ToString().Equals(" "))
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
  /// affected tiles by event     %12;
  /// Total Ants                  %13; 
  /// </summary>
  private string returnDencodedVariable(int encoding)
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
        encodingString = predictWeather();
        break;

      case 10:
        encodingString = "\n";
        break;

      //case 11:
      //  encodingString = GameManager.Instance.antsLostEvent.ToString();
      //  break;
      //
      //case 12:
      //  encodingString = GameManager.Instance.affectedTilesEvent;
      //  break;

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
  private void getTutorialMessages()
  {
    foreach (var item in MessageSystemDataInstance.tutorialMessages)
    {
        TutorialMessages message = new TutorialMessages();
        message.messageText = applyVariableDecoding(item.messageText);
        message.voiceRecording = item.voiceRecording;
        message.speaker = item.speaker;
        currentTutorialMessageQueue.Enqueue(message);
    }
  }

  /// <summary>
  /// Pushes the general messages into the queue
  /// </summary>
  private void getGeneralMessages()
  {
    foreach (var item in MessageSystemDataInstance.generalMessages)
    {
      Message currentMessage = new Message();
      if (item.eventName.Equals("positiveIncome") && GameManager.Instance.income > 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentGeneralMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("negativeIncome") && GameManager.Instance.income < 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentGeneralMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("antGrowth") && GameManager.Instance.growth > 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentGeneralMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("stagnation") && GameManager.Instance.freeAnts == 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentGeneralMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("starvation") && GameManager.Instance.resources == 0 && GameManager.Instance.income < 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentGeneralMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("overpopulation") && GameManager.Instance.totalAnts > GameManager.Instance.currentMaximumPopulationCapacity)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentGeneralMessageQueue.Enqueue(currentMessage);
      }
    }
  }

  /// <summary>
  /// Pushes the warning messages into the queue
  /// </summary>
  private void getWarningMessages()
  {
    foreach (var item in MessageSystemDataInstance.warningMessages)
    {
      Message currentMessage = new Message();
      if (item.eventName.Equals("depletion") && checkTileDepletion())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentWarningMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("negativeIncome") && GameManager.Instance.income < 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentWarningMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("overpopulation") && GameManager.Instance.totalAnts > GameManager.Instance.currentMaximumPopulationCapacity)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentWarningMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("stagnation") && GameManager.Instance.freeAnts == 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentWarningMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("antDistance") && checkAntTileDistance())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentWarningMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("fullStorage") && GameManager.Instance.maxResourceStorage == GameManager.Instance.resources)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentWarningMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("lowFertility") && checkTilesFertility())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentWarningMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("starvation") && GameManager.Instance.resources == 0 && GameManager.Instance.income < 0)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentWarningMessageQueue.Enqueue(currentMessage);
      }
    }
  }

  /// <summary>
  /// Pushes the strategic messages into the queue
  /// </summary>
  private void getStrategicMessages()
  {
    //because we have to check 2 messages
    bool newWeek = checkNewWeek();
    
    foreach (var item in MessageSystemDataInstance.strategicMessages)
    {
      Message currentMessage = new Message();
      if (item.eventName.Equals("seasonChange") && checkSeasonChange())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentSeasonMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("winterPrediction") && checkWinterPrediction())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentSeasonMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("goalReached") && newWeek && checkWeeklyGoal())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentSeasonMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("goalMissed") && newWeek && !checkWeeklyGoal())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentSeasonMessageQueue.Enqueue(currentMessage);
      } 
    }
  }

  /// <summary>
  /// Pushes the event messages into the queue
  /// </summary>
  private void getEventMessages()
  {
    foreach (var item in MessageSystemDataInstance.eventMessages)
    {
      EventMessages currentMessage = new EventMessages();
      
      if (item.eventName.Equals("heavyRain") && heavyRainCheck())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentEventMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("heavyFog") && heavyFogCheck())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentEventMessageQueue.Enqueue(currentMessage);
      }
      else if (item.eventName.Equals("flood") && floodCheck())
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentEventMessageQueue.Enqueue(currentMessage);
      }
    }
  }

  private void getWinterMessages()
  {
    foreach (var item in MessageSystemDataInstance.winterMessages)
    {
      Message currentMessage = new Message();
      if (item.eventName.Equals("winterStart") && (GameManager.Instance.maxTurnCount - GameManager.Instance.currentTurnCount) == GameManager.Instance.seasonLength)
      {
        currentMessage.eventName = item.eventName;
        currentMessage.messageText = applyVariableDecoding(item.messageText);
        currentMessage.speaker = item.speaker;
        //currentMessage.portrait = insertPortraitHere;
        currentWinterMessageQueue.Enqueue(currentMessage);
      }
    }
  }
  /// <summary>
  /// Saves a tile from the MapTurn in the NextTurnScript in the MessageQueue
  /// </summary>
  /// <param name="tile"></param>
  public void saveTileForMessage(GameManager.Tile tile) 
  {
    if(tile.fertilityState < 3) 
    {
      fertilityTilesMessage.Enqueue(tile);
    }
    
    if (tile.reservedResources <= 20)
    {
      fertilityTilesMessage.Enqueue(tile);
    }

    if(tile.distanceAntHill > 10 && tile.assignedAnts > 0) 
    {
      distanceAntsMessage.Enqueue(tile);
    }
  }
}
