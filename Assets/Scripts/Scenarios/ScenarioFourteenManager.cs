using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioFourteenManager : MonoBehaviour
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
        Maxim = 1,
        Niko = 0,
        Jannik = 2,
        Leonie = 3,
        Niklas = 4,
        Mia = 5
    }


    private void Awake()
    {

        if (gamePlayManager)
        {
            gamePlayManager.currentScenario = "SC14";
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

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopSittingIdleLookAroundAnything(); // stop the initial intro animations for all kids
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].StartMyRandomLookingOrWrittingAnimations();

        }

        yield return new WaitForSeconds(8f);


        StartCoroutine(TriggerMainActionTwo());
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


        Debug.Log("Main Action Two");
        StartCoroutine(MainActionTwoRoutine());

    }


    IEnumerator MainActionTwoRoutine()
    {

        yield return new WaitForSeconds(1.0f); // timeframe is already covered for nearly 10 seconds

        // Increase to 50% TODO
        this.PlayBGAudio();
        this.AudioSetVolume(0.01f);    // 50%

        playerMovement.LookToPlayer(true, kids[(int)E_KidName.Niko].transform);


        kids[(int)E_KidName.Niko].InitiateGoToSpot(GameObject.Find("StationQueue1").transform);
        yield return new WaitForSeconds(1f);
        kids[(int)E_KidName.Maxim].InitiateGoToSpot(GameObject.Find("StationQueue2").transform);

        yield return new WaitUntil(() => kids[(int)E_KidName.Maxim].reachedSpot);
        kids[(int)E_KidName.Maxim].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.BluePaper, true);
        yield return new WaitUntil(() => kids[(int)E_KidName.Niko].reachedSpot);
        kids[(int)E_KidName.Niko].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.YellowPaper, true);

        kids[(int)E_KidName.Niko].GoToAndSitInChair();
        kids[(int)E_KidName.Maxim].GoToAndSitInChair();


        yield return new WaitForSeconds(4f);


        kids[(int)E_KidName.Niko].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.YellowPaper, false);
        kids[(int)E_KidName.Maxim].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.BluePaper, false);

        playerMovement.LookToPlayer(false, kids[(int)E_KidName.Niko].transform);

        gamePlayManager.StartPhaseFive();


    }

    public void MainActionThree()
    {
        StartCoroutine(MainActionThreeRoutine());
    }

    IEnumerator MainActionThreeRoutine()
    {

        // Audio Mumbling reduce to faint noise BG
        this.PlayBGAudio();
        this.AudioSetVolume(0.01f);   // Default

        yield return new WaitForSeconds(5f);

        this.StopBGAudio();

        // Teachers Audio



        string textToTalk = "Bitte legt eure Stifte weg und schaut nach vorne.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
        }

        yield return new WaitForSeconds(3.0f);


        lookAtTeacher = true;
        StartCoroutine(LookAtTeacher());


        textToTalk = "Es ist toll, wie ihr an euren Aufgaben arbeitet, aber die Stunde ist in zehn Minuten vorbei.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        yield return new WaitForSeconds(1f);

        lookAtTeacher = false;
        yield return new WaitForSeconds(3f);

        StartCoroutine(TriggerTeacherQuestionPhase());
    }
    bool lookAtTeacher = false;
    IEnumerator LookAtTeacher()
    {
        while (lookAtTeacher)
        {
            for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
            {
                gamePlayManager.studentsActions[i].StopLookAtSomeone();
                gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
                gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
                gamePlayManager.studentsActions[i].LookAtSomeone(playerMovement.transform);

            }
            yield return new WaitForEndOfFrame();
        }
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


        this.StopBGAudio();
        // Students look at teacher with great focus then they WORK

        lookAtTeacher = true;
        StartCoroutine(LookAtTeacher());

        yield return new WaitForSeconds(4f);

        lookAtTeacher = false;

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
        }

        //this.PlayBGAudio();
        //this.AudioSetVolume(0.01f);

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(4f);
        gamePlayManager.StartPhaseSeven(2.0f);
    }

    private void StudentReactionTwo()
    {
        StartCoroutine(SRTwoRoutine());
    }

    IEnumerator SRTwoRoutine()
    {

        //this.StopBGAudio();
        this.PlayBGAudio();
        this.AudioSetVolume(0.01f);


        // Students look at teacher with great focus then they WORK

        lookAtTeacher = true;
        StartCoroutine(LookAtTeacher());

        yield return new WaitForSeconds(4f);

        lookAtTeacher = false;

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            if (gamePlayManager.studentsActions[i].name == "Jannik" || gamePlayManager.studentsActions[i].name == "Niklas" || gamePlayManager.studentsActions[i].name == "Mia")
                continue;
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
        }

        yield return new WaitForSeconds(1f);

        kids[(int)E_KidName.Niklas].StopLookAtSomeone();
        kids[(int)E_KidName.Niklas].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Niklas].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Niklas].studentAnimation.MB33_WorkOnSheets(false);
        kids[(int)E_KidName.Niklas].studentAnimation.RaiseHand(true);
        kids[(int)E_KidName.Niklas].SetMyMood(MoodIndicator.Middle);


        kids[(int)E_KidName.Mia].StopLookAtSomeone();
        kids[(int)E_KidName.Mia].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Mia].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Mia].studentAnimation.MB33_WorkOnSheets(false);
        kids[(int)E_KidName.Mia].studentAnimation.RaiseHand(true);
        kids[(int)E_KidName.Mia].SetMyMood(MoodIndicator.Middle);

        kids[(int)E_KidName.Jannik].StopLookAtSomeone();
        kids[(int)E_KidName.Jannik].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Jannik].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Jannik].studentAnimation.MB33_WorkOnSheets(false);
        kids[(int)E_KidName.Jannik].studentAnimation.RaiseHand(true);
        kids[(int)E_KidName.Jannik].SetMyMood(MoodIndicator.Middle);

        yield return new WaitForSeconds(3f);

        playerMovement.MovePlayer(true, GameObject.Find("JannikTeacherPos").transform);
        playerMovement.LookToPlace(true, GameObject.Find("JannikTeacherLookAt").transform);




        yield return new WaitForSeconds(8f);

        gamePlayManager.StartPhaseSeven(1.0f);

    }

    private void StudentReactionThree()
    {
        StartCoroutine(SRThreeRoutine());
    }

    IEnumerator SRThreeRoutine()
    {

        //this.StopBGAudio();
        this.PlayBGAudio();
        this.AudioSetVolume(0.03f);


        // Students look at teacher with great focus then they WORK

        lookAtTeacher = true;
        StartCoroutine(LookAtTeacher());

        yield return new WaitForSeconds(4f);

        lookAtTeacher = false;

        yield return new WaitForSeconds(1f);

        //this.PlayBGAudio();
        //this.AudioSetVolume(0.01f);

        //Have Jannik start writing
        kids[(int)E_KidName.Jannik].StopLookAtSomeone();
        kids[(int)E_KidName.Jannik].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Jannik].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Jannik].studentAnimation.Sitting(true);
        yield return new WaitForSeconds(0.1f);
        kids[(int)E_KidName.Jannik].studentAnimation.SitAgitated(false);
        yield return new WaitForSeconds(0.1f);
        kids[(int)E_KidName.Jannik].studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(1f);

        //Set Jannik's mood
        kids[(int)E_KidName.Jannik].SetMyMood(MoodIndicator.Middle);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
        }

            int Count1 = 0;
        for (int i = 1; i < gamePlayManager.studentsActions.Count; i++) {
            //start at i = 1 because Jannik is [0] and he should not be included.
            if (Count1 > 11)
                break;
            if (gamePlayManager.dataManager.GetStudentAcheivement(gamePlayManager.studentsActions[i].name) == "High")
            {
                gamePlayManager.studentsActions[i].StopLookAtSomeone();
                gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
                gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
                gamePlayManager.studentsActions[i].studentAnimation.RaiseHand(true);
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
                Count1++;
            }

            if (gamePlayManager.dataManager.GetStudentAcheivement(gamePlayManager.studentsActions[i].name) == "Middle")
            {
                gamePlayManager.studentsActions[i].StopLookAtSomeone();
                gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
                gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
                gamePlayManager.studentsActions[i].studentAnimation.RaiseHand(true);
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
                Count1++;
            }
        }

        yield return new WaitForSeconds(1f);

        //Have Jannik start writing (again, because for some unkown reason, he stops)
        kids[(int)E_KidName.Jannik].StopLookAtSomeone();
        kids[(int)E_KidName.Jannik].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Jannik].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Jannik].studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(4f);

        int count = 0;
        for (int i = 1; i < gamePlayManager.studentsActions.Count; i++) {
            //start at i = 1 because Jannik is [0] and he should not be included.


            if (count > 3)
                break;
            if (gamePlayManager.dataManager.GetStudentAcheivement(gamePlayManager.studentsActions[i].name) == "Low") {

                gamePlayManager.studentsActions[i].StopLookAtSomeone();
                gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
                gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
                gamePlayManager.studentsActions[i].studentAnimation.MB19_Protest(true);
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Bad);
                count++;

            }
        }

        //Have Jannik start writing (again, because for some unkown reason, he stops)
        kids[(int)E_KidName.Jannik].StopLookAtSomeone();
        kids[(int)E_KidName.Jannik].StopMyRandomLookingAnimations();
        //kids[(int)E_KidName.Jannik].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Jannik].studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(4f);


        kids[(int)E_KidName.Maxim].StopLookAtSomeone();
        kids[(int)E_KidName.Maxim].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Maxim].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Maxim].studentAnimation.MB19_Protest(true);
        kids[(int)E_KidName.Maxim].SetMyMood(MoodIndicator.Bad);

        //Have Jannik start writing (again, because for some unkown reason, he stops)
        kids[(int)E_KidName.Jannik].StopLookAtSomeone();
        kids[(int)E_KidName.Jannik].StopMyRandomLookingAnimations();
        //kids[(int)E_KidName.Jannik].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Jannik].studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(4f);

        kids[(int)E_KidName.Leonie].StopLookAtSomeone();
        kids[(int)E_KidName.Leonie].StopMyRandomLookingAnimations();
        kids[(int)E_KidName.Leonie].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Leonie].studentAnimation.MB19_Protest(true);
        kids[(int)E_KidName.Leonie].SetMyMood(MoodIndicator.Bad);

        //Have Jannik start writing (again, because for some unkown reason, he stops)
        kids[(int)E_KidName.Jannik].StopLookAtSomeone();
        kids[(int)E_KidName.Jannik].StopMyRandomLookingAnimations();
        //kids[(int)E_KidName.Jannik].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Jannik].studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(4f);

        //kids[(int)E_KidName.Jannik].StopLookAtSomeone();
        //kids[(int)E_KidName.Jannik].StopMyRandomLookingAnimations();
        //kids[(int)E_KidName.Jannik].studentAnimation.ResetAllAnim();
        //kids[(int)E_KidName.Jannik].studentAnimation.MB33_WorkOnSheets(true);
        //yield return new WaitForSeconds(1f);
        //kids[(int)E_KidName.Jannik].SetMyMood(MoodIndicator.Middle);

        //this.AudioSetVolume(0.03f);

        yield return new WaitForSeconds(9f);

        gamePlayManager.StartPhaseSeven(10.0f);
    }

    private void StudentReactionFour()
    {
        StartCoroutine(SRFourRoutine());
    }

    IEnumerator SRFourRoutine()
    {
        //this.StopBGAudio();
        this.PlayBGAudio();
        this.AudioSetVolume(0.05f);


        // Students look at teacher with great focus then they WORK

        lookAtTeacher = true;
        StartCoroutine(LookAtTeacher());

        yield return new WaitForSeconds(4f);

        lookAtTeacher = false;

        yield return new WaitForSeconds(1f);

        //this.PlayBGAudio();
        //this.AudioSetVolume(0.01f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
        }

        //Student Bags Packing

        gamePlayManager.studentsActions[1].SetStudentBag(true);
        gamePlayManager.studentsActions[1].studentAnimation.SearchForPencil(true);
        yield return new WaitForSeconds(0.5f);

        gamePlayManager.studentsActions[2].SetStudentBag(true);
        gamePlayManager.studentsActions[2].studentAnimation.SearchForPencil(true);
        yield return new WaitForSeconds(0.5f);

        gamePlayManager.studentsActions[6].SetStudentBag(true);
        gamePlayManager.studentsActions[6].studentAnimation.SearchForPencil(true);
        yield return new WaitForSeconds(0.5f);

        gamePlayManager.studentsActions[8].SetStudentBag(true);
        gamePlayManager.studentsActions[8].studentAnimation.SearchForPencil(true);
        yield return new WaitForSeconds(0.5f);

        gamePlayManager.studentsActions[11].SetStudentBag(true);
        gamePlayManager.studentsActions[11].studentAnimation.SearchForPencil(true);
        yield return new WaitForSeconds(0.5f);

        gamePlayManager.studentsActions[16].SetStudentBag(true);
        gamePlayManager.studentsActions[16].studentAnimation.SearchForPencil(true);
        yield return new WaitForSeconds(0.5f);

        gamePlayManager.studentsActions[20].SetStudentBag(true);
        gamePlayManager.studentsActions[20].studentAnimation.SearchForPencil(true);
        yield return new WaitForSeconds(0.5f);

        this.isPacking = true;
        StartCoroutine(PackStuff());
        yield return new WaitForSeconds(4f);

        // Whisper 

        gamePlayManager.studentsActions[3].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        gamePlayManager.studentsActions[4].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        gamePlayManager.studentsActions[5].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        gamePlayManager.studentsActions[7].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        gamePlayManager.studentsActions[12].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        gamePlayManager.studentsActions[17].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        gamePlayManager.studentsActions[18].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        yield return new WaitForSeconds(4f);

        //Working

        gamePlayManager.studentsActions[3].studentAnimation.MB33_WorkOnSheets(true);
        gamePlayManager.studentsActions[3].SetMyMood(MoodIndicator.Bad);
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[4].studentAnimation.MB33_WorkOnSheets(true);
        gamePlayManager.studentsActions[4].SetMyMood(MoodIndicator.Bad);
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[5].studentAnimation.MB33_WorkOnSheets(true);
        gamePlayManager.studentsActions[5].SetMyMood(MoodIndicator.Bad);
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[7].studentAnimation.MB33_WorkOnSheets(true);
        gamePlayManager.studentsActions[7].SetMyMood(MoodIndicator.Bad);
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[12].studentAnimation.MB33_WorkOnSheets(true);
        gamePlayManager.studentsActions[12].SetMyMood(MoodIndicator.Bad);
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[17].studentAnimation.MB33_WorkOnSheets(true);
        gamePlayManager.studentsActions[17].SetMyMood(MoodIndicator.Bad);
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[18].studentAnimation.MB33_WorkOnSheets(true);
        gamePlayManager.studentsActions[18].SetMyMood(MoodIndicator.Bad);
        yield return new WaitForSeconds(1f);

        kids[(int)E_KidName.Jannik].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Jannik].studentAnimation.MB33_WorkOnSheets(true);

        yield return new WaitForSeconds(3f);

        playerMovement.MovePlayer(true, GameObject.Find("JannikTeacherPos").transform);
        playerMovement.LookToPlace(true, GameObject.Find("JannikTeacherLookAt").transform);

        yield return new WaitForSeconds(7f);

        isPacking = false;
        gamePlayManager.StartPhaseSeven(2.0f);
    }

    public GameObject[] stuffToPack;
    public bool isPacking { get; private set; } = false;
    IEnumerator PackStuff()
    {
        while (isPacking)
        {
            foreach (GameObject g in stuffToPack)
            {
                g.SetActive(false);
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForEndOfFrame();

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
        }

        //Student Bags Packing

        gamePlayManager.studentsActions[1].SetStudentBag(false);
        gamePlayManager.studentsActions[2].SetStudentBag(false);
        gamePlayManager.studentsActions[6].SetStudentBag(false);
        gamePlayManager.studentsActions[8].SetStudentBag(false);
        gamePlayManager.studentsActions[11].SetStudentBag(false);
        gamePlayManager.studentsActions[16].SetStudentBag(false);
        gamePlayManager.studentsActions[20].SetStudentBag(false);

        this.AudioSetVolume(0.01f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
        //playerMovement.MovePlayer(true, GameObject.Find("TeacherDeskPoint").transform);
        foreach (GameObject g in stuffToPack)
        {
            g.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
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
