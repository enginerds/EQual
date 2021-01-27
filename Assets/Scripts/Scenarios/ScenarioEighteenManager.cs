using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioEighteenManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    public Transform tableOnePoint;
    public AudioSource audioInClass;
    public AudioSource schoolBellAudio;

    string questionString = "";


    public GameObject TeacherDesk;
    public Transform door;
    public StudentAction kidLeonie, kidJannik,kidMaxim;
    public StudentAction[] randomKids = new StudentAction[6];
    private StudentAction[] NineRandomKids;
    public GameObject[] studentPencil;

    public Transform MainActionTable1WalkToPoint;
    private int SR1_StudentsProcessed = 0;
    private bool SR1_Done = false;

    private void Awake()
    {
        gamePlayManager.currentScenario = "SC19";
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
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint.rotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
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

        switch (value)
        {

            case "1":
                questionString = "Leonie (SFB ESE) ein Wartesignal geben und Klasse bitten, in zwei Minuten einen Abschluss zu finden. Kinder, die bereits fertig sind, auffordern, die Plakate zu holen. Anschließend zu Leonie gehen und ihre Frage klären. Jannik (SFB LE) bei Bedarf unterstützen.";

                StudentReactionOne();
                break;
            case "2":
                questionString = "Leonie (SFB ESE) ein Wartesignal geben und Klasse bitten, in zwei Minuten einen Abschluss zu finden. Kinder, die bereits fertig sind, auffordern, sich leise am Platz zu beschäftigen. Anschließend zu Leonie gehen und ihre Frage klären.  Jannik (SFB LE) bei Bedarf unterstützen.";

                StudentReactionTwo();
                break;
            case "3":
                questionString = "Zu Leonie (SFB ESE) gehen und ihre Frage klären. Währenddessen alle Kinder bitten, pro Paar ein Plakat abzuholen und ab jetzt ihre Ergebnisse mit dem Nachbarkind auszutauschen. Jannik (SFB LE) und sein Partnerkind unterstützen.";

                StudentReactionThree();
                break;
            case "4":
                questionString = "Leonies (SFB ESE) Frage im Plenum klären. Die geplante Arbeitszeit um 5 Minuten ausdehnen, um allen Kindern die Möglichkeit zu geben, die Aufgaben zu beenden. Den Kindern, die bereits fertig sind, eine weitere kurze Aufgabe geben.";
                StudentReactionFour();

                break;
        }

        //showTempText();
        gamePlayManager.playerActionDataHandler.PlayerAction = questionString;

        if (value == "1") {
            
            for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {

                if (gamePlayManager.studentsActions[i] != kidLeonie && gamePlayManager.studentsActions[i] != kidJannik &&
                    gamePlayManager.studentsActions[i] != randomKids[0] &&
                    gamePlayManager.studentsActions[i] != randomKids[1] &&
                    gamePlayManager.studentsActions[i] != randomKids[2] &&
                    gamePlayManager.studentsActions[i] != randomKids[3] &&
                    gamePlayManager.studentsActions[i] != randomKids[4] &&
                    gamePlayManager.studentsActions[i] != randomKids[5]) // add randoms kids too
                {
                    //do nothing
                } else {
                    //remove worksheet paper from students' hands
                    gamePlayManager.studentsActions[i].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, false);
                }
            }
                
            //have longer delay before showing pop-up
            gamePlayManager.StartPhaseSeven(35.0f);

        } else {
            gamePlayManager.StartPhaseSeven(20.0f);
        }
        

    }

    void showTempText() 
    {
     //   gamePlayManager.userInterfaceManager.ShowOrHideTempPanel(true);
     //   gamePlayManager.userInterfaceManager.SetTempText(questionString);
    }



    public void MainActionOne()
    {
        // all students starts reading and continues for 10 seconds
        Debug.Log("Main Action 1 starts");
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].scenarioStart = false;
            StartCoroutine(StudentWriteOnSheet(gamePlayManager.studentsActions[i]));
        }

        StartCoroutine(TriggerMainActionTwo());
    }
    IEnumerator StudentWriteOnSheet(StudentAction stu)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1f));
        stu.studentAnimation.ResetAllAnim();
        stu.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(3.0f);
        stu.studentAnimation.MB33_WorkOnSheets(true);
    }

    IEnumerator TriggerMainActionTwo()
    {

        yield return new WaitForSeconds(10f);

        Debug.Log("Main Action 2 starts");
        audioInClass.volume = 0.02f;

        //position player to have better view of Table 1
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, MainActionTable1WalkToPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint.rotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1);


        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) // everyone except leonie and jannik
        {
            if (gamePlayManager.studentsActions[i] != kidLeonie && 
                gamePlayManager.studentsActions[i] != kidJannik &&
                gamePlayManager.studentsActions[i] != randomKids[0] &&
                gamePlayManager.studentsActions[i] != randomKids[1] &&
                gamePlayManager.studentsActions[i] != randomKids[2] &&
                gamePlayManager.studentsActions[i] != randomKids[3] &&
                gamePlayManager.studentsActions[i] != randomKids[4] &&
                gamePlayManager.studentsActions[i] != randomKids[5] ) 
            {
                gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
                StartCoroutine(StudentsDropPencils(gamePlayManager.studentsActions[i], i));
            }
            
        }
        for (int i = 0; i < randomKids.Length; i++) // everyone except leonie and jannik
        {
            randomKids[i].studentAnimation.ResetAllAnim();
            randomKids[i].studentAnimation.MB33_WorkOnSheets(true);
        }
        kidLeonie.studentAnimation.ResetAllAnim();
        kidJannik.studentAnimation.ResetAllAnim();
        kidLeonie.studentAnimation.MB33_WorkOnSheets(true);
        kidJannik.studentAnimation.MB33_WorkOnSheets(true);

        yield return new WaitForSeconds(10.0f); //adjust this delay
        gamePlayManager.StartPhaseFour();
        

    }
    // kidMaxim.GetComponent<StudentAction>().chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform
    
    IEnumerator StudentsDropPencils(StudentAction stu, int i)
    {
        //stu.GetComponent<StudentAction>().chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Pencil.GetComponent<Animator>().SetBool("isThrow", true);
        //pencilAnim.SetBool("isThrow", true);
        //studentPencil[i].GetComponent<Animator>().SetBool("isThrow", true); // add student throw animation
        studentPencil[i].GetComponent<Rigidbody>().AddForce(Vector3.forward *  - 300f);
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1f));
        stu.studentAnimation.MB9_LookAround(true);
        // start looking around animation
        yield return null;
    }

    

    public void MainActionTwo()
    {
        StartCoroutine(TriggerMainActionThree());

    }

    //public student



    IEnumerator TriggerMainActionThree()
    {
        // random kids will continue with mb33
        // remaining 9 kids will do talk animation
        // leonie will raise hand
        //jannik will continue with mb33



        //yield return new WaitForSeconds(10.0f);
        Debug.Log("main action 3 starts");

        audioInClass.volume = 0.05f;

        

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) // everyone except leonie and jannik
        {
            //if (gamePlayManager.studentsActions[i] != kidLeonie && gamePlayManager.studentsActions[i] != kidJannik &&
            //    gamePlayManager.studentsActions[i] != randomKids[0] &&
            //    gamePlayManager.studentsActions[i] != randomKids[1] &&
            //    gamePlayManager.studentsActions[i] != randomKids[2] &&
            //    gamePlayManager.studentsActions[i] != randomKids[3] &&
            //    gamePlayManager.studentsActions[i] != randomKids[4] &&
            //    gamePlayManager.studentsActions[i] != randomKids[5]) 
            //{
            //    gamePlayManager.studentsActions[i].studentAnimation.MB9_LookAround(false);
            //    gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
            //    StartCoroutine(TriggerStudentTalkToFriends(gamePlayManager.studentsActions[i]));
            //}

            if (gamePlayManager.studentsActions[i] != kidLeonie && gamePlayManager.studentsActions[i] != kidJannik &&
                gamePlayManager.studentsActions[i] != randomKids[0] &&
                gamePlayManager.studentsActions[i] != randomKids[1] &&
                gamePlayManager.studentsActions[i] != randomKids[2] &&
                gamePlayManager.studentsActions[i] != randomKids[3]) 
            {
                gamePlayManager.studentsActions[i].studentAnimation.MB9_LookAround(false);
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
                StartCoroutine(TriggerStudentTalkToFriends(gamePlayManager.studentsActions[i]));
            }

        }
        kidLeonie.studentAnimation.ResetAllAnim();
        yield return new WaitForSeconds(0.5f);
        kidLeonie.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
        kidLeonie.studentAnimation.RaiseHand(true);
        yield return new WaitForSeconds(4.0f); // edit delay after testing

        //return player position to front of class
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint.rotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);

        yield return new WaitForSeconds(4.0f);


        gamePlayManager.StartPhaseFive();


    }

    IEnumerator TriggerStudentTalkToFriends(StudentAction stu)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.5f));
        stu.studentAnimation.VI11_TalkToFriendsLeftAndRight();
        
    }



    public void MainActionThree()
    {
        
       StartCoroutine(TriggerTeacherQuestionPhase());

        //Teacher Question Panel
       
    }

    IEnumerator TriggerTeacherQuestionPhase() 
    {
        yield return new WaitForSeconds(1.0f);
        audioInClass.Stop();
        gamePlayManager.StartPhaseSix();
    }


    private void StudentReactionOne()
    {
        StartCoroutine(TriggerStudentReactionOne());
    }
    IEnumerator TriggerStudentReactionOne()
    {
        audioInClass.volume = 0f;

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {

            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);

            if (gamePlayManager.studentsActions[i] != kidJannik &&
                gamePlayManager.studentsActions[i] != randomKids[0] &&
                gamePlayManager.studentsActions[i] != randomKids[1] &&
                gamePlayManager.studentsActions[i] != randomKids[2] &&
                gamePlayManager.studentsActions[i] != randomKids[3] &&
                gamePlayManager.studentsActions[i] != randomKids[4] &&
                gamePlayManager.studentsActions[i] != randomKids[5]) // add randoms kids too
            {
                //if(i < 9)
                //NineRandomKids[i] = gamePlayManager.studentsActions[i];
                gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
                gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
            }
            else
            {
                StartCoroutine(SR1WalkTowardsFrontDesk(gamePlayManager.studentsActions[i]));
            }

        }


        yield return new WaitForSeconds(10f);
        //yield return new WaitUntil(() => SR1_Done = true);

    }

    IEnumerator SR1WalkTowardsFrontDesk(StudentAction stu)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1.5f));
        stu.studentAnimation.ResetAllAnim();
        stu.InitiateGoToSpot(TeacherDesk.transform);
        yield return new WaitUntil((() => stu.reachedSpot));
        stu.studentAnimation.TakeItem(true);
        yield return new WaitForSeconds(1f);
        stu.studentAnimation.TakeItem(false);
        stu.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        stu.GoToAndSitInChair();
        yield return new WaitUntil((() => stu.reachedSpot));
        
        stu.studentAnimation.MB30_PeepToSideLeftOrRight(true);
        
    }

    


    private void StudentReactionTwo()
    {
        StartCoroutine(TriggerStudentReactionTwo());

    }
    IEnumerator TriggerStudentReactionTwo() {

        audioInClass.volume = 0.02f;    //audio = low
        audioInClass.Play();

        //initialize moods to Good for kids not working
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            if (gamePlayManager.studentsActions[i] != kidLeonie && gamePlayManager.studentsActions[i] != kidJannik &&
                gamePlayManager.studentsActions[i] != randomKids[0] &&
                gamePlayManager.studentsActions[i] != randomKids[1]) 
            {
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
            }
        }

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            if (gamePlayManager.studentsActions[i] != kidLeonie && gamePlayManager.studentsActions[i] != kidJannik &&
                gamePlayManager.studentsActions[i] != randomKids[0] &&
                gamePlayManager.studentsActions[i] != randomKids[1]) 
            {
                gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
                StartCoroutine(SRTwoLoop(gamePlayManager.studentsActions[i]));
            }
        }
    }

    IEnumerator SRTwoLoop(StudentAction stu)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.5f));
        stu.studentAnimation.MB9_LookAround(true);
        stu.SetMyMood(MoodIndicator.Middle);
    }

    private void StudentReactionThree()
    {
        StartCoroutine(TriggerStudentReactionThree());
    }
    IEnumerator TriggerStudentReactionThree()
    {

        audioInClass.volume = 0.05f;  //audio = middle
        audioInClass.Play();

        //initialize moods to Good for kids not working
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            if (gamePlayManager.studentsActions[i] != kidMaxim) {
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
            } else {
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
            }
        }

        yield return new WaitForSeconds(1f);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1MoveToPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, kidLeonie.gameObject.transform);
        
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            if (gamePlayManager.studentsActions[i] != kidMaxim) {
                StartCoroutine(LeanToStudent(gamePlayManager.studentsActions[i]));
            }
        }
        
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1MoveToPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().Table1MoveToPoint.rotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1);

        yield return new WaitForSeconds(2f);
        kidMaxim.SetMyMood(MoodIndicator.Bad);
        
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            //change everyone's mood except Maxim's, which has already been set to Bad
            if (gamePlayManager.studentsActions[i] != kidMaxim) {
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
            }
        }

    }

    IEnumerator LeanToStudent(StudentAction s) {
        s.studentAnimation.ResetAllAnim();
        
        s.studentAnimation.MB30_PeepToSideLeftOrRight(true);
        yield return new WaitForSeconds(4.5f);
        s.studentAnimation.MB30_PeepToSideLeftOrRight(false);
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.8f, 2f));

        s.studentAnimation.MB30_PeepToSideLeftOrRight(true);
        yield return new WaitForSeconds(4.5f);
        s.studentAnimation.MB30_PeepToSideLeftOrRight(false);
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.8f, 2f));

        s.studentAnimation.MB30_PeepToSideLeftOrRight(true);
        yield return new WaitForSeconds(4.5f);
        s.studentAnimation.MB30_PeepToSideLeftOrRight(false);
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.8f, 2f));

        s.SetMyMood(MoodIndicator.Middle);

    }

    private void StudentReactionFour()
    {
        StartCoroutine(TriggerStudentReactionFour());
    }
    IEnumerator TriggerStudentReactionFour()
    {
        audioInClass.volume = 0.2f;    //audio = high
        audioInClass.Play();

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) // everyone 
        {
            gamePlayManager.studentsActions[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);

        }
        yield return new WaitForSeconds(4f);
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) // everyone 
        {
            if (i < 5)
            {
                gamePlayManager.studentsActions[i].studentAnimation.MB9_LookAround(true);
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Bad);
            }
            else if (i < 13)
            {
                gamePlayManager.studentsActions[i].studentAnimation.MB30_PeepToSideLeftOrRight(true);
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Bad);
            }
            else
            {
                gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
                gamePlayManager.studentsActions[i].StartLookingAtTabletAndWrittingAnimations();
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
            }

        }
        

    }





    public void Reset()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint.rotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);

        // reset audio volume
        audioInClass.volume = 0.02f;

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) // everyone 
        {
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
            StartCoroutine(StudentsBackToSeat(gamePlayManager.studentsActions[i]));

        }
        // reset all anim
        // reset all mood
        // reset player position

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) // everyone except leonie and jannik
        {
            if (gamePlayManager.studentsActions[i] != kidLeonie && gamePlayManager.studentsActions[i] != kidJannik &&
                gamePlayManager.studentsActions[i] != randomKids[0] &&
                gamePlayManager.studentsActions[i] != randomKids[1] &&
                gamePlayManager.studentsActions[i] != randomKids[2] &&
                gamePlayManager.studentsActions[i] != randomKids[3] &&
                gamePlayManager.studentsActions[i] != randomKids[4] &&
                gamePlayManager.studentsActions[i] != randomKids[5])
            {
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
            }

        }


        gamePlayManager.StartPhaseSix();
    }



    IEnumerator StudentsBackToSeat(StudentAction stu)
    {
        //stu.InitiateGoToSpot(stu.chairPoint,5f);
        //yield return new WaitForSeconds(5f);
        //stu.studentNavMesh.ResetNavigationSpeed();
        stu.gameObject.SetActive(false);
        stu.gameObject.transform.position = stu.chairPoint.position;
        stu.gameObject.SetActive(true);
        stu.GoToAndSitInChair();
        yield return new WaitForSeconds(2f);
    }
}
