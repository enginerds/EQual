using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioNineteenManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    public Transform tableOnePoint;
    public AudioSource audioInClass;
    public AudioSource schoolBellAudio;

    string questionString = "";

    public GameObject teacherPopUpPanel;

    public GameObject classRuleBoard;
    public Transform door;
    public StudentAction kidLeonie, kidJannik, kidMaxim, kidShirin,KidZoe,KidSophie;

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
                questionString = "lesen sie laut vor und geben das Zeichen zum Abschreiben. Bei Bedarf gehen Sie zu den Schülerinnen und Schülern mit sonderpädagogischem Förderbedarf und überprüfen die Eintragungen.";

                StudentReactionOne();
                break;
            case "2":
                questionString = "lassen sie von einem Kind mit sonderpädagogischem Förderbedarf vorlesen.  Anschließend gehen Sie durch die Klasse, damit sich alle aufgefordert fühlen, tatsächlich etwas aufzuschreiben.";

                StudentReactionTwo();
                break;
            case "3":
                questionString = "lassen die Kinder in eigenem Tempo lesen, bevor sie die Aufgaben abschreiben. Dabei gehen Sie durch die Klasse, damit sich alle aufgefordert fühlen, tatsächlich etwas aufzuschreiben.";

                StudentReactionThree();
                break;
            case "4":
                questionString = "lassen die Kinder in eigenem Tempo lesen, bevor sie die Aufgaben abschreiben. Sie gehen zu den Kindern mit sonderpädagogischem Förderbedarf hin und prüfen die Eintragungen.";
                StudentReactionFour();

                break;
        }

        //showTempText();
        gamePlayManager.playerActionDataHandler.PlayerAction = questionString;
        gamePlayManager.StartPhaseSeven(25.0f);

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
            gamePlayManager.studentsActions[i].studentAnimation.MB21_readTextBook(true);
        }


        //Set low student mumbling
        //audioInClass.volume = 0.08f;
        audioInClass.volume = 0f;
        audioInClass.Play();


        StartCoroutine(TriggerMainActionTwo());

    }

    IEnumerator TriggerMainActionTwo()
    {
        yield return new WaitForSeconds(10f);
        Debug.Log("Main Action 2 starts");


        if (gamePlayManager.LOG_ENABLED) SpeechManager.Instance.StartTalking("Bitte packt eure Bücher jetzt ein und holt eure Hausaufgabenhefte raus.");
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            // add random delay 1s to 4s
            StartCoroutine(StudentsTakeBookFromBag(gamePlayManager.studentsActions[i]));
        }

        yield return new WaitForSeconds(2.0f);
        gamePlayManager.StartPhaseFour();
        

    }

    IEnumerator StudentsTakeBookFromBag(StudentAction stu)
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 3f));
        stu.StopLookAtSomeone();
        stu.StopMyRandomLookingAnimations();
        stu.studentAnimation.ResetAllAnim();
        // each student play iwo25 animation 
        stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Bag.SetActive(true);
        yield return new WaitForSeconds(1f);
        stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().NoteBook.SetActive(true);
        stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Book.SetActive(false);
        yield return new WaitForSeconds(3f);

        stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Bag.SetActive(false);
        Debug.Log(stu.gameObject.name + " complete taking book from bag");
        yield return null;
    }

    public void MainActionTwo()
    {
        StartCoroutine(TriggerMainActionThree());

    }




    IEnumerator TriggerMainActionThree()
    {
        Debug.Log("main action 3 starts");
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
        }
        yield return new WaitForSeconds(3.0f); // edit delay after testing

        //show teacher pop-up message
        //teacherPopUpPanel.SetActive(true);
        //StartCoroutine(TeacherMessage());
        
        gamePlayManager.StartPhaseFive();


    }

    IEnumerator TeacherMessage() {
        yield return new WaitForSeconds(4);
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


    private void StudentReactionOne()
    {
        StartCoroutine(TriggerStudentReactionOne());
    }
    IEnumerator TriggerStudentReactionOne()
    {
        //Set student mumbling to zero
        audioInClass.volume = 0f;
        audioInClass.Play();

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();

            if (gamePlayManager.studentsActions[i] != kidLeonie)
            {
                StartCoroutine(SRLoop1(gamePlayManager.studentsActions[i]));
            }
            else
            {
                gamePlayManager.studentsActions[i].studentAnimation.MB9_LookAround(true);
                //StartCoroutine(SR1Loop1Leonie(gamePlayManager.studentsActions[i]));
            }
        }
        yield return new WaitForSeconds(6f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1MoveToPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().Table1MoveToPoint.rotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1);
        //   GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = tableOnePoint.rotation;
        yield return new WaitForSeconds(2f);
        kidLeonie.StopLookAtSomeone();
        kidLeonie.studentAnimation.ResetAllAnim();
        yield return new WaitForSeconds(0.5f);
        kidLeonie.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
        yield return new WaitForSeconds(1f);
        kidLeonie.LookAtSomeone(kidLeonie.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
        kidLeonie.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(1f);
        kidLeonie.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(1f);
        kidLeonie.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(2f);
        kidLeonie.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(2f);
        schoolBellAudio.Play();
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            StartCoroutine(StudentWalkOutOfClass(gamePlayManager.studentsActions[i]));
        }
    }

    IEnumerator SRLoop1(StudentAction stu)
    {
        //yield return new WaitForSeconds(Random.Range(0.1f, 1.5f));
        stu.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(5f);
        stu.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(5);
        stu.studentAnimation.MB33_WorkOnSheets(true);
    }
    /*IEnumerator SR1Loop1Leonie(StudentAction stu)
    {
        yield return new WaitForSeconds(1f);
        stu.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
        yield return new WaitForSeconds(1f);
        stu.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
        yield return new WaitForSeconds(1f);
        stu.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
    }*/
    IEnumerator StudentWalkOutOfClass(StudentAction stu)
    {
        stu.studentAnimation.MB33_WorkOnSheets(false);
        //play iwo25 animation
        stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Bag.SetActive(true);
        yield return new WaitForSeconds(1f);
        stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().NoteBook.SetActive(false);
        stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Book.SetActive(false);
        yield return new WaitForSeconds(1f);
        stu.chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Bag.SetActive(false);
        yield return new WaitForSeconds(1f);
        stu.SetStudentBag(true);
        stu.StopLookAtSomeone();
        stu.StopMyRandomLookingAnimations();
        stu.studentAnimation.ResetAllAnim();
        stu.InitiateGoToSpot(door.transform); // to go outside
    }

    private void StudentReactionTwo()
    {
        StartCoroutine(TriggerStudentReactionTwo());

    }
    IEnumerator TriggerStudentReactionTwo()
    {
        //Set student mumbling to low
        audioInClass.volume = 0.08f;
        audioInClass.Play();

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();

            if (gamePlayManager.studentsActions[i] != kidLeonie && gamePlayManager.studentsActions[i] != kidJannik && gamePlayManager.studentsActions[i] != kidMaxim &&  gamePlayManager.studentsActions[i] != kidShirin)
            {
                gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
                //StartCoroutine(SRLoop1(gamePlayManager.studentsActions[i]));
            }
            else
            {
                gamePlayManager.studentsActions[i].studentAnimation.VI11_TalkToFriendsLeftAndRight();
                
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
        yield return new WaitForSeconds(8f);
        // school bell rings audio
        schoolBellAudio.Play();
        yield return new WaitForSeconds(1f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1);
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            if (gamePlayManager.studentsActions[i] != kidLeonie && gamePlayManager.studentsActions[i] != kidJannik && gamePlayManager.studentsActions[i] != kidMaxim && gamePlayManager.studentsActions[i] != kidShirin)
            {
                StartCoroutine(StudentWalkOutOfClass(gamePlayManager.studentsActions[i]));
            }
            else
            {
                 gamePlayManager.studentsActions[i].StopLookAtSomeone();
                gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
                gamePlayManager.studentsActions[i].studentAnimation.VI11_TalkToFriendsStop();
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
                gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
                //StartCoroutine(SRLoop1(gamePlayManager.studentsActions[i]));
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
        yield return new WaitForSeconds(6f);
        Debug.Log(" SR 2 Finished");
    }


    private void StudentReactionThree()
    {
        // all students except leonie (B7) , Jannik (D7) and student at B6 and D6 start writing on homework notebook
        // these four students start quity talk to each other vi5 
        // leoniw and jannik mood = bad
        // delay
        // school bell rings
        // all student put notebook in bag iwo25  
        // leave the classroom
        // these six students start writing in their homework notebook(mb33)
        StartCoroutine(TriggerStudentReactionThree());
    }
    IEnumerator TriggerStudentReactionThree()
    {
        StudentAction RandomKid1, RandomKid2;
        RandomKid1 = GetRandomStudent1R3();
        RandomKid2 = GetRandomStudent2R3(RandomKid1);

        //Set student mumbling to high
        audioInClass.volume = 0.4f;
        audioInClass.Play();

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();

            if (gamePlayManager.studentsActions[i] != kidLeonie && gamePlayManager.studentsActions[i] != kidJannik && gamePlayManager.studentsActions[i] != kidMaxim && gamePlayManager.studentsActions[i] != kidShirin && gamePlayManager.studentsActions[i] != RandomKid1 && gamePlayManager.studentsActions[i] != RandomKid2)
            {
                StartCoroutine(SRLoop1(gamePlayManager.studentsActions[i]));
            }
            else if(gamePlayManager.studentsActions[i] != RandomKid1 || gamePlayManager.studentsActions[i] != RandomKid2)
            {
                gamePlayManager.studentsActions[i].studentAnimation.VI11_TalkToFriendsLeftAndRight();
                //gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
            }
            else
            {
                gamePlayManager.studentsActions[i].studentAnimation.MB9_LookAround(true);
            }
        }
        yield return new WaitForSeconds(6f);
        // school bell rings audio
        schoolBellAudio.Play();
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            if (gamePlayManager.studentsActions[i] != kidLeonie && gamePlayManager.studentsActions[i] != kidJannik && gamePlayManager.studentsActions[i] != kidMaxim && gamePlayManager.studentsActions[i] != kidShirin && gamePlayManager.studentsActions[i] != RandomKid1 && gamePlayManager.studentsActions[i] != RandomKid2)
            {
                StartCoroutine(StudentWalkOutOfClass(gamePlayManager.studentsActions[i]));
            }
            else
            {
                gamePlayManager.studentsActions[i].StopLookAtSomeone();
                gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
                gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
                gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
                if(gamePlayManager.studentsActions[i] == kidLeonie || gamePlayManager.studentsActions[i] == kidJannik)
                {
                    gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Bad);
                }
            }

        }
        //kidLeonie, kidJannik,kidMaxim, KidShirin,KidZoe,KidSophie;
        kidMaxim.SetMyMood(MoodIndicator.Middle);
        kidShirin.SetMyMood(MoodIndicator.Middle);
        KidZoe.SetMyMood(MoodIndicator.Middle);
        KidSophie.SetMyMood(MoodIndicator.Middle);
    }

    StudentAction GetRandomStudent1R3()
    {
        int randomIndex = Random.Range(0, gamePlayManager.studentsActions.Count - 1);
        bool gotStu = false;
        while(gotStu)
        {
            if (gamePlayManager.studentsActions[randomIndex] != kidLeonie &&
                gamePlayManager.studentsActions[randomIndex] != kidJannik &&
                gamePlayManager.studentsActions[randomIndex] != kidShirin &&
                gamePlayManager.studentsActions[randomIndex] != kidMaxim)
            {
                gotStu = true;
            }
            else
                randomIndex = Random.Range(0, gamePlayManager.studentsActions.Count - 1);
        }
        return gamePlayManager.studentsActions[randomIndex];
    }
    StudentAction GetRandomStudent2R3(StudentAction randomKid1)
    {
        int randomIndex = Random.Range(0, gamePlayManager.studentsActions.Count - 1);
        bool gotStu = false;
        while (gotStu)
        {
            if (gamePlayManager.studentsActions[randomIndex] != kidLeonie &&
                gamePlayManager.studentsActions[randomIndex] != kidJannik &&
                gamePlayManager.studentsActions[randomIndex] != kidShirin &&
                gamePlayManager.studentsActions[randomIndex] != kidMaxim  &&
                gamePlayManager.studentsActions[randomIndex] != randomKid1)
            {
                gotStu = true;
            }
            else
                randomIndex = Random.Range(0, gamePlayManager.studentsActions.Count - 1);
        }
        return gamePlayManager.studentsActions[randomIndex];
    }

    private void StudentReactionFour()
    {
        // all students except leonie (B7) stu19, Jannik (D7) and maxim  start writing on homework notebook
        // these four students start quity talk to each other vi5 
        // teacher reprimands Jannik audio
        // jannik mood = bad
        // jannik protest mb12 and audio 
        // starts writing
        // delay
        // school bell rings
        // all student put notebook in bag iwo25  
        // leave the classroom
        // these six students start writing in their homework notebook(mb33)
        StartCoroutine(TriggerStudentReactionFour());
    }
    IEnumerator TriggerStudentReactionFour()
    {
        StudentAction RandomKid1, RandomKid2, RandomKid3, RandomKid4;
        RandomKid1 = GetRandomStudent1R3();
        RandomKid2 = RandomKid1.GetComponent<StudentAction>().myNeighbourStudent;
        RandomKid3 = GetRandomStudent1R3();
        RandomKid4 = RandomKid3.GetComponent<StudentAction>().myNeighbourStudent;

        int count = 0;

        //Set student mumbling to high
        audioInClass.volume = 0.4f;
        audioInClass.Play();

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();

            if (gamePlayManager.studentsActions[i] != kidJannik && gamePlayManager.studentsActions[i] != kidMaxim && gamePlayManager.studentsActions[i] != RandomKid1 && gamePlayManager.studentsActions[i] != RandomKid2 && gamePlayManager.studentsActions[i] != RandomKid3 && gamePlayManager.studentsActions[i] != RandomKid4)
            {
                //Only have a total of 10 kids start writing (i.e. not the whole class).  To prevent clustering, take only even-numbered students.
                if (i % 2 == 0 && count < 10) {
                    count += 1;
                    StartCoroutine(SRLoop1(gamePlayManager.studentsActions[i]));
                }
                
            }
            else
            {
                //total of six kids here...
                gamePlayManager.studentsActions[i].studentAnimation.VI11_TalkToFriendsLeftAndRight();
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
            }
        }
        yield return new WaitForSeconds(3f);
        string TeacherTextToSpeak = "Jannik, schreib die Hausaufgaben ab, die Stunde ist gleich zu Ende.";
        if (gamePlayManager.LOG_ENABLED) SpeechManager.Instance.StartTalking(TeacherTextToSpeak);
        yield return new WaitForSeconds(4.0f);
        kidJannik.SetMyMood(MoodIndicator.Bad);
        kidJannik.StopLookAtSomeone();
        kidJannik.StopMyRandomLookingAnimations();
        kidJannik.studentAnimation.ResetAllAnim();
        
        
        kidJannik.StopLookAtSomeone();
        kidJannik.StopMyRandomLookingAnimations();
        kidJannik.studentAnimation.ResetAllAnim();

        kidMaxim.StopLookAtSomeone();
        kidMaxim.StopMyRandomLookingAnimations();
        kidMaxim.studentAnimation.ResetAllAnim();

        RandomKid1.StopLookAtSomeone();
        RandomKid1.StopMyRandomLookingAnimations();
        RandomKid1.studentAnimation.ResetAllAnim();

        if(RandomKid2 != null)
        {
            RandomKid2.StopLookAtSomeone();
            RandomKid2.StopMyRandomLookingAnimations();
            RandomKid2.studentAnimation.ResetAllAnim();
        }

        kidJannik.studentAnimation.MB19_Protest(true);
        string StudentTextToSpeak = "Jahaa";
        if (gamePlayManager.LOG_ENABLED) SpeechManager.Instance.StartTalking(StudentTextToSpeak, true);
        yield return new WaitForSeconds(2.0f);


        kidJannik.studentAnimation.MB33_WorkOnSheets(true);

        
        yield return new WaitForSeconds(6.0f);
        // school bell rings audio
        schoolBellAudio.Play();

        //set talking kids' moods to middle (from good)
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            if (gamePlayManager.studentsActions[i] == kidJannik || gamePlayManager.studentsActions[i] == kidMaxim || gamePlayManager.studentsActions[i] == RandomKid1 || gamePlayManager.studentsActions[i] == RandomKid2 || gamePlayManager.studentsActions[i] == RandomKid3 || gamePlayManager.studentsActions[i] == RandomKid4) {
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
            }
        }

            yield return new WaitForSeconds(2f);
        //kidMaxim.SetMyMood(MoodIndicator.Bad);
        //RandomKid1.SetMyMood(MoodIndicator.Bad);
        //if (RandomKid2 != null)
        //{
        //    RandomKid2.SetMyMood(MoodIndicator.Bad);
        //}

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            StartCoroutine(StudentWalkOutOfClass(gamePlayManager.studentsActions[i]));
        }
    }





    public void Reset()
    {
        //   GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayerToOriginalPostion(true);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint.rotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookingAtTabletAndWrittingAnimations();
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().NoteBook.SetActive(true);
            gamePlayManager.studentsActions[i].SetStudentBag(false);
            //gamePlayManager.studentsActions[i].GoToAndSitInChair();
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
            //StartCoroutine(StudentsBackToSeat(gamePlayManager.studentsActions[i]));
        }
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            StartCoroutine(StudentsBackToSeat(gamePlayManager.studentsActions[i]));
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
