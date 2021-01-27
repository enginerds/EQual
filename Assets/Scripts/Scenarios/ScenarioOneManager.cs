using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioOneManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    public Transform tableOnePoint;
    public Transform tableFourPoint;
    public Transform TeacherGoToDesk;
    public Transform TeacherFrontOfClassPoint;
    public AudioSource audioInClass;

    string questionString = "";

    public AudioSource StudentAudio;
    public StudentAction[] SixRandomStudents;
    public StudentAction[] EightRandomStudents;
    public StudentAction KidJannik;
    public StudentAction KidLena;
    public StudentAction KidHanna;
    public StudentAction kidShirin;
    public StudentAction[] SR1StudentsInPair;
    public StudentAction[] SR2StudentsInPair_1;
    public StudentAction[] SR2StudentsInPair_2;
    public StudentAction[] SR3StudentsInPair_1;
    public StudentAction[] SR3StudentsInPair_2;
    public GameObject TeacherDesk;
    public GameObject TeacherDesk_outer;
    public int[] ChairNumbers = new int[21];
    public int ChairNoHanna;
    public int ChairNoLena;

    private void Awake()
    {
        gamePlayManager.currentScenario = "SC1";
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
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        //GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint.rotation;
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
        gamePlayManager.ReadyForInitialSituation = true;
    }
    public void StudentReaction(string teacherReactionIndex)
    {

        string value = teacherReactionIndex;
        gamePlayManager.CurrentPhase = "SR-" + value;
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TeacherActionSelected", value, true);
        string taKey = (gamePlayManager.userInterfaceManager.noOfNoCounts == 0) ? "TA1" : "TA2";
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData(taKey, value);
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TASQuestion", gamePlayManager.userInterfaceManager.teacherSelectedQuestion);
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TASTimeStarted", gamePlayManager.GetCurrentTime());
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("Revision", gamePlayManager.userInterfaceManager.noOfNoCounts.ToString());

        

        switch (value)
        {

            case "1":
                questionString = "Die Kinder leistungsbezogen in Paare einteilen und entsprechendes Material vorgeben. Jannik (SFB LE) arbeitet ohne Partner/in und wird bei Bedarf von einem leistungsstärkeren Kind unterstützt.";

                StudentReactionOne();
                gamePlayManager.StartPhaseSeven(36.0f);
                break;
            case "2":
                questionString = "Nebeneinandersitzende Kinder bilden jeweils ein Paar und entscheiden dann gemeinsam, welche Version sie nehmen. Jannik (SFB LE) und sein Nachbarkind werden bei Bedarf von Ihnen unterstützt.";

                StudentReactionTwo();
                gamePlayManager.StartPhaseSeven(69.0f);
                break;
            case "3":
                questionString = "Die Kinder anhand der Leistungsniveaus in Paare einteilen und entsprechendes Material vorgeben. Jannik (SFB LE) mit einem Kind mit schwachem Leistungsniveau zusammen einteilen und beide bei Bedarf bei der Bearbeitung unterstützen.";

                StudentReactionThree();
                gamePlayManager.StartPhaseSeven(35.0f);
                break;
            case "4":
                questionString = "Kinder dürfen die Partner/innen selber wählen und entscheiden dann gemeinsam, welche Version des Arbeitsblattes sie nehmen. Jannik (SFB LE) erhält die differenzierte Version und arbeitet mit Ihnen zusammen.";
                StudentReactionFour();
                gamePlayManager.StartPhaseSeven(109.0f);

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
        ChairNoHanna = KidHanna.ChairNumber;
        ChairNoLena = KidLena.ChairNumber;


        StartCoroutine(TriggerMainActionOne());
    }
    IEnumerator TriggerMainActionOne()
    {
        //Student mumbling low
        audioInClass.volume = 0.1f;
        audioInClass.Play();
        
        Debug.Log("Main Action 1 starts");

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        // GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint.rotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].FindMyNeighbour();
            ChairNumbers[i] = gamePlayManager.studentsActions[i].ChairNumber;
        }

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopSittingIdleLookAroundAnything(); // stop the initial intro animations for all kids
            gamePlayManager.studentsActions[i].scenarioStart = false;
            StartCoroutine(StudentsLookAround(gamePlayManager.studentsActions[i]));

        }
        yield return new WaitForSeconds(5.0f);
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {

            gamePlayManager.studentsActions[i].studentAnimation.MB9_LookAround(false);
            StartCoroutine(StudentsLookAtTeacherWithDelay(gamePlayManager.studentsActions[i]));

        }

        //Student mumbling zero
        audioInClass.volume = 0.0f;
        audioInClass.Play();

        string TeacherQuestion = "Lest in Partnerarbeit diesen Text und beantwortet anschließend die Fragen darunter. Es gibt zwei Texte: einfach oder schwierig.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(TeacherQuestion));

        
        yield return new WaitForSeconds(6.0f);

        StartCoroutine(TriggerMainActionTwo());
    }
    IEnumerator StudentsLookAround(StudentAction stu)
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        stu.studentAnimation.MB9_LookAround(true);
    }
    IEnumerator StudentsLookAtTeacherWithDelay(StudentAction stu)
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 2f));
        stu.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
    }
    IEnumerator StudentsRaiseHands(StudentAction stu)
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 2f));
        stu.studentAnimation.RaiseHandAndKeep(true);
        stu.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
    }




    IEnumerator TriggerMainActionTwo()
    {
        yield return null;
        Debug.Log("Main Action 2 starts");

        for (int i = 0; i < SixRandomStudents.Length; i++)
        {
            //StartCoroutine(StudentsRaiseHands(SixRandomStudents[i]));
        }
        
        yield return new WaitForSeconds(5.0f);
        gamePlayManager.StartPhaseFour();


    }



    public void MainActionTwo()
    {
        StartCoroutine(TriggerMainActionThree());

    }



    IEnumerator TriggerMainActionThree()
    {
        

        Debug.Log("Main Action 3 starts");
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].studentAnimation.RaiseHand(false);
        }
        string TeacherQuestion = "Wer von euch kann die Aufgabe bitte wiederholen?";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(TeacherQuestion));
        yield return new WaitForSeconds(4.0f);
        for (int i = 0; i < EightRandomStudents.Length; i++)
        {
            StartCoroutine(StudentsRaiseHands(EightRandomStudents[i]));
        }
        yield return new WaitForSeconds(4.0f);
        for (int i = 0; i < EightRandomStudents.Length; i++)
        {
            EightRandomStudents[i].studentAnimation.RaiseHand(false);
        }

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1MoveToPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, kidShirin.gameObject.transform);
        StudentAudio.Play();
        yield return new WaitForSeconds(4.0f);
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
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        //GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint.rotation;
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, TeacherGoToDesk);
        // GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint.rotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
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
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
        }
        yield return new WaitForSeconds(0.1f);

        bool lastStudent = false;
        for (int i = 0; i < SR1StudentsInPair.Length; i++) {
            
            if (i == SR1StudentsInPair.Length - 1) {
                lastStudent = true;
            }

            StartCoroutine(SR1StudentWalkTowardsFrontDesk(SR1StudentsInPair[i], lastStudent));
        }
    }

    IEnumerator SR1StudentWalkTowardsFrontDesk(StudentAction stu, bool lastStudent)
    {
        int rand = Random.Range(1, 3);
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1.5f));
        //stu.studentAnimation.ResetAllAnim();
        stu.InitiateGoToSpot(TeacherDesk.transform);
        yield return new WaitUntil((() => stu.reachedSpot));
        stu.studentAnimation.TakeItem(true);
        yield return new WaitForSeconds(1f);

        if(stu == KidJannik.myNeighbourStudent)
        {
            stu.rightHandStudyMaterials.YellowPaper.SetActive(true);
        }
        else
        {
            if (rand == 1) // book yellow
                    stu.rightHandStudyMaterials.YellowPaper.SetActive(true);
            if (rand == 2) // book blue
                    stu.rightHandStudyMaterials.BluePaper.SetActive(true);
        }

        
        stu.studentAnimation.TakeItem(false);
        stu.GoToAndSitInChair();
        yield return new WaitUntil((() => stu.reachedSpot));

        if (stu == KidJannik.myNeighbourStudent)
        {
                stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().YellowPaper.SetActive(true);
                stu.rightHandStudyMaterials.YellowPaper.SetActive(false);
        }
        else
        {
            if (rand == 1) // book yellow
            {
                stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().YellowPaper.SetActive(true);
                stu.rightHandStudyMaterials.YellowPaper.SetActive(false);
            }
            if (rand == 2) // book blue
            {
                stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().BluePaper.SetActive(true);
                stu.rightHandStudyMaterials.BluePaper.SetActive(false);
            }
        }
    
        yield return new WaitForSeconds(4f);
        if (rand == 1) // book yellow
        {
            stu.LookAtSomeone(stu.rightHandStudyMaterials.YellowPaper.transform);
            stu.myNeighbourStudent.LookAtSomeone(stu.rightHandStudyMaterials.YellowPaper.transform);
        }
        if (rand == 2) // book blue
        {
            stu.LookAtSomeone(stu.rightHandStudyMaterials.BluePaper.transform);
            stu.myNeighbourStudent.LookAtSomeone(stu.rightHandStudyMaterials.BluePaper.transform);
        }
        yield return new WaitForSeconds(5f);

        if (lastStudent == true) {
            //wait for last student to to be seated before showing clock
            yield return new WaitForSeconds(10f);

            //show 10 minute time lapse
            gamePlayManager.SetTimeScaleTextToMinutes();
            gamePlayManager.StartTimer(true);

            yield return new WaitForSeconds(10f);

            gamePlayManager.StopTimer();

        }
    }


    IEnumerator TriggerStudentReactionTwo()
    {
        //Student mumbling low
        audioInClass.volume = 0.1f;
        audioInClass.Play();

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].studentAnimation.VI7_TalkToFriendsLeftAndRight(true);
        }
        yield return new WaitForSeconds(3.0f);
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].studentAnimation.VI7_TalkToFriendsLeftAndRight(false);
        }

        bool lastStudent = false;
        
        //Yellow group
        for (int i = 0; i < SR2StudentsInPair_1.Length; i++)
        {
            StartCoroutine(SR2StudentWalkTowardsFrontDesk(SR2StudentsInPair_1[i], 1, lastStudent));
        }
        
        //Blue group
        for (int i = 0; i < SR2StudentsInPair_2.Length; i++)
        {
            if (i == SR2StudentsInPair_2.Length - 1) {
                lastStudent = true;
            }
            StartCoroutine(SR2StudentWalkTowardsFrontDesk(SR2StudentsInPair_2[i], 2, lastStudent));
        }
        //yield return new WaitForSeconds(25.0f);
        //for (int i = 0; i < SR2StudentsInPair_1.Length; i++)
        //{
        //    SR2StudentsInPair_1[i].SetMyMood(MoodIndicator.Middle);
        //}
    }
    
    IEnumerator SR2StudentWalkTowardsFrontDesk(StudentAction stu,int val, bool lastStudent)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1.5f));
        stu.studentAnimation.ResetAllAnim();
        stu.InitiateGoToSpot(TeacherDesk.transform);
        yield return new WaitUntil((() => stu.reachedSpot));
        stu.studentAnimation.TakeItem(true);
        yield return new WaitForSeconds(1f);
        if(val == 1)
        {
            stu.rightHandStudyMaterials.YellowPaper.SetActive(true);
        }
        else
        {
            stu.rightHandStudyMaterials.BluePaper.SetActive(true);
        }
        //stu.LookAtSomeone(stu.chairPoint);
        yield return new WaitForSeconds(0.5f);
        stu.GoToAndSitInChair();
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil((() => stu.reachedSpot));

        if (val == 1) // book yellow
        {
            stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().YellowPaper.SetActive(true);
            stu.rightHandStudyMaterials.YellowPaper.SetActive(false);
        }
        else
        {
            stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().BluePaper.SetActive(true);
            stu.rightHandStudyMaterials.BluePaper.SetActive(false);
        }
        yield return new WaitForSeconds(1f);
        //stu.LookAtSomeone(stu.rightHandStudyMaterials.YellowPaper.transform);
        //stu.myNeighbourStudent.LookAtSomeone(stu.rightHandStudyMaterials.YellowPaper.transform);
        stu.studentAnimation.MB33_WorkOnSheets(true);
        stu.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(4f);

        if (lastStudent == true) {
            //wait for last student to to be seated before showing clock
            yield return new WaitForSeconds(10f);

            //show 10 minute time lapse
            gamePlayManager.SetTimeScaleTextToMinutes();
            gamePlayManager.StartTimer(true);

            yield return new WaitForSeconds(10f);

            gamePlayManager.StopTimer();

            //move to final phase, where students with blue books become bored.
            StartCoroutine(SR2_FinalPhase());

        }

    }

    IEnumerator SR2_FinalPhase() {
        //This handles the final moments of SR2.

        for (int i = 0; i < SR2StudentsInPair_2.Length; i++) {
            //set mood to Middle
            SR2StudentsInPair_2[i].SetMyMood(MoodIndicator.Middle);

            //engage in random "bored" animation
            SR2StudentsInPair_2[i].studentAnimation.ResetAllAnim();
            int val = UnityEngine.Random.Range(0, 3);

            if (i == 0) {
                SR2StudentsInPair_2[i].studentAnimation.MB9_LookAround(true);
                yield return new WaitForSeconds(2.0f);
            } else if (i == 1) {
                SR2StudentsInPair_2[i].studentAnimation.MB19_Protest(true);
                yield return new WaitForSeconds(2.0f);
            } else if (i == 2) {
                SR2StudentsInPair_2[i].studentAnimation.MB28_TapOnNeighbour(true);
                yield return new WaitForSeconds(2.0f);
            }

            yield return new WaitForSeconds(2.0f);

            //Move camera to center of classroom for player to be able to see all tables.
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, TeacherFrontOfClassPoint);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1);
            yield return new WaitForSeconds(3.0f);

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, TeacherFrontOfClassPoint);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table2);
            yield return new WaitForSeconds(3.0f);

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, TeacherFrontOfClassPoint);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table3);
            yield return new WaitForSeconds(3.0f);

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, TeacherFrontOfClassPoint);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);
            yield return new WaitForSeconds(3.0f);

        }
    }

    IEnumerator SR3StudentWalkTowardsFrontDesk(StudentAction stu, int val)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1.5f));
        stu.studentAnimation.ResetAllAnim();
        stu.InitiateGoToSpot(TeacherDesk.transform);
        yield return new WaitUntil((() => stu.reachedSpot));
        stu.studentAnimation.TakeItem(true);
        yield return new WaitForSeconds(1f);
        if (val == 1)
        {
            stu.rightHandStudyMaterials.YellowPaper.SetActive(true);
        }
        else
        {
            stu.rightHandStudyMaterials.BluePaper.SetActive(true);
        }
        stu.studentAnimation.TakeItem(false);
        
        

        stu.GoToAndSitInChair();
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil((() => stu.reachedSpot));
        if (val == 1) // book yellow
        {
            stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().YellowPaper.SetActive(true);
            stu.rightHandStudyMaterials.YellowPaper.SetActive(false);
        }
        else
        {
            stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().BluePaper.SetActive(true);
            stu.rightHandStudyMaterials.BluePaper.SetActive(false);
        }
        yield return new WaitForSeconds(4f);
        //stu.LookAtSomeone(stu.rightHandStudyMaterials.YellowPaper.transform);
        //stu.myNeighbourStudent.LookAtSomeone(stu.rightHandStudyMaterials.YellowPaper.transform);
        if(stu != KidJannik || stu != KidJannik.myNeighbourStudent)
        {
            stu.studentAnimation.MB33_WorkOnSheets(true);
            stu.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(true);
        }
        yield return new WaitForSeconds(4f);

    }



    IEnumerator TriggerStudentReactionThree()
    {

        //Student mumbling low
        audioInClass.volume = 0.1f;
        audioInClass.Play();

        KidHanna.FindMyChair("Chair", ChairNoLena);
        KidLena.FindMyChair("Chair", ChairNoHanna);
        KidHanna.FindMyNeighbour();
        KidLena.FindMyNeighbour();
        KidJannik.FindMyNeighbour();
        KidJannik.myNeighbourStudent = KidHanna;
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);

        }
        yield return new WaitForSeconds(1.0f);
        for (int i = 0; i < SR2StudentsInPair_1.Length; i++)
        {
            StartCoroutine(SR3StudentWalkTowardsFrontDesk(SR2StudentsInPair_1[i], 1)); // sit in different seats .. add this
        }
        for (int i = 0; i < SR2StudentsInPair_2.Length; i++)
        {
            StartCoroutine(SR3StudentWalkTowardsFrontDesk(SR2StudentsInPair_2[i], 2));
        }

        //Student mumbling middle
        audioInClass.volume = 0.3f;
        audioInClass.Play();

        yield return new WaitForSeconds(18.0f);
        
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, tableFourPoint);
        KidJannik.studentAnimation.MB33_WorkOnSheets(false);
        KidJannik.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(false);
        KidJannik.SetMyMood(MoodIndicator.Bad);
        KidJannik.myNeighbourStudent.SetMyMood(MoodIndicator.Bad);
        KidJannik.studentAnimation.MB9_LookAround(true);
        KidJannik.myNeighbourStudent.studentAnimation.MB9_LookAround(true);
        yield return new WaitForSeconds(4.0f);
        KidJannik.studentAnimation.MB33_WorkOnSheets(true);
        KidJannik.myNeighbourStudent.studentAnimation.MB33_WorkOnSheets(true);
    }
    IEnumerator TriggerStudentReactionFour()
    {
        //Student mumbling middle
        audioInClass.volume = 0.3f;
        audioInClass.Play();

        //move player into position
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, TeacherGoToDesk);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
        }
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[1], 3));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[2], 10));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[3], 6));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[4], 11));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[5], 8));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[6], 9));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[7], 19));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[8], 7));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[9], 18));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[10], 21));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[11], 13));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[12], 26));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[13], 12));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[14], 2));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[15], 27));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[16], 1));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[17], 25));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[18], 14));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[19], 15));
        StartCoroutine(SR4StudentWalkTowardsNewChair(gamePlayManager.studentsActions[20], 0));
        
        yield return new WaitForSeconds(10.0f);
        gamePlayManager.studentsActions[1].myNeighbourStudent = gamePlayManager.studentsActions[14];
        gamePlayManager.studentsActions[2].myNeighbourStudent = gamePlayManager.studentsActions[4];
        gamePlayManager.studentsActions[3].myNeighbourStudent = gamePlayManager.studentsActions[8];
        gamePlayManager.studentsActions[4].myNeighbourStudent = gamePlayManager.studentsActions[2];
        gamePlayManager.studentsActions[5].myNeighbourStudent = gamePlayManager.studentsActions[6];
        gamePlayManager.studentsActions[6].myNeighbourStudent = gamePlayManager.studentsActions[5];
        gamePlayManager.studentsActions[7].myNeighbourStudent = gamePlayManager.studentsActions[9];
        gamePlayManager.studentsActions[8].myNeighbourStudent = gamePlayManager.studentsActions[3];
        gamePlayManager.studentsActions[9].myNeighbourStudent = gamePlayManager.studentsActions[7];
        gamePlayManager.studentsActions[10].myNeighbourStudent = gamePlayManager.studentsActions[13];
        gamePlayManager.studentsActions[11].myNeighbourStudent = gamePlayManager.studentsActions[12];
        gamePlayManager.studentsActions[12].myNeighbourStudent = gamePlayManager.studentsActions[11];
        gamePlayManager.studentsActions[13].myNeighbourStudent = gamePlayManager.studentsActions[10];
        gamePlayManager.studentsActions[14].myNeighbourStudent = gamePlayManager.studentsActions[1];
        gamePlayManager.studentsActions[15].myNeighbourStudent = gamePlayManager.studentsActions[17];
        gamePlayManager.studentsActions[16].myNeighbourStudent = gamePlayManager.studentsActions[20];
        gamePlayManager.studentsActions[17].myNeighbourStudent = gamePlayManager.studentsActions[15];
        gamePlayManager.studentsActions[18].myNeighbourStudent = gamePlayManager.studentsActions[19];
        gamePlayManager.studentsActions[19].myNeighbourStudent = gamePlayManager.studentsActions[18];
        gamePlayManager.studentsActions[20].myNeighbourStudent = gamePlayManager.studentsActions[16];

        //for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        //{
        //    gamePlayManager.studentsActions[i].GoToAndSitInChair();
        //}
        //yield return new WaitForSeconds(10f);


        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
        }
        yield return new WaitForSeconds(1f);


        //Students walk to Teacher desk, one pair at a time

        //gamePlayManager.studentsActions[1].myNeighbourStudent = gamePlayManager.studentsActions[14];
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[1], 2, 0f));
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[14], 2, 1f));
        yield return new WaitForSeconds(5f);

        //gamePlayManager.studentsActions[2].myNeighbourStudent = gamePlayManager.studentsActions[4];
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[2], 2, 0f));
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[4], 2, 1f));
        yield return new WaitForSeconds(5f);

        //gamePlayManager.studentsActions[3].myNeighbourStudent = gamePlayManager.studentsActions[8];;
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[3], 2, 0f));
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[8], 2, 1f));
        yield return new WaitForSeconds(5f);

        //gamePlayManager.studentsActions[5].myNeighbourStudent = gamePlayManager.studentsActions[6];
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[5], 2, 0f));
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[6], 2, 1f));
        yield return new WaitForSeconds(5f);

        //gamePlayManager.studentsActions[7].myNeighbourStudent = gamePlayManager.studentsActions[9];
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[7], 2, 0f));
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[9], 2, 1f));
        yield return new WaitForSeconds(5f);

        //gamePlayManager.studentsActions[10].myNeighbourStudent = gamePlayManager.studentsActions[13];
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[10], 2, 0f));
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[13], 2, 1f));
        yield return new WaitForSeconds(5f);

        //gamePlayManager.studentsActions[11].myNeighbourStudent = gamePlayManager.studentsActions[12];
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[11], 1, 0f));
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[12], 1, 1f));
        yield return new WaitForSeconds(5f);

        //gamePlayManager.studentsActions[15].myNeighbourStudent = gamePlayManager.studentsActions[17];
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[15], 2, 0f));
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[17], 2, 1f));
        yield return new WaitForSeconds(5f);

        //gamePlayManager.studentsActions[16].myNeighbourStudent = gamePlayManager.studentsActions[20];
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[16], 2, 0f));
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[20], 2, 1f));
        yield return new WaitForSeconds(5f);

        //gamePlayManager.studentsActions[18].myNeighbourStudent = gamePlayManager.studentsActions[19];
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[18], 2, 0f));
        StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[19], 2, 1f));
        yield return new WaitForSeconds(5f);

        

        //old
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[0], 2, 1f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[1], 1, 0f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[2], 2, 2f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[3], 2, 3f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[4], 2, 2f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[5], 2, 4f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[6], 2, 4f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[7], 2, 5f)); 
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[8], 1, 3f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[9], 2, 5f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[10], 1, 6f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[11], 2, 7f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[12], 1, 7f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[13], 2, 6f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[14], 1, 0f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[15], 1, 8f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[16], 2, 9f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[17], 1, 8f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[18], 1, 10f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[19], 1, 10f));
        //StartCoroutine(SR4StudentWalkTowardsFrontDesk(gamePlayManager.studentsActions[20], 2, 9f));

        yield return new WaitForSeconds(26.0f);

        //Student mumbling high
        audioInClass.volume = 0.5f;
        audioInClass.Play();

        //All the students who got a blue book get bored.
        StartCoroutine(StudentsWithBlueBooksGetBored(gamePlayManager.studentsActions[1]));
        StartCoroutine(StudentsWithBlueBooksGetBored(gamePlayManager.studentsActions[14]));
        StartCoroutine(StudentsWithBlueBooksGetBored(gamePlayManager.studentsActions[2]));
        StartCoroutine(StudentsWithBlueBooksGetBored(gamePlayManager.studentsActions[4]));
        StartCoroutine(StudentsWithBlueBooksGetBored(gamePlayManager.studentsActions[3]));
        StartCoroutine(StudentsWithBlueBooksGetBored(gamePlayManager.studentsActions[8]));
        StartCoroutine(StudentsWithBlueBooksGetBored(gamePlayManager.studentsActions[5]));
        StartCoroutine(StudentsWithBlueBooksGetBored(gamePlayManager.studentsActions[6]));
        StartCoroutine(StudentsWithBlueBooksGetBored(gamePlayManager.studentsActions[7]));
        StartCoroutine(StudentsWithBlueBooksGetBored(gamePlayManager.studentsActions[9]));
        StartCoroutine(StudentsWithBlueBooksGetBored(gamePlayManager.studentsActions[10]));
        StartCoroutine(StudentsWithBlueBooksGetBored(gamePlayManager.studentsActions[13]));
        StartCoroutine(StudentsWithBlueBooksGetBored(gamePlayManager.studentsActions[15]));
        StartCoroutine(StudentsWithBlueBooksGetBored(gamePlayManager.studentsActions[16]));
        StartCoroutine(StudentsWithBlueBooksGetBored(gamePlayManager.studentsActions[20]));
        StartCoroutine(StudentsWithBlueBooksGetBored(gamePlayManager.studentsActions[18]));
        StartCoroutine(StudentsWithBlueBooksGetBored(gamePlayManager.studentsActions[19]));
        

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, tableFourPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);
        KidJannik.SetMyMood(MoodIndicator.Bad);
        yield return new WaitForSeconds(4.0f);
        KidJannik.studentAnimation.MB33_WorkOnSheets(true);
    }

    IEnumerator SR4StudentWalkTowardsNewChair(StudentAction stu,int NewChairNumber)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1.5f));
        stu.FindMyChair("Chair", NewChairNumber);
        stu.GoToAndSitInChair();
    }

    IEnumerator StudentsWithBlueBooksGetBored(StudentAction stu) {
        stu.studentAnimation.MB33_WorkOnSheets(false);
        stu.SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 5f));
        stu.studentAnimation.MB9_LookAround(true);
    }

    void SR4HighAcheivingStudentsEvent(StudentAction stu)
    {
        stu.studentAnimation.MB33_WorkOnSheets(false);
        stu.SetMyMood(MoodIndicator.Middle);
        stu.studentAnimation.MB9_LookAround(true);
        
    }

    IEnumerator SR4StudentWalkTowardsFrontDesk(StudentAction stu, int val,float delay)
    {
        yield return new WaitForSeconds(delay);
        stu.studentAnimation.ResetAllAnim();

        //walk to the Teacher's desk and stand and talk to your neighbor upon arriving
        //stu.InitiateGoToSpot(TeacherDesk.transform);
        stu.InitiateGoToSpot(TeacherDesk_outer.transform);
        yield return new WaitUntil((() => stu.reachedSpot));
        yield return new WaitUntil((() => stu.myNeighbourStudent.reachedSpot));
        stu.studentAnimation.MB42_StandCrossArms(true);
        yield return new WaitForSeconds(3f);
        stu.studentAnimation.MB42_StandCrossArms(false);

        //stu.studentAnimation.WalkingToStandUpIdle(true);
        stu.InitiateGoToSpot(TeacherDesk.transform);
        yield return new WaitForSeconds(1f);

        //take booklet and return to your seat
        
        //stu.studentAnimation.ResetAllAnim();
        //yield return new WaitForSeconds(1f);
        stu.studentAnimation.TakeItem(true);
        yield return new WaitForSeconds(0.2f);
        if (val == 1) {
            stu.rightHandStudyMaterials.YellowPaper.SetActive(true);
        } else {
            stu.rightHandStudyMaterials.BluePaper.SetActive(true);
        }
        stu.studentAnimation.TakeItem(false);
        //stu.GoToAndSitInChair();
            
        yield return new WaitUntil((() => stu.reachedSpot));

        if (val == 1) // book yellow
        {
            stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().YellowPaper.SetActive(true);
            stu.rightHandStudyMaterials.YellowPaper.SetActive(true);
            yield return new WaitForSeconds(2f);
            stu.GoToAndSitInChair();
        }
        else
        {
            stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().BluePaper.SetActive(true);
            stu.rightHandStudyMaterials.BluePaper.SetActive(true);
            yield return new WaitForSeconds(2f);
            stu.GoToAndSitInChair();
        }
        yield return new WaitForSeconds(1f);
        //stu.LookAtSomeone(stu.rightHandStudyMaterials.YellowPaper.transform);
        //stu.myNeighbourStudent.LookAtSomeone(stu.rightHandStudyMaterials.YellowPaper.transform);
        yield return new WaitForSeconds(2f);
        if(stu != KidJannik)
            stu.rightHandStudyMaterials.YellowPaper.SetActive(false);
            stu.rightHandStudyMaterials.BluePaper.SetActive(false);
        stu.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(3f);

    }



    public void Reset()
    {
        //KidHanna.FindMyChair("Chair", ChairNoHanna);
        //KidLena.FindMyChair("Chair", ChairNoLena);
        //KidHanna.FindMyNeighbour();
        //KidLena.FindMyNeighbour();
        //KidJannik.FindMyNeighbour();

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint.rotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].ChairNumber = ChairNumbers[i];
            gamePlayManager.studentsActions[i].FindMyChair("Chair", ChairNumbers[i]);
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
            gamePlayManager.studentsActions[i].chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().BluePaper.SetActive(false);
            gamePlayManager.studentsActions[i].chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().YellowPaper.SetActive(false);
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
