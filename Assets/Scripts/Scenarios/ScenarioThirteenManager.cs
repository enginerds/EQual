using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioThirteenManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    public PlayerMovement playerMovement;

    public Transform tabSimonePoint;
    public GameObject MainCamera;
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
        Maxim = 0,   // 0
        Jannik = 1   // 9

    }


    private void Awake()
    {

        if (gamePlayManager)
        {
            gamePlayManager.currentScenario = "SC13";
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
        this.PlayBGAudio();
        this.AudioSetVolume(0.02f);    // 50%

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopSittingIdleLookAroundAnything(); // stop the initial intro animations for all kids
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].StartMyTalkOrLaughAnimations();

        }

        yield return new WaitForSeconds(5f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopSittingIdleLookAroundAnything(); // stop the initial intro animations for all kids
            gamePlayManager.studentsActions[i].StopMyTalkOrLaughAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
        }

        this.lookAtTeacher = true;
        StartCoroutine(LookAtTeacher());

        //play Audio
        string textToTalk = "Guten Morgen alle miteinander!";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        yield return new WaitForSeconds(3f);

        StartCoroutine(TriggerMainActionTwo());
    }

    public GameObject teacherBook;
    IEnumerator TriggerMainActionTwo()
    {



        yield return new WaitForSeconds(1.0f);
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

        this.StopBGAudio();
        //yield return new WaitForSeconds(1.0f); // timeframe is already covered for nearly 10 seconds
        teacherBook.SetActive(true);

        //this.StopBGAudio();
        yield return new WaitForSeconds(1.0f); // timeframe is already covered for nearly 10 seconds

        string textToTalk = "Mit diesem Buch habt ihr ja in der letzten Stunde gearbeitet. Nennt doch mal möglichst viele Inhalte oder Stichpunkte, die ihr euch gemerkt habt.";
        if (gamePlayManager.LOG_ENABLED)
            yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));


        gamePlayManager.StartPhaseFive();


    }

    public void MainActionThree()
    {
        StartCoroutine(MainActionThreeRoutine());
    }

    IEnumerator MainActionThreeRoutine()
    {

        // Audio Mumbling reduce to faint noise BG
        this.StopBGAudio();

        yield return new WaitForSeconds(2f);

        // Teachers Audio

        //string textToTalk = "Mit diesem Buch habt ihr ja in der letzten Stunde gearbeitet. Nennt doch mal möglichst viele Inhalte oder Stichpunkte, die ihr euch gemerkt habt.";
        //if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        yield return new WaitForSeconds(1f);


        yield return new WaitForSeconds(3f);

        teacherBook.SetActive(false);
        lookAtTeacher = false;

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

        //elevate camera to better see students
        float addedCameraHeight = 0.75f;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().ElevateCamera(addedCameraHeight);


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

    public GameObject popupWindow;
    IEnumerator SROneRoutine()
    {
        

        this.StopBGAudio();
        // Students look at teacher with great focus then they WORK

        lookAtTeacher = false;
        yield return new WaitForSeconds(0.1f);


        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
        }

        // Pick 15 kids one of them Maxim

        for (int i = 2; i < kids.Length; i++)
        {
            gamePlayManager.studentsActions[i].studentAnimation.RaiseHand(true);
        }

        kids[(int)E_KidName.Maxim].studentAnimation.RaiseHand(true);

        yield return new WaitForSeconds(5f);

        //Teacher Picks 1 kid random - popup

        kids[3].studentAnimation.RaiseHand(false);
        kids[3].LookAtSomeone(playerMovement.transform);
        popupWindow.SetActive(true);
        kids[3].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        string textToTalk = "Also ich habe mir gemerkt, dass.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk, true));
        yield return new WaitForSeconds(1.5f);
        popupWindow.SetActive(false);
        kids[3].studentAnimation.ResetAllAnim();



        yield return new WaitForSeconds(3f);

        //Teacher Picks 1 kid random - popup

        kids[5].studentAnimation.RaiseHand(false);
        kids[5].LookAtSomeone(playerMovement.transform);
        popupWindow.SetActive(true);
        kids[5].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        textToTalk = "Also ich habe mir gemerkt, dass.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk, true));
        yield return new WaitForSeconds(1.5f);
        popupWindow.SetActive(false);
        kids[5].studentAnimation.ResetAllAnim();


        // Now Jannik Raises hands
        yield return new WaitForSeconds(2f);
        kids[(int)E_KidName.Jannik].studentAnimation.RaiseHand(true);
        yield return new WaitForSeconds(3f);
        textToTalk = "Jannik, du bitte.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        yield return new WaitForSeconds(1f);
        kids[(int)E_KidName.Jannik].studentAnimation.RaiseHand(false);
        kids[(int)E_KidName.Jannik].LookAtSomeone(playerMovement.transform);
        popupWindow.SetActive(true);
        kids[(int)E_KidName.Jannik].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        textToTalk = "Also ich habe mir gemerkt, dass.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk, true));
        yield return new WaitForSeconds(1.5f);
        popupWindow.SetActive(false);
        kids[(int)E_KidName.Jannik].studentAnimation.ResetAllAnim();


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

        this.StopBGAudio();

        //set student mumbling to low
        AudioSetVolume(0.01f);
        PlayBGAudio();

        // Students look at teacher with great focus then they WORK


        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
        }

        // Pick 15 kids one of them Maxim

        for (int i = 2; i < kids.Length; i++)
        {
            gamePlayManager.studentsActions[i].studentAnimation.RaiseHand(true);
        }

        kids[(int)E_KidName.Maxim].studentAnimation.RaiseHand(true);

        // Now maxim is picked
        yield return new WaitForSeconds(4f);

        kids[(int)E_KidName.Maxim].studentAnimation.RaiseHand(false);

        //teacher speaks to Maxim
        string textToTalk = "Maxim, du bitte.";
        if (gamePlayManager.LOG_ENABLED)
        yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        yield return new WaitForSeconds(1.5f);

        kids[(int)E_KidName.Maxim].LookAtSomeone(playerMovement.transform);
        popupWindow.SetActive(true);
        kids[(int)E_KidName.Maxim].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        // 2 kids start looking random and mood drops
        kids[3].StartMyRandomLookingOrWrittingAnimations();
        kids[3].SetMyMood(MoodIndicator.Middle);
        kids[7].StartMyRandomLookingOrWrittingAnimations();
        kids[7].SetMyMood(MoodIndicator.Middle);
        textToTalk = "Also ich habe mir gemerkt, dass.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk, true));
        yield return new WaitForSeconds(1.5f);
        popupWindow.SetActive(false);
        kids[(int)E_KidName.Maxim].studentAnimation.ResetAllAnim();



        // Now Random is picked
        yield return new WaitForSeconds(4f);

        kids[4].studentAnimation.RaiseHand(false);
        kids[4].LookAtSomeone(playerMovement.transform);
        popupWindow.SetActive(true);
        kids[4].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        // 2 kids start looking random and mood drops
        kids[5].StartMyRandomLookingOrWrittingAnimations();
        kids[5].SetMyMood(MoodIndicator.Middle);
        kids[8].StartMyRandomLookingOrWrittingAnimations();
        kids[8].SetMyMood(MoodIndicator.Middle);
        textToTalk = "Also ich habe mir gemerkt, dass.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk, true));
        yield return new WaitForSeconds(1.5f);
        popupWindow.SetActive(false);
        kids[4].studentAnimation.ResetAllAnim();



        // Now Random is picked
        yield return new WaitForSeconds(4f);

        kids[11].studentAnimation.RaiseHand(false);
        kids[11].LookAtSomeone(playerMovement.transform);
        popupWindow.SetActive(true);
        kids[11].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        // 2 kids start looking random and mood drops
        kids[12].StartMyRandomLookingOrWrittingAnimations();
        kids[12].SetMyMood(MoodIndicator.Middle);
        kids[9].StartMyRandomLookingOrWrittingAnimations();
        kids[9].SetMyMood(MoodIndicator.Middle);
        textToTalk = "Also ich habe mir gemerkt, dass.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk, true));
        yield return new WaitForSeconds(1.5f);
        popupWindow.SetActive(false);
        kids[11].studentAnimation.ResetAllAnim();


        // Now Random is picked
        yield return new WaitForSeconds(4f);

        kids[13].studentAnimation.RaiseHand(false);
        kids[13].LookAtSomeone(playerMovement.transform);
        popupWindow.SetActive(true);
        kids[13].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        // 2 kids start looking random and mood drops
        kids[14].StartMyRandomLookingOrWrittingAnimations();
        kids[14].SetMyMood(MoodIndicator.Middle);
        kids[16].StartMyRandomLookingOrWrittingAnimations();
        kids[16].SetMyMood(MoodIndicator.Middle);
        textToTalk = "Also ich habe mir gemerkt, dass.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk, true));
        yield return new WaitForSeconds(1.5f);
        popupWindow.SetActive(false);
        kids[13].studentAnimation.ResetAllAnim();


        // Now Random is picked
        yield return new WaitForSeconds(4f);

        kids[15].studentAnimation.RaiseHand(false);
        kids[15].LookAtSomeone(playerMovement.transform);
        popupWindow.SetActive(true);
        kids[15].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        // 2 kids start looking random and mood drops
        kids[6].StartMyRandomLookingOrWrittingAnimations();
        kids[6].SetMyMood(MoodIndicator.Middle);
        kids[10].StartMyRandomLookingOrWrittingAnimations();
        kids[10].SetMyMood(MoodIndicator.Middle);
        textToTalk = "Also ich habe mir gemerkt, dass.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk, true));
        yield return new WaitForSeconds(1.5f);
        popupWindow.SetActive(false);
        kids[15].studentAnimation.ResetAllAnim();


        yield return new WaitForSeconds(4f);

        gamePlayManager.StartPhaseSeven(1.0f);

    }

    private void StudentReactionThree()
    {
        StartCoroutine(SRThreeRoutine());
    }

    IEnumerator SRThreeRoutine()
    {

        this.StopBGAudio();

        //set student mumbling to medium
        AudioSetVolume(0.03f);
        PlayBGAudio();

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
        }

        yield return new WaitForSeconds(1f);
        // Pick 15 kids one of them Maxim

        for (int i = 3; i < kids.Length; i++)
        {
            kids[i].studentAnimation.RaiseHand(true);
        }

        kids[(int)E_KidName.Maxim].studentAnimation.RaiseHand(true);

        yield return new WaitForSeconds(2f);

        //Teacher Picks 1 kid random - popup

        kids[5].studentAnimation.RaiseHand(false);
        kids[5].LookAtSomeone(playerMovement.transform);
        popupWindow.SetActive(true);
        kids[5].studentAnimation.VI11_TalkToFriendsLeftAndRight();

        string textToTalk = "Also ich habe mir gemerkt, dass.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk, true));
        yield return new WaitForSeconds(1.5f);
        popupWindow.SetActive(false);
        kids[5].studentAnimation.ResetAllAnim();
        kids[5].SetMyMood(MoodIndicator.Middle);

        //teacher responds after 1st kid speaks
        textToTalk = "Ok, dann nehmt euch ab jetzt bitte gegenseitig dran.";
        if (gamePlayManager.LOG_ENABLED)
            yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        yield return new WaitForSeconds(1.5f);

        // kid Picks 1 random kid
        yield return new WaitForSeconds(4f);

        kids[11].studentAnimation.RaiseHand(false);
        kids[11].LookAtSomeone(playerMovement.transform);
        popupWindow.SetActive(true);
        kids[11].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        // 3 kids start looking random and mood drops
        kids[3].StartMyRandomLookingOrWrittingAnimations();
        kids[3].SetMyMood(MoodIndicator.Middle);
        kids[7].StartMyRandomLookingOrWrittingAnimations();
        kids[7].SetMyMood(MoodIndicator.Middle);
        kids[5].StartMyRandomLookingOrWrittingAnimations();
        kids[5].SetMyMood(MoodIndicator.Middle);
        //Maxim mood bad, hands down
        kids[(int)E_KidName.Maxim].studentAnimation.RaiseHand(false);
        kids[(int)E_KidName.Maxim].studentAnimation.ResetAllAnim();
        kids[(int)E_KidName.Maxim].SetMyMood(MoodIndicator.Bad);
        kids[(int)E_KidName.Maxim].studentAnimation.MB9_LookAround(true); //StartMyRandomLookingOrWrittingAnimations();
        textToTalk = "Also ich habe mir gemerkt, dass.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk, true));
        yield return new WaitForSeconds(1.5f);
        popupWindow.SetActive(false);
        kids[11].studentAnimation.ResetAllAnim();
        kids[11].SetMyMood(MoodIndicator.Middle);

        foreach (StudentAction sa in remainingKids)
        {
            sa.SetMyMood(MoodIndicator.Middle);
        }

        //this.PlayBGAudio();
        //this.AudioSetVolume(0.01f);

        // kid Picks 1 random kid
        yield return new WaitForSeconds(4f);

        kids[4].studentAnimation.RaiseHand(false);
        kids[4].LookAtSomeone(playerMovement.transform);
        popupWindow.SetActive(true);
        kids[4].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        // 3 kids start looking random and mood drops
        kids[8].StartMyRandomLookingOrWrittingAnimations();
        kids[8].SetMyMood(MoodIndicator.Middle);
        kids[12].StartMyRandomLookingOrWrittingAnimations();
        kids[12].SetMyMood(MoodIndicator.Middle);
        kids[9].StartMyRandomLookingOrWrittingAnimations();
        kids[9].SetMyMood(MoodIndicator.Middle);
        textToTalk = "Also ich habe mir gemerkt, dass.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk, true));
        yield return new WaitForSeconds(1.5f);
        popupWindow.SetActive(false);
        kids[4].studentAnimation.ResetAllAnim();
        kids[4].SetMyMood(MoodIndicator.Middle);

        // kid Picks 1 random kid
        yield return new WaitForSeconds(4f);

        kids[13].studentAnimation.RaiseHand(false);
        kids[13].LookAtSomeone(playerMovement.transform);
        popupWindow.SetActive(true);
        kids[13].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        // 3 kids start looking random and mood drops
        kids[14].StartMyRandomLookingOrWrittingAnimations();
        kids[14].SetMyMood(MoodIndicator.Middle);
        kids[6].StartMyRandomLookingOrWrittingAnimations();
        kids[6].SetMyMood(MoodIndicator.Middle);
        kids[16].StartMyRandomLookingOrWrittingAnimations();
        kids[16].SetMyMood(MoodIndicator.Middle);
        textToTalk = "Also ich habe mir gemerkt, dass.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk, true));
        yield return new WaitForSeconds(1.5f);
        popupWindow.SetActive(false);
        kids[13].studentAnimation.ResetAllAnim();
        kids[13].SetMyMood(MoodIndicator.Middle);

        // kid Picks 1 random kid
        yield return new WaitForSeconds(4f);

        kids[15].studentAnimation.RaiseHand(false);
        kids[15].LookAtSomeone(playerMovement.transform);
        popupWindow.SetActive(true);
        kids[15].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        // 3 kids start looking random and mood drops
        kids[10].StartMyRandomLookingOrWrittingAnimations();
        kids[10].SetMyMood(MoodIndicator.Middle);
        kids[1].StartMyRandomLookingOrWrittingAnimations();
        kids[1].SetMyMood(MoodIndicator.Middle);
        kids[2].StartMyRandomLookingOrWrittingAnimations();
        kids[2].SetMyMood(MoodIndicator.Middle);
        textToTalk = "Also ich habe mir gemerkt, dass.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk, true));
        yield return new WaitForSeconds(1.5f);
        popupWindow.SetActive(false);
        kids[15].studentAnimation.ResetAllAnim();
        kids[15].SetMyMood(MoodIndicator.Middle);

        gamePlayManager.studentsActions[16].SetMyMood(MoodIndicator.Middle);
        //this.AudioSetVolume(0.015f);
        yield return new WaitForSeconds(4f);

        gamePlayManager.StartPhaseSeven(2.0f);
    }

    private void StudentReactionFour()
    {
        StartCoroutine(SRFourRoutine());


    }

    IEnumerator SRFourRoutine()
    {
        this.StopBGAudio();

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
        }

        yield return new WaitForSeconds(1f);
        // Pick 15 kids one of them Maxim

        for (int i = 3; i < kids.Length; i++)
        {
            kids[i].studentAnimation.RaiseHand(true);
        }

        kids[(int)E_KidName.Maxim].studentAnimation.RaiseHand(true);

        yield return new WaitForSeconds(3f);

        // Teachers Audio

        string textToTalk = "eins, zwei, drei, vier, fünf, sechs, sieben, acht, neun, zehn, elf, zwölf, dreizehn, vierzehn, fünfzehn";    //"1 2 3 4 5 6 7 8 9 10 11 12 13 14 15";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        yield return new WaitForSeconds(1f);

        kids[(int)E_KidName.Maxim].studentAnimation.RaiseHand(false);
        kids[(int)E_KidName.Maxim].studentAnimation.MB33_WorkOnSheets(true);


        // Now Random is picked
        yield return new WaitForSeconds(3f);

        kids[2].studentAnimation.RaiseHand(false);
        kids[2].LookAtSomeone(playerMovement.transform);
        popupWindow.SetActive(true);
        kids[2].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        // 2 kids start looking random and mood drops
        kids[15].StartMyRandomLookingOrWrittingAnimations();
        kids[15].SetMyMood(MoodIndicator.Middle);
        kids[16].StartMyRandomLookingOrWrittingAnimations();
        kids[16].SetMyMood(MoodIndicator.Middle);
        textToTalk = "Also ich habe mir gemerkt, dass.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk, true));
        yield return new WaitForSeconds(1.5f);
        popupWindow.SetActive(false);
        kids[2].studentAnimation.ResetAllAnim();
        kids[2].SetMyMood(MoodIndicator.Middle);


        // Now Random is picked
        yield return new WaitForSeconds(3f);

        kids[3].studentAnimation.RaiseHand(false);
        kids[3].LookAtSomeone(playerMovement.transform);
        popupWindow.SetActive(true);
        kids[3].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        // 2 kids start looking random and mood drops
        kids[14].StartMyRandomLookingOrWrittingAnimations();
        kids[14].SetMyMood(MoodIndicator.Middle);
        kids[13].StartMyRandomLookingOrWrittingAnimations();
        kids[13].SetMyMood(MoodIndicator.Middle);
        kids[12].StartMyRandomLookingOrWrittingAnimations();
        kids[12].SetMyMood(MoodIndicator.Middle);
        kids[11].StartMyRandomLookingOrWrittingAnimations();
        kids[11].SetMyMood(MoodIndicator.Middle);
        kids[10].StartMyRandomLookingOrWrittingAnimations();
        kids[10].SetMyMood(MoodIndicator.Middle);
        textToTalk = "Also ich habe mir gemerkt, dass.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk, true));
        yield return new WaitForSeconds(1.5f);
        popupWindow.SetActive(false);
        kids[3].studentAnimation.ResetAllAnim();
        kids[3].SetMyMood(MoodIndicator.Middle);

        // Now Random is picked
        yield return new WaitForSeconds(3f);

        //mood of students drop to Middle after two students speak
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
        }

            this.PlayBGAudio();
        this.AudioSetVolume(0.01f);

        kids[4].studentAnimation.RaiseHand(false);
        kids[4].LookAtSomeone(playerMovement.transform);
        popupWindow.SetActive(true);
        kids[4].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        // 2 kids start looking random and mood drops
        kids[9].StartMyRandomLookingOrWrittingAnimations();
        kids[9].SetMyMood(MoodIndicator.Middle);
        kids[8].StartMyRandomLookingOrWrittingAnimations();
        kids[8].SetMyMood(MoodIndicator.Middle);
        kids[7].StartMyRandomLookingOrWrittingAnimations();
        kids[7].SetMyMood(MoodIndicator.Middle);
        kids[6].StartMyRandomLookingOrWrittingAnimations();
        kids[6].SetMyMood(MoodIndicator.Middle);
        kids[2].StartMyRandomLookingOrWrittingAnimations();
        kids[2].SetMyMood(MoodIndicator.Middle);
        kids[1].StartMyTalkOrLaughAnimations();
        kids[1].SetMyMood(MoodIndicator.Middle);
        kids[3].StartMyTalkOrLaughAnimations();
        kids[3].SetMyMood(MoodIndicator.Middle);

        textToTalk = "Also ich habe mir gemerkt, dass.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk, true));
        yield return new WaitForSeconds(1.5f);
        popupWindow.SetActive(false);
        kids[4].studentAnimation.ResetAllAnim();
        kids[4].SetMyMood(MoodIndicator.Middle);


        // Now Random is picked
        yield return new WaitForSeconds(3f);
        this.AudioSetVolume(0.015f);
        kids[5].studentAnimation.RaiseHand(false);
        kids[5].LookAtSomeone(playerMovement.transform);
        popupWindow.SetActive(true);
        kids[5].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        // 2 kids start looking random and mood drops

        kids[6].StopLookAtSomeone();
        kids[6].StopMyRandomLookingAnimations();
        kids[6].studentAnimation.ResetAllAnim();
        kids[6].StartMyTalkOrLaughAnimations();
        kids[6].SetMyMood(MoodIndicator.Bad);

        kids[10].StopLookAtSomeone();
        kids[10].StopMyRandomLookingAnimations();
        kids[10].studentAnimation.ResetAllAnim();
        kids[10].StartMyTalkOrLaughAnimations();
        kids[10].SetMyMood(MoodIndicator.Bad); ;
        remainingKids[0].StartMyRandomLookingOrWrittingAnimations(); ;
        remainingKids[0].SetMyMood(MoodIndicator.Middle);
        remainingKids[1].StartMyRandomLookingOrWrittingAnimations();
        remainingKids[1].SetMyMood(MoodIndicator.Middle);
        remainingKids[2].StartMyRandomLookingOrWrittingAnimations();
        remainingKids[2].SetMyMood(MoodIndicator.Middle);
        remainingKids[3].StartMyRandomLookingOrWrittingAnimations();
        remainingKids[3].SetMyMood(MoodIndicator.Middle);
        textToTalk = "Also ich habe mir gemerkt, dass.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk, true));
        yield return new WaitForSeconds(1.5f);
        popupWindow.SetActive(false);
        kids[5].studentAnimation.ResetAllAnim();
        kids[5].SetMyMood(MoodIndicator.Middle);

        gamePlayManager.studentsActions[16].SetMyMood(MoodIndicator.Middle);

        //mood of students drop to Bad after two students speak
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Bad);
        }

        yield return new WaitForSeconds(3f);




        gamePlayManager.StartPhaseSeven(2.0f);
    }
    public StudentAction[] remainingKids;


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
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
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
