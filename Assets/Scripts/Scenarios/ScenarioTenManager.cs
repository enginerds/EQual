using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class ScenarioTenManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    public Transform teacherDeskPoint;
    public Transform classEntryPoint;
    public AudioSource audioInClass;
    public MainObjectsManager mainObjectManager;
    public StudentAction KidMaxim;
    public StudentAction KidLeonie;
    public StudentAction KidSimon;
    public StudentAction kidJulian;

    string questionString = "";



    private void Awake()
    {
        gamePlayManager.currentScenario = "SC10";
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

        //set student mumbling to low
        audioInClass.volume = 0.01f;
        if (!audioInClass.isPlaying) audioInClass.Play();

        
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
        string taKey = (gamePlayManager.userInterfaceManager.noOfNoCounts == 0) ? "TA1" : "TA2";
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData(taKey, value);
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TASQuestion", gamePlayManager.userInterfaceManager.teacherSelectedQuestion);
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TASTimeStarted", gamePlayManager.GetCurrentTime());
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("Revision", gamePlayManager.userInterfaceManager.noOfNoCounts.ToString());


        //enable player to look around and move to support seeing all kids during SRs.
        GameObject.FindGameObjectWithTag("Player").GetComponent<RigidbodyFirstPersonController>().MoveAndLookLocked = false;

        switch (value)
        {

            case "1":
                questionString = "Sie lassen erst einmal alle Kinder einzeln Vorschläge aufschreiben und besprechen diese dann im Plenum.";

                StudentReactionOne();
                gamePlayManager.StartPhaseSeven(25.0f);
                break;
            case "2":
                questionString = "Sie lassen die Kinder in Gruppen Vorschläge erarbeiten und bitten dann Leonie und Maxim (beide SFB ESE), die Ergebnisse der Gruppentische zu sammeln.";

                StudentReactionTwo();
                gamePlayManager.StartPhaseSeven(25.0f);
                break;
            case "3":
                questionString = "Sie lassen die Kinder in Gruppen Vorschläge erarbeiten, erinnern an die Eigenverantwortung und fordern explizit dazu auf, dass alle Kinder Vorschläge machen.";

                StudentReactionThree();
                gamePlayManager.StartPhaseSeven(25.0f);
                break;
            case "4":
                questionString = "Sie lassen die Kinder in Gruppen Vorschläge erarbeiten und setzen Leonie und Maxim (beide SFB ESE) an einen Tisch, um eine aktive Beteiligung zu motivieren.";
                StudentReactionFour();
                gamePlayManager.StartPhaseSeven(28.0f);

                break;
        }

        //showTempText();
        gamePlayManager.playerActionDataHandler.PlayerAction = questionString;

    }

    void showTempText() {

     //   gamePlayManager.userInterfaceManager.ShowOrHideTempPanel(true);
     //   gamePlayManager.userInterfaceManager.SetTempText(questionString);


    }




    public void MainActionOne()
    {
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, teacherDeskPoint);
        //GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = teacherDeskPoint.rotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        // GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint.rotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopSittingIdleLookAroundAnything(); 
            gamePlayManager.studentsActions[i].scenarioStart = false;
            StartCoroutine(TeleportStudentAndBackToSeat(gamePlayManager.studentsActions[i]));
        }
        StartCoroutine(TriggerMainActionTwo());

    }

    IEnumerator TeleportStudentAndBackToSeat(StudentAction stu)
    {
        
        stu.gameObject.SetActive(false);
        stu.gameObject.transform.position = classEntryPoint.position;
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 2.5f));
        stu.gameObject.SetActive(true);
        stu.GoToAndSitInChair();
        yield return new WaitForSeconds(2f);
    }

    IEnumerator TriggerMainActionTwo()
    {

        yield return new WaitForSeconds(20.0f);
        Debug.Log("Main Action 2");
        yield return null;
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
        }
        string TeacherQuestion = "Guten Morgen alle miteinander.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(TeacherQuestion));
        yield return new WaitForSeconds(8.0f);
        gamePlayManager.StartPhaseFour();
        

    }



    public void MainActionTwo()
    {
        StartCoroutine(TriggerMainActionThree());

    }




    IEnumerator TriggerMainActionThree()
    {
        yield return new WaitForSeconds(1.0f); // timeframe is already covered for nearly 10 seconds

        string TeacherQuestion = "Unsere Klassenfahrt steht ja bevor und deswegen werden wir uns in dieser Stunde mit der Planung des Nachmittagsprogramms beschäftigen. Welche Vorschläge für Aktivitäten habt ihr? Sammelt die Vorschläge auf einem Plakat an den Gruppentischen.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(TeacherQuestion));
        yield return new WaitForSeconds(10.0f);
        gamePlayManager.StartPhaseFive();

    }

    public void MainActionThree()
    {


        
       StartCoroutine(TriggerTeacherQuestionPhase());


        //Teacher Question Panel
       
    }

    IEnumerator TriggerTeacherQuestionPhase() 
    {

        yield return new WaitForSeconds(5.0f);
        gamePlayManager.StartPhaseSix();

    }


    private void StudentReactionOne()
    {
        StartCoroutine(TriggerStudentReactionOne());
    }
    private void StudentReactionTwo()
    {
        StartCoroutine(TriggerStudentReactionTwo());
    }
    private void StudentReactionThree()
    {
        StartCoroutine(TriggerStudentReactionThree());
    }
    private void StudentReactionFour()
    {
        StartCoroutine(TriggerStudentReactionFour());
    }


    IEnumerator TriggerStudentReactionOne()
    {
        //set student mumbling to zero
        audioInClass.volume = 0f;
        audioInClass.Play();

        //enable player to look around and move to support seeing all kids during SRs.
        GameObject.FindGameObjectWithTag("Player").GetComponent<RigidbodyFirstPersonController>().MoveAndLookLocked = false;

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
        }

        //show 15 minute time lapse
        gamePlayManager.SetTimeScaleTextToMinutes();
        gamePlayManager.StartTimer(true);

        yield return new WaitForSeconds(15f);

        gamePlayManager.StopTimer();

        //yield return new WaitForSeconds(5.0f);

        string TeacherQuestion = "Wer möchte seine Idee vorstellen?";
        if (gamePlayManager.LOG_ENABLED) 
            yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(TeacherQuestion));
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            StartCoroutine(SR1Activity(gamePlayManager.studentsActions[i]));
        }
        yield return new WaitForSeconds(5f);

    }

    IEnumerator SR1Activity(StudentAction stu)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 2.5f));
        stu.studentAnimation.MB33_WorkOnSheets(false);
        stu.studentAnimation.RaiseHand(true);
        stu.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
    }


    IEnumerator TriggerStudentReactionTwo()
    {
        //set student mumbling to low
        audioInClass.volume = 0.01f;
        audioInClass.Play();

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            if(gamePlayManager.studentsActions[i] == KidMaxim || gamePlayManager.studentsActions[i] == kidJulian || gamePlayManager.studentsActions[i] == KidLeonie || gamePlayManager.studentsActions[i] == KidSimon)
            {
                gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
            }
            else
            {
                gamePlayManager.studentsActions[i].studentAnimation.VI7_TalkToFriendsLeftAndRight();
            }
        }
        // player camera movement..
        yield return new WaitForSeconds(3.0f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, mainObjectManager.Table1MoveToPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, mainObjectManager.Table1);
        yield return new WaitForSeconds(3.0f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, mainObjectManager.Table2MoveToPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, mainObjectManager.Table2);
        yield return new WaitForSeconds(3.0f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, mainObjectManager.Table3MoveToPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, mainObjectManager.Table3);
        yield return new WaitForSeconds(3.0f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, mainObjectManager.Table4MoveToPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, mainObjectManager.Table4);
        yield return new WaitForSeconds(3.0f);

    }
    IEnumerator TriggerStudentReactionThree()
    {
        //set student mumbling to medium
        audioInClass.volume = 0.03f;
        audioInClass.Play();

        //enable player to look around and move to support seeing all kids during SRs.
        GameObject.FindGameObjectWithTag("Player").GetComponent<RigidbodyFirstPersonController>().MoveAndLookLocked = false;

        yield return new WaitForSeconds(1.0f);

        //Maxim
        StartCoroutine(SR3MaximActivity(gamePlayManager.studentsActions[20]));

        //Other students
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            if (i != 20) {
                gamePlayManager.studentsActions[i].StopLookAtSomeone();
                gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
                gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            }
            
            if (i == 20 ) {
                //Maxim, do nothing - already handled previously
            }
            else if(gamePlayManager.studentsActions[i] == KidLeonie)
            {
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Bad);
                gamePlayManager.studentsActions[i].studentAnimation.MB9_LookAround(true);
            }
            else
            {
                gamePlayManager.studentsActions[i].studentAnimation.VI7_TalkToFriendsLeftAndRight();
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    IEnumerator SR3MaximActivity(StudentAction stu)
    {
        stu.SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(3f);
        stu.studentAnimation.RaiseHand(true);
        //stu.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
    }



    IEnumerator TriggerStudentReactionFour()
    {
        //set student mumbling to medium
        audioInClass.volume = 0.03f;
        audioInClass.Play();

        //enable player to look around and move to support seeing all kids during SRs.
        GameObject.FindGameObjectWithTag("Player").GetComponent<RigidbodyFirstPersonController>().MoveAndLookLocked = false;

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
        }
        KidMaxim.FindMyChair("Chair", 15);
        yield return new WaitForSeconds(1.0f);
        KidMaxim.GoToAndSitInChair();

        for (int i = 0; i < mainObjectManager.studentsAtTable1.Count ; i++)
        {
            mainObjectManager.studentsAtTable1[i].studentAnimation.VI7_TalkToFriendsLeftAndRight();
            //StartCoroutine(StudentMoodWithDelay(mainObjectManager.studentsAtTable1[i], MoodIndicator.Middle,5f) );
        }

        //set student mumbling to high
        audioInClass.volume = 0.08f;
        audioInClass.Play();

        for (int i = 0; i < mainObjectManager.studentsAtTable2.Count; i++)
        {
            mainObjectManager.studentsAtTable2[i].studentAnimation.MB33_WorkOnSheets(true);
            yield return new WaitForSeconds(0.5f);
            //StartCoroutine(StudentMoodWithDelay(mainObjectManager.studentsAtTable2[i], MoodIndicator.Bad, 5f));
        }
        for (int i = 0; i < mainObjectManager.studentsAtTable3.Count; i++)
        {
            mainObjectManager.studentsAtTable3[i].studentAnimation.MB33_WorkOnSheets(true);
            yield return new WaitForSeconds(0.5f);
            //StartCoroutine(StudentMoodWithDelay(mainObjectManager.studentsAtTable3[i], MoodIndicator.Middle, 10f));
        }
        for (int i = 0; i < mainObjectManager.studentsAtTable4.Count; i++)
        {
            mainObjectManager.studentsAtTable4[i].studentAnimation.MB33_WorkOnSheets(true);
            yield return new WaitForSeconds(0.5f);
            //StartCoroutine(StudentMoodWithDelay(mainObjectManager.studentsAtTable4[i], MoodIndicator.Middle, 10f));
        }
        yield return new WaitForSeconds(1.0f);
        //audioInClass.volume = 0.05f;
        //yield return new WaitForSeconds(6.0f);
        //audioInClass.volume = 0.08f;

        //yield return new WaitUntil((() => KidMaxim.reachedSpot));
        KidMaxim.SetMyMood(MoodIndicator.Good);
        KidMaxim.studentAnimation.ResetAllAnim();
        KidMaxim.studentAnimation.VI7_TalkToFriendsLeftAndRight();

        for (int i = 0; i < mainObjectManager.studentsAtTable1.Count; i++) {
            mainObjectManager.studentsAtTable1[i].SetMyMood(MoodIndicator.Middle);
        }

        //audioInClass.volume = 0.08f;

        yield return new WaitForSeconds(3.0f);
        audioInClass.volume = 0.14f;

        for (int i = 0; i < mainObjectManager.studentsAtTable1.Count; i++) {
            mainObjectManager.studentsAtTable1[i].SetMyMood(MoodIndicator.Bad);
        }

        //nearby students get upset at the increased noise
        for (int i = 0; i < mainObjectManager.studentsAtTable2.Count; i++) {
            mainObjectManager.studentsAtTable2[i].SetMyMood(MoodIndicator.Middle);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.9f));
            mainObjectManager.studentsAtTable2[i].SetMyMood(MoodIndicator.Bad);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1.5f));
            StartCoroutine(GetUpset(mainObjectManager.studentsAtTable2[i]));
        }
        for (int i = 0; i < mainObjectManager.studentsAtTable3.Count; i++) {
            mainObjectManager.studentsAtTable3[i].SetMyMood(MoodIndicator.Middle);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.9f));
            mainObjectManager.studentsAtTable3[i].SetMyMood(MoodIndicator.Bad);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1.5f));
            StartCoroutine(GetUpset(mainObjectManager.studentsAtTable3[i]));
        }
        for (int i = 0; i < mainObjectManager.studentsAtTable4.Count; i++) {
            mainObjectManager.studentsAtTable4[i].SetMyMood(MoodIndicator.Middle);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.9f));
            mainObjectManager.studentsAtTable4[i].SetMyMood(MoodIndicator.Bad);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1.5f));
            StartCoroutine(GetUpset(mainObjectManager.studentsAtTable4[i]));
        }

        

        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, mainObjectManager.Table1MoveToPoint);
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, mainObjectManager.Table1);


    }

    IEnumerator GetUpset(StudentAction stu) {
        stu.StopLookAtSomeone();
        stu.StopMyRandomLookingAnimations();
        stu.studentAnimation.ResetAllAnim();

        stu.studentAnimation.MB19_Protest(true);
        yield return new WaitForSeconds(3.0f);

        stu.StopLookAtSomeone();
        stu.StopMyRandomLookingAnimations();
        stu.studentAnimation.ResetAllAnim();

        stu.studentAnimation.MB33_WorkOnSheets(true);

    }

    IEnumerator StudentMoodWithDelay(StudentAction stu, MoodIndicator mood, float delay)
    {
        yield return new WaitForSeconds(delay);
        stu.SetMyMood(mood);
    }


    public void Reset()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint.rotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);

        audioInClass.volume = 0.01f;
        KidMaxim.FindMyChair("Chair", 20);
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
            StartCoroutine(StudentsBackToSeat(gamePlayManager.studentsActions[i]));
        }

        gamePlayManager.StartPhaseSix();


    }
    IEnumerator StudentsBackToSeat(StudentAction stu)
    {
        stu.gameObject.SetActive(false);
        stu.gameObject.transform.position = stu.chairPoint.position;
        stu.gameObject.SetActive(true);
        stu.GoToAndSitInChair();
        yield return new WaitForSeconds(2f);
    }
}
