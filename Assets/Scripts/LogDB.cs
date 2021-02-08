using UnityEngine;
using System.Collections;
using System.IO;
using Leguar.TotalJSON;
using System;
using System.Collections.Generic;

public class LogDB : MonoBehaviour
{
    public static LogDB instance = null; // This is the singleton class that will be used.
  
    public string PlayerID = "";
    public string logDataFileName = ""; // this is the same as FILE_NAME but without Path, so this can be used by AWS to store in S3 bucket
    public bool allowChangeSelectionFeature = false; //  not related to LogDB but this is a singleton, so i'm saving this feature that will be used 
    public float timeIntervalToSaveLog = 5f;
    public GameObject exitConfirmationPanel;
    public string currentScenario { get; private set; } = string.Empty;
    // added a few more ID's as per JIRA EQ-123 from 312101 to 312300

    

    public int[] validIDs { get; set; } = { 222001,222002,222003,222004,222005,222006,222007,222008,222009,222010,222011,222012,222013,222014,222015,222016,222017,222018,222019,222020,222021,222022,222023,222024,222025,222026,222027,222028,222029,222030,222031,222032,222033,222034,222035,222036,222037,222038,222039,222040,322001,322002,322003,322004,322005,322006,322007,322008,322009,322010,322011,322012,322013,322014,322015,322016,322017,322018,322019,322020,322021,322022,322023,322024,322025,322026,322027,322028,322029,322030,322031,322032,322033,322034,322035,322036,322037,322038,322039,322040,322041,322042,322043,322044,322045,322046,322047,322048,322049,322050,322051,322052,322053,322054,322055,322056,322057,322058,322059,322060,322061,322062,322063,322064,322065,322066,322067,322068,322069,322070,322071,322072,322073,322074,322075,322076,322077,322078,322079,322080,322081,322082,322083,322084,322085,322086,322087,322088,322089,322090,322091,322092,322093,322094,322095,322096,322097,322098,322099,322100,322101,322102,322103,322104,322105,322106,322107,322108,322109,322110,322111,322112,322113,322114,322115,322116,322117,322118,322119,322120,322121,322122,322123,322124,322125,322126,322127,322128,322129,322130,322131,322132,322133,322134,322135,322136,322137,322138,322139,322140,322141,322142,322143,322144,322145,322146,322147,322148,322149,322150,322151,322152,322153,322154,322155,322156,322157,322158,322159,322160,322161,322162,322163,322164,322165,322166,322167,322168,322169,322170,322171,322172,322173,322174,322175,322176,322177,322178,322179,322180,322181,322182,322183,322184,322185,322186,322187,322188,322189,322190,322191,322192,322193,322194,322195,322196,322197,322198,322199,322200

    };

    private string FILE_PATH = "";
    private string FILE_NAME = "";
    private JSON LogData = null;

    private bool fileNameAlreadySet, logDataUpdated, startSavingProcess;
    
    private string currentSecnarioMasterKey;

    private int currentTAS = 0;
    private int currentStudentSelectedID = 0;

    private float timelapse = 0f;
    public bool exitSaveCalled { get; set; }  = false;

    private JSON currentScenarioMasterObject, currentTeacherActionsObject, currentPlayerActionsObject, currentStudentActionsObject;

    private JArray currentTASData, currentPlayerLookingAtData, currentPlayerMovedToData, currentPlayerSelectedStudentData, currentStudentLookingAtData, currentStudentAnimationsData, currentStudentSetupData;

    private int[] PopulateNewIDs(int startingValue, int endingValue) {
        int[] newIDs;
        List<int> newIDValues = new List<int>();
        for (int i = startingValue; i <= endingValue; i++) {
            newIDValues.Add(i);
        }

        newIDs = newIDValues.ToArray();

        return newIDs;
    }
    
    /*
     *
     *Log Data Format
     *
   {
       "PlayerID" : "UserID",
       "CreationTimeStamp" : "TimeStamp",
	    "ScenariosIncluded":[
			{
        	   		"SceneName":"Scene1 name",
				"SceneCode" : "SC1",
				"SceneFileToLoad":"Scenefile name"
			}
   
           	],      
 	    "ScenariosAttemted":{
           "SC1":true,
           "SC2":true
           },
        "SC1":{
                "GettingToKnowYourClass":{
                    "start1":"3434343",
                    "end1":"3434343"
                },
                "InitialSituation":{
                    "start2":"3434343",
                    "end2":"3434343"
                },
                "TeacherActions":
                {
                    "total":1,
                    "TASData":[
                           {
                               "TeacherActionSelected": "1",
                               "TASQuestion": "SLHSJHFKJSF",
                               "TASTimeStarted": "45454",
                               "TASTimeFinished": "45454",
                               "TASComment": "dfgfdgfg",
                               "TASMCQSelected": "DFFRD",
                               "Revision" : "0"
                            }
                        ]
                },
                "PlayerActions":
                {
		            "IntroPopupWindowShownAt" : "434353",
		            "IntroPopupWindowAcceptedAt" : "434353",
                    "LookingAt" : [
                        {
                           "Target" : "Object Name",
                           "Time" : "34343",
                           "CurrentCell" : "CellNumber",
			                "CurrentActualPos" : {"x":1.0,"y":2.0,"z":3.0}
			                "CurrentActualLookingDirection" : {"x":1.0,"y":2.0,"z":3.0}
                       }
                    ],
		            "MovedTo" : [
                        {
                           "FromCell" : "Cell Name",
                           "CurrentCell" : "CellNumber",
                           "Time" : "34343",
			                "CurrentActualPos" : {"x":1.0,"y":2.0,"z":3.0}
                        }
                    ],

 		            "SelectedStudent" : [
                            {
                                "SelectionID": 1,
                                "StudentName" : "Name",
                                "TimeStamp" : "34343",
                                "Duration" : "34343",
                                "Phase" : "MA-I"
                            }
                    ]
                }
                "StudentSetup": [
                       {
                           "StudentName" : "Name",
                           "Chair" : "ChairNumber",
                           "Desk" : "DeskNumber",
                           "CubbyBox" : "TableNumber",
                           "NeighbourStudent":"Student Name"
                       }
                    ]          
                "StudentActions":
                {
                   "LookingAt" : [
                       {
                           "StudentName" : "Name",
                           "Target" : "Object Name",
                           "Time" : "34343",
                           "CurrentCell" : "CellNumber"
			                "CurrentActualPos" : {"x":1.0,"y":2.0,"z":3.0}
                        }
                    ],
                   "Animations" : [
                        {
                           "StudentName" : "Name",
                           "AnimationName" : " Name",
                           "Time" : "34343",
                           "CurrentCell" : "CellNumber"
                        }
                   ]
                }           
          }
     
      }
     *
     *
     *
     * 
     * */






    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // Do not destroy this object, when we load a new scene.
        DontDestroyOnLoad(gameObject);
        if (LogData == null) Init();

        validIDs = PopulateNewIDs(322001, 322100);
    }


    public void ExitGame()
    {
        #if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif

    }

    private void Update()
    {
        if (Input.GetKey("escape"))
        {
            Debug.Log(currentScenario);
            if (!exitSaveCalled)
                if (currentScenario != "")
                {
                    SaveLogDataToFile();
                    exitSaveCalled = true;
                }
            if (exitConfirmationPanel !=null)exitConfirmationPanel.SetActive(true);
        }
    }

    

    private void LateUpdate()
    {
        if(startSavingProcess)
           if(logDataUpdated)
            {
                timelapse += Time.unscaledDeltaTime;
                if (timelapse >= timeIntervalToSaveLog)
                {
                    timelapse = 0f;
                    SaveLogDataToFile();
                }
            }
    }


    public void SetCurrentScenario(string curScenario)
    {
        currentScenario = curScenario;
    }


    public void SetSavingProcessStart()
    {
        startSavingProcess = true;
    }



    // this has to be called per scenario at start of the scenario, as it creates the basic Empty object for a scenario with its key structures
    // better to be called after the PlayerID is set from GameManager, as this attempts to find an existing User file with same date to modify , or else it create a new one
    public bool SetScenarioMasterLogObject(string key) 
    {
        if (LogData == null) Init();

        if (key != null && key != "")
        {
            currentSecnarioMasterKey = key;
            currentTAS = 0;


            if (currentSecnarioMasterKey != "")
            {


                // initiate basics Log object Arrays
                JArray TASData = new JArray();
                JArray PlayerLookingAtData = new JArray();
                JArray PlayerMovedToData = new JArray();
                JArray PlayerSelectedStudentData = new JArray();
                JArray StudentLookingAtData = new JArray();
                JArray StudentAnimationsData = new JArray();
                JArray StudentSetupData = new JArray();


                JSON ScenarioMasterObject = new JSON();

                JSON TeacherActionsObject = new JSON();
                JSON PlayerActionsObject = new JSON();
                JSON StudentActionsObject = new JSON();

                JSON GettingToKnowYourClassObject = new JSON();

                JSON InitialSituationObject = new JSON();

                // Populate TeacherActions
                TeacherActionsObject.AddOrReplace("TASData", TASData);

                // Populate PlayerActions
                PlayerActionsObject.AddOrReplace("LookingAt", PlayerLookingAtData);
                PlayerActionsObject.AddOrReplace("MovedTo", PlayerMovedToData);
                PlayerActionsObject.AddOrReplace("SelectedStudent", PlayerSelectedStudentData);

                // Populate StudentActions
                StudentActionsObject.AddOrReplace("LookingAt", StudentLookingAtData);
                StudentActionsObject.AddOrReplace("Animations", StudentAnimationsData);

                // Populate GettingToKnowYourClassObject
                GettingToKnowYourClassObject.AddOrReplace("start1", "");
                GettingToKnowYourClassObject.AddOrReplace("end1", "");

                // Populate InitialSituationObject
                InitialSituationObject.AddOrReplace("start2", "");
                InitialSituationObject.AddOrReplace("end2", "");

                // Add all the above and StudentSetup to the Current Scenario Object
                ScenarioMasterObject.AddOrReplace("GettingToKnowYourClass", GettingToKnowYourClassObject);
                ScenarioMasterObject.AddOrReplace("InitialSituation", InitialSituationObject);

                ScenarioMasterObject.AddOrReplace("TeacherActions", TeacherActionsObject);
                ScenarioMasterObject.AddOrReplace("StudentSetup", StudentSetupData);
                ScenarioMasterObject.AddOrReplace("PlayerActions", PlayerActionsObject);
                ScenarioMasterObject.AddOrReplace("StudentActions", StudentActionsObject);

              

                if (LogData.ContainsKey(currentSecnarioMasterKey))
                    LogData.AddOrReplace(currentSecnarioMasterKey, ScenarioMasterObject);
                else
                    LogData.Add(currentSecnarioMasterKey, ScenarioMasterObject);
            }
            return true;
        }
        currentSecnarioMasterKey = "";
        Debug.LogAssertion("Key for New attepmt to set Scenario Master Log Object is empty or null");
        return false;
    }


        public bool SetStudentSetupData(StudentSetupData ssdata)  
    {
        if(currentSecnarioMasterKey !="")
            if (ssdata != null)
            {
                JSON tempobj = JSON.Serialize(ssdata);
                LogData.GetJSON(currentSecnarioMasterKey).GetJArray("StudentSetup").Add(tempobj);
                //currentStudentSetupData.Add(tempobj);
              logDataUpdated = true;
                return true;
            }
            else
            {
                Debug.LogAssertion("Data for New attepmt to save Student Setup is empty or null or not set properly, please recheck");
                return false;
            }
        Debug.LogAssertion("Please use SetScenarioMasterLogObject with appropriate key to start logging this into the correct scenario");
        return false;
    }

    public bool SetStudentActionsAnimationsData(StudentActionsAnimationsData saadata)
    {
        if (currentSecnarioMasterKey != "")
            if (saadata != null)
            {
                JSON tempobj = JSON.Serialize(saadata);
                LogData.GetJSON(currentSecnarioMasterKey).GetJSON("StudentActions").GetJArray("Animations").Add(tempobj);
                // currentStudentAnimationsData.Add(tempobj);
                logDataUpdated = true;
                return true;
            }
            else
            {
                Debug.LogAssertion("Data for New attepmt to save Student Actions Animations at is empty or null or not set properly, please recheck");
                return false;
            }
        Debug.LogAssertion("Please use SetScenarioMasterLogObject with appropriate key to start logging this into the correct scenario");
        return false;
    }

    public bool SetStudentActionsLookingAtData(StudentActionsLookingAtData saladata)
    {
        if (currentSecnarioMasterKey != "")
            if (saladata != null)
            {
                JSON tempobj = JSON.Serialize(saladata);
                LogData.GetJSON(currentSecnarioMasterKey).GetJSON("StudentActions").GetJArray("LookingAt").Add(tempobj);
                //currentStudentLookingAtData.Add(tempobj);
                logDataUpdated = true;
                return true;
            }
            else
            {
                Debug.LogAssertion("Data for New attepmt to save Student Actions Looking at is empty or null or not set properly, please recheck");
                return false;
            }
        Debug.LogAssertion("Please use SetScenarioMasterLogObject with appropriate key to start logging this into the correct scenario");
        return false;
    }


    public bool SetPlayerActionsLookingAtData(PlayerActionsLookingAtData padata)
    {
        if (currentSecnarioMasterKey != "")
            if (padata != null)
            {
                JSON tempobj = JSON.Serialize(padata);
                LogData.GetJSON(currentSecnarioMasterKey).GetJSON("PlayerActions").GetJArray("LookingAt").Add(tempobj);
                //currentPlayerLookingAtData.Add(tempobj);
                logDataUpdated = true;
                return true;
            }
            else
            {
                Debug.LogAssertion("Data for New attepmt to save Player Actions Looking at is empty or null or not set properly, please recheck");
                return false;
            }
        Debug.LogAssertion("Please use SetScenarioMasterLogObject with appropriate key to start logging this into the correct scenario");
        return false;
    }

    public bool SetPlayerActionsMovedToData(PlayerActionsMovedToData pamtdata)
    {
        if (currentSecnarioMasterKey != "")
            if (pamtdata != null)
            {
                JSON tempobj = JSON.Serialize(pamtdata);
                LogData.GetJSON(currentSecnarioMasterKey).GetJSON("PlayerActions").GetJArray("MovedTo").Add(tempobj);
                //currentPlayerMovedToData.Add(tempobj);
                logDataUpdated = true;
                return true;
            }
            else
            {
                Debug.LogAssertion("Data for New attepmt to save Player Actions Moved To is empty or null or not set properly, please recheck");
                return false;
            }
        Debug.LogAssertion("Please use SetScenarioMasterLogObject with appropriate key to start logging this into the correct scenario");
        return false;
    }

    public bool SetPlayerActionsSelectedStudentData(PlayerActionsSelectedStudentData passdata)
    {
        if (currentSecnarioMasterKey != "")
            if (passdata != null)
            {
                JSON tempobj = JSON.Serialize(passdata);
                LogData.GetJSON(currentSecnarioMasterKey).GetJSON("PlayerActions").GetJArray("SelectedStudent").Add(tempobj);
                //currentPlayerSelectedStudentData.Add(tempobj);
                logDataUpdated = true;
                return true;
            }
            else
            {
                Debug.LogAssertion("Data for New attepmt to save Player Actions Selected Student is empty or null or not set properly, please recheck");
                return false;
            }
        Debug.LogAssertion("Please use SetScenarioMasterLogObject with appropriate key to start logging this into the correct scenario");
        return false;
    }




    // if isNewAttempt is false,  then look for current SelectedStudent and use that to replace the data/ add data to that object
    public bool SetPlayerActionsSelectedStudentData(string key, string value, bool isNewAttempt = false)
    {
        if (currentSecnarioMasterKey != "")
            if (isNewAttempt)
            {
                if (key != null && key != "")
                {
                    if (key != "SelectionID" && key != "StudentName" && key != "TimeStamp")
                    {
                        Debug.LogAssertion("Key for New attepmt of Student Selection is not an expected key");
                        return false;
                    }
                    currentStudentSelectedID = LogData.GetJSON(currentSecnarioMasterKey).GetJSON("PlayerActions").GetJArray("SelectedStudent").Length;
                    JSON StudentSelectedSubActionsObject = new JSON(); // inner object of Teacher Sub Action
                    StudentSelectedSubActionsObject.Add("SelectionID", currentStudentSelectedID);
                    StudentSelectedSubActionsObject.Add(key, value);
                    LogData.GetJSON(currentSecnarioMasterKey).GetJSON("PlayerActions").GetJArray("SelectedStudent").Add(StudentSelectedSubActionsObject);
                    //currentTASData.Add(TeacherSubActionsObject);
                    logDataUpdated = true;
                    return true;
                }
                Debug.LogAssertion("Key for New attepmt of Student Selection is empty or null");
                return false;
            }
            else
            {
                if (key != null && key != "")
                {
                    if (key != "SelectionID" && key != "StudentName" && key != "TimeStamp" && key != "Duration" && key != "Phase")
                    {
                        Debug.LogAssertion("Key for adidng or replaceing a part of exiting of Student Selection  is not an expected key");
                        return false;
                    }
                   
                   if (key == "Duration" && value == "00:00")
                        LogData.GetJSON(currentSecnarioMasterKey).GetJSON("PlayerActions").GetJArray("SelectedStudent").RemoveAt(currentStudentSelectedID);
                   else
                        LogData.GetJSON(currentSecnarioMasterKey).GetJSON("PlayerActions").GetJArray("SelectedStudent").GetJSON(currentStudentSelectedID).AddOrReplace(key, value);

                        
                    logDataUpdated = true;
                    return true;
                }
                Debug.LogAssertion("Key for adidng or replacning a part of exiting of Student Selection is empty or null");
                return false;
            }
        Debug.LogAssertion("Please use SetScenarioMasterLogObject with appropriate key to start logging this into the correct scenario");
        return false;
    }




    public bool SetScenariosAttendedValueForKey(string key, string value)
    {
            if (key != null && key != "")
            {
                if (!key.Contains("SC"))
                {
                    Debug.LogAssertion("Key for attepmt of Scenario Attempted is not an expected key");
                    return false;
                }
                if (LogData.GetJSON("ScenariosAttempted").ContainsKey(key))
                    LogData.GetJSON("ScenariosAttempted").AddOrReplace(key, value);
                else
                    LogData.GetJSON("ScenariosAttempted").Add(key, value);
                logDataUpdated = true;
                return true;
            }
            else
            {
                Debug.LogAssertion("Data for New attepmt to save Scenarios Attempted is empty or null or not set properly, please recheck");
                return false;
            }
    }



    public bool SetGettingToKnowYourClassValueForKey(string key, string value)
    {
        if (currentSecnarioMasterKey != "")
            if (key != null && key != "")
            {

                if (key != "start1" && key != "end1")
                {
                    Debug.LogAssertion("Key for attepmt of Getting to know your class is not an expected key");
                    return false;
                }
                if (LogData.GetJSON(currentSecnarioMasterKey).GetJSON("GettingToKnowYourClass").ContainsKey(key))
                    LogData.GetJSON(currentSecnarioMasterKey).GetJSON("GettingToKnowYourClass").AddOrReplace(key, value);
                else
                    LogData.GetJSON(currentSecnarioMasterKey).GetJSON("GettingToKnowYourClass").Add(key, value);
                logDataUpdated = true;
                return true;
            }
            else
            {
                Debug.LogAssertion("Data for New attepmt to save Getting to know your class is empty or null or not set properly, please recheck");
                return false;
            }
        Debug.LogAssertion("Please use SetScenarioMasterLogObject with appropriate key to start logging this into the correct scenario");
        return false;
    }

    public bool SetInitialSituationValueForKey(string key, string value)
    {
        if (currentSecnarioMasterKey != "")
            if (key != null && key != "")
            {

                if (key != "start2" && key != "end2")
                {
                    Debug.LogAssertion("Key for attepmt of Initial Situation is not an expected key");
                    return false;
                }
                if (LogData.GetJSON(currentSecnarioMasterKey).GetJSON("InitialSituation").ContainsKey(key))
                    LogData.GetJSON(currentSecnarioMasterKey).GetJSON("InitialSituation").AddOrReplace(key, value);
                else
                    LogData.GetJSON(currentSecnarioMasterKey).GetJSON("InitialSituation").Add(key, value);
                logDataUpdated = true;
                return true;
            }
            else
            {
                Debug.LogAssertion("Data for New attepmt to save Initial Situation is empty or null or not set properly, please recheck");
                return false;
            }
        Debug.LogAssertion("Please use SetScenarioMasterLogObject with appropriate key to start logging this into the correct scenario");
        return false;
    }


    public bool SetPlayerActionsValueForKey(string key, string value)
    {
        if (currentSecnarioMasterKey != "")
            if (key != null && key != "")
            {

                if (key != "IntroPopupWindowShownAt" && key != "IntroPopupWindowAcceptedAt")
                {
                    Debug.LogAssertion("Key for attepmt of Player Action is not an expected key");
                    return false;
                }
                if (LogData.GetJSON(currentSecnarioMasterKey).GetJSON("PlayerActions").ContainsKey(key))
                    LogData.GetJSON(currentSecnarioMasterKey).GetJSON("PlayerActions").AddOrReplace(key, value);
                else
                    LogData.GetJSON(currentSecnarioMasterKey).GetJSON("PlayerActions").Add(key, value);
                logDataUpdated = true;
                return true;
            }
            else
            {
                Debug.LogAssertion("Data for New attepmt to save Player Actions Looking at is empty or null or not set properly, please recheck");
                return false;
            }
        Debug.LogAssertion("Please use SetScenarioMasterLogObject with appropriate key to start logging this into the correct scenario");
        return false;
    }




    // if isNewAttempt is false,  then look for current Teacher Action and use that to replace the data/ add data to that object
    public bool SetTeacherActionData(string key, string value, bool isNewAttempt = false)
    {
        if (currentSecnarioMasterKey != "")
            if (isNewAttempt)
            {
                if (key != null && key != "")
                {
                    if (key != "TeacherActionSelected" && key != "TA1" && key != "TA2" && key != "TASQuestion" && key != "TASTimeStarted" && key != "Model" && key != "Revision")
                    {
                        Debug.LogAssertion("Key for New attepmt of Teacher Action is not an expected key");
                        return false;
                    }
                    currentTAS = LogData.GetJSON(currentSecnarioMasterKey).GetJSON("TeacherActions").GetJArray("TASData").Length;
                    JSON TeacherSubActionsObject = new JSON(); // inner object of Teacher Sub Action
                    TeacherSubActionsObject.Add(key, value);
                    LogData.GetJSON(currentSecnarioMasterKey).GetJSON("TeacherActions").GetJArray("TASData").Add(TeacherSubActionsObject);
                    //currentTASData.Add(TeacherSubActionsObject);
                    logDataUpdated = true;
                    return true;
                }
                Debug.LogAssertion("Key for New attepmt of Teacher Action is empty or null");
                return false;
            }
            else
            {
                if (key != null && key != "")
                {
                    if (key != "TeacherActionSelected" && key != "TA1" && key != "TA2" && key != "TASQuestion" && key != "TASTimeStarted" && key != "Model" && key != "Revision" && key != "TASTimeFinished" && key != "TASComment")
                    {
                        Debug.LogAssertion("Key for adidng or replacning a part of exiting of Teacher Action is not an expected key");
                        return false;
                    }
                    LogData.GetJSON(currentSecnarioMasterKey).GetJSON("TeacherActions").GetJArray("TASData").GetJSON(currentTAS).AddOrReplace(key, value);
                    //currentTASData.GetJSON(currentTAS).AddOrReplace(key, value);
                    logDataUpdated = true;
                    return true;
                }
                Debug.LogAssertion("Key for adidng or replacning a part of exiting of Teacher Action is empty or null");
                return false;
            }
        Debug.LogAssertion("Please use SetScenarioMasterLogObject with appropriate key to start logging this into the correct scenario");
        return false;
    }











    /*

    public bool SetValueForUniqueKey(string key, string value)
    {
        if (key != null && key != "")
        {
            if (LogData.ContainsKey(key))
                LogData.AddOrReplace(key, value);
            else
                LogData.Add(key, value);
            return true;
        }
        return false;
    }

    public void SetValueForKey(string key, string value)
    {
        LogData.Add(key, value);
    }

    public void SetValueForKey(string key, int value)
    {
        LogData.Add(key, value);
    }

    public void SetValueForKey(string key, float value)
    {
        LogData.Add(key, value);
    }
    public void SetValueForKey(string key, bool value)
    {
        LogData.Add(key, value);
    }
    public void SetValueForKey(string key, object value)
    {
        LogData.Add(key, value);
    }
 
    */


    public void SetFilenameSuffix(string fileNameSuffix)
    {
        if(!fileNameAlreadySet)
        if (fileNameSuffix != null && fileNameSuffix != "")
        {
                // FILE_NAME = "/PlayerData_" + fileNameSuffix + "_" + DateTime.UtcNow.ToShortDateString() + ".json";              
                FILE_NAME = DataFileName();
            
                fileNameAlreadySet = true;
        }
    }

    public void SetFilenameSuffix()
    {
        if (!fileNameAlreadySet)
            SetFilenameSuffix(PlayerID);
    }


    public void SaveLogDataToFile()
    {

        //if (PlayerID != null && PlayerID !="")
        //    LogData.AddOrReplace("PlayerID", PlayerID); // incase PlayerID is not set already

        
            /*
            // Populate TeacherActions
            currentTeacherActionsObject.AddOrReplace("TASData", currentTASData);

            // Populate PlayerActions
            currentPlayerActionsObject.AddOrReplace("LookingAt", currentPlayerLookingAtData);
            currentPlayerActionsObject.AddOrReplace("MovedTo", currentPlayerMovedToData);
            currentPlayerActionsObject.AddOrReplace("SelectedStudent", currentPlayerSelectedStudentData);

            // Populate StudentActions
            currentStudentActionsObject.AddOrReplace("LookingAt", currentStudentLookingAtData);
            currentStudentActionsObject.AddOrReplace("Animations", currentStudentAnimationsData);

            // Add all the above and StudentSetup to the Current Scenario Object
            currentScenarioMasterObject.AddOrReplace("TeacherActions", currentTeacherActionsObject);
            currentScenarioMasterObject.AddOrReplace("StudentSetup", currentStudentSetupData);
            currentScenarioMasterObject.AddOrReplace("PlayerActions", currentPlayerActionsObject);
            currentScenarioMasterObject.AddOrReplace("StudentActions", currentStudentActionsObject);

            if (LogData.ContainsKey(currentSecnarioMasterKey))
                LogData.AddOrReplace(currentSecnarioMasterKey, currentScenarioMasterObject);
            else
                LogData.Add(currentSecnarioMasterKey, currentScenarioMasterObject);

            */
            string jsonAsString = LogData.CreatePrettyString();
            SetFilenameSuffix(PlayerID);
        //Debug.Log("Saving to File : " + FILE_PATH + FILE_NAME + " Data = " + jsonAsString);
        //    Debug.Log("Saving to File : " + DataFileName() + " Data = " + jsonAsString);
        Debug.Log("Saving to File : " + FILE_NAME  + " Data = " + jsonAsString);
        //StreamWriter writer = new StreamWriter(FILE_PATH + FILE_NAME);
        //  StreamWriter writer = new StreamWriter(DataFileName());
        StreamWriter writer = new StreamWriter(FILE_NAME);
        writer.WriteLine(jsonAsString);
            writer.Close();

        // Try to Push it to S
        #if UNITY_WEBGL && !UNITY_EDITOR
            Amazon_Stuff.Instance.PostObject(logDataFileName,string.Empty, jsonAsString);
        #else
            Amazon_Stuff.Instance.PostObject(logDataFileName,FILE_NAME, string.Empty);
        #endif


        logDataUpdated = false; // set it to false when the data is saved, it becomes true when the data is modified and is not saved.

    }

    public  JSON LoadLogFileToJsonObject()
    {
        StreamReader reader = new StreamReader(FILE_PATH);
        string jsonAsString = reader.ReadToEnd();
        reader.Close();
        JSON jsonObject = JSON.ParseString(jsonAsString);
        return jsonObject;
    }

    public JSON LoadLogFileToJsonObject(string filePath)
    {
        StreamReader reader = new StreamReader(filePath);
        string jsonAsString = reader.ReadToEnd();
        reader.Close();
        JSON jsonObject = JSON.ParseString(jsonAsString);
        return jsonObject;
    }

    string DataFileName()
    {
        string filePath = Application.dataPath + "/LogData/";
        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);

        logDataFileName = PlayerID + "_" + string.Format("{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now) + ".json";

        return filePath + PlayerID + "_" + string.Format("{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now) + ".json";
    }

    void Init()
    {
           
        FILE_PATH = Application.dataPath;

      


        // initialised here but used in SetUserIDAndInitDB called from User id acceptence screen
        LogData = new JSON();
     


        // FILE_NAME = "/PlayerData_" + PlayerID + "_" + DateTime.UtcNow.ToShortDateString() + ".json";

        /*
        if (System.IO.File.Exists(FILE_PATH + FILE_NAME))
        {
            JSON tempData = LoadLogFileToJsonObject(FILE_PATH + FILE_NAME);
                LogData = tempData;
        }
        else
        {
            // file does not exist, so lets start working on a new object and then save it later for this user
            LogData = new JSON();
            LogData.Add("PlayerID", PlayerID);
            LogData.Add("CreationTimeStamp", DateTime.UtcNow.ToShortDateString());
            JSON ScenariosAdded = new JSON();
            LogData.Add("ScenariosAttemted", ScenariosAdded);
        }
        */



        /*
         * // initiate basics Log object Arrays
            currentTASData = new JArray();
            currentPlayerLookingAtData = new JArray();
            currentPlayerMovedToData = new JArray();
            currentPlayerSelectedStudentData = new JArray();
            currentStudentLookingAtData = new JArray();
            currentStudentAnimationsData = new JArray();
            currentStudentSetupData = new JArray();


            currentScenarioMasterObject = new JSON();


            currentTeacherActionsObject = new JSON();
            currentPlayerActionsObject = new JSON();
            currentStudentActionsObject = new JSON();
            */
        // add all Log Objects to Debug in Editor

      

        /*currentScenarioMasterObject.DebugInEditor("My Scenario Master Object");
        currentTeacherActionsObject.DebugInEditor("My Teacher Actions  Object");
        currentPlayerActionsObject.DebugInEditor("My SPlayer Actions Object");
        currentStudentActionsObject.DebugInEditor("My Students Action Object");
        currentTASData.DebugInEditor("My TAS Array Object");
        currentPlayerLookingAtData.DebugInEditor("My Player Looking At Array Object");
        currentPlayerMovedToData.DebugInEditor("My Player  Moved to Array Object");
        currentPlayerSelectedStudentData.DebugInEditor("My Player Selected Student Array Object");
        currentStudentLookingAtData.DebugInEditor("My Student Looking At Array Object");
        currentStudentAnimationsData.DebugInEditor("My Student Animations Array Object");
        currentStudentSetupData.DebugInEditor("My Student Setup Array Object");
        */
        startSavingProcess = false;
    }


    public void SetUserIDAndInitDB(string uid)
    {
        if(uid !="")
        {
            PlayerID = uid;

            // set File name
                FILE_NAME = DataFileName();
            fileNameAlreadySet = true;
            print(FILE_PATH + FILE_NAME);
            print(FILE_NAME);



            LogData.Add("PlayerID", PlayerID);
            LogData.Add("CreationTimeStamp", DateTime.UtcNow.ToShortDateString());
            JSON ScenariosAdded = new JSON();
            LogData.Add("ScenariosAttempted", ScenariosAdded);

            LogData.DebugInEditor("My Event Logs Object");

        }
    }


}

#region LOGJSONSubTypeData 

[System.Serializable]
public class StudentSetupData : IEquatable<StudentSetupData>
{
    public string StudentName;
    public int Chair;
    public string Desk;
    public int CubbyBox;
    public string NeighbourStudent;

    public bool Equals(StudentSetupData other)
    {
        throw new NotImplementedException();
    }
    public override string ToString()
    {
        return "[StudentSetupData:  StudentName = \"" + StudentName + "\", Chair = " + Chair.ToString() + "\", Desk = " + Desk + "\", CubbyBox = " + CubbyBox.ToString() + "\", NeighbourStudent = " + NeighbourStudent + "]";
    }
}

[System.Serializable]
public class PlayerActionsLookingAtData : IEquatable<PlayerActionsLookingAtData>
{
    public string target;
    public string time;
    public string CurrentCell;
    public Vector3 CurrentActualPos;
    public Vector3 CurrentActualLookingDirection;

    public bool Equals(PlayerActionsLookingAtData other)
    {
        throw new NotImplementedException();
    }
    public override string ToString()
    {
        return "[PlayerActionsLookingAtData:  target = \"" + target + "\", time = " + time + "\", CurrentCell = " + CurrentCell + "\", CurrentActualPos = " + CurrentActualPos + "\", CurrentActualLookingDirection = " + CurrentActualLookingDirection + "]";
    }
}

[System.Serializable]
public class PlayerActionsMovedToData : IEquatable<PlayerActionsMovedToData>
{
    public string FromCell;
    public string CurrentCell;
    public string time;
    public Vector3 CurrentActualPos;

    public bool Equals(PlayerActionsMovedToData other)
    {
        throw new NotImplementedException();
    }
    public override string ToString()
    {
        return "[PlayerActionsMovedToData:  FromCell = \"" + FromCell + "\", CurrentCell = " + CurrentCell  + "\", time = " + time  + "\", CurrentActualPos = " + CurrentActualPos + "]";
    }
}

/*
 *"SelectedStudent" : [
                            {
                                "StudentName" : "Name",
                                "TimeStamp" : "34343",
                                "Duration" : "34343",
                                "Phase" : "MA-I"
                            }
                    ]
                }
*/
[System.Serializable]
public class PlayerActionsSelectedStudentData : IEquatable<PlayerActionsSelectedStudentData>
{
    public string StudentName;
    public string TimeStamp;
    public string Duration;
    public string Phase;

    public bool Equals(PlayerActionsSelectedStudentData other)
    {
        throw new NotImplementedException();
    }
    public override string ToString()
    {
        return "[PlayerActionsLookingAtData:  StudentName = \"" + StudentName + "\", TimeStamp = " + TimeStamp + "\", Duration = " + Duration + "\", Phase = " + Phase + "]";
    }
}

[System.Serializable]
public class StudentActionsLookingAtData : IEquatable<StudentActionsLookingAtData>
{
    public string StudentName;
    public string target;
    public string time;
   // public string CurrentCell;
    public Vector3 CurrentActualPos;

    public bool Equals(StudentActionsLookingAtData other)
    {
        throw new NotImplementedException();
    }
    public override string ToString()
    {
        //  return "[StudentActionsLookingAtData:  StudentName = " + StudentName + "\", target = \"" + target + "\", time = " + time + "\", CurrentCell = " + CurrentCell + "\", CurrentActualPos = " + CurrentActualPos + "]";
        return "[StudentActionsLookingAtData:  StudentName = " + StudentName + "\", target = \"" + target + "\", time = " + time + "\", CurrentActualPos = " + CurrentActualPos + "]";

    }
}

[System.Serializable]
public class StudentActionsAnimationsData : IEquatable<StudentActionsAnimationsData>
{
    public string StudentName;
    public string AnimationName;
    public string time;
  //  public string CurrentCell;

    public bool Equals(StudentActionsAnimationsData other)
    {
        throw new NotImplementedException();
    }
    public override string ToString()
    {
        //  return "[StudentActionsAnimationsData:  StudentName = \"" + StudentName + "\", AnimationName = " + AnimationName + "\", time = " + time + "\", CurrentCell = " + CurrentCell + "]";
        return "[StudentActionsAnimationsData:  StudentName = \"" + StudentName + "\", AnimationName = " + AnimationName + "\", time = " + time + "]";
    }
}

#endregion


