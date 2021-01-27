using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ScenarioTwelveManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    public Transform tableOnePoint;
    public List<AudioSource> audiosInClass;
    public AudioSource MaximAudio1, MaximAudio2;

    string questionString = "";

    public Animator pencilAnim;
    public Animator pencilCaseAnim;

    public GameObject inHandPencil, inLeftHandPencil;
    public GameObject inHandPencil_Tom;
    public StudentAction kidMaxim;

    public GameObject window02;

    private void Awake()
    {
        gamePlayManager.currentScenario = "SC2";
    }
        private void Start()
    {
        gamePlayManager.initialActionForScenarioIsCommon = true;
        gamePlayManager.userInterfaceManager.ShowOrHideInitLoadingPanel(true);
        StartCoroutine(StartTheScene());
    }

    IEnumerator StartTheScene()
    {

        yield return new WaitForSeconds(1.0f);
       
        gamePlayManager.StartWithSittingPos();


        yield return new WaitForSeconds(2.0f);
        gamePlayManager.userInterfaceManager.ShowOrHideInitLoadingPanel(false);

        gamePlayManager.userInterfaceManager.ShowOrHideIntroPanel(true);
        yield return new WaitUntil(() => gamePlayManager.InitialIntroComplete);

        gamePlayManager.userInterfaceManager.ShowOrHideKnowYourClassPanel(true);
        gamePlayManager.audioSource.Play();
        yield return new WaitForSeconds(3.0f);
        // show the skip button for the ShowOrHideKnowYourClassPanel
        if (gamePlayManager.getToKnowSkipBtn != null) gamePlayManager.getToKnowSkipBtn.gameObject.SetActive(true);
    }

    public void WhatToDoBeforeInitialSituation()
    {
        // nothing to do, let us continue to Initial Situation
        gamePlayManager.ReadyForInitialSituation = true;
    }

    public void StudentReaction(string teacherReactionIndex)
    {

        string value = teacherReactionIndex;
        gamePlayManager.CurrentPhase = "SR-" + value;
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TeacherActionSelected", value,true);
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TASQuestion", gamePlayManager.userInterfaceManager.teacherSelectedQuestion);
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TASTimeStarted", gamePlayManager.GetCurrentTime());

        // get the background students back to their random looking and writing sequence.
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].studentAnimation.ResetCurrentAnim();
            if (i != 16 && i!=19) // except for Leonie and tom
            {
                gamePlayManager.studentsActions[i].StartMyRandomLookingOrWrittingAnimations();
            }
        }
        
        switch (value)
        {

            case "1":
                questionString = "Sie loben Maxim, dass er so gut gearbeitet hat und erklären ihm, dass die anderen noch etwas Zeit und Ruhe brauchen und er daher leise seine Lösungen überprüfen soll.";

                StudentReactionOne();
                break;
            case "2":
                questionString = "Sie erklären Maxim, dass die anderen Kinder noch etwas Zeit und Ruhe brauchen und geben ihm eine motivierende Bonusaufgabe.";

                StudentReactionTwo();
                break;
            case "3":
                questionString = "Sie loben Maxim, dass er so gut gearbeitet hat und fordern ihn freundlich auf, seinem Nachbarkind bei der Aufgabenbearbeitung zu helfen.";

                StudentReactionThree();
                break;
            case "4":
                questionString = "Sie fordern Maxim freundlich auf, die anderen Kinder nicht zu stören und erlauben ihm zur Belohnung, sich mit einem Spiel auf dem Tablet zu beschäftigen.";
                StudentReactionFour();

                break;
        }

        //showTempText();
        gamePlayManager.playerActionDataHandler.PlayerAction = questionString;
        
        if (value != "4") {
            gamePlayManager.StartPhaseSeven(20.0f);
        }
       

    }

    void showTempText() {

     //   gamePlayManager.userInterfaceManager.ShowOrHideTempPanel(true);
     //   gamePlayManager.userInterfaceManager.SetTempText(questionString);


    }

    public void MainActionOne()
    {
        UnityEngine.Debug.Log("main action 1 starts");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, tableOnePoint.transform);
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);

        // All kids  is looking at tablets and writing on paper (mb25), 

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            //gamePlayManager.studentsActions[i].StopLookAtSomeone();
            //gamePlayManager.studentsActions[i].StopSittingIdleLookAroundAnything(); // stop the initial intro animations for all kids
            gamePlayManager.studentsActions[i].scenarioStart = false;
            gamePlayManager.studentsActions[i].StartLookingAtTabletAndWrittingAnimations();
  
        }
        
        //kidMaxim.studentAnimation.SitAgitated(true);
        StartCoroutine(TriggerMainActionTwo());

    }

    IEnumerator TriggerMainActionTwo()
    {
        yield return new WaitForSeconds(2.0f);  //previously 10 seconds
        kidMaxim.StopLookingAtTabletAndWrittingAnimations();
        //kidMaxim.studentAnimation.SitAgitated(true);
        kidMaxim.StopLookAtSomeone();
        kidMaxim.StopMyRandomLookingAnimations();
        kidMaxim.studentAnimation.ResetAllAnim();
        kidMaxim.studentAnimation.EE2_GetUpsetSeated(true);    //.IWO29_FidgetingOnChair(true);

        UnityEngine.Debug.Log("main action 2 starts");
        

        yield return new WaitForSeconds(1.0f);
        //gamePlayManager.studentsActions[1].SetMyMood(MoodIndicator.Middle); // students from table 1  || task EQ-103 
        //gamePlayManager.studentsActions[2].SetMyMood(MoodIndicator.Middle);
        //gamePlayManager.studentsActions[3].SetMyMood(MoodIndicator.Middle);
        //gamePlayManager.studentsActions[4].SetMyMood(MoodIndicator.Middle);
        //gamePlayManager.studentsActions[16].SetMyMood(MoodIndicator.Middle);
        //kidMaxim.StopLookingAtTabletAndWrittingAnimations();

        int count = 0;
        foreach (StudentAction s in gamePlayManager.studentsActions)
        {
            count += 1;
            int LookOutsideChance = Random.Range(1, 2);

            //pause Maxim's fidgeting, otherwise animation loops too long
            if (count == 7) {
                kidMaxim.studentAnimation.EE2_GetUpsetSeated(false);
                yield return new WaitForSeconds(1.0f);
            }

            if (count == 19) {
                kidMaxim.studentAnimation.EE2_GetUpsetSeated(true);
                yield return new WaitForSeconds(1.0f);
            }

            switch (LookOutsideChance)
            {
                case 1:
                    s.LookAtWindowRoutine();
                    yield return new WaitForSeconds(Random.Range(0.1f, 1f));
                    s.LookAtWindowRoutineStop();
                    s.LookAtSomeone(s.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
                    break;
                case 2:
                    break;
            }
            
            if (s != kidMaxim)
            {
                yield return new WaitForSeconds(Random.Range(0.1f, 1f));
                s.studentAnimation.MB33_WorkOnSheets(true);
            }
        }
        
        yield return new WaitForSeconds(0.1f);

        count = 0;
        foreach (StudentAction s in gamePlayManager.studentsActions)
        {
            s.LookAtWindowRoutine();
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            s.LookAtWindowRoutineStop();
        }

        kidMaxim.studentAnimation.EE2_GetUpsetSeated(false);
        yield return new WaitForSeconds(1.0f);

        yield return new WaitForSeconds(0.1f);
        gamePlayManager.StartPhaseFour();
    }



    public void MainActionTwo()
    {
        StartCoroutine(TriggerMainActionThree());
    }



    IEnumerator TriggerMainActionThree()
    {
        UnityEngine.Debug.Log("main action 3 starts");
        yield return new WaitForSeconds(1.0f); // timeframe is already covered for nearly 10 seconds
        

        kidMaxim.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);

        gamePlayManager.studentsActions[2].StopLookingAtTabletAndWrittingAnimations();
        gamePlayManager.studentsActions[3].StopLookingAtTabletAndWrittingAnimations();
        gamePlayManager.studentsActions[4].StopLookingAtTabletAndWrittingAnimations();
        gamePlayManager.studentsActions[1].StopLookingAtTabletAndWrittingAnimations();
        gamePlayManager.studentsActions[16].StopLookingAtTabletAndWrittingAnimations();

        gamePlayManager.studentsActions[2].LookAtSomeone(kidMaxim.gameObject.transform);
        gamePlayManager.studentsActions[3].LookAtSomeone(kidMaxim.gameObject.transform);
        gamePlayManager.studentsActions[4].LookAtSomeone(kidMaxim.gameObject.transform);
        gamePlayManager.studentsActions[1].LookAtSomeone(kidMaxim.gameObject.transform);
        gamePlayManager.studentsActions[16].LookAtSomeone(kidMaxim.gameObject.transform);

        
        //  SpeechManager.Instance.StartTalking(teacherTextToSpeak);
        yield return new WaitForSeconds(1f);
        string studentTextToSpeak = "Fertig! Darf ich jetzt raus?";
        if (gamePlayManager.LOG_ENABLED) SpeechManager.Instance.StartTalking(studentTextToSpeak, true);
        //MaximAudio1.Play();

        yield return new WaitForSeconds(5f);

        gamePlayManager.studentsActions[1].SetMyMood(MoodIndicator.Middle); // students from table 1  || task EQ-103 
        gamePlayManager.studentsActions[2].SetMyMood(MoodIndicator.Middle);
        gamePlayManager.studentsActions[3].SetMyMood(MoodIndicator.Middle);
        gamePlayManager.studentsActions[4].SetMyMood(MoodIndicator.Middle);
        gamePlayManager.studentsActions[16].SetMyMood(MoodIndicator.Middle);

        kidMaxim.studentAnimation.SitAgitated(false);
        kidMaxim.studentAnimation.IWO29_FidgetingOnChair(false);
        kidMaxim.studentAnimation.RaiseHand(true);
        kidMaxim.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);

        yield return new WaitForSeconds(5f);
        gamePlayManager.StartPhaseFive();
    }

    public void MainActionThree()
    {
        StartCoroutine(TriggerTeacherQuestionPhase()); //Teacher Question Panel
    }

    IEnumerator TriggerTeacherQuestionPhase() 
    {
        yield return new WaitForSeconds(7.0f);
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookingAtTabletAndWrittingAnimations();
        }
        gamePlayManager.StartPhaseSix();
    }


    IEnumerator GradualVolumeChange(int numSeconds, AudioSource audioSource, float endVolume) {

        float increment = ((endVolume - audioSource.volume) / numSeconds);

        for (int i = 0; i <= numSeconds; i++) {
            yield return new WaitForSeconds(1.0f);
            audioSource.volume += increment;
        }
    }

    private void StudentReactionOne()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4MoveToPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, kidMaxim.gameObject.transform);

        //Student mumbling for 3 seconds...
        audiosInClass[0].volume = 0.5f;
        audiosInClass[0].Play();
        StartCoroutine(GradualVolumeChange(3, audiosInClass[0], 0));

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);

            if (gamePlayManager.studentsActions[i] != kidMaxim)
            {
                gamePlayManager.studentsActions[i].StartLookingAtTabletAndWrittingAnimations();
            }
            else
            {
                kidMaxim.studentAnimation.MB18_NodHead(true);
                StartCoroutine(TriggerMaximToStopHeadNod());
                //kidMaxim.LookAtSomeone(kidMaxim.GetComponent<StudentAction>().chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
                
            }
        }

    }
    IEnumerator TriggerMaximToStopHeadNod()
    {
        yield return new WaitForSeconds(2.0f);
        kidMaxim.studentAnimation.MB18_NodHead(false);
    }


    private void StudentReactionTwo()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4MoveToPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, kidMaxim.gameObject.transform);

        //Student mumbling for 5 seconds...
        audiosInClass[0].volume = 0.5f;
        audiosInClass[0].Play();
        StartCoroutine(GradualVolumeChange(5, audiosInClass[0], 0));

        gamePlayManager.studentsActions[1].SetMyMood(MoodIndicator.Good); // students from table 1
        gamePlayManager.studentsActions[2].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[3].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[4].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[16].SetMyMood(MoodIndicator.Good);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            if (gamePlayManager.studentsActions[i] != kidMaxim)
            {
                StartCoroutine(TriggerOtherKidsWrittingAnimationsWithDelay(i));
            }
            else
            {
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
                StartCoroutine(TriggerMaximToShrugShouldersAndLookAtTablet());
            }
        }
    }

    IEnumerator TriggerOtherKidsWrittingAnimationsWithDelay(int i)
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 1.5f));
        gamePlayManager.studentsActions[i].StartLookingAtTabletAndWrittingAnimations();
    }

    IEnumerator TriggerMaximToShrugShouldersAndLookAtTablet()
    {
        for (int i = 0; i < 4; i++) {
            kidMaxim.StopLookingAtTabletAndWrittingAnimations();
            kidMaxim.studentAnimation.MB34_ShrugShoulders(true);
            yield return new WaitForSeconds(1.5f);
            kidMaxim.studentAnimation.MB34_ShrugShoulders(false);
            kidMaxim.studentAnimation.MB33_WorkOnSheets(true);
            yield return new WaitForSeconds(2.0f);
            kidMaxim.studentAnimation.MB33_WorkOnSheets(false);
            kidMaxim.LookAtSomeone(window02.transform);// maxim looks outside
            yield return new WaitForSeconds(2.0f);
            kidMaxim.studentAnimation.MB34_ShrugShoulders(true);
            yield return new WaitForSeconds(1.5f);
            kidMaxim.studentAnimation.MB34_ShrugShoulders(false);
            kidMaxim.LookAtSomeone(kidMaxim.GetComponent<StudentAction>().chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        }
        
    }




    private void StudentReactionThree()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4MoveToPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, kidMaxim.gameObject.transform);

        //Student mumbling for 3 seconds...
        audiosInClass[0].volume = 0.4f;
        audiosInClass[0].Play();
        

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            if (gamePlayManager.studentsActions[i] != kidMaxim)
            {
                gamePlayManager.studentsActions[i].StartLookingAtTabletAndWrittingAnimations();
            }
            else
            {
                StartCoroutine(TriggerMaximToLookAtNextKidsTablet());
            }
        }
    }

    IEnumerator TriggerMaximToLookAtNextKidsTablet()
    {
        kidMaxim.StopLookingAtTabletAndWrittingAnimations();
        kidMaxim.LookAtSomeone(gamePlayManager.studentsActions[1].GetComponent<StudentAction>().chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        yield return new WaitForSeconds(0.5f);
        kidMaxim.studentAnimation.MB18_NodHead(true);
        yield return new WaitForSeconds(1.5f);
        kidMaxim.studentAnimation.MB18_NodHead(false);
        yield return new WaitForSeconds(1.5f);
        kidMaxim.LookAtSomeone(gamePlayManager.studentsActions[1].GetComponent<StudentAction>().chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        StartCoroutine(TriggerMaximDistractNextKid());
    }
    IEnumerator TriggerMaximDistractNextKid()
    {
        yield return new WaitForSeconds(1f);
        kidMaxim.LookAtSomeone(gamePlayManager.studentsActions[1].GetComponent<StudentAction>().chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        yield return new WaitForSeconds(1f);
        kidMaxim.LookAtSomeone(gamePlayManager.studentsActions[1].GetComponent<StudentAction>().chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        yield return new WaitForSeconds(1f);
        kidMaxim.LookAtSomeone(gamePlayManager.studentsActions[1].GetComponent<StudentAction>().chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        yield return new WaitForSeconds(1f);
        kidMaxim.studentAnimation.MB28_TapOnNeighbour(true);
        yield return new WaitForSeconds(2f);
        kidMaxim.studentAnimation.MB28_TapOnNeighbour(false);
        kidMaxim.LookAtSomeone(gamePlayManager.studentsActions[1].GetComponent<StudentAction>().chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        ///////////////////////////////
        gamePlayManager.studentsActions[2].SetMyMood(MoodIndicator.Bad);
        gamePlayManager.studentsActions[3].SetMyMood(MoodIndicator.Bad);
        gamePlayManager.studentsActions[4].SetMyMood(MoodIndicator.Bad);
        gamePlayManager.studentsActions[16].SetMyMood(MoodIndicator.Bad);

        gamePlayManager.studentsActions[2].StopLookingAtTabletAndWrittingAnimations();
        gamePlayManager.studentsActions[3].StopLookingAtTabletAndWrittingAnimations();
        gamePlayManager.studentsActions[4].StopLookingAtTabletAndWrittingAnimations();
        gamePlayManager.studentsActions[16].StopLookingAtTabletAndWrittingAnimations();

        //gamePlayManager.studentsActions[1].SetMyMood(MoodIndicator.Good);

        gamePlayManager.studentsActions[2].studentAnimation.MB39_ShakesHead(true);
        gamePlayManager.studentsActions[3].studentAnimation.MB39_ShakesHead(true);
        gamePlayManager.studentsActions[4].studentAnimation.MB39_ShakesHead(true);
        gamePlayManager.studentsActions[16].studentAnimation.MB39_ShakesHead(true);
        yield return new WaitForSeconds(2f);
        gamePlayManager.studentsActions[2].studentAnimation.MB39_ShakesHead(false);
        gamePlayManager.studentsActions[3].studentAnimation.MB39_ShakesHead(false);
        gamePlayManager.studentsActions[4].studentAnimation.MB39_ShakesHead(false);
        gamePlayManager.studentsActions[16].studentAnimation.MB39_ShakesHead(false);

        //kidMaxim.studentAnimation.MB39_ShakesHead(true);
        yield return new WaitForSeconds(1f);
        //kidMaxim.studentAnimation.MB39_ShakesHead(false);
        kidMaxim.LookAtSomeone(gamePlayManager.studentsActions[1].GetComponent<StudentAction>().chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        gamePlayManager.studentsActions[1].studentAnimation.MB39_ShakesHead(true);
        yield return new WaitForSeconds(2f);
        gamePlayManager.studentsActions[1].studentAnimation.MB39_ShakesHead(false);
    }

    private void StudentReactionFour()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4MoveToPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, kidMaxim.gameObject.transform);

        //Student mumbling for 3 seconds...
        //audiosInClass[0].volume = 0.75f;
        //audiosInClass[0].Play();
        

        StartCoroutine(TriggerStudentReactionFourOtherStudentsReaction());
    }

    IEnumerator TriggerStudentReactionFourOtherStudentsReaction()
    {
        kidMaxim.StopMyRandomLookingAnimations();
        kidMaxim.SetMyMood(MoodIndicator.Middle);
        //kidMaxim.StopLookingAtTabletAndWrittingAnimations();
        string studentTextToSpeak = "Juhu! Spielen!";
        //  SpeechManager.Instance.StartTalking(teacherTextToSpeak);

        //if (gamePlayManager.LOG_ENABLED) SpeechManager.Instance.StartTalking(studentTextToSpeak);
        MaximAudio2.Play();
        kidMaxim.StartLookingAtTabletAndWrittingAnimations();
        yield return new WaitForSeconds(9.0f);
        
        gamePlayManager.studentsActions[1].SetMyMood(MoodIndicator.Middle); // students from table 4
        gamePlayManager.studentsActions[17].SetMyMood(MoodIndicator.Middle);
        gamePlayManager.studentsActions[3].SetMyMood(MoodIndicator.Middle);
        gamePlayManager.studentsActions[4].SetMyMood(MoodIndicator.Middle);
        //gamePlayManager.studentsActions[16].SetMyMood(MoodIndicator.Middle);
        kidMaxim.SetMyMood(MoodIndicator.Middle);

        gamePlayManager.studentsActions[9].SetMyMood(MoodIndicator.Middle);  // students from table 3
        gamePlayManager.studentsActions[15].SetMyMood(MoodIndicator.Middle);
        gamePlayManager.studentsActions[7].SetMyMood(MoodIndicator.Middle);
        gamePlayManager.studentsActions[6].SetMyMood(MoodIndicator.Middle);
        gamePlayManager.studentsActions[5].SetMyMood(MoodIndicator.Middle);

        gamePlayManager.studentsActions[1].StopLookingAtTabletAndWrittingAnimations(); // students from table 1 & 2
        gamePlayManager.studentsActions[17].StopLookingAtTabletAndWrittingAnimations();
        gamePlayManager.studentsActions[3].StopLookingAtTabletAndWrittingAnimations();
        gamePlayManager.studentsActions[4].StopLookingAtTabletAndWrittingAnimations();
        //gamePlayManager.studentsActions[16].StopLookingAtTabletAndWrittingAnimations();
        //kidMaxim.StopLookingAtTabletAndWrittingAnimations();

        gamePlayManager.studentsActions[9].StopLookingAtTabletAndWrittingAnimations();
        gamePlayManager.studentsActions[15].StopLookingAtTabletAndWrittingAnimations();
        gamePlayManager.studentsActions[7].StopLookingAtTabletAndWrittingAnimations();
        gamePlayManager.studentsActions[6].StopLookingAtTabletAndWrittingAnimations();
        gamePlayManager.studentsActions[5].StopLookingAtTabletAndWrittingAnimations();

        yield return new WaitForSeconds(1.0f);

        gamePlayManager.studentsActions[1].LookAtSomeone(kidMaxim.gameObject.transform);
        gamePlayManager.studentsActions[17].LookAtSomeone(kidMaxim.gameObject.transform);
        gamePlayManager.studentsActions[3].LookAtSomeone(kidMaxim.gameObject.transform);
        gamePlayManager.studentsActions[4].LookAtSomeone(kidMaxim.gameObject.transform);
        //gamePlayManager.studentsActions[16].LookAtSomeone(kidMaxim.gameObject.transform);

        gamePlayManager.studentsActions[9].LookAtSomeone(kidMaxim.gameObject.transform);
        gamePlayManager.studentsActions[15].LookAtSomeone(kidMaxim.gameObject.transform);
        gamePlayManager.studentsActions[7].LookAtSomeone(kidMaxim.gameObject.transform);
        gamePlayManager.studentsActions[6].LookAtSomeone(kidMaxim.gameObject.transform);
        gamePlayManager.studentsActions[5].LookAtSomeone(kidMaxim.gameObject.transform);

        yield return new WaitForSeconds(1.0f);

        kidMaxim.LookAtSomeone(kidMaxim.GetComponent<StudentAction>().chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(TriggerMaximToDistractNextKid());

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            if (gamePlayManager.studentsActions[i] != kidMaxim || gamePlayManager.studentsActions[i] != gamePlayManager.studentsActions[1])
            {
                yield return new WaitForSeconds(Random.Range(1.0f,3.0f));
                gamePlayManager.studentsActions[i].StartLookingAtTabletAndWrittingAnimations();
                //gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
            }
        }

    }

    IEnumerator TriggerMaximToDistractNextKid()
    {
        yield return new WaitForSeconds(3.0f);

        kidMaxim.StopLookingAtTabletAndWrittingAnimations();

        UnityEngine.Debug.Log("SR 4 trigger");
        kidMaxim.studentAnimation.MB28_TapOnNeighbour(true);
        kidMaxim.SetMyMood(MoodIndicator.Good);

        yield return new WaitForSeconds(3.0f);

        //kidMaxim.studentAnimation.MB28_TapOnNeighbour(false);
        //kidMaxim.LookAtSomeone(gamePlayManager.studentsActions[1].transform);  //(kidMaxim.GetComponent<StudentAction>().chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);

        yield return new WaitForSeconds(2.0f);

        gamePlayManager.studentsActions[1].StopLookingAtTabletAndWrittingAnimations();
        //gamePlayManager.studentsActions[1].LookAtSomeone(kidMaxim.transform);    //(kidMaxim.GetComponent<StudentAction>().chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        gamePlayManager.studentsActions[1].studentAnimation.IWO29_FidgetsInChair(true); //using this animation for better visuals to player
        gamePlayManager.studentsActions[1].SetMyMood(MoodIndicator.Good);

        yield return new WaitForSeconds(4.0f);

        gamePlayManager.studentsActions[2].SetMyMood(MoodIndicator.Bad);
        gamePlayManager.studentsActions[3].SetMyMood(MoodIndicator.Bad);
        gamePlayManager.studentsActions[4].SetMyMood(MoodIndicator.Bad);
        gamePlayManager.studentsActions[16].SetMyMood(MoodIndicator.Bad);

        yield return new WaitForSeconds(1.0f);

        kidMaxim.StartMyTalkOrLaughAnimations();
        gamePlayManager.studentsActions[1].StartMyTalkOrLaughAnimations();
        yield return new WaitForSeconds(4.0f);
        //kidMaxim.LookAtSomeone(kidMaxim.GetComponent<StudentAction>().chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        //gamePlayManager.studentsActions[1].LookAtSomeone(kidMaxim.GetComponent<StudentAction>().chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);


        int count = 0;
        for (int i = 0; i < 18; i++) {
            if (i == 3 || i == 4 || i == 16 || i == 17) {
                gamePlayManager.studentsActions[i].StopLookingAtTabletAndWrittingAnimations();
                gamePlayManager.studentsActions[i].studentAnimation.EE2_Upset(true);
                yield return new WaitForSeconds(4.0f);
                gamePlayManager.studentsActions[i].studentAnimation.EE2_Upset(false);
                gamePlayManager.studentsActions[i].StartLookingAtTabletAndWrittingAnimations();
                
                count += 1;

                if (count >= 4) {
                    gamePlayManager.StartPhaseSeven(1);
                }
            }
        }

        
    }



    public void Reset()
    {
        gamePlayManager.studentsActions[1].SetMyMood(MoodIndicator.Middle); // students from table 1 & 2
        gamePlayManager.studentsActions[2].SetMyMood(MoodIndicator.Middle);
        gamePlayManager.studentsActions[3].SetMyMood(MoodIndicator.Middle);
        gamePlayManager.studentsActions[4].SetMyMood(MoodIndicator.Middle);
        gamePlayManager.studentsActions[16].SetMyMood(MoodIndicator.Middle);

        gamePlayManager.studentsActions[9].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[15].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[7].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[6].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[5].SetMyMood(MoodIndicator.Good);

        kidMaxim.GetComponent<StudentAction>().SetMyMood(MoodIndicator.Good);

        kidMaxim.GetComponent<StudentAction>().StopLookingAtTabletAndWrittingAnimations();
        gamePlayManager.StartPhaseSix();
    }



}
