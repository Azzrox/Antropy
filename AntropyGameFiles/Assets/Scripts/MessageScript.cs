using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System;
using Newtonsoft.Json;
using static GameManager;
using UnityEditor;

public class MessageScript : MonoBehaviour
{
  private GameManager gameManager;

  public MessageSystem MessageSystemDataInstance;

  private void Awake()
  {
    gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
  }
  
  //TODO Implementation with the rest of the weather
  public string TurnsUntilWinter()
  {
    return "";
  }

  //TODO Implementation missing
  public string TurnsUntilStarving()
  {
    return "";
  }

  //TODO Implementation missing
  public bool checkWeeklyGoal()
  {
    return false;
  }

  //TODO Implementation missing
  public bool checkTilesFertility()
  {
    return false;
  }

  public bool checkAntTileDistance()
  {
    return false;
  }

  //TODO Implementation missing
  public bool checkTileDepletion()
  {
    return false;
  }

  //TODO Implementation missing
  public string predictWeather()
  {
    //(confident | uncertain | indicate nothing good)
    return "";
  }

  //TODO Implementation missing
  public bool checkSeasonChange()
  {
    return false;
  }

  //TODO Implementation missing
  public bool checkWinterPrediction()
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

  public List<String> returnMessageFileString(TextAsset textAsset)
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

  public void LoadMessagesFromJson()
  {
    string path = Application.persistentDataPath + "/messagefile.json";
    if (File.Exists(path))
    {
      MessageSystemDataInstance = JsonConvert.DeserializeObject<MessageSystem>(File.ReadAllText(path));
    }
  }

  public void EnableMessageSystem() 
  {
    CreateJsonFromTxt();
    LoadMessagesFromJson();
  }

  public string manipulateMessageString(string MessageString)
  {
    return applyColourEncoding(applyVariableDecoding(MessageString));
  }

  string applyVariableDecoding(string MessageString)
  {
    string adaptedString = "";
    for (int i = 0; i < MessageString.Length; i++)
    {
      if (MessageString[i] == '%')
      {
        if(MessageString[i + 2].ToString().Equals(""))
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
  string returnDencodedVariable(int encoding)
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
  string applyColourEncoding(string MessageString)
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

  string pickDecodecColourStart(string colour)
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
      colourPick = "<color=magenta>";
    }
    return colourPick;
  }

  public List<string> getTutorialMessages(string tutorialIndex)
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
  public TutorialMessages getTutorialMessage(string tutorialIndex)
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

  public void startTutorialSequence(int sequenceIndex) 
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

    Debug.Log(message.messageText);
    //..

    //play voice recording
    //message.voiceRecording.Play();
  }

  public List<string> getGeneralMessages()
  {
    List<string> formatedMessages = new List<string>();
    foreach (var item in MessageSystemDataInstance.generalMessages)
    {
      if (item.eventName.Equals("positiveIncome") && gameManager.income > 0)
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
      else if (item.eventName.Equals("negativeIncome") && gameManager.income < 0)
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
      else if (item.eventName.Equals("antGrowth") && gameManager.growth > 0)
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
      else if (item.eventName.Equals("stagnation") && gameManager.freeAnts == 0)
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
      else if (item.eventName.Equals("starvation") && gameManager.resources == 0 && gameManager.income < 0)
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
      else if (item.eventName.Equals("overpopulation") && gameManager.totalAnts > gameManager.currentMaximumPopulationCapacity)
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
    }
    return formatedMessages;
  }

  public List<string> getWarningMessages()
  {
    List<string> formatedMessages = new List<string>();
    foreach (var item in MessageSystemDataInstance.warningMessages)
    {
      if (item.eventName.Equals("depletion") && checkTileDepletion())
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
      else if (item.eventName.Equals("negativeIncome") && gameManager.income < 0)
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
      else if (item.eventName.Equals("overpopulation") && gameManager.totalAnts > gameManager.currentMaximumPopulationCapacity)
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
      else if (item.eventName.Equals("stagnation") && gameManager.freeAnts == 0)
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
      else if (item.eventName.Equals("antDistance") && checkAntTileDistance())
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
      else if (item.eventName.Equals("fullStorage") && gameManager.maxResourceStorage == gameManager.resources)
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
      else if (item.eventName.Equals("lowFertility") && checkTilesFertility())
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
      else if (item.eventName.Equals("starvation") && gameManager.resources == 0 && gameManager.income < 0)
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
    }
    return formatedMessages;
  }

  public List<string> getWeatherSeasonMessages()
  {
    List<string> formatedMessages = new List<string>();
    foreach (var item in MessageSystemDataInstance.seasonMessages)
    {
      if (item.eventName.Equals("seasonChange") && checkSeasonChange())
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
      else if (item.eventName.Equals("winterPrediction") && checkWinterPrediction())
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
      else if (item.eventName.Equals("goal_reached") && checkWeeklyGoal())
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
      else if (item.eventName.Equals("goal_missed") && !checkWeeklyGoal())
      {
        formatedMessages.Add(manipulateMessageString(item.messageText));
      }
    }
    return formatedMessages;
  }
}
