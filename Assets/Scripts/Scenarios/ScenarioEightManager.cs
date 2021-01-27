using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScenarioEightManager : MonoBehaviour
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


    private enum E_KidName
    {

        Janik = 0

    }

    public List<Transform> kidsOutsideSpots;
    public bool allKidsReachedBreak = false;
    public GameObject[] sheetsOnTeachersDesk;

    IEnumerator TriggerStudentsGoingOutForBreak()
    {
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].InitiateGoToSpot(kidsOutsideSpots[i], 5f);
            yield return new WaitForSeconds(0.1f);
        }
        StartCoroutine(TriggerCheckStudentsReachedForBreak());
        yield return new WaitUntil(() => allKidsReachedBreak);
        yield return new WaitForSeconds(1f);
    }


    IEnumerator TriggerCheckStudentsReachedForBreak()
    {
        allKidsReachedBreak = false;
        bool[] reachedSpots = new bool[gamePlayManager.studentsActions.Count];

        while (!allKidsReachedBreak)
        {
            for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
            {
                reachedSpots[i] = gamePlayManager.studentsActions[i].reachedSpot;
                yield return new WaitForSeconds(0f);
            }
            if (reachedSpots.All(x => x)) allKidsReachedBreak = true;
        }

        yield return new WaitForSeconds(1f);
    }


    private void Awake()
    {

        if (gamePlayManager)
        {
            gamePlayManager.currentScenario = "SC8";
            gamePlayManager.StartPrepForStudentReaction += PrepForSRs;
        }
    }
    private void Start()
    {

        initPlayerTransform = Instantiate(new GameObject("InitTransform"), playerMovement.transform.position, playerMovement.transform.rotation).transform;
        mainObjectManager = GetComponent<MainObjectsManager>();
        this.PlayBGAudio();
        gamePlayManager.initialActionForScenarioIsCommon = true;
        gamePlayManager.userInterfaceManager.ShowOrHideInitLoadingPanel(true);
        StartCoroutine(StartTheScene());
        this.AudioSetVolume(0.01f);
        foreach (GameObject g in sheetsOnTeachersDesk)
        {
            g.SetActive(false);
        }
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

        string textToTalk = "Ihr habt nun schon einiges über unser neues Thema erfahren. Bevor ihr jetzt gleich weiter daran arbeitet, machen wir eine kurze Bewegungspause";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        yield return new WaitForSeconds(1f);


        yield return new WaitForSeconds(1f);

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

        string textToTalk = "Popcornsimulator";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        yield return new WaitForSeconds(1f);

        StartCoroutine(TriggerStudentsGoingOutForBreak());

        yield return new WaitForSeconds(2f);

        textToTalk = "Popcorn!";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        yield return new WaitForSeconds(1f);

        // do jumping jAPACK

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].studentAnimation.MB37_StandJumpingJacks(true);
        }

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

        // Teachers Audio

        string textToTalk = "Sooo, das tat gut. Dann setzt euch bitte wieder hin, ihr bekommt jetzt einen Auftrag für den Rest der Stunde";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        yield return new WaitForSeconds(1f);

        // Shake Head
        // TODO : Random Head Shake
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].GoToAndSitInChair();
        }


        //Have player look at teacher's desk
        playerMovement.LookToPlace(true, GameObject.Find("SpotAtCuboardNearWindow").transform);
        yield return new WaitForSeconds(1.0f);

        //Spawn Sheets
        foreach (GameObject g in sheetsOnTeachersDesk)
        {
            g.SetActive(true);
        }

        yield return new WaitForSeconds(3.0f);

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

    public GameObject hintCard;

    IEnumerator SROneRoutine()
    {

        //show 5 minute time lapse
        gamePlayManager.SetTimeScaleTextToMinutes();
        gamePlayManager.StartTimer(true);

        yield return new WaitForSeconds(5f);

        gamePlayManager.StopTimer();


        //set student mumbling to zero
        this.AudioSetVolume(0.0f);
        this.PlayBGAudio();
        
        // Janik - 0 , MIA - 10, Maxim - 9 , Simon - 20

        hintCard.SetActive(true);

        

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            //gamePlayManager.studentsActions[i].StartMyRandomLookingOrWrittingAnimations();
            gamePlayManager.studentsActions[i].ResetAndWorkOnSheets();
        }

        //kids[(int)E_KidName.Janik].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Janik].studentAnimation.ResetAllAnim();
        yield return new WaitForSeconds(1f);
        kids[(int)E_KidName.Janik].LookAtSomeone(hintCard.transform);
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Janik].StopLookAtSomeone();
        kids[(int)E_KidName.Janik].studentAnimation.MB33_WorkOnSheets(true);

        yield return new WaitForSeconds(3f);

        //kids[(int)E_KidName.Janik].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Janik].studentAnimation.ResetAllAnim();
        yield return new WaitForSeconds(1f);
        kids[(int)E_KidName.Janik].LookAtSomeone(hintCard.transform);
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Janik].StopLookAtSomeone();
        kids[(int)E_KidName.Janik].studentAnimation.MB33_WorkOnSheets(true);

        yield return new WaitForSeconds(1f);

        gamePlayManager.StartPhaseSeven(2.0f);



    }

    private void StudentReactionTwo()
    {
        StartCoroutine(SRTwoRoutine());
    }

    public Transform initPlayerTransform;
    public Transform teacherJannikSpot;
    IEnumerator SRTwoRoutine()
    {

        //show 5 minute time lapse
        gamePlayManager.SetTimeScaleTextToMinutes();
        gamePlayManager.StartTimer(true);

        yield return new WaitForSeconds(5f);

        gamePlayManager.StopTimer();

        //set student mumbling to zero
        this.AudioSetVolume(0.0f);
        this.PlayBGAudio();

        kids[(int)E_KidName.Janik].SetMyMood(MoodIndicator.Middle);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StartMyRandomLookingOrWrittingAnimations();
        }

        kids[(int)E_KidName.Janik].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Janik].studentAnimation.ResetAllAnim();
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Janik].LookAtSomeone(playerMovement.transform);
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Janik].StopLookAtSomeone();
        kids[(int)E_KidName.Janik].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Janik].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Janik].studentAnimation.RaiseHandAndKeep(true);

        yield return new WaitForSeconds(3f);
        // Teacher Walks towards Jannik , Jannik Hand down ,  looks and speaks , Teacher Walks back

        playerMovement.MovePlayer(true, teacherJannikSpot);
        playerMovement.LookToPlace(true, kids[(int)E_KidName.Janik].transform);


        kids[(int)E_KidName.Janik].studentAnimation.RaiseHandAndKeep(false);


        string textToTalk = "Guck mal hier, das hilft dir weiter.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        else
            yield return new WaitForSeconds(5f);

        yield return new WaitForSeconds(1f);

        //kids[(int)E_KidName.Janik].studentAnimation.MB33_WorkOnSheets(true);
        kids[(int)E_KidName.Janik].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Janik].StartMyRandomLookingOrWrittingAnimations();
        kids[(int)E_KidName.Janik].SetMyMood(MoodIndicator.Good);


        playerMovement.MovePlayer(true, initPlayerTransform);
        //playerMovement.LookToPlace(true, initPlayerTransform);

        yield return new WaitForSeconds(18f);

        gamePlayManager.StartPhaseSeven(1.0f);

    }

    private void StudentReactionThree()
    {
        StartCoroutine(SRThreeRoutine());
    }

    IEnumerator SRThreeRoutine()
    {

        //show 5 minute time lapse
        gamePlayManager.SetTimeScaleTextToMinutes();
        gamePlayManager.StartTimer(true);

        yield return new WaitForSeconds(5f);

        gamePlayManager.StopTimer();

        //set student mumbling to low
        this.AudioSetVolume(0.015f);
        this.PlayBGAudio();

        kids[(int)E_KidName.Janik].SetMyMood(MoodIndicator.Middle);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StartMyRandomLookingOrWrittingAnimations();
        }

        kids[(int)E_KidName.Janik].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Janik].studentAnimation.ResetAllAnim();
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Janik].LookAtSomeone(playerMovement.transform);
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Janik].StopLookAtSomeone();
        kids[(int)E_KidName.Janik].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Janik].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Janik].studentAnimation.RaiseHandAndKeep(true);

        yield return new WaitForSeconds(3f);
        // Teacher Walks towards Jannik , Jannik Hand down ,  looks and speaks , Teacher Walks back

        playerMovement.MovePlayer(true, teacherJannikSpot);
        playerMovement.LookToPlace(true, kids[(int)E_KidName.Janik].transform);


        kids[(int)E_KidName.Janik].studentAnimation.RaiseHandAndKeep(false);


        string textToTalk = "Guck mal hier, das hilft dir weiter.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        else
            yield return new WaitForSeconds(5f);

        yield return new WaitForSeconds(1f);

        kids[(int)E_KidName.Janik].studentAnimation.MB33_WorkOnSheets(true);
        kids[(int)E_KidName.Janik].SetMyMood(MoodIndicator.Good);


        playerMovement.MovePlayer(true, initPlayerTransform);

        yield return new WaitForSeconds(3f);

        mainObjectManager.studentsAtTable1[0].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable1[0].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);
        mainObjectManager.studentsAtTable1[1].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable1[1].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);
        mainObjectManager.studentsAtTable1[2].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable1[2].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);

        mainObjectManager.studentsAtTable2[0].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable2[0].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);
        mainObjectManager.studentsAtTable2[1].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable2[1].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);
        mainObjectManager.studentsAtTable2[2].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable2[2].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);

        mainObjectManager.studentsAtTable3[2].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable3[2].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);
        mainObjectManager.studentsAtTable3[0].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable3[0].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);

        mainObjectManager.studentsAtTable4[0].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable4[0].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);
        mainObjectManager.studentsAtTable4[4].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable4[4].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable1[0].reachedSpot);
        mainObjectManager.studentsAtTable1[0].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable1[0].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable1[0].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable1[1].reachedSpot);
        mainObjectManager.studentsAtTable1[1].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable1[1].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable1[1].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable1[2].reachedSpot);
        mainObjectManager.studentsAtTable1[2].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable1[2].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable1[2].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable2[0].reachedSpot);
        mainObjectManager.studentsAtTable2[0].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable2[0].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable2[0].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable2[1].reachedSpot);
        mainObjectManager.studentsAtTable2[1].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable2[1].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable2[1].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable2[2].reachedSpot);
        mainObjectManager.studentsAtTable2[2].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable2[2].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable2[2].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable3[2].reachedSpot);
        mainObjectManager.studentsAtTable3[2].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable3[2].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable3[2].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable3[0].reachedSpot);
        mainObjectManager.studentsAtTable3[0].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable3[0].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable3[0].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable4[0].reachedSpot);
        mainObjectManager.studentsAtTable4[0].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable4[0].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable4[0].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable4[4].reachedSpot);
        mainObjectManager.studentsAtTable4[4].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable4[4].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable4[4].GoToAndSitInChair();

        yield return new WaitForSeconds(5f);

        gamePlayManager.StartPhaseSeven(5.0f);
    }

    private void StudentReactionFour()
    {
        StartCoroutine(SRFourRoutine());
    }

    private bool isActiveSR4 = false;
    IEnumerator SRFourRoutine()
    {
        //show 5 minute time lapse
        gamePlayManager.SetTimeScaleTextToMinutes();
        gamePlayManager.StartTimer(true);

        yield return new WaitForSeconds(5f);

        gamePlayManager.StopTimer();

        //set student mumbling to low
        this.AudioSetVolume(0.015f);
        this.PlayBGAudio();

        isActiveSR4 = true;

        kids[(int)E_KidName.Janik].SetMyMood(MoodIndicator.Bad);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StartMyRandomLookingOrWrittingAnimations();
        }

        StartCoroutine(SR4JannikRoutine());

        yield return new WaitForSeconds(8f);

        //After 8 seconds, 10 students get up and go to teacher's desk
        mainObjectManager.studentsAtTable1[0].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable1[0].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);
        mainObjectManager.studentsAtTable1[1].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable1[1].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);
        mainObjectManager.studentsAtTable1[2].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable1[2].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);

        mainObjectManager.studentsAtTable2[0].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable2[0].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);
        mainObjectManager.studentsAtTable2[1].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable2[1].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);
        mainObjectManager.studentsAtTable2[2].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable2[2].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);

        mainObjectManager.studentsAtTable3[2].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable3[2].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);
        mainObjectManager.studentsAtTable3[0].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable3[0].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);

        mainObjectManager.studentsAtTable4[0].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable4[0].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);
        mainObjectManager.studentsAtTable4[4].InitiateGoToSpot(GameObject.Find("TeacherDeskPoint").transform);
        mainObjectManager.studentsAtTable4[4].SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(1f);

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable1[0].reachedSpot);
        mainObjectManager.studentsAtTable1[0].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable1[0].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable1[0].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable1[1].reachedSpot);
        mainObjectManager.studentsAtTable1[1].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable1[1].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable1[1].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable1[2].reachedSpot);
        mainObjectManager.studentsAtTable1[2].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable1[2].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable1[2].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable2[0].reachedSpot);
        mainObjectManager.studentsAtTable2[0].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable2[0].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable2[0].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable2[1].reachedSpot);
        mainObjectManager.studentsAtTable2[1].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable2[1].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable2[1].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable2[2].reachedSpot);
        mainObjectManager.studentsAtTable2[2].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable2[2].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable2[2].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable3[2].reachedSpot);
        mainObjectManager.studentsAtTable3[2].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable3[2].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable3[2].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable3[0].reachedSpot);
        mainObjectManager.studentsAtTable3[0].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable3[0].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable3[0].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable4[0].reachedSpot);
        mainObjectManager.studentsAtTable4[0].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable4[0].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable4[0].GoToAndSitInChair();

        yield return new WaitUntil(() => mainObjectManager.studentsAtTable4[4].reachedSpot);
        mainObjectManager.studentsAtTable4[4].transform.position = GameObject.Find("TeacherDeskPoint").transform.position;
        mainObjectManager.studentsAtTable4[4].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        mainObjectManager.studentsAtTable4[4].GoToAndSitInChair();

        //All students resume work
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            gamePlayManager.studentsActions[i].StartMyRandomLookingOrWrittingAnimations();
        }


        yield return new WaitForSeconds(14f);

        isActiveSR4 = false;

        gamePlayManager.StartPhaseSeven(2.0f);
    }

    IEnumerator SR4JannikRoutine()
    {
        while (isActiveSR4)
        {
            kids[(int)E_KidName.Janik].StopLookAtSomeone();
            kids[(int)E_KidName.Janik].StopMyRandomLookingAnimations();
            kids[(int)E_KidName.Janik].studentAnimation.ResetAllAnim();
            kids[(int)E_KidName.Janik].LookAtWindowRoutine();

            yield return new WaitForSeconds(3f);

            kids[(int)E_KidName.Janik].StopLookAtSomeone();
            kids[(int)E_KidName.Janik].StopMyRandomLookingAnimations();
            kids[(int)E_KidName.Janik].studentAnimation.ResetAllAnim();
            kids[(int)E_KidName.Janik].LookAtWindowRoutineStop();
            kids[(int)E_KidName.Janik].studentAnimation.MB33_WorkOnSheets(true);

            yield return new WaitForSeconds(5f);
        }
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
            gamePlayManager.studentsActions[i].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, false);
        }

        this.AudioSetVolume(0.01f);

        playerMovement.MovePlayer(true, initPlayerTransform);

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
