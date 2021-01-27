using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioSixManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    public PlayerMovement playerMovement;

    public Transform tableOnePoint;
    public List<AudioSource> audiosInClass;

    string questionString = "";

    public Animator pencilAnim;
    public Animator pencilCaseAnim;

    public GameObject inHandPencil, inLeftHandPencil;
    public GameObject inHandPencil_Tom;

    public StudentAction[] kids;
    public GameObject[] boxFilledCenter;

    public MainObjectsManager mainObjectManager;
    

    private enum E_KidName
    {
        //Table2
        Mia,
        Shirin,

        //Table 3
        Maxim,
        Niko,

        //Table 1
        Leon,
        Hannah,

        //Table 4
        Janik,
        Lena,
    }


    private void Awake()
    {
    
        if (gamePlayManager)
        {
            gamePlayManager.currentScenario = "SC6";
            gamePlayManager.StartPrepForStudentReaction += PrepForSRs;
        }
    }
        private void Start()
    {
        mainObjectManager = GetComponent<MainObjectsManager>();
        this.PlayBGAudio();
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

    void showTempText() {

     //   gamePlayManager.userInterfaceManager.ShowOrHideTempPanel(true);
     //   gamePlayManager.userInterfaceManager.SetTempText(questionString);


    }

    // SAc: Edit, We have removed Julia from the group of kids as she is not in the 9 year old list as per configuration, so all the index numbers will have to be modified as per this change where it is needed



    public void MainActionOne()
    {
        //gamePlayManager.userInterfaceManager.ShowOrHideTempPanel(true);
        //gamePlayManager.userInterfaceManager.SetTempText("Student searches for a pen in her pencil case (iwo 27) and shrugs shoulders(mb_34)");
        //playerMovement.MovePlayer(true, GameObject.Find("TeacherDeskPoint").transform);
        //GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = tableOnePoint.rotation;

        // All kids Except Tom is working on their WorkSheets, every 2-3 seconds : random student(other than tom) ,

        //looks around class - intersting spots, or Outside window for 2- 3 seconds

        StartCoroutine(MainActionOneRoutine());

    }

    bool lookNeeded = false;

    IEnumerator MainActionOneRoutine()
    {
        this.StopBGAudio();
        
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopSittingIdleLookAroundAnything(); // stop the initial intro animations for all kids
            gamePlayManager.studentsActions[i].LookAtSomeone(playerMovement.transform);

            //gamePlayManager.studentsActions[i].StopLookAtSomeone();
            //gamePlayManager.studentsActions[i].StopSittingIdleLookAroundAnything(); // stop the initial intro animations for all kids
            //gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            //gamePlayManager.studentsActions[i].scenarioStart = false;
            //gamePlayManager.studentsActions[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
            //yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }

        lookNeeded = true;

        StartCoroutine(LookAtTeacher());

        /*
        string textToTalk = "Zu unserem neuen Reihenthema sollt ihr heute in Tischgruppen jeweils ein Plakat erstellen." +
                    "Darauf sollt ihr möglichst viele wichtige Informationen zu dem Thema sammeln. Dafür dürft ihr die Tablets nutzen ";
                    
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));

        yield return new WaitForSeconds(1f);

         textToTalk = "außerdem braucht ihr Plakat und Eddings und, wenn ihr möchtet, buntes Papier und Klebe";

        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        */

        string textToTalk = "Zu unserem neuen Reihenthema sollt ihr heute in Tischgruppen jeweils ein Plakat erstellen.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        yield return new WaitForSeconds(1f);

        textToTalk = "Darauf sollt ihr möglichst viele wichtige Informationen zu dem Thema sammeln.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        yield return new WaitForSeconds(1f);

        textToTalk = "Dafür dürft ihr die Tablets nutzen, außerdem braucht ihr Plakat und Eddings und, wenn ihr möchtet, buntes Papier und Klebe";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));

        lookNeeded = false;

        //placing audio here (moved from MainActionTwoRoutine()) to have it start immediately after Teacher speaks 
        this.PlayBGAudio();
        this.AudioSetVolume(0.03f);

        yield return new WaitForSeconds(1f);

        StartCoroutine(TriggerMainActionTwo());
    }

    IEnumerator LookAtTeacher()
    {
        while(lookNeeded)
        {
            for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
            {
                gamePlayManager.studentsActions[i].LookAtSomeone(playerMovement.transform);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator TriggerMainActionTwo()
    {
        yield return null;
        for (int i = 0; i < gamePlayManager.studentsActions.Count/2; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StartMyTalkOrLaughAnimations();
        }

        yield return new WaitForSeconds(8.0f);
 //       gamePlayManager.studentsActions[16].StopMyRandomLookingAnimations();
        gamePlayManager.StartPhaseFour();


        for (int i = 0; i < gamePlayManager.studentsActions.Count / 2; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyTalkOrLaughAnimations();
        }

    }



    public void MainActionTwo()
    {
        //gamePlayManager.userInterfaceManager.SetTempText("Student asks her neigbor with ESE for a pen (vi_11)");
        //gamePlayManager.MainActionTwoDelay = 10.0f;
        // Tom tips on Leonie shoulder (mb 28)
        // Tom asks Leonie for pen (mb 28)
        // Other students (Ivan(20) and Hannah(8) ) one by one on table 1 stop writing and look at Tom and Leonie (mb10)
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
        this.AudioSetVolume(0.03f);    // 50%

        //playerMovement.LookToPlayer(true, gamePlayManager.studentsActions[1].transform);


        for(int i = 0; i < gamePlayManager.studentsActions.Count; i +=2)
        {

            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].WisperOrWriteRoutine();
        }

        

        yield return new WaitForSeconds(1f);

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

        // Teachers Audio

        string textToTalk = "Habt ihr noch Fragen zu der Aufgabe?";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));

        // Shake Head
        // TODO : Random Head Shake
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].studentAnimation.MB39_ShakesHead(true);
        }

        yield return new WaitForSeconds(2f);
        // AUDIO 2
        string textToTalk2 = "Prima, dann kann es ja losgehen. Bitte holt euch das Material, das ihr benötig";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk2));


        yield return new WaitForSeconds(2.0f);

        StartCoroutine(TriggerTeacherQuestionPhase());
    }

    IEnumerator TriggerTeacherQuestionPhase() {

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
        playerMovement.MovePlayer(true, GameObject.Find("TeacherDeskPoint").transform);

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


        //this.PlayBGAudio();
        this.StopBGAudio();
        // Janik - 0 , MIA - 10, Maxim - 9 , Leon - 20


        playerMovement.LookToPlayer(true, kids[(int)E_KidName.Mia].transform);
        kids[(int)E_KidName.Mia].StopLookAtSomeone();
        kids[(int)E_KidName.Mia].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Mia].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        kids[(int)E_KidName.Mia].InitiateGoToSpot(GameObject.Find("StationQueue1").transform);

        kids[(int)E_KidName.Niko].StopLookAtSomeone();
        kids[(int)E_KidName.Niko].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Niko].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        kids[(int)E_KidName.Niko].InitiateGoToSpot(GameObject.Find("StationQueue2").transform);

        kids[(int)E_KidName.Janik].StopLookAtSomeone();
        kids[(int)E_KidName.Janik].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Janik].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        kids[(int)E_KidName.Janik].InitiateGoToSpot(GameObject.Find("StationQueue3").transform);

        kids[(int)E_KidName.Leon].StopLookAtSomeone();
        kids[(int)E_KidName.Leon].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Leon].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        kids[(int)E_KidName.Leon].InitiateGoToSpot(GameObject.Find("StationQueue4").transform);

        yield return new WaitUntil(() => kids[(int)E_KidName.Mia].reachedSpot);
        kids[(int)E_KidName.Mia].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Book,true);
        kids[(int)E_KidName.Mia].GoToAndSitInChair();

        playerMovement.LookToPlayer(true, kids[(int)E_KidName.Niko].transform);


        yield return new WaitUntil(() => kids[(int)E_KidName.Niko].reachedSpot);
        kids[(int)E_KidName.Niko].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Book, true);
        kids[(int)E_KidName.Niko].GoToAndSitInChair();

        playerMovement.LookToPlayer(true, kids[(int)E_KidName.Janik].transform);

        yield return new WaitUntil(() => kids[(int)E_KidName.Janik].reachedSpot);
        kids[(int)E_KidName.Janik].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Book, true);
        kids[(int)E_KidName.Janik].GoToAndSitInChair();

        playerMovement.LookToPlayer(true, kids[(int)E_KidName.Leon].transform);

        yield return new WaitUntil(() => kids[(int)E_KidName.Leon].reachedSpot);
        kids[(int)E_KidName.Leon].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Book, true);
        kids[(int)E_KidName.Leon].GoToAndSitInChair();

        yield return new WaitUntil(() => kids[(int)E_KidName.Mia].reachedSpot);
        this.ToggleBoxFilledTable(true, 2);

        yield return new WaitUntil(() => kids[(int)E_KidName.Niko].reachedSpot);
        this.ToggleBoxFilledTable(true, 3);

        yield return new WaitUntil(() => kids[(int)E_KidName.Janik].reachedSpot);
        this.ToggleBoxFilledTable(true, 4);

        yield return new WaitUntil(() => kids[(int)E_KidName.Leon].reachedSpot);
        this.ToggleBoxFilledTable(true, 1);


        yield return new WaitForSeconds(6f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
            gamePlayManager.studentsActions[i].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Book, false);
        }

        gamePlayManager.StartPhaseSeven(10.0f);



    }

    private void StudentReactionTwo()
    {
        StartCoroutine(SRTwoRoutine());
    }

    IEnumerator SRTwoRoutine()
    {

        this.PlayBGAudio();

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(false);

        }

        playerMovement.LookToPlace(true, mainObjectManager.Table4);
        playerMovement.MovePlayer(true, mainObjectManager.Table4MoveToPoint);

        yield return new WaitForSeconds(5f);

        this.ToggleBoxFilledTable(true, 4);

        //Increase BG audio to 60%
        this.AudioSetVolume(0.02f);

        for(int i = 0; i < mainObjectManager.studentsAtTable4.Count; i++)
        {
            mainObjectManager.studentsAtTable4[i].StopLookAtSomeone();
            mainObjectManager.studentsAtTable4[i].StopMyRandomLookingAnimations();
            mainObjectManager.studentsAtTable4[i].studentAnimation.ResetAllAnim();
            mainObjectManager.studentsAtTable4[i].studentAnimation.MB33_WorkOnSheets(true);
        }

        playerMovement.LookToPlace(true, mainObjectManager.Table3);
        playerMovement.MovePlayer(true, mainObjectManager.Table3MoveToPoint);

        yield return new WaitForSeconds(5f);

        this.ToggleBoxFilledTable(true, 3);

        //Increase BG audio to 60%
        this.AudioSetVolume(0.01f);

        for (int i = 0; i < mainObjectManager.studentsAtTable3.Count; i++)
        {
            mainObjectManager.studentsAtTable3[i].StopLookAtSomeone();
            mainObjectManager.studentsAtTable3[i].StopMyRandomLookingAnimations();
            mainObjectManager.studentsAtTable3[i].studentAnimation.ResetAllAnim();
            mainObjectManager.studentsAtTable3[i].studentAnimation.MB33_WorkOnSheets(true);
        }

        playerMovement.LookToPlace(true, mainObjectManager.Table2);
        playerMovement.MovePlayer(true, mainObjectManager.Table2MoveToPoint);

        yield return new WaitForSeconds(5f);

        this.ToggleBoxFilledTable(true, 2);

        //Increase BG audio to 60%
        this.AudioSetVolume(0.005f);

        for (int i = 0; i < mainObjectManager.studentsAtTable2.Count; i++)
        {
            mainObjectManager.studentsAtTable2[i].StopLookAtSomeone();
            mainObjectManager.studentsAtTable2[i].StopMyRandomLookingAnimations();
            mainObjectManager.studentsAtTable2[i].studentAnimation.ResetAllAnim();
            mainObjectManager.studentsAtTable2[i].studentAnimation.MB33_WorkOnSheets(true);
        }

        playerMovement.LookToPlace(true, mainObjectManager.Table1);
        playerMovement.MovePlayer(true, mainObjectManager.Table1MoveToPoint);

        yield return new WaitForSeconds(5f);

        this.ToggleBoxFilledTable(true, 1);

        //Increase BG audio to 60%
        this.AudioSetVolume(0.0025f);

        for (int i = 0; i < mainObjectManager.studentsAtTable1.Count; i++)
        {
            mainObjectManager.studentsAtTable1[i].StopLookAtSomeone();
            mainObjectManager.studentsAtTable1[i].StopMyRandomLookingAnimations();
            mainObjectManager.studentsAtTable1[i].studentAnimation.ResetAllAnim();
            mainObjectManager.studentsAtTable1[i].studentAnimation.MB33_WorkOnSheets(true);
        }

        playerMovement.MovePlayerToOriginalPostion(true);
        gamePlayManager.StartPhaseSeven(10.0f);

    }

    private void StudentReactionThree()
    {
        StartCoroutine(SRThreeRoutine());
    }

    IEnumerator SRThreeRoutine()
    {

        this.PlayBGAudio();
        this.AudioSetVolume(0.03f);

        playerMovement.LookToPlayer(true, kids[(int)E_KidName.Mia].transform);

        kids[(int)E_KidName.Maxim].SetMyMood(MoodIndicator.Middle);

        //have Maxim stand up (this is instead of him rocking in his chair, as that animation is not implemented)
        kids[(int)E_KidName.Maxim].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Maxim].studentAnimation.MA16_SittingToStandUpIdle(true);


        kids[(int)E_KidName.Janik].SetMyMood(MoodIndicator.Bad);

        kids[(int)E_KidName.Mia].StopLookAtSomeone();
        kids[(int)E_KidName.Mia].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Mia].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        kids[(int)E_KidName.Mia].InitiateGoToSpot(GameObject.Find("StationQueue1").transform);

        yield return new WaitForSeconds(0.5f);

        kids[(int)E_KidName.Niko].StopLookAtSomeone();
        kids[(int)E_KidName.Niko].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Niko].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        kids[(int)E_KidName.Niko].InitiateGoToSpot(GameObject.Find("StationQueue1").transform);

        yield return new WaitForSeconds(0.5f);

        //this.AudioSetVolume(0.6f);

        kids[(int)E_KidName.Leon].StopLookAtSomeone();
        kids[(int)E_KidName.Leon].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Leon].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        kids[(int)E_KidName.Leon].InitiateGoToSpot(GameObject.Find("StationQueue1").transform);

        yield return new WaitForSeconds(0.5f);

        kids[(int)E_KidName.Janik].StopLookAtSomeone();
        kids[(int)E_KidName.Janik].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Janik].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        kids[(int)E_KidName.Janik].InitiateGoToSpot(GameObject.Find("StationQueue1").transform);


        yield return new WaitUntil(() => kids[(int)E_KidName.Mia].reachedSpot);
        kids[(int)E_KidName.Mia].transform.position = GameObject.Find("StationQueue1").transform.position;
        kids[(int)E_KidName.Mia].LookAtTransformXZ(GameObject.Find("FilledBox1Pos").transform);
        //Rummage
        yield return new WaitForSeconds(1.5f);



        kids[(int)E_KidName.Mia].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.BluePaper, true);
        kids[(int)E_KidName.Mia].SetMyLeftHandStudyMaterialsVisibilty(StudyMaterial.YellowPaper, true);
        kids[(int)E_KidName.Mia].GoToAndSitInChair();

        playerMovement.LookToPlayer(true, kids[(int)E_KidName.Niko].transform);

        yield return new WaitUntil(() => kids[(int)E_KidName.Niko].reachedSpot);
        kids[(int)E_KidName.Niko].transform.position = GameObject.Find("StationQueue1").transform.position;
        kids[(int)E_KidName.Niko].LookAtTransformXZ(GameObject.Find("FilledBox2Pos").transform);
        //Rummage
        yield return new WaitForSeconds(1.5f);
        kids[(int)E_KidName.Niko].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.BluePaper, true);
        kids[(int)E_KidName.Niko].SetMyLeftHandStudyMaterialsVisibilty(StudyMaterial.YellowPaper, true);
        kids[(int)E_KidName.Niko].GoToAndSitInChair();

        playerMovement.LookToPlayer(true, kids[(int)E_KidName.Leon].transform);

        yield return new WaitUntil(() => kids[(int)E_KidName.Leon].reachedSpot);
        kids[(int)E_KidName.Leon].transform.position = GameObject.Find("StationQueue1").transform.position;
        kids[(int)E_KidName.Leon].LookAtTransformXZ(GameObject.Find("FilledBox1Pos").transform);
        //Rummage
        yield return new WaitForSeconds(1.5f);
        kids[(int)E_KidName.Leon].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.BluePaper, true);
        kids[(int)E_KidName.Leon].SetMyLeftHandStudyMaterialsVisibilty(StudyMaterial.YellowPaper, true);
        kids[(int)E_KidName.Leon].GoToAndSitInChair();

        playerMovement.LookToPlayer(true, kids[(int)E_KidName.Janik].transform);

        yield return new WaitUntil(() => kids[(int)E_KidName.Janik].reachedSpot);
        kids[(int)E_KidName.Janik].transform.position = GameObject.Find("StationQueue1").transform.position;
        kids[(int)E_KidName.Janik].LookAtTransformXZ(GameObject.Find("FilledBox2Pos").transform);
        //Rummage

        string textToTalk2 = "Guck mal, Jannik, das fehlt dir doch noch, oder?";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk2));

        kids[(int)E_KidName.Janik].SetMyMood(MoodIndicator.Middle);


        yield return new WaitForSeconds(1.5f);
        kids[(int)E_KidName.Janik].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.BluePaper, true);
        kids[(int)E_KidName.Janik].SetMyLeftHandStudyMaterialsVisibilty(StudyMaterial.YellowPaper, true);
        kids[(int)E_KidName.Janik].GoToAndSitInChair();


        for (int i = 0; i < mainObjectManager.studentsAtTable1.Count; i++)
        {
            mainObjectManager.studentsAtTable1[i].StopLookAtSomeone();
            mainObjectManager.studentsAtTable1[i].StopMyRandomLookingAnimations();
            mainObjectManager.studentsAtTable1[i].studentAnimation.ResetAllAnim();
            mainObjectManager.studentsAtTable1[i].studentAnimation.MB33_WorkOnSheets(true);
        }

        gamePlayManager.StartPhaseSeven(10.0f);
    }

    private void StudentReactionFour()
    {
        StartCoroutine(SRFourRoutine());
    }

    IEnumerator SRFourRoutine()
    {
        this.PlayBGAudio();
        this.AudioSetVolume(0.075f);

        kids[(int)E_KidName.Janik].SetMyMood(MoodIndicator.Bad);
        kids[(int)E_KidName.Maxim].SetMyMood(MoodIndicator.Bad);


        kids[(int)E_KidName.Mia].StopLookAtSomeone();
        kids[(int)E_KidName.Mia].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Mia].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Mia].InitiateGoToSpot(GameObject.Find("BoardQueueL1").transform);

        yield return new WaitForSeconds(2f);

        kids[(int)E_KidName.Shirin].StopLookAtSomeone();
        kids[(int)E_KidName.Shirin].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Shirin].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Shirin].InitiateGoToSpot(GameObject.Find("BoardQueueR1").transform);

        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

        kids[(int)E_KidName.Niko].StopLookAtSomeone();
        kids[(int)E_KidName.Niko].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Niko].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Niko].InitiateGoToSpot(GameObject.Find("BoardQueueR2").transform);

        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

        //kids[(int)E_KidName.Maxim].StopLookAtSomeone();
        //kids[(int)E_KidName.Maxim].StopMyRandomLookingAnimations();
        //kids[(int)E_KidName.Maxim].studentAnimation.ResetAllAnim();
        //kids[(int)E_KidName.Maxim].InitiateGoToSpot(GameObject.Find("BoardQueueL2").transform);

        //Have Simon get up to get supplies instead of Maxim
        gamePlayManager.studentsActions[7].StopLookAtSomeone();
        gamePlayManager.studentsActions[7].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[7].studentAnimation.ResetAllAnim();
        gamePlayManager.studentsActions[7].InitiateGoToSpot(GameObject.Find("BoardQueueL2").transform);

        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

        //Have Maxim stand while Simon and Niko are off getting supplies for the table.
        kids[(int)E_KidName.Maxim].StopLookAtSomeone();
        kids[(int)E_KidName.Maxim].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Maxim].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Maxim].studentAnimation.MA16_SittingToStandUpIdle(true);

        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

        kids[(int)E_KidName.Hannah].StopLookAtSomeone();
        kids[(int)E_KidName.Hannah].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Hannah].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Hannah].InitiateGoToSpot(GameObject.Find("BoardQueueL3").transform);

        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));


        kids[(int)E_KidName.Leon].StopLookAtSomeone();
        kids[(int)E_KidName.Leon].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Leon].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Leon].InitiateGoToSpot(GameObject.Find("BoardQueueR3").transform);

        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

        kids[(int)E_KidName.Lena].StopLookAtSomeone();
        kids[(int)E_KidName.Lena].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Lena].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Lena].InitiateGoToSpot(GameObject.Find("BoardQueueL4").transform);

        yield return new WaitUntil(() => kids[(int)E_KidName.Mia].reachedSpot);
        kids[(int)E_KidName.Mia].transform.position = GameObject.Find("BoardQueueL1").transform.position;
        kids[(int)E_KidName.Mia].LookAtTransformXZ(GameObject.Find("Cubby_12").transform);


        yield return new WaitUntil(() => kids[(int)E_KidName.Shirin].reachedSpot);
        kids[(int)E_KidName.Shirin].transform.position = GameObject.Find("BoardQueueR1").transform.position;
        kids[(int)E_KidName.Shirin].LookAtTransformXZ(GameObject.Find("Cubby_13").transform);
        kids[(int)E_KidName.Shirin].SetMyMood(MoodIndicator.Middle);

        yield return new WaitUntil(() => kids[(int)E_KidName.Niko].reachedSpot);
        kids[(int)E_KidName.Niko].transform.position = GameObject.Find("BoardQueueR2").transform.position;
        kids[(int)E_KidName.Niko].LookAtTransformXZ(GameObject.Find("Cubby_13").transform);
        kids[(int)E_KidName.Niko].SetMyMood(MoodIndicator.Middle);

        //Simon, instead of Maxim
        yield return new WaitUntil(() => gamePlayManager.studentsActions[7].reachedSpot);
        gamePlayManager.studentsActions[7].transform.position = GameObject.Find("BoardQueueL2").transform.position;
        gamePlayManager.studentsActions[7].LookAtTransformXZ(GameObject.Find("Cubby_12").transform);
        gamePlayManager.studentsActions[7].SetMyMood(MoodIndicator.Middle);

        yield return new WaitUntil(() => kids[(int)E_KidName.Hannah].reachedSpot);
        kids[(int)E_KidName.Hannah].transform.position = GameObject.Find("BoardQueueL3").transform.position;
        kids[(int)E_KidName.Hannah].LookAtTransformXZ(GameObject.Find("Cubby_12").transform);
        kids[(int)E_KidName.Hannah].SetMyMood(MoodIndicator.Middle);

        yield return new WaitUntil(() => kids[(int)E_KidName.Leon].reachedSpot);
        kids[(int)E_KidName.Leon].transform.position = GameObject.Find("BoardQueueR3").transform.position;
        kids[(int)E_KidName.Leon].LookAtTransformXZ(GameObject.Find("Cubby_13").transform);
        kids[(int)E_KidName.Leon].SetMyMood(MoodIndicator.Middle);

        yield return new WaitUntil(() => kids[(int)E_KidName.Lena].reachedSpot);
        kids[(int)E_KidName.Lena].transform.position = GameObject.Find("BoardQueueL4").transform.position;
        kids[(int)E_KidName.Lena].LookAtTransformXZ(GameObject.Find("Cubby_12").transform);
        kids[(int)E_KidName.Lena].SetMyMood(MoodIndicator.Middle);







        //Rummage Mia
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Mia].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Tablet, true);
        kids[(int)E_KidName.Mia].GoToAndSitInChair();

        //Rummage Shirin
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Shirin].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Pen, true);
        kids[(int)E_KidName.Shirin].GoToAndSitInChair();

        //Rummage Niko
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Niko].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Pen, true);
        kids[(int)E_KidName.Niko].GoToAndSitInChair();

        //Rummage Simon (instead of Maxim)
        yield return new WaitForSeconds(2f);
        gamePlayManager.studentsActions[7].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Tablet, true);
        gamePlayManager.studentsActions[7].GoToAndSitInChair();


        //Rummage Hannah
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Hannah].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Tablet, true);
        kids[(int)E_KidName.Hannah].GoToAndSitInChair();


        //Rummage Leon
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Leon].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Pen, true);
        kids[(int)E_KidName.Leon].GoToAndSitInChair();

        //Rummage Lena
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Lena].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Tablet, true);
        kids[(int)E_KidName.Lena].GoToAndSitInChair();

        yield return new WaitUntil(() => kids[(int)E_KidName.Mia].studentAnimation.IsSittingNow);
        kids[(int)E_KidName.Mia].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Tablet, false);
        yield return new WaitUntil(() => kids[(int)E_KidName.Shirin].studentAnimation.IsSittingNow);
        kids[(int)E_KidName.Shirin].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Pen, false);
        yield return new WaitUntil(() => kids[(int)E_KidName.Niko].studentAnimation.IsSittingNow);
        kids[(int)E_KidName.Niko].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Pen, false);
        
        //Simon returns; Maxim sits down.
        yield return new WaitUntil(() => gamePlayManager.studentsActions[7].studentAnimation.IsSittingNow);
        gamePlayManager.studentsActions[7].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Tablet, false);
        kids[(int)E_KidName.Maxim].studentAnimation.MA16_SittingToStandUpIdle(false);
        kids[(int)E_KidName.Maxim].studentAnimation.Sitting(true);

        yield return new WaitUntil(() => kids[(int)E_KidName.Hannah].studentAnimation.IsSittingNow);
        kids[(int)E_KidName.Hannah].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Tablet, false);
        yield return new WaitUntil(() => kids[(int)E_KidName.Leon].studentAnimation.IsSittingNow);
        kids[(int)E_KidName.Leon].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Pen, false);
        yield return new WaitUntil(() => kids[(int)E_KidName.Lena].studentAnimation.IsSittingNow);
        kids[(int)E_KidName.Lena].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Tablet, false);

        playerMovement.LookToPlayer(true, kids[(int)E_KidName.Janik].transform);
        playerMovement.MovePlayer(true, GameObject.Find("JanikTeacherPos").transform);

        yield return new WaitForSeconds(2f);

        kids[(int)E_KidName.Janik].StopLookAtSomeone();
        kids[(int)E_KidName.Janik].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Janik].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Janik].InitiateGoToSpot(GameObject.Find("BoardQueueR1").transform);

        yield return new WaitUntil(() => kids[(int)E_KidName.Janik].reachedSpot);
        kids[(int)E_KidName.Janik].transform.position = GameObject.Find("BoardQueueR1").transform.position;
        kids[(int)E_KidName.Janik].LookAtTransformXZ(GameObject.Find("Cubby_13").transform);


        playerMovement.MovePlayer(true, GameObject.Find("JanikTeacherPos_Board").transform);
        yield return new WaitForSeconds(2f);

        Debug.Log("Playing Audio for JANIK");

        string textToTalk2 = "Guck mal, Jannik, das fehlt dir doch noch, oder?";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk2));


        kids[(int)E_KidName.Janik].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Pen, true);
        kids[(int)E_KidName.Janik].GoToAndSitInChair();

        

        gamePlayManager.StartPhaseSeven(10.0f);
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

        this.ResetAllHandItems();

        this.AudioSetVolume(0.01f);
        this.ToggleBoxFilledTable(false, 1);
        this.ToggleBoxFilledTable(false, 2);
        this.ToggleBoxFilledTable(false, 3);
        this.ToggleBoxFilledTable(false, 4);

        playerMovement.MovePlayer(true, GameObject.Find("TeacherDeskPoint").transform);

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

    public void ToggleBoxFilledTable(bool state , int tableNo)
    {
        boxFilledCenter[tableNo - 1].SetActive(state);
    }

    public void ResetAllHandItems()
    {
        for(int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Book, false);
            gamePlayManager.studentsActions[i].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.BluePaper, false);
            gamePlayManager.studentsActions[i].SetMyLeftHandStudyMaterialsVisibilty(StudyMaterial.YellowPaper, false);
            gamePlayManager.studentsActions[i].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Tablet, false);
            gamePlayManager.studentsActions[i].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.Pen, false);
        }
    }
}
