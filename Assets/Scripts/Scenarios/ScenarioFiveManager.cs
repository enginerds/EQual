using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class ScenarioFiveManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    public Transform tableOnePoint;
    public AudioSource audioInClass; 
    public AudioSource teacherMumblingAudio; 
    string questionString = "";

    public GameObject teacherPopUpPanel;
    public GameObject SR_PopUpPanel;

    public List<StudentAction> SNStudents;
    public List<StudentAction> HighAchievingStudents;
    public List<StudentAction> RandomStudents;
    public List<StudentAction> SR3_RandomStudentList13;
    public StudentAction kidJannik;

    private void Awake()
    {
        gamePlayManager.currentScenario = "SC5";
    }
        private void Start()
    {
        gamePlayManager.initialActionForScenarioIsCommon = true;
        gamePlayManager.userInterfaceManager.ShowOrHideInitLoadingPanel(true);
        StartCoroutine(StartTheScene());
    }

    private void GetHighAchievingStudentList()
    {
        /*
        // students select the Material based on their ability
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            StudentData studentData = gamePlayManager.dataManager.GetStudentData(gamePlayManager.studentsActions[i].name);
            if (studentData.LevelOfAchiement == "High")
            {
                HighAchievingStudents.Add(gamePlayManager.studentsActions[i]);
            }

        }*/

        HighAchievingStudents.Add(gamePlayManager.studentsActions[18]);
        HighAchievingStudents.Add(gamePlayManager.studentsActions[14]);
        HighAchievingStudents.Add(gamePlayManager.studentsActions[1]);
        HighAchievingStudents.Add(gamePlayManager.studentsActions[17]);
        HighAchievingStudents.Add(gamePlayManager.studentsActions[15]);
        HighAchievingStudents.Add(gamePlayManager.studentsActions[19]);

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
                questionString = "Die Kinder, die noch mehr als eine Aufgabe bearbeiten müssen, auffordern mit der wichtigsten Aufgabe (nach den von Ihnen gesetzten Prioritäten) zu beginnen. Bei den Kindern mit sonderpädagogischem Förderbedarf individuell Bearbeitungsreihenfolgen vorgeben.";

                StudentReactionOne();
                break;
            case "2":
                questionString = "Die Kinder fragen, welche Aufgaben ihnen noch fehlen, und dann eine Bearbeitungsreihenfolge danach festlegen, welche Aufgaben den meisten Kindern noch fehlen. Jannik (SFB LE) nur bei Bedarf unterstützen.";

                StudentReactionTwo();
                break;
            case "3":
                questionString = "Die Kinder fragen, welche Aufgaben ihnen noch fehlen, und für alle dann individuell Bearbeitungsreihenfolgen vorgeben. Jannik (SFB LE) unterstützen.";

                StudentReactionThree();
                break;
            case "4":
                questionString = "Die Kinder auffordern, mit der Bearbeitung in beliebiger Reihenfolge zu beginnen und bei den Kindern mit sonderpädagogischem Förderbedarf individuell Bearbeitungsreihenfolgen vorgeben.";
                StudentReactionFour();

                break;
        }

        //showTempText();
        gamePlayManager.playerActionDataHandler.PlayerAction = questionString;
        gamePlayManager.StartPhaseSeven(20.0f);

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

        // GameObject.FindGameObjectWithTag("Player").GetComponent<RigidbodyFirstPersonController>().MoveLocked = true;
        // GameObject.FindGameObjectWithTag("Player").GetComponent<RigidbodyFirstPersonController>().LookLocked = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        //  GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().Table1MoveToPoint.rotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);

        GetHighAchievingStudentList();
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].scenarioStart = false;
            gamePlayManager.studentsActions[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform);
        }

     
       
        StartCoroutine(TriggerMainActionTwo());

    }

    IEnumerator TriggerMainActionTwo()
    {
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform);
        }
        yield return new WaitForSeconds(5f);
        string TeacherTextToSpeak = "Bitte holt alle euren Wochenplan aus der Tasche.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(TeacherTextToSpeak));

        yield return new WaitForSeconds(5f);
        Debug.Log("Main Action 2 starts");


        
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            // add random delay 1s to 4s
            StartCoroutine(StudentsTakeBookFromBag(gamePlayManager.studentsActions[i]));
        }

        yield return new WaitForSeconds(5.0f);
        gamePlayManager.StartPhaseFour();
        

    }

    IEnumerator StudentsTakeBookFromBag(StudentAction stu)
    {
        
        stu.StopLookAtSomeone();
        stu.StopMyRandomLookingAnimations();
        stu.studentAnimation.ResetAllAnim();
        // each student play iwo25 animation 
        stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Bag.SetActive(true);
        yield return new WaitForSeconds(Random.Range(2f, 4f));
        stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().WeeklyPlan.SetActive(true);

        yield return new WaitForSeconds(1f);

        stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Bag.SetActive(false);
        Debug.Log(stu.gameObject.name + " complete taking book from bag");
        yield return null;
    }

    public void MainActionTwo()
    {
        StartCoroutine(TriggerMainActionThree());

    }

    bool lookNeeded = false;

    IEnumerator LookAtTeacher()
    {
        while (lookNeeded)
        {
            for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
            {
                gamePlayManager.studentsActions[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform);
            }
            yield return new WaitForEndOfFrame();
        }
    }


    IEnumerator TriggerMainActionThree()
    {
        Debug.Log("main action 3 starts");
        lookNeeded = true;

        StartCoroutine(LookAtTeacher());

        string TeacherQuestion1 = "So, heute ist Freitag und die letzte Stunde, um noch den Wochenplan zu beenden. Wer ist schon komplett fertig?";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(TeacherQuestion1));


        yield return new WaitForSeconds(7.0f); // edit delay after testing


        string TeacherQuestion2 = "Wer hat noch eine Aufgabe?";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(TeacherQuestion2));
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < HighAchievingStudents.Count; i++)
        {
            HighAchievingStudents[i].studentAnimation.RaiseHandAndKeep(true);
        }
        yield return new WaitForSeconds(2.0f); // edit delay after testing
        for (int i = 0; i < HighAchievingStudents.Count; i++)
        {
            HighAchievingStudents[i].studentAnimation.RaiseHandAndKeep(false);
        }

        //students with high acheivements raise hands..stop

        string TeacherQuestion3 = "Wer hat noch mehr als zwei Aufgaben?";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(TeacherQuestion3));
        
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            bool excludeStudent = false;
            for (int j = 0; j < HighAchievingStudents.Count; j++) {
                if (gamePlayManager.studentsActions[i].name == HighAchievingStudents[j].name) {
                    excludeStudent = true;
                }
            }

            //only raise hand if NOT on eof the HighAchievingStudents
            if (excludeStudent == false) {
                gamePlayManager.studentsActions[i].studentAnimation.RaiseHandAndKeep(true);
            }
                
        }
        yield return new WaitForSeconds(2.0f); // edit delay after testing
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].studentAnimation.RaiseHandAndKeep(false);
        }

        lookNeeded = false;

        //show teacher pop-up message
        teacherPopUpPanel.SetActive(true);
        StartCoroutine(TeacherMessage());

        //gamePlayManager.StartPhaseFive();


    }

    IEnumerator TeacherMessage() {
        yield return new WaitForSeconds(5);
        teacherPopUpPanel.SetActive(false);

        yield return new WaitForSeconds(1);

        gamePlayManager.StartPhaseFive();
    }

    public void MainActionThree()
    {
        
       StartCoroutine(TriggerTeacherQuestionPhase());

        //Teacher Question Panel
       
    }

    IEnumerator TriggerTeacherQuestionPhase() 
    {
        yield return new WaitForSeconds(1.0f);
        gamePlayManager.StartPhaseSix();
    }

    IEnumerator SR_Message(float duration) {
        yield return new WaitForSeconds(duration);
        SR_PopUpPanel.SetActive(false);
    }


    private void StudentReactionOne()
    {
        SR_PopUpPanel.SetActive(true);
        StartCoroutine(SR_Message(5f));

        audioInClass.volume = 0f;
        
        StartCoroutine(TriggerStudentReactionOne());
    }
    IEnumerator TriggerStudentReactionOne()
    {
        teacherMumblingAudio.Play();
        yield return new WaitForSeconds(5f);
        teacherMumblingAudio.Stop();
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();

            StartCoroutine(SRWorkOnSheetsWithRandomDelay(gamePlayManager.studentsActions[i]));
            
        }
        yield return new WaitForSeconds(5f);
        //random student looks outside window

        
    }

    IEnumerator SRWorkOnSheetsWithRandomDelay(StudentAction stu)
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 1.5f));
        stu.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(Random.Range(2f, 3f));
        stu.studentAnimation.MB33_WorkOnSheets(false);
        stu.LookAtSomeone(stu.chairPoint.gameObject.GetComponent<ChairDetails>().GetAWindowToLookAt());
        yield return new WaitForSeconds(2f);
        stu.StopLookAtSomeone();
        stu.studentAnimation.MB33_WorkOnSheets(true);
    }
    

    private void StudentReactionTwo()
    {
        SR_PopUpPanel.SetActive(true);
        StartCoroutine(SR_Message(10f));

        audioInClass.volume = 0f;

        StartCoroutine(TriggerStudentReactionTwo());

    }
    IEnumerator TriggerStudentReactionTwo()
    {
        teacherMumblingAudio.Play();
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();

            if (gamePlayManager.studentsActions[i].ESEStudent || gamePlayManager.studentsActions[i].LEStudent) // if student is SN
            {
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
                StartCoroutine(SNStudentsLooksOutsideAndAroundWithDelay(gamePlayManager.studentsActions[i]));
            }
            else
            {
                gamePlayManager.studentsActions[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
            }
        }

        yield return new WaitForSeconds(3f);
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
            if (i == 0 || i == 1 || i == 2) // if students are 1,2 or 3 . assign 3 random students later
                StartCoroutine(SNStudentsLooksOutsideAndAroundWithDelay(gamePlayManager.studentsActions[i]));

        }
        kidJannik.SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(7f);
        teacherMumblingAudio.Stop();
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            if (gamePlayManager.studentsActions[i] != kidJannik)
                gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
            else
                gamePlayManager.studentsActions[i].studentAnimation.RaiseHandAndKeep(true);

        }
    }

    IEnumerator SNStudentsLooksOutsideAndAroundWithDelay(StudentAction stu)
    {
        stu.StopLookAtSomeone();
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        stu.studentAnimation.LookAround(true);
        yield return new WaitForSeconds(2f);
        stu.studentAnimation.LookAround(false);
        //stu.LookAtSomeone(stu.chairPoint.gameObject.GetComponent<ChairDetails>().GetAWindowToLookAt());
        //stu.LookAtWindowRoutine();

        //look out window
        stu.studentAnimation.ResetAllAnim();
        stu.LookAtSomeone(GameObject.Find("SpotAtCuboardNearWindow").transform);
        yield return new WaitForSeconds(5f);

        stu.StopLookAtSomeone();
        //stu.LookAtWindowRoutineStop();
        stu.studentAnimation.LookAround(true);
    }

    IEnumerator SNStudentsLooksOutsideAndAroundWithDelay(StudentAction stu,float delay)
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 2f));
        stu.studentAnimation.LookAround(true);
        yield return new WaitForSeconds(2f);
        stu.studentAnimation.LookAround(false);
        stu.LookAtSomeone(stu.chairPoint.gameObject.GetComponent<ChairDetails>().GetAWindowToLookAt());
        yield return new WaitForSeconds(2f);
        stu.StopLookAtSomeone();
        stu.studentAnimation.LookAround(true);
    }


    IEnumerator TurnStudentMoodWithDelay(StudentAction stu, MoodIndicator mood, float delay)
    {
        yield return new WaitForSeconds(delay);
        stu.SetMyMood(mood);
    }






    private void StudentReactionThree()
    {
        SR_PopUpPanel.SetActive(true);
        StartCoroutine(SR_Message(13f));

        audioInClass.volume = 0.1f;

        StartCoroutine(TriggerStudentReactionThree());
    }
    IEnumerator TriggerStudentReactionThree()
    {
        teacherMumblingAudio.Play();
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);

            if (gamePlayManager.studentsActions[i].ESEStudent || gamePlayManager.studentsActions[i].LEStudent) // if student is SN
            {
                StartCoroutine(TurnStudentMoodWithDelay(gamePlayManager.studentsActions[i], MoodIndicator.Bad, Random.Range(0.1f, 5f)));
            }
            else
            {
                StartCoroutine(TurnStudentMoodWithDelay(gamePlayManager.studentsActions[i], MoodIndicator.Middle, Random.Range(0.1f, 10f)));
            }
                
        }
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            if (gamePlayManager.studentsActions[i].ESEStudent || gamePlayManager.studentsActions[i].LEStudent) // if student is SN
            {
                StartCoroutine(SNStudentsLooksOutsideAndAroundWithDelay(gamePlayManager.studentsActions[i]));
            }

        }
        yield return new WaitForSeconds(4f);

        StartCoroutine(SNStudentsLooksOutsideAndAroundWithDelay(gamePlayManager.studentsActions[0], 0f));
        StartCoroutine(SNStudentsLooksOutsideAndAroundWithDelay(gamePlayManager.studentsActions[1], 1f));
        StartCoroutine(SNStudentsLooksOutsideAndAroundWithDelay(gamePlayManager.studentsActions[2], 2f));
        StartCoroutine(SNStudentsLooksOutsideAndAroundWithDelay(gamePlayManager.studentsActions[3], 3f));
        StartCoroutine(SNStudentsLooksOutsideAndAroundWithDelay(gamePlayManager.studentsActions[4], 4f));
        StartCoroutine(SNStudentsLooksOutsideAndAroundWithDelay(gamePlayManager.studentsActions[5], 5f));
        yield return new WaitForSeconds(2f);

        teacherMumblingAudio.Stop();

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            if(i <=13  ) //or SN students
            {
                StartCoroutine(SRWorkOnSheetsWithRandomDelay(gamePlayManager.studentsActions[i]));
            }
            else
            {
                StartCoroutine(SRWorkOnSheetsWithRandomDelayAfterWorkOnSheets(gamePlayManager.studentsActions[i]));
            }

        }
    }

    IEnumerator SRWorkOnSheetsWithRandomDelayAfterWorkOnSheets(StudentAction stu)
    {
        stu.studentAnimation.VI11_TalkToFriendsLeftAndRight();
        yield return new WaitForSeconds(Random.Range(0.1f, 3f));
        stu.studentAnimation.VI11_TalkToFriendsStop();
        stu.studentAnimation.MB33_WorkOnSheets(true);
    }

    private void StudentReactionFour()
    {
        SR_PopUpPanel.SetActive(true);
        StartCoroutine(SR_Message(5f));

        audioInClass.volume = 0.2f;

        StartCoroutine(TriggerStudentReactionFour());
    }
    IEnumerator TriggerStudentReactionFour()
    {
        teacherMumblingAudio.Play();
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
        }
        yield return new WaitForSeconds(5f);
        teacherMumblingAudio.Stop();
        // mumbling of students voice increases....

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            StartCoroutine(R4Action(gamePlayManager.studentsActions[i], i));
        }
        yield return new WaitForSeconds(5f);

        // mumbling of students voice to default...
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {

            StartCoroutine(SRWorkOnSheetsWithRandomDelay(gamePlayManager.studentsActions[i]));
        }
    }

    IEnumerator R4Action(StudentAction stu , int i)
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 3f));
        
        if (i < 7)
        {
            stu.LookAtSomeone(stu.chairPoint.gameObject.GetComponentInChildren<StudyMaterialType>().GetStudyMaterial(StudyMaterial.WorkSheet).transform);
            stu.SetMyMood(MoodIndicator.Middle);
        }
        else if (i >= 7 && i < 10)
        {
            stu.LookAtWindowRoutine();
            stu.SetMyMood(MoodIndicator.Middle);
        }
        else //if (i >= 10 && i < 16)
            stu.studentAnimation.VI11_TalkToFriendsLeftAndRight();
    }



    public void Reset()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayerToOriginalPostion(true);
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().NoteBook.SetActive(true);
            gamePlayManager.studentsActions[i].SetStudentBag(false);
            //gamePlayManager.studentsActions[i].GoToAndSitInChair();
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
            //StartCoroutine(StudentsBackToSeat(gamePlayManager.studentsActions[i]));
        }
        gamePlayManager.StartPhaseSix();

    }
}
