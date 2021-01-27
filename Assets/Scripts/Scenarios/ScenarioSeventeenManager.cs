using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;

public class ScenarioSeventeenManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    public Transform tableOnePoint;
    public Transform tableFourPoint;
    public AudioSource audioInClass;

    string questionString = "";

    public Animator pencilAnim;
    public Animator pencilCaseAnim;

    public GameObject inHandPencil, inLeftHandPencil;
    public GameObject inHandPencil_Tom;

    public StudentAction KidJannik;
    public StudentAction KidSimon;
    public StudentAction[] StudentsInPair;
    public Transform newChair;


    private void Awake()
    {
        gamePlayManager.currentScenario = "SC17";
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
                questionString = "Sie bitten Janniks Nachbarn, diesem sein Vorgehen bei der Internetrecherche zu erklären und mit ihm gemeinsam die Fragen zu beantworten.";

                StudentReactionOne();
                break;
            case "2":
                questionString = "Sie wiederholen mit Jannik und seinem Nachbarn gemeinsam die Schritte zur Internetrecherche zu der ersten Frage.";

                StudentReactionTwo();
                break;
            case "3":
                questionString = "Sie bitten Janniks Nachbarn, mit Simon, der gerade alleine arbeitet, weiter zu arbeiten. Mit Jannik gehen Sie Schritt für Schritt die Internetrecherche zu der ersten Frage durch.";

                StudentReactionThree();
                break;
            case "4":
                questionString = "Sie bitten Janniks Nachbarn, Jannik jetzt auch mal an dem Tablet arbeiten zu lassen.";
                StudentReactionFour();

                break;
        }

        //showTempText();
        gamePlayManager.playerActionDataHandler.PlayerAction = questionString;
        gamePlayManager.StartPhaseSeven(20.0f);

    }

    void showTempText() {

     //   gamePlayManager.userInterfaceManager.ShowOrHideTempPanel(true);
     //   gamePlayManager.userInterfaceManager.SetTempText(questionString);


    }

    // SAc: Edit, We have removed Julia from the group of kids as she is not in the 9 year old list as per configuration, so all the index numbers will have to be modified as per this change where it is needed



    public void MainActionOne()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, tableOnePoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
        //GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = tableOnePoint.rotation;        
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayerToOriginalPostion(true);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].scenarioStart = false;
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();   
            
        }
        for (int i = 0; i < StudentsInPair.Length; i++)
        {
            StudentsInPair[i].chairPoint.gameObject.GetComponent<ChairDetails>().gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.SetActive(true);
            if (StudentsInPair[i] != KidJannik)
            {
                if (i < 3)
                    StartCoroutine(AllStudentsRandom4Activity(StudentsInPair[i]));
                else
                    StartCoroutine(AllStudentsActivity(StudentsInPair[i]));
            }
            else
            {
                StartCoroutine(JannikActivity(StudentsInPair[i]));
                StartCoroutine(JannikNeighbourActivity(StudentsInPair[i].myNeighbourStudent));
                
            }
        }

        
        StartCoroutine(TriggerMainActionOne());
    }
    private IEnumerator AllStudentsRandom4Activity(StudentAction stu)
    {
        yield return new WaitForSeconds(Random.Range(1.0f, 4.0f));
        stu.LookAtSomeone(stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        if (stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.LookAtSomeone(stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        yield return new WaitForSeconds(3.0f);
        stu.studentAnimation.MB33_WorkOnSheets(true);
        if (stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(4.0f);
        stu.studentAnimation.MB33_WorkOnSheets(false);
        if (stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(false);

        //yield return new WaitForSeconds(Random.Range(1.0f, 4.0f));
        stu.studentAnimation.VI11_TalkToFriendsLeftAndRight();
        if (stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.studentAnimation.VI11_TalkToFriendsLeftAndRight();
        yield return new WaitForSeconds(4.0f);
        stu.studentAnimation.VI11_TalkToFriendsStop();
        if (stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.studentAnimation.VI11_TalkToFriendsStop();

        //yield return new WaitForSeconds(Random.Range(1.0f, 4.0f));
        stu.LookAtSomeone(stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        if (stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.LookAtSomeone(stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        yield return new WaitForSeconds(3.0f);
        stu.studentAnimation.MB33_WorkOnSheets(true);
        if (stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(4.0f);
        stu.studentAnimation.MB33_WorkOnSheets(false);
        if (stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(false);
        yield return null;

        //StartCoroutine(TriggerMainActionTwo());
    }
    private IEnumerator AllStudentsActivity(StudentAction stu)
    {
        yield return new WaitForSeconds(Random.Range(1.0f, 4.0f));
        stu.LookAtSomeone(stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        if(stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.LookAtSomeone(stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        yield return new WaitForSeconds(3.0f);
        stu.studentAnimation.MB33_WorkOnSheets(true);
        if (stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(4.0f);
        stu.studentAnimation.MB33_WorkOnSheets(false);
        if (stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(false);

        //yield return new WaitForSeconds(Random.Range(1.0f, 4.0f));
        stu.LookAtSomeone(stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        if (stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.LookAtSomeone(stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        yield return new WaitForSeconds(3.0f);
        stu.studentAnimation.MB33_WorkOnSheets(true);
        if (stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(4.0f);
        stu.studentAnimation.MB33_WorkOnSheets(false);
        if (stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(false);

        //yield return new WaitForSeconds(Random.Range(1.0f, 4.0f));
        stu.LookAtSomeone(stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        if (stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.LookAtSomeone(stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        yield return new WaitForSeconds(3.0f);
        stu.studentAnimation.MB33_WorkOnSheets(true);
        if (stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(4.0f);
        stu.studentAnimation.MB33_WorkOnSheets(false);
        if (stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(false);
        yield return null;

        //StartCoroutine(TriggerMainActionTwo());
    }
    private IEnumerator JannikNeighbourActivity(StudentAction stu)
    {
        yield return new WaitForSeconds(Random.Range(1.0f, 4.0f));
        stu.LookAtSomeone(stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        yield return new WaitForSeconds(3.0f);
        stu.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(4.0f);
        stu.studentAnimation.MB33_WorkOnSheets(false);

        //yield return new WaitForSeconds(Random.Range(1.0f, 4.0f));
        stu.LookAtSomeone(stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        yield return new WaitForSeconds(3.0f);
        stu.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(4.0f);
        stu.studentAnimation.MB33_WorkOnSheets(false);

        //yield return new WaitForSeconds(Random.Range(1.0f, 4.0f));
        stu.LookAtSomeone(stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        yield return new WaitForSeconds(3.0f);
        stu.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(4.0f);
        stu.studentAnimation.MB33_WorkOnSheets(false);
    }
    private IEnumerator JannikActivity(StudentAction stu)
    {
        yield return null;
        stu.SetMyMood(MoodIndicator.Bad);
        stu.studentAnimation.MB9_LookAround(true);
        if (stu.myNeighbourStudent != null)
            stu.myNeighbourStudent.LookAtSomeone(stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
    }



    private IEnumerator TriggerMainActionOne()
    {
        yield return null;

        StartCoroutine(TriggerMainActionTwo());
    }

    IEnumerator TriggerMainActionTwo()
    {
        yield return null;
        
        yield return new WaitForSeconds(8.0f);
        gamePlayManager.StartPhaseFour();
        

    }



    public void MainActionTwo()
    {
        StartCoroutine(TriggerMainActionThree());
    }




    IEnumerator TriggerMainActionThree()
    {
        yield return new WaitForSeconds(10.0f);
        gamePlayManager.StartPhaseFive();


    }

    public void MainActionThree()
    {
       StartCoroutine(TriggerTeacherQuestionPhase());
    }

    IEnumerator TriggerTeacherQuestionPhase() 
    {
        yield return new WaitForSeconds(10.0f);
        gamePlayManager.StartPhaseSix();
    }


    private void StudentReactionOne()
    {
        StartCoroutine(TriggerStudentReactionOne());
        
    }
    IEnumerator TriggerStudentReactionOne()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, tableFourPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);

        //Set Jannik's mood to Bad
        KidJannik.SetMyMood(MoodIndicator.Bad);

        for (int i = 0; i < StudentsInPair.Length; i++)
        {
            if (StudentsInPair[i] != KidJannik)
            {
                StartCoroutine(AllStudentsActivity(StudentsInPair[i]));
            }
        }
        

        KidJannik.myNeighbourStudent.StopLookAtSomeone();
        KidJannik.myNeighbourStudent.StopMyRandomLookingAnimations();
        KidJannik.myNeighbourStudent.studentAnimation.ResetAllAnim();

        yield return new WaitForSeconds(1f);
        KidJannik.LookAtNeighbourRoutine();
        KidJannik.myNeighbourStudent.studentAnimation.VI11_TalkToFriendsLeftAndRight();
        yield return new WaitForSeconds(4f);
        KidJannik.myNeighbourStudent.studentAnimation.VI11_TalkToFriendsStop();

        //Jannik's mood turns Good after neighbor stops talking
        KidJannik.SetMyMood(MoodIndicator.Good);

        KidJannik.LookAtNeighbourRoutineStop();
        KidJannik.LookAtSomeone(KidJannik.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        KidJannik.myNeighbourStudent.LookAtSomeone(KidJannik.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        yield return new WaitForSeconds(3f);
        KidJannik.studentAnimation.MB33_WorkOnSheets(true);
        KidJannik.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(true);
    }


    private void StudentReactionTwo()
    {
        StartCoroutine(TriggerStudentReactionTwo());
    }

    IEnumerator TriggerStudentReactionTwo()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, tableFourPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);

        //Set Jannik's mood to Bad; set his neighbor's to Middle
        KidJannik.SetMyMood(MoodIndicator.Good);
        yield return new WaitForSeconds(2f);

        KidJannik.myNeighbourStudent.SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < StudentsInPair.Length; i++) {
            if (StudentsInPair[i] != KidJannik) {
                StartCoroutine(AllStudentsActivity(StudentsInPair[i]));
            }
        }

        //KidJannik.SetMyMood(MoodIndicator.Good);
        //KidJannik.myNeighbourStudent.SetMyMood(MoodIndicator.Middle);

        yield return new WaitForSeconds(0.5f);

        KidJannik.LookAtSomeone(KidJannik.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        KidJannik.myNeighbourStudent.LookAtSomeone(KidJannik.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        yield return new WaitForSeconds(8f);

        KidJannik.studentAnimation.MB9_LookAround(true);
        KidJannik.myNeighbourStudent.studentAnimation.MB9_LookAround(true);
        yield return new WaitForSeconds(8f);

        KidJannik.studentAnimation.MB33_WorkOnSheets(true);
        KidJannik.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(true);
        KidJannik.myNeighbourStudent.SetMyMood(MoodIndicator.Good);

        yield return new WaitForSeconds(5f);
    }



    private void StudentReactionThree()
    {
        StartCoroutine(TriggerStudentReactionThree());
    }

    IEnumerator TriggerStudentReactionThree()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, tableFourPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);

        //Set Jannik's mood to Bad; set his neighbor's to Middle
        KidJannik.SetMyMood(MoodIndicator.Middle);

        for (int i = 0; i < StudentsInPair.Length; i++) {
            if (StudentsInPair[i] != KidJannik) {
                StartCoroutine(AllStudentsActivity(StudentsInPair[i]));
            }
        }

        //KidJannik.SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(0.5f);
        //KidJannik.myNeighbourStudent.ChairNumber = 6;

        KidJannik.myNeighbourStudent.FindMyChair("Chair", 6);
        KidJannik.myNeighbourStudent.GoToAndSitInChair();
        KidJannik.myNeighbourStudent.SetMyMood(MoodIndicator.Middle);

        KidJannik.LookAtSomeone(KidJannik.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        KidSimon.LookAtSomeone(KidSimon.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        KidJannik.myNeighbourStudent.LookAtSomeone(KidSimon.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        yield return new WaitForSeconds(5f);
        KidJannik.studentAnimation.MB33_WorkOnSheets(true);
        KidJannik.SetMyMood(MoodIndicator.Good);

        KidSimon.studentAnimation.MB33_WorkOnSheets(true);
        KidSimon.SetMyMood(MoodIndicator.Bad);
        KidJannik.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(3f);

        KidSimon.LookAtSomeone(KidSimon.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        KidJannik.myNeighbourStudent.LookAtSomeone(KidSimon.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        yield return new WaitForSeconds(3f);
        KidSimon.studentAnimation.MB33_WorkOnSheets(true);
        KidJannik.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(3f);

    }

    private void StudentReactionFour()
    {
        StartCoroutine(TriggerStudentReactionFour());
    }

    IEnumerator TriggerStudentReactionFour()
    {
        //Jannik's mood starts out Bad; his neighbor's is Good
        KidJannik.SetMyMood(MoodIndicator.Bad);
        KidJannik.myNeighbourStudent.SetMyMood(MoodIndicator.Good);
        yield return new WaitForSeconds(2f);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, tableFourPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);

        //other students in the class engage in work
        for (int i = 0; i<StudentsInPair.Length; i++) {
            if (StudentsInPair[i] != KidJannik) {
                StartCoroutine(AllStudentsActivity(StudentsInPair[i]));
}
        }
        
        //Jannik begins working with the tablet for 5 seconds while his neighbor watches, during which, Jannik's mood turns Good and his neighbor's mood turns Bad.
        KidJannik.LookAtSomeone(KidJannik.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        KidJannik.myNeighbourStudent.LookAtSomeone(KidJannik.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        KidJannik.SetMyMood(MoodIndicator.Good);
        KidJannik.myNeighbourStudent.SetMyMood(MoodIndicator.Bad);
        yield return new WaitForSeconds(5f);

        //KidJannik.studentAnimation.MB9_LookAround(true);
        
        //Jannik looks away and begins to protest; his mood turns Bad
        KidJannik.StopLookAtSomeone();
        KidJannik.LookAtSomeone(GameObject.Find("SpotAtCuboardNearWindow").transform);
        KidJannik.studentAnimation.MB19_Protest(true);
        KidJannik.SetMyMood(MoodIndicator.Bad);

        //Jannik's neighbor works with the tablet; his mood turns Middle
        KidJannik.myNeighbourStudent.LookAtSomeone(KidJannik.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        KidJannik.myNeighbourStudent.SetMyMood(MoodIndicator.Middle);
        
        yield return new WaitForSeconds(5f);
    }
    


    public void Reset() // gets called when Player presses No
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint.rotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);

        StartCoroutine(TriggerStudentResetPos());


        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
        }
        gamePlayManager.StartPhaseSix();
    }
    IEnumerator TriggerStudentResetPos()
    {
        //KidJannik.myNeighbourStudent.ChairNumber = 4;
        KidJannik.myNeighbourStudent.FindMyChair("Chair", 4);
        KidJannik.myNeighbourStudent.GoToAndSitInChair();   // hard reset position
        yield return new WaitUntil((() => KidJannik.myNeighbourStudent.reachedSpot));
    }
}
