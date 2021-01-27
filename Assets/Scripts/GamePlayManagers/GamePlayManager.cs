using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

[System.Serializable]
public class MainActions : UnityEvent { }


public class GamePlayManager : MonoBehaviour
{

    [Header ("Managers")]

    public UserInterfaceManager userInterfaceManager;
    public PlayerActionDataHandler playerActionDataHandler;
    public DataManager dataManager;
    public GameObject timeCanvas;
    public Transform timerSeconds, timerMinutes, timerHours;
    public GameObject timeText;
    public Button getToKnowSkipBtn;
    public List<StudentAction> studentsActions;
    public List<StudentAction> studentsAsNeighboursActions;
    public List<Transform> LookatPointsOfInsterst;

    public string currentScenario = ""; // id of the scenario that is loaded in the game now.
    public bool timeStarted,initialActionForScenarioIsCommon;  // scenario managers should set this to true if they have the same initial action sequense as listed below
    public bool lockRotation=false;

    private string currentSelectedStudent = "";
    public bool InitialIntroComplete { set; get; } = false;
    public bool ReadyForInitialSituation { set; get; } = false;
    /*
     * Students sit at their tables and look around at random locations (mb25+mb9/mb15/mb17),
     * mb25 = students sit on the seat and
     * mb9 = students look around class
     * mb15 = students look at neighbours
     * mb17 = students look out of windows
     * 7 pairs of students (random locations) quietly whisper with each other (vi11) [soft background mumbling]
     * */

    public bool LOG_ENABLED=false;

    public event Action<GameObject> NewStudentSelected; 

    private string playerID;
    private GameObject previouslySelectedStudent;
    private int timewrapFactor = 1;
    float degreesPerHour = 30f;
    float degreesPerMinute = 6f;
    float intialHour, initialMinutes;
    private string timeScaleText = "Mins";
    private float currentTimeElapsed;
    private bool _isRunning;
    public bool isStudentsInteractable = true;

    [Space(20)]
    public AudioSource audioSource;
       

    [Space(20)]
    [SerializeField]
    public MainActions whatToDoBeforeInitialSituation, mainActionOne, mainActionTwo, mainActionThree;
    public float MainActionOneDelay, MainActionTwoDelay, MainActionThreeDelay;


    private bool stopthedelay;
    public string CurrentPhase { get; set; } = "";

    private void Awake()
    {
        if (getToKnowSkipBtn != null) getToKnowSkipBtn.gameObject.SetActive(false);
        TimeSpan timeSpan = DateTime.Now.TimeOfDay;
               print(" " + timeSpan.TotalMinutes + " hours : "+timeSpan.TotalSeconds );
                print(" " + (timeSpan.TotalMinutes * degreesPerHour).ToString() + " hours : " + (timeSpan.Seconds * degreesPerMinute).ToString());
        timerHours.localRotation = Quaternion.Euler(0f, 0f, 360f - (float)timeSpan.TotalHours * degreesPerHour);
        timerMinutes.localRotation = Quaternion.Euler(0f, 0f, 360f - (float)timeSpan.TotalMinutes * degreesPerMinute);
        intialHour = (float)timeSpan.TotalHours * degreesPerHour;
        initialMinutes = (float)timeSpan.TotalMinutes * degreesPerHour;

    }




    #region Phase 1
    //This function is called when user click the start button.
    public event System.Action StartTheNavMesh, StartWithSitOnDesk;

    public event System.Action StartPrepForStudentReaction;

    // we use this as the students should be seated before the user presses start button
    public void FirstScenarioStartWithSittingPos() {
        if (LOG_ENABLED)
        {
            if (LogDB.instance.PlayerID == null || LogDB.instance.PlayerID == "")
            {
                Debug.LogWarning("First secnario : User id was not found from the Login area, so creating one.");
                Guid guid = Guid.NewGuid();
                playerID = guid.ToString("N");
                LogDB.instance.PlayerID = playerID;
            }
            else
                playerID = LogDB.instance.PlayerID;
        }
        else
            playerID = "test";
        playerActionDataHandler.PlayerID = playerID;
        StartWithSitOnDesk();
        //  if(audioSource.isPlaying) audioSource.Pause();
        CurrentPhase = "GETTOKNOW";
    }
    // we start the phase two in scenario 1 only when the user presses the start button.
    public void ScenarioOneStartPhaseTwo()
    {
        //set timer canvas invisble
        timeCanvas.SetActive(true);
        // timeWrapButton.gameObject.SetActive(true);
         
        StartCoroutine(WaitTillIntroIsFinishedAndThenProceedPhaseTwo());
  //      if (!timeStarted) StartTimer();
    }

    // for every other Scenarios we use this.. unless that scenario is triggered by user intervention.
    public void StartWithSittingPos()
    {
        //set timer canvas invisble

         timeCanvas.SetActive(false);
        //     timeWrapButton.gameObject.SetActive(true);
        if (LOG_ENABLED)
        {
            if (LogDB.instance.PlayerID == null || LogDB.instance.PlayerID == "")
            {
                Debug.LogWarning("Secnario start with sitting : User id was not found from the Login area, so creating one.");
                Guid guid = Guid.NewGuid();
                playerID = guid.ToString("N");
                LogDB.instance.PlayerID = playerID;
            }
            else
                playerID = LogDB.instance.PlayerID;
        }
        else
            playerID = "test";
        playerActionDataHandler.PlayerID = playerID;
        if (!timeStarted) StartTimer(false);
        CurrentPhase = "GETTOKNOW";
        StartWithSitOnDesk();
        StartCoroutine(WaitTillIntroIsFinishedAndThenProceedPhaseTwo());
    }


    // Not needed to use this anymore as we will be calling the above function in Scenario 1 also, because client wants the students to be already seated
    public void CreatePlayerID()
    {
        //Start The Phase
        if (LOG_ENABLED)
        {
            if (LogDB.instance.PlayerID == null || LogDB.instance.PlayerID == "")
            {
                Debug.LogWarning("Create Player : User id was not found from the Login area, so creating one.");
                Guid guid = Guid.NewGuid();
                playerID = guid.ToString("N");
                LogDB.instance.PlayerID = playerID;
            }
            else
                playerID = LogDB.instance.PlayerID;
        }
        else
            playerID = "test";
        playerActionDataHandler.PlayerID = playerID;
        CurrentPhase = "GETTOKNOW";
        StartTheNavMesh();

        StartCoroutine(WaitTillIntroIsFinishedAndThenProceedPhaseTwo());

        // timeWrapButton.gameObject.SetActive(true);



    }

    public void WrapTime()
    {
        switch (timewrapFactor)
        {
            case 1:
                Time.timeScale = 2;
 //               timeWrapButton.GetComponentInChildren<Text>().text = " 5x >>";
                timewrapFactor = 2;
                break;
            case 2:
                Time.timeScale = 5;
 //               timeWrapButton.GetComponentInChildren<Text>().text = " 1x >>";
                timewrapFactor = 5;
                break;
            case 5:
                Time.timeScale = 1;
 //              timeWrapButton.GetComponentInChildren<Text>().text = " 2x >>";
                timewrapFactor = 1;
                break;
            default:
                Time.timeScale = 1;
 //               timeWrapButton.GetComponentInChildren<Text>().text = " 2x >>";
                timewrapFactor = 1;
                break;
        }

    }

    public void WrapTime(int wrapFactor)
    {
        timewrapFactor = wrapFactor;
        WrapTime();
    }
   

    void Update()
    {
        if (!_isRunning) return;

     //   if (getToKnowSkipBtn != null) if(getToKnowSkipBtn.gameObject.activeInHierarchy);
        currentTimeElapsed += Time.deltaTime;
        var timeSpan = TimeSpan.FromSeconds(currentTimeElapsed);
 //        print(" " + timeSpan.Minutes + " hours : " + timeSpan.Seconds);
 //        print(" " + ((timeSpan.TotalHours + timeSpan.Minutes)).ToString() + " hours : " + (timeSpan.TotalMinutes + timeSpan.Seconds).ToString());
        //       timeText.GetComponent<Text>().text = string.Format("{0:D2} hours : {1:D2} "+ timeScaleText, timeSpan.Minutes, timeSpan.Seconds);
        timerHours.localRotation = Quaternion.Euler(0f, 0f, 360f - (float)(intialHour + (timeSpan.Minutes * degreesPerHour)));
        timerMinutes.localRotation = Quaternion.Euler(0f, 0f, 360f - (float)(initialMinutes + (timeSpan.Seconds * degreesPerMinute)));
        //  TimeSpan timeSpan = DateTime.Now.TimeOfDay; 
      //  print(" " + timeSpan.TotalMinutes + " hours : "+timeSpan.TotalSeconds );
      //  print(" " + ((timeSpan.TotalHours + timeSpan.Minutes)).ToString() + " hours : " + (timeSpan.TotalMinutes + timeSpan.Seconds).ToString());
      //  timerHours.localRotation = Quaternion.Euler(0f,0f, 360f-(float)(timeSpan.TotalHours+timeSpan.Minutes) * degreesPerHour);
      //  timerMinutes.localRotation = Quaternion.Euler(0f, 0f, 360f- (float)(timeSpan.TotalMinutes+timeSpan.Seconds) * degreesPerMinute);
//        timerSeconds.localRotation = Quaternion.Euler(0f, 0f, 360f - timeSpan.Milliseconds * degreesPerSecond);

    }

    public void SetTimeScaleTextToSeconds()
    {
        timeScaleText = "Sec";
    }
    public void SetTimeScaleTextToMinutes()
    {
        timeScaleText = "Mins";
    }
    public void StartTimer(bool showClock)
    {
        timeStarted = true;
       if(showClock) if (!timeCanvas.activeInHierarchy) timeCanvas.SetActive(true);
        _isRunning = true;
    }
    public void ShowTimer(bool showClock)
    {
        if (showClock) if (!timeCanvas.activeInHierarchy) timeCanvas.SetActive(true);
    }

    public void ResetTimer()
    {
        currentTimeElapsed = 0;
    }

    public string GetCurrentTime()
    {
        var timeSpan = TimeSpan.FromSeconds(currentTimeElapsed);
       return string.Format("{0:D2}:{1:D2} ", timeSpan.Minutes, timeSpan.Seconds);
    }

    public TimeSpan GetCurrentTimeSpan()
    {
        return TimeSpan.FromSeconds(currentTimeElapsed);
    }

    public void StopTimer()
    {
        _isRunning = false;
        if (timeCanvas.activeInHierarchy) timeCanvas.SetActive(true);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void TimeWrapButtonEnable(bool whatToDo)
    {
      //  WrapTime(0);
        if (whatToDo)
            timeCanvas.SetActive(true);
        else
            timeCanvas.SetActive(false);
    }


    // this function is used to show the Student and also log the student's name, time stamp and Phase
    public void CurrentlySelectedStudent(GameObject studentName, string timeStamp, bool mouseEnteredIn)
    {

        /*if (previouslySelectedStudent != null) {
            previouslySelectedStudent = studentName;
        }*/
     //  print(studentName.name + "called selected Student...");
        if (isStudentsInteractable)
        {
            StudentData studentData = dataManager.GetStudentData(studentName.name);
            if (studentData != null)
            {
                userInterfaceManager.PopulateStudentInfo(studentData);
                /*
                *"SelectedStudent" : [
                            {
                                "SelectionID":1,
                                "StudentName" : "Name",
                                "TimeStamp" : "34343",
                                "Duration" : "34343",
                                "Phase" : "MA-I"
                            }
                    ]
                }
                */

                currentSelectedStudent = studentName.name;
                //logging student selected data to LogDB
                if (LOG_ENABLED) LogDB.instance.SetPlayerActionsSelectedStudentData("StudentName", studentName.name, true);
                if (LOG_ENABLED) LogDB.instance.SetPlayerActionsSelectedStudentData("TimeStamp", timeStamp);
                if (LOG_ENABLED) LogDB.instance.SetPlayerActionsSelectedStudentData("Phase", CurrentPhase);
            }
        }
    }

    // this function is use used only to log the student's duration of being looked at
    public void CurrentlySelectedStudent(GameObject studentName,string duration) {

        // called only for closing the duration of the student who got selected
        /*if (previouslySelectedStudent != null) {
            previouslySelectedStudent = studentName;
        }*/
        /*
        *       "SelectedStudent" : [
                    {
                         "SelectionID":1,
                        "StudentName" : "Name",
                        "TimeStamp" : "34343",
                        "Duration" : "34343",
                        "Phase" : "MA-I"
                    }
            ]
        }
        */
        //logging student selected data to LogDB
        if (currentSelectedStudent == studentName.name)
            if (LOG_ENABLED) LogDB.instance.SetPlayerActionsSelectedStudentData("Duration", duration);
    }

    #endregion

    //Initial situation
    #region Phase 2

    public void StartPhaseTwo() {       
        //   ResetTimer();
        //   WrapTime(0);
        //  timeWrapButton.gameObject.SetActive(false);
        //     SetTimeScaleTextToSeconds();


        //  PlayNextPhase(StartPhaseThree, 30f); // originally
        // ResetTimer();
        //  if (LOG_ENABLED) LogDB.instance.SetPlayerActionsValueForKey("IntroPopupWindowShownAt", GetCurrentTime());
        userInterfaceManager.ShowOrHideKnowYourClassPanel(false);


        // Lock player from controlling their movement and rotation.
        Debug.Log("(GamePlayManager) | StartPhaseTwo() | MoveAndLookLocked = true");
        GameObject.FindGameObjectWithTag("Player").GetComponent<RigidbodyFirstPersonController>().MoveAndLookLocked = true;

        StartCoroutine(WaitTillSomethingExtraToBeDoneBeforeShowingInitialSituation());
    }


    IEnumerator WaitTillSomethingExtraToBeDoneBeforeShowingInitialSituation()
    {
        if (whatToDoBeforeInitialSituation != null)
        {
//            Debug.Log("Wait For Something before Initial Situaton is not null");
            ReadyForInitialSituation = false;
            whatToDoBeforeInitialSituation.Invoke();
        }
        else
        {
   //         Debug.Log("Wait For Something before Initial Situaton is null, skipping it");
            ReadyForInitialSituation = true;
        }
            

        yield return new WaitUntil(() => ReadyForInitialSituation);
        CurrentPhase = "INITIAL-SITUATION";
        if (!timeStarted) StartTimer(true);
        Debug.Log(CurrentPhase + " started...");
        userInterfaceManager.ShowOrHideQuestionPanel(true);
        PlayNextPhase(StartPhaseThree, 30f); // Starts Phase Three afer 30 seconds

    }

    IEnumerator WaitTillIntroIsFinishedAndThenProceedPhaseTwo()
    {
        yield return new WaitUntil(() => InitialIntroComplete);
        PlayNextPhase(StartPhaseTwo, 180f); // client needs this to be for 3 whole minutes  as per JIRA EQ-65. 
    }


    #endregion

    //Main Action I
    #region Phase 3

    public void StartPhaseThree()
    {
        //   WrapTime(0); // reset the timeScale

        userInterfaceManager.ShowOrHideQuestionPanel(false);
        CurrentPhase = "MA-I";
       // if (LOG_ENABLED) LogDB.instance.SetPlayerActionsValueForKey("IntroPopupWindowAcceptedAt", GetCurrentTime());
        SetTimeScaleTextToMinutes();
        if (LOG_ENABLED) LogDB.instance.SetSavingProcessStart(); // asks the LogDB to start the timer coutdown for saving of log file 
        //StopTimer();
        mainActionOne.Invoke();
        
    }
    #endregion


    //Main Action II
    #region Phase 4

    public void StartPhaseFour()
    {
        //  StartTimer();
        CurrentPhase = "MA-II";
        mainActionTwo.Invoke();
      
    }

  
    #endregion

    //Main Action III
    #region Phase 5

    public void StartPhaseFive()
    {
        CurrentPhase = "MA-III";
        mainActionThree.Invoke();
    }

    #endregion

    //Teacher Action (TA)
    #region Phase 6

    public void StartPhaseSix()
    {
        foreach (StudentAction s in studentsActions) s.SaveMyMood();

        //  WrapTime(0); // reset the timeScale
        //  ResetTimer();
        //  StopTimer();
        GameObject.FindGameObjectWithTag("Player").GetComponent<RigidbodyFirstPersonController>().enabled = true;
        StartPrepForStudentReaction?.Invoke();
        userInterfaceManager.ShowOrHideTeacherActionPanel(true);

        CurrentPhase = "TA";
    }

    #endregion

    //Student Reaction (SR)
    #region Phase 7

    public void StartPhaseSeven(float delay)
    {
     //   if(timeCanvas.activeInHierarchy)timeWrapButton.gameObject.SetActive(true);
        PlayNextPhase(StartPhaseEight, delay);
    }

    #endregion

    //Reflection
    #region Phase 8

    public void StartPhaseEight()
    {
      //  WrapTime(0); // reset the timeScale
     //   ResetTimer();
        //StopTimer();
        
        GameObject.FindGameObjectWithTag("Player").GetComponent<RigidbodyFirstPersonController>().enabled = false;
        userInterfaceManager.ShowOrHideTempPanel(false);
        
       // show Teacher action panel only once as per JIRA EQ-47
       if (userInterfaceManager.noOfNoCounts == 0)
            userInterfaceManager.ShowOrHideDecisionPanel(true);
        else
           userInterfaceManager.InitiateShowMultiChoiceAnswerPanelAndContinueExitScenario();
       
       
    }

    #endregion


    #region Generic Functions

    public void stopTheDelay()
    {
        stopthedelay = true;

        // a simple hack to make the camera not get fixed to where the player turned it before locking the camera, as per Jira EQ-28
        //Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);
        Quaternion foo = Camera.main.transform.rotation;
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0, foo.eulerAngles.y, foo.eulerAngles.z));
    }

    private delegate void NextPhase();

    private void PlayNextPhase(NextPhase nextPhase, float delay) {
        StartCoroutine(StartNextPhaseAfterGivenDelay(nextPhase, delay));
    }
   
    private IEnumerator StartNextPhaseAfterGivenDelay(NextPhase nextPhase,float delay) {
        // changing this to check if there is any skip used while wait for delay is happening...
        // so we comment the original code and wait for every second until the time asked in delay is complete or is been interuptted by user pressing buttons to interupt.
        //   yield return new WaitForSeconds(delay);
        //    nextPhase();
        if(delay >=1f)
        {
            var startTime = 0f;
            while (startTime < delay)
            {
                if (!stopthedelay)
                {
                    yield return new WaitForSeconds(1f);
                    startTime += 1.0f;
                }
                else
                    startTime = delay;
            }
            stopthedelay = false;
        }
        else
        {
            yield return new WaitForSeconds(delay);
        }
        nextPhase();
        yield return null;

    }

    #endregion
}
