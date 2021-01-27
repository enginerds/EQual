using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioFifteenManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    public PlayerMovement playerMovement;

    public Transform tabSimonePoint;
    public List<AudioSource> audiosInClass;

    string questionString = "";

    public Animator pencilAnim;
    public Animator pencilCaseAnim;

    public GameObject inHandPencil, inLeftHandPencil;
    public GameObject inHandPencil_Tom;

    public StudentAction[] kids;
    public GameObject[] boxFilledCenter;

    public MainObjectsManager mainObjectManager;

    public GameObject[] postedWorksheets;
    public GameObject[] tablePosters;

    private enum E_KidName
    {
        Mia = 1,
        Leonie = 0,
        Pia = 2,
        Janik = 3,
        Tom = 4,
        Zoe = 5,
        Simon = 7,
        Finn = 8,
        Maxim = 9,
        Niko = 6,
        Julian = 10,
        Niklas = 11
    }


    private void Awake()
    {

        if (gamePlayManager)
        {
            gamePlayManager.currentScenario = "SC15";
            gamePlayManager.StartPrepForStudentReaction += PrepForSRs;
        }
    }
    private void Start()
    {
        mainObjectManager = GetComponent<MainObjectsManager>();
        this.PlayBGAudio();
        gamePlayManager.initialActionForScenarioIsCommon = true;
        gamePlayManager.userInterfaceManager.ShowOrHideInitLoadingPanel(true);
        this.AudioSetVolume(0.01f);
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
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TeacherActionSelected", value, true);
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TASQuestion", gamePlayManager.userInterfaceManager.teacherSelectedQuestion);
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TASTimeStarted", gamePlayManager.GetCurrentTime());

        // get the background students back to their random looking and writing sequence.
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].studentAnimation.ResetCurrentAnim();
            if (i != 16 && i != 19) // except for Leonie and tom
            {
                gamePlayManager.studentsActions[i].StartMyRandomLookingOrWrittingAnimations();
            }
        }

        switch (value)
        {

            case "1":
                questionString = "Den Unterrichtsfluss aufrechterhalten und ein sich meldendes Kind drannehmen.";

                StudentReactionOne();
                break;
            case "2":
                questionString = "Den Schüler mit SFB ESE direkt loben, dass er einen Stift geliehen hat.";

                StudentReactionTwo();
                break;
            case "3":
                questionString = "Die beiden flüsternden Kinder freundlich ermahnen und auf die Stillarbeit hinweisen. ";

                StudentReactionThree();
                break;
            case "4":
                questionString = "Den Schüler nonverbal durch bestätigendes Zunicken loben.";
                StudentReactionFour();

                break;
        }

        //showTempText();
        gamePlayManager.playerActionDataHandler.PlayerAction = questionString;


    }

    void showTempText()
    {

        //   gamePlayManager.userInterfaceManager.ShowOrHideTempPanel(true);
        //   gamePlayManager.userInterfaceManager.SetTempText(questionString);


    }

    // SAc: Edit, We have removed Julia from the group of kids as she is not in the 9 year old list as per configuration, so all the index numbers will have to be modified as per this change where it is needed



    public void MainActionOne()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);

        StartCoroutine(MainActionOneRoutine());

    }

    bool lookNeeded = false;

    IEnumerator MainActionOneRoutine()
    {
        //this.StopBGAudio();
        Debug.Log("PEEEEEEEEEEEEPING TO SIDE");
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopSittingIdleLookAroundAnything(); // stop the initial intro animations for all kids
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();

        }

        lookNeeded = true;

        StartCoroutine(LookAtPaper());



        yield return new WaitForSeconds(15f);
        this.lookNeeded = false;

        StartCoroutine(TriggerMainActionTwo());
    }

    IEnumerator LookAtPaper()
    {
        while (lookNeeded)
        {

            for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
            {
                gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(false);
                gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            }

            // Table 1


            mainObjectManager.studentsAtTable1[2].LookAtSomeone(GameObject.Find("Book1_LookAt_Table1").transform);
            mainObjectManager.studentsAtTable1[1].LookAtSomeone(GameObject.Find("Book1_LookAt_Table1").transform);

            mainObjectManager.studentsAtTable1[4].LookAtSomeone(GameObject.Find("Book2_LookAt_Table1").transform);
            mainObjectManager.studentsAtTable1[3].LookAtSomeone(GameObject.Find("Book2_LookAt_Table1").transform);

            mainObjectManager.studentsAtTable1[0].LookAtSomeone(GameObject.Find("Book3_LookAt_Table1").transform);
            mainObjectManager.studentsAtTable1[5].LookAtSomeone(GameObject.Find("Book3_LookAt_Table1").transform);


            // Table 2


            mainObjectManager.studentsAtTable2[0].LookAtSomeone(GameObject.Find("Book1_LookAt_Table2").transform);
            //mainObjectManager.studentsAtTable2[1].LookAtSomeone(GameObject.Find("Book1_LookAt_Table2").transform);

            mainObjectManager.studentsAtTable2[1].LookAtSomeone(GameObject.Find("Book2_LookAt_Table2").transform);
            mainObjectManager.studentsAtTable2[2].LookAtSomeone(GameObject.Find("Book2_LookAt_Table2").transform);

            mainObjectManager.studentsAtTable2[3].LookAtSomeone(GameObject.Find("Book3_LookAt_Table2").transform);
            mainObjectManager.studentsAtTable2[4].LookAtSomeone(GameObject.Find("Book3_LookAt_Table2").transform);


            //Table 3


            mainObjectManager.studentsAtTable3[0].LookAtSomeone(GameObject.Find("Book1_LookAt_Table3").transform);
            mainObjectManager.studentsAtTable3[1].LookAtSomeone(GameObject.Find("Book1_LookAt_Table3").transform);

            mainObjectManager.studentsAtTable3[2].LookAtSomeone(GameObject.Find("Book2_LookAt_Table3").transform);
            mainObjectManager.studentsAtTable3[3].LookAtSomeone(GameObject.Find("Book2_LookAt_Table3").transform);

            mainObjectManager.studentsAtTable3[4].LookAtSomeone(GameObject.Find("Book3_LookAt_Table3").transform);
            //mainObjectManager.studentsAtTable3[5].LookAtSomeone(GameObject.Find("Book3_LookAt_Table3").transform);


            //Table 4


            mainObjectManager.studentsAtTable4[0].LookAtSomeone(GameObject.Find("Book1_LookAt_Table4").transform);
            mainObjectManager.studentsAtTable4[1].LookAtSomeone(GameObject.Find("Book1_LookAt_Table4").transform);

            mainObjectManager.studentsAtTable4[2].LookAtSomeone(GameObject.Find("Book2_LookAt_Table4").transform);
            mainObjectManager.studentsAtTable4[3].LookAtSomeone(GameObject.Find("Book2_LookAt_Table4").transform);

            mainObjectManager.studentsAtTable4[4].LookAtSomeone(GameObject.Find("Book3_LookAt_Table4").transform);


            yield return new WaitForSeconds(3f);

            for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
            {
                gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
            }

            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator TriggerMainActionTwo()
    {
        yield return null;

        yield return new WaitForSeconds(8.0f);
        //       gamePlayManager.studentsActions[16].StopMyRandomLookingAnimations();
        gamePlayManager.StartPhaseFour();

    }



    public void MainActionTwo()
    {
        //gamePlayManager.userInterfaceManager.SetTempText("Student asks her neigbor with ESE for a pen (vi_11)");
        //gamePlayManager.MainActionTwoDelay = 10.0f;
        // Tom tips on Leonie shoulder (mb 28)
        // Tom asks Leonie for pen (mb 28)
        // Other students (Ivan(20) and Finn(8) ) one by one on table 1 stop writing and look at Tom and Leonie (mb10)
        // Leonie nods (mb18)
        // Tom and Leonie whisper (vi11 for 3 seconds)

        Debug.Log("Main Action Two");
        StartCoroutine(MainActionTwoRoutine());

    }


    IEnumerator MainActionTwoRoutine()
    {

        yield return new WaitForSeconds(1.0f); // timeframe is already covered for nearly 10 seconds

        // Increase to 50% TODO
        this.PlayBGAudio();
        this.AudioSetVolume(0.02f);    // 50%

        //playerMovement.LookToPlayer(true, gamePlayManager.studentsActions[1].transform);


        //students look at teacher
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].LookAtSomeone(playerMovement.transform);
        }

        string textToTalk = "So, schreibt bitte noch den letzten Satz auf und legt dann eure Stifte zur Seite";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        yield return new WaitForSeconds(6f);

        gamePlayManager.StartPhaseFive();


    }

    public void MainActionThree()
    {
        StartCoroutine(MainActionThreeRoutine());
    }

    IEnumerator MainActionThreeRoutine()
    {

        // Audio Mumbling reduce to faint noise BG
        this.AudioSetVolume(0.01f);   // Default

        //students work for 5 more seconds, then stop and look at teacher
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
        }

        yield return new WaitForSeconds(5f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
        }

        float timeElapsed = 0f;
        while (timeElapsed <= 5f)
        {
            timeElapsed += Time.deltaTime;

            for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
            {
                gamePlayManager.studentsActions[i].LookAtSomeone(playerMovement.transform);
            }
            yield return new WaitForEndOfFrame();
        }


        yield return new WaitForSeconds(1.0f);

        StartCoroutine(TriggerTeacherQuestionPhase());
    }

    IEnumerator TriggerTeacherQuestionPhase()
    {

        //Tom is back to do his worksheets / random look arounds
        //yield return new WaitForSeconds(5.0f);

        this.StopBGAudio();

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
        }

        yield return new WaitForSeconds(1.0f);


        //playerMovement.LookToPlace(true, GameObject.Find("ClassMiddlePoint").transform);
        //playerMovement.MovePlayer(true, GameObject.Find("TeacherDeskPoint").transform);

        // inLeftHandPencil.SetActive(false);
        gamePlayManager.StartPhaseSix();
        //gamePlayManager.userInterfaceManager.ShowOrHideTempPanel(false);

    }


    private void StudentReactionOne()
    {

        Debug.Log("Student Reaction One ");
        StartCoroutine(SROneRoutine());
    }

    IEnumerator SROneRoutine()
    {


        //set student mumbling to zero
        this.AudioSetVolume(0.0f);
        this.PlayBGAudio();

        // Janik - 0 , MIA - 10, Maxim - 9 , Simon - 20



        this.lookNeeded = true;
        StartCoroutine(LookAtPaper());
        yield return new WaitForSeconds(10f);

        lookNeeded = false;

        //ten sutdent go : Janik and MAxim one of the 10

        kids[(int)E_KidName.Finn].InitiateGoToSpot(GameObject.Find("BoardQueueL1").transform);
        kids[(int)E_KidName.Pia].InitiateGoToSpot(GameObject.Find("BoardQueueL1").transform);
        kids[(int)E_KidName.Simon].InitiateGoToSpot(GameObject.Find("BoardQueueR1").transform);
        kids[(int)E_KidName.Mia].InitiateGoToSpot(GameObject.Find("BoardQueueL1").transform);
        kids[(int)E_KidName.Niko].InitiateGoToSpot(GameObject.Find("BoardQueueL1").transform);
        kids[(int)E_KidName.Tom].InitiateGoToSpot(GameObject.Find("BoardQueueR1").transform);
        kids[(int)E_KidName.Leonie].InitiateGoToSpot(GameObject.Find("BoardQueueR1").transform);
        kids[(int)E_KidName.Zoe].InitiateGoToSpot(GameObject.Find("BoardQueueR1").transform);


        //yield return new WaitUntil(() => gamePlayManager.studentsActions[(int)E_KidName.Finn].reachedSpot);
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Finn].transform.position = GameObject.Find("BoardQueueL1").transform.position;
        kids[(int)E_KidName.Finn].LookAtTransformXZ(GameObject.Find("BoardQueueL1").transform);
        postedWorksheets[0].SetActive(true);
        kids[(int)E_KidName.Finn].GoToAndSitInChair();

        //yield return new WaitUntil(() => gamePlayManager.studentsActions[(int)E_KidName.Pia].reachedSpot);
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Pia].transform.position = GameObject.Find("BoardQueueL1").transform.position;
        kids[(int)E_KidName.Pia].LookAtTransformXZ(GameObject.Find("BoardQueueL1").transform);
        postedWorksheets[1].SetActive(true);
        kids[(int)E_KidName.Pia].GoToAndSitInChair();


        //yield return new WaitUntil(() => gamePlayManager.studentsActions[(int)E_KidName.Simon].reachedSpot);
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Simon].transform.position = GameObject.Find("BoardQueueR1").transform.position;
        kids[(int)E_KidName.Simon].LookAtTransformXZ(GameObject.Find("BoardQueueR1").transform);
        postedWorksheets[2].SetActive(true);
        kids[(int)E_KidName.Simon].GoToAndSitInChair();


        //yield return new WaitUntil(() => gamePlayManager.studentsActions[(int)E_KidName.Mia].reachedSpot);
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Mia].transform.position = GameObject.Find("BoardQueueL1").transform.position;
        kids[(int)E_KidName.Mia].LookAtTransformXZ(GameObject.Find("BoardQueueL1").transform);
        postedWorksheets[3].SetActive(true);
        kids[(int)E_KidName.Mia].GoToAndSitInChair();

        //yield return new WaitUntil(() => gamePlayManager.studentsActions[(int)E_KidName.Niko].reachedSpot);
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Niko].transform.position = GameObject.Find("BoardQueueL1").transform.position;
        kids[(int)E_KidName.Niko].LookAtTransformXZ(GameObject.Find("BoardQueueL1").transform);
        postedWorksheets[4].SetActive(true);
        kids[(int)E_KidName.Niko].GoToAndSitInChair();


        //yield return new WaitUntil(() => gamePlayManager.studentsActions[(int)E_KidName.Tom].reachedSpot);
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Tom].transform.position = GameObject.Find("BoardQueueR1").transform.position;
        kids[(int)E_KidName.Tom].LookAtTransformXZ(GameObject.Find("BoardQueueR1").transform);
        postedWorksheets[5].SetActive(true);
        kids[(int)E_KidName.Tom].GoToAndSitInChair();

        //yield return new WaitUntil(() => gamePlayManager.studentsActions[(int)E_KidName.Leonie].reachedSpot);
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Leonie].transform.position = GameObject.Find("BoardQueueR1").transform.position;
        kids[(int)E_KidName.Leonie].LookAtTransformXZ(GameObject.Find("BoardQueueR1").transform);
        postedWorksheets[6].SetActive(true);
        kids[(int)E_KidName.Leonie].GoToAndSitInChair();

        //yield return new WaitUntil(() => gamePlayManager.studentsActions[(int)E_KidName.Zoe].reachedSpot);
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Zoe].transform.position = GameObject.Find("BoardQueueR1").transform.position;
        kids[(int)E_KidName.Zoe].LookAtTransformXZ(GameObject.Find("BoardQueueR1").transform);
        postedWorksheets[7].SetActive(true);
        kids[(int)E_KidName.Zoe].GoToAndSitInChair();



        kids[(int)E_KidName.Janik].InitiateGoToSpot(GameObject.Find("BoardQueueL1").transform);
        //yield return new WaitUntil(() => gamePlayManager.studentsActions[(int)E_KidName.Janik].reachedSpot);
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Janik].transform.position = GameObject.Find("BoardQueueL1").transform.position;
        kids[(int)E_KidName.Janik].LookAtTransformXZ(GameObject.Find("BoardQueueL1").transform);
        postedWorksheets[8].SetActive(true);
        yield return new WaitForSeconds(1f);
        kids[(int)E_KidName.Janik].GoToAndSitInChair();


        kids[(int)E_KidName.Maxim].InitiateGoToSpot(GameObject.Find("BoardQueueL1").transform);
        //yield return new WaitUntil(() => gamePlayManager.studentsActions[(int)E_KidName.Maxim].reachedSpot);
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Maxim].transform.position = GameObject.Find("BoardQueueL1").transform.position;
        kids[(int)E_KidName.Maxim].LookAtTransformXZ(GameObject.Find("BoardQueueL1").transform);
        postedWorksheets[9].SetActive(true);
        kids[(int)E_KidName.Maxim].GoToAndSitInChair();

        //gamePlayManager.StartPhaseSeven(2.0f);

        yield return new WaitForSeconds(15f);

        gamePlayManager.StartPhaseSeven(2.0f);



    }

    private void StudentReactionTwo()
    {
        StartCoroutine(SRTwoRoutine());
    }

    IEnumerator SRTwoRoutine()
    {

        //show table posters
        for (int i = 0; i < tablePosters.Length; i++) {
            tablePosters[i].SetActive(true);
        }
        
        //set student mumbling to low
        this.AudioSetVolume(0.015f);
        this.PlayBGAudio();

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {

            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);

            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
        }

        kids[(int)E_KidName.Janik].SetMyMood(MoodIndicator.Middle);
        kids[(int)E_KidName.Leonie].SetMyMood(MoodIndicator.Middle);
        kids[(int)E_KidName.Maxim].SetMyMood(MoodIndicator.Middle);

        yield return new WaitForSeconds(10f);

        // Raise Hands except L , j , m

        kids[(int)E_KidName.Finn].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Finn].studentAnimation.RaiseHand(true);

        kids[(int)E_KidName.Mia].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Mia].studentAnimation.RaiseHand(true);

        kids[(int)E_KidName.Niko].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Niko].studentAnimation.RaiseHand(true);

        kids[(int)E_KidName.Pia].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Pia].studentAnimation.RaiseHand(true);

        kids[(int)E_KidName.Simon].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Simon].studentAnimation.RaiseHand(true);

        kids[(int)E_KidName.Tom].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Tom].studentAnimation.RaiseHand(true);

        kids[(int)E_KidName.Zoe].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Zoe].studentAnimation.RaiseHand(true);

        kids[(int)E_KidName.Julian].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Julian].studentAnimation.RaiseHand(true);

        kids[(int)E_KidName.Niklas].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Niklas].studentAnimation.RaiseHand(true);

        yield return new WaitForSeconds(15f);

        //hide table posters (as this is the only SR where they should be visible)
        for (int i = 0; i < tablePosters.Length; i++) {
            tablePosters[i].SetActive(false);
        }

        gamePlayManager.StartPhaseSeven(2.0f);

    }

    private void StudentReactionThree()
    {
        StartCoroutine(SRThreeRoutine());
    }

    IEnumerator SRThreeRoutine()
    {

        //set student mumbling to middle
        this.AudioSetVolume(0.03f);
        this.PlayBGAudio();

        kids[(int)E_KidName.Janik].SetMyMood(MoodIndicator.Middle);
        kids[(int)E_KidName.Leonie].SetMyMood(MoodIndicator.Middle);
        kids[(int)E_KidName.Maxim].SetMyMood(MoodIndicator.Middle);

        kids[(int)E_KidName.Janik].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Leonie].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Maxim].studentAnimation.ResetAllAnim();

        yield return new WaitForSeconds(3f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            
            if (gamePlayManager.studentsActions[i].name == "Jannik" || gamePlayManager.studentsActions[i].name == "Maxim" || gamePlayManager.studentsActions[i].name == "Leonie") {
                //do nothing
                Debug.Log("index = " + i + "; " + "name = " + gamePlayManager.studentsActions[i].name);
            } else {
                gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
                gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
               
            }
        }


        yield return new WaitForSeconds(10f);

        // Raise Hands except L , j , m
        //kids[(int)E_KidName.Finn].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Finn].studentAnimation.MB33_WorkOnSheets(false);
        kids[(int)E_KidName.Finn].studentAnimation.ResetAllAnim();
        yield return new WaitForSeconds(.1f);
        kids[(int)E_KidName.Finn].studentAnimation.RaiseHand(true);

        //kids[(int)E_KidName.Mia].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Mia].studentAnimation.MB33_WorkOnSheets(false);
        kids[(int)E_KidName.Mia].studentAnimation.ResetAllAnim();
        yield return new WaitForSeconds(.1f);
        kids[(int)E_KidName.Mia].studentAnimation.RaiseHand(true);

        //kids[(int)E_KidName.Niko].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Niko].studentAnimation.MB33_WorkOnSheets(false);
        kids[(int)E_KidName.Niko].studentAnimation.ResetAllAnim();
        yield return new WaitForSeconds(.1f);
        kids[(int)E_KidName.Niko].studentAnimation.RaiseHand(true);

        //kids[(int)E_KidName.Pia].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Pia].studentAnimation.MB33_WorkOnSheets(false);
        kids[(int)E_KidName.Pia].studentAnimation.ResetAllAnim();
        yield return new WaitForSeconds(.1f);
        kids[(int)E_KidName.Pia].studentAnimation.RaiseHand(true);

        //kids[(int)E_KidName.Simon].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Simon].studentAnimation.MB33_WorkOnSheets(false);
        kids[(int)E_KidName.Simon].studentAnimation.ResetAllAnim();
        yield return new WaitForSeconds(.1f);
        kids[(int)E_KidName.Simon].studentAnimation.RaiseHand(true);

        //kids[(int)E_KidName.Tom].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Tom].studentAnimation.MB33_WorkOnSheets(false);
        kids[(int)E_KidName.Tom].studentAnimation.ResetAllAnim();
        yield return new WaitForSeconds(.1f);
        kids[(int)E_KidName.Tom].studentAnimation.RaiseHand(true);

        //kids[(int)E_KidName.Zoe].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Zoe].studentAnimation.MB33_WorkOnSheets(false);
        kids[(int)E_KidName.Zoe].studentAnimation.ResetAllAnim();
        yield return new WaitForSeconds(.1f);
        kids[(int)E_KidName.Zoe].studentAnimation.RaiseHand(true);

        yield return new WaitForSeconds(15f);

        gamePlayManager.StartPhaseSeven(2.0f);
    }

    private void StudentReactionFour()
    {
        StartCoroutine(SRFourRoutine());
    }

    IEnumerator SRFourRoutine()
    {
        //set student mumbling to high
        this.AudioSetVolume(0.045f);
        this.PlayBGAudio();

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
        }

        //this time break is [apparently] needed to ensure the code above finishes before starting the loop below (otherwise, we don't see the hands being raised).
        yield return new WaitForSeconds(3f);

        bool toggle = false;
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            if (i != 0 && i != 9 && i != 16) {
                //Nine students raise their hands (but not Jannik (0), Maxim (9), or Leonie (16))
                if (i == 2 || i == 4 || i == 6 || i == 8 || i == 10 || i == 12 || i == 14 || i == 18 || i == 20) {
                    gamePlayManager.studentsActions[i].studentAnimation.RaiseHand(true);

                    //set these kids' moods to Good
                    gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
                } else {
                    //the remaining students whisper to their neighbor
                    if (toggle == false) {
                        //gamePlayManager.studentsActions[i].studentAnimation.TalkRightLeft(1f);
                        gamePlayManager.studentsActions[i].studentAnimation.MB30_PeepToSide(true, 0.3f);
                        toggle = true;
                    } else {
                        //gamePlayManager.studentsActions[i].studentAnimation.TalkRightLeft(-1f);
                        gamePlayManager.studentsActions[i].studentAnimation.MB30_PeepToSide(true, 0.8f);
                        toggle = false;
                    }

                    //set whispering kids' moods to Middle
                    gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
                }
            } 
        }

        //Set Jannik's, Leonie's, and Maxim's mood to Middle
        kids[(int)E_KidName.Janik].SetMyMood(MoodIndicator.Middle);
        kids[(int)E_KidName.Leonie].SetMyMood(MoodIndicator.Middle);
        kids[(int)E_KidName.Maxim].SetMyMood(MoodIndicator.Middle);

        //mainObjectManager.studentsAtTable1[5].studentAnimation.ResetAllAnim();
        //mainObjectManager.studentsAtTable1[5].studentAnimation.RaiseHandAndKeep(true);

        //mainObjectManager.studentsAtTable2[3].studentAnimation.ResetAllAnim();
        //mainObjectManager.studentsAtTable2[3].studentAnimation.RaiseHandAndKeep(true);

        //mainObjectManager.studentsAtTable3[2].studentAnimation.ResetAllAnim();
        //mainObjectManager.studentsAtTable3[3].studentAnimation.ResetAllAnim();
        //mainObjectManager.studentsAtTable3[4].studentAnimation.ResetAllAnim();
        //mainObjectManager.studentsAtTable3[2].studentAnimation.RaiseHandAndKeep(true);
        //mainObjectManager.studentsAtTable3[3].studentAnimation.RaiseHandAndKeep(true);
        //mainObjectManager.studentsAtTable3[4].studentAnimation.RaiseHandAndKeep(true);

        //mainObjectManager.studentsAtTable4[2].studentAnimation.ResetAllAnim();
        //mainObjectManager.studentsAtTable4[3].studentAnimation.ResetAllAnim();
        //mainObjectManager.studentsAtTable4[2].studentAnimation.RaiseHandAndKeep(true);
        //mainObjectManager.studentsAtTable4[3].studentAnimation.RaiseHandAndKeep(true);

        //mainObjectManager.studentsAtTable4[4].studentAnimation.ResetAllAnim();
        //mainObjectManager.studentsAtTable4[2].studentAnimation.ResetAllAnim();
        //mainObjectManager.studentsAtTable4[4].studentAnimation.RaiseHandAndKeep(true);
        //mainObjectManager.studentsAtTable4[2].studentAnimation.RaiseHandAndKeep(true);

        ////Six Random Pairs are Whispering
        //// Table 1
        //mainObjectManager.studentsAtTable1[2].studentAnimation.TalkRightLeft(1f);
        //mainObjectManager.studentsAtTable1[1].studentAnimation.TalkRightLeft(-1f);
        //mainObjectManager.studentsAtTable1[4].studentAnimation.TalkRightLeft(1f);
        //mainObjectManager.studentsAtTable1[3].studentAnimation.TalkRightLeft(-1f);

        //mainObjectManager.studentsAtTable1[2].SetMyMood(MoodIndicator.Middle);
        //mainObjectManager.studentsAtTable1[1].SetMyMood(MoodIndicator.Middle);
        //mainObjectManager.studentsAtTable1[4].SetMyMood(MoodIndicator.Middle);
        //mainObjectManager.studentsAtTable1[3].SetMyMood(MoodIndicator.Middle);

        ////Table 2
        //mainObjectManager.studentsAtTable2[1].studentAnimation.TalkRightLeft(1f);
        //mainObjectManager.studentsAtTable2[2].studentAnimation.TalkRightLeft(-1f);
        //mainObjectManager.studentsAtTable2[3].studentAnimation.TalkRightLeft(1f);
        //mainObjectManager.studentsAtTable2[4].studentAnimation.TalkRightLeft(-1f);

        //mainObjectManager.studentsAtTable2[1].SetMyMood(MoodIndicator.Middle);
        //mainObjectManager.studentsAtTable2[2].SetMyMood(MoodIndicator.Middle);
        //mainObjectManager.studentsAtTable2[3].SetMyMood(MoodIndicator.Middle);
        //mainObjectManager.studentsAtTable2[4].SetMyMood(MoodIndicator.Middle);
        ////Table 3
        //mainObjectManager.studentsAtTable3[0].studentAnimation.TalkRightLeft(1f);
        //mainObjectManager.studentsAtTable3[1].studentAnimation.TalkRightLeft(-1f);

        //mainObjectManager.studentsAtTable3[0].SetMyMood(MoodIndicator.Middle);
        //mainObjectManager.studentsAtTable3[1].SetMyMood(MoodIndicator.Middle);
        ////Table 4
        //mainObjectManager.studentsAtTable4[0].studentAnimation.TalkRightLeft(1f);
        //mainObjectManager.studentsAtTable4[1].studentAnimation.TalkRightLeft(-1f);

        //mainObjectManager.studentsAtTable4[0].SetMyMood(MoodIndicator.Middle);
        //mainObjectManager.studentsAtTable4[1].SetMyMood(MoodIndicator.Middle);

        //Jannik looks out Window
        kids[(int)E_KidName.Janik].LookAtWindowRoutine();
        //Leonie Paints
        kids[(int)E_KidName.Leonie].studentAnimation.MB33_WorkOnSheets(true);


        yield return new WaitForSeconds(15f);


        gamePlayManager.StartPhaseSeven(2.0f);
    }


    public void Reset()
    {

        StartCoroutine(ResetRoutine());
    }

    IEnumerator ResetRoutine()
    {
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].GoToAndSitInChair();
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
        }

        this.AudioSetVolume(0.01f);

        //playerMovement.MovePlayer(true, GameObject.Find("TeacherDeskPoint").transform);

        gamePlayManager.StartPhaseSix();
        yield return new WaitForSeconds(2f);


    }

    public void PrepForSRs()
    {
        Debug.Log("Prep for SR called.........");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);

    }


    public void PlayBGAudio()
    {
        for (int i = 0; i < audiosInClass.Count; i++)
        {
            audiosInClass[i].Play();
        }

    }

    public void AudioSetVolume(float value)
    {

        value = Mathf.Clamp01(value);
        for (int i = 0; i < audiosInClass.Count; i++)
        {
            audiosInClass[i].volume = value;
        }
    }

    public void StopBGAudio()
    {
        for (int i = 0; i < audiosInClass.Count; i++)
        {
            audiosInClass[i].Stop();
        }
    }



}
