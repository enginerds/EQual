using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class ScenarioSevenManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    public Transform risedStudentLcoaiton, Table4WindowPoint, Table3WindowPoint;
    string questionString = "";


    private List<StudentAction> randomDisturbedStudents, random10WorkingStudents, random9whisperingStudents;
    private List<StudentAction> Table3TalkingKids;
    private List<Transform> Table3TalkingKidsTransform;

    public bool sc11LookatWindowActive;
    Transform sc11LookatWindowPoint;

    public AudioSource table1KidsTalking, table2KidsTalking, table3KidsTalking, table4KidsTalking;

    public GameObject CenterOfSeatsParent, CenterOfSeatsAssignmentPile;
    public List<GameObject> MainClassRoomFurnitureToHide;
    public string CenterChairBaseName = "CircleSetChair";
    //   public PlayableDirector pd;
    public List<StudentAction> SR2RandomNineStudents;
    public List<StudentAction> SR2RestOfStudents;

    public List<StudentAction> SR3RandomTwelveStudents;
    public List<StudentAction> FourStudentsFromTable1and2;
    public List<StudentAction> SR3RestOfStudents;

    public List<StudentAction> SR4Students_2;
    public List<StudentAction> SR4Students_3;
    public List<StudentAction> SR4Students_4;
    public GameObject panel_P1;
    public GameObject panel_P2;
    public GameObject panel_P3;
    public Transform LeonieChair;
    private void Start()
    {
        randomDisturbedStudents = new List<StudentAction>();
        random10WorkingStudents = new List<StudentAction>();
        random9whisperingStudents = new List<StudentAction>();
        Table3TalkingKids = new List<StudentAction>();
        Table3TalkingKidsTransform = new List<Transform>();
        gamePlayManager.userInterfaceManager.ShowOrHideInitLoadingPanel(true);
      //  CenterOfSeatsAssignmentPile.SetActive(false);
        CenterOfSeatsParent.SetActive(false);
        StartCoroutine(StartTheScene());
    }

    IEnumerator StartTheScene()
    {
        yield return new WaitForSeconds(1.0f);
        gamePlayManager.StartWithSittingPos();

        /* not needed as we are already doing this in TriggerInitialAction
        // initiate them as Students will write on shees instead of 
        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
            studentAction.studentScenarioValues.scenario11Start = true;
            //   studentAction.studentAnimation.MB33_WorkOnSheets(true);
        }
        */
        StartCoroutine(TriggerInitialAction());
        yield return new WaitForSeconds(3.0f);
        gamePlayManager.userInterfaceManager.ShowOrHideInitLoadingPanel(false);

        gamePlayManager.userInterfaceManager.ShowOrHideIntroPanel(true);
        yield return new WaitUntil(() => gamePlayManager.InitialIntroComplete);

        gamePlayManager.userInterfaceManager.ShowOrHideKnowYourClassPanel(true);
        gamePlayManager.audioSource.Play();
        yield return new WaitForSeconds(3.0f);
        if (table1KidsTalking.isPlaying) table1KidsTalking.Pause();
        if (table2KidsTalking.isPlaying) table2KidsTalking.Pause();
        if (table3KidsTalking.isPlaying) table3KidsTalking.Pause();
        if (table4KidsTalking.isPlaying) table4KidsTalking.Pause();

        // show the skip button for the ShowOrHideKnowYourClassPanel
        if (gamePlayManager.getToKnowSkipBtn != null) gamePlayManager.getToKnowSkipBtn.gameObject.SetActive(true);

        //  pd.Play(); // play the teacher looking through 180 degrees (approx)
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
        string taKey = (gamePlayManager.userInterfaceManager.noOfNoCounts == 0) ? "TA1" : "TA2";
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData(taKey, value);
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TASQuestion", gamePlayManager.userInterfaceManager.teacherSelectedQuestion);
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TASTimeStarted", gamePlayManager.GetCurrentTime());
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("Revision", gamePlayManager.userInterfaceManager.noOfNoCounts.ToString());

        // print(value + " is about to be initiated");
        switch (value)
        {

            case "1":
                questionString = "Die Organisation des Tagesplans an der Tafel fortsetzen und gleichzeitig die Schülerinnen und Schüler um Ruhe bitten bzw. ermahnen leiser zu sein.";
                Debug.Log("callin SR1");
                StudentReactionOne();
                //gamePlayManager.playerActionDataHandler.PlayerAction = questionString;
                //gamePlayManager.StartPhaseSeven(33.0f); // originally 30.0f, but it is not enough for registering all the action that happens
                break;
            case "2":
                questionString = "Die Organisation des Tagesplans an der Tafel  unterbrechen und sich der Klasse wieder zuwenden. Die störenden Schülerinnen und Schüler anschauen und non-verbal ermahnen.";

                StudentReactionTwo();
                //gamePlayManager.playerActionDataHandler.PlayerAction = questionString;
                //gamePlayManager.StartPhaseSeven(33.0f); // originally 30.0f, but it is not enough for registering all the action that happens
                break;
            case "3":
                questionString = "Die Organisation des Tagesplans an der Tafel fortsetzen und durch „Ich höre, dass ihr redet.“ zu verstehen geben, dass sie leise weiterarbeiten sollen.";

                StudentReactionThree();
                //gamePlayManager.playerActionDataHandler.PlayerAction = questionString;
                //gamePlayManager.StartPhaseSeven(40.0f); // originally 30.0f, but it is not enough for registering all the action that happens
                break;
            case "4":
                questionString = "Die Organisation des Tagesplans an der Tafel  unterbrechen und zu den störenden Schülerinnen und Schülern hingehen. Diese um Ruhe bitten bzw. ermahnen ruhiger zu sein.";
                StudentReactionFour();
                //gamePlayManager.playerActionDataHandler.PlayerAction = questionString;
                //gamePlayManager.StartPhaseSeven(40.0f); // originally 30.0f, but it is not enough for registering all the action that happens

                break;
        }

        gamePlayManager.playerActionDataHandler.PlayerAction = questionString;
        gamePlayManager.StartPhaseSeven(25.0f);
        //showTempText();


    }

    void showTempText()
    {

     //   gamePlayManager.userInterfaceManager.ShowOrHideTempPanel(true);
     //   gamePlayManager.userInterfaceManager.SetTempText(questionString);
//

    }










//    Students sit at their tables and look around at random locations(mb25+mb9/mb15/mb17),
//    7 pairs of students(random locations) quietly whisper with each other(vi11) [soft background mumbling]
    IEnumerator TriggerInitialAction()
    {
        print("Starting InitialAction");

        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
            studentAction.studentScenarioValues.scenario11Start = true;
            yield return new WaitForSeconds(Random.Range(0f, 1f));
//            studentAction.studentAnimation.MB33_WorkOnSheets(true);
            //   studentAction.studentAnimation.MB33_WorkOnSheets(true);
        }
        


        /*
        print("All kids are writing their Worksheets");
        if (gamePlayManager.studentsActions[4].studentAnimation.GetAnimStateBool("vi7_Talk_ON")) gamePlayManager.studentsActions[4].studentAnimation.VI7_TalkToFriendsStop();
        gamePlayManager.studentsActions[4].LookAtWindowRoutine(Table4WindowPoint);
        //  print(gamePlayManager.studentsActions[4].name + " is starting Window Look Routine");
        if (gamePlayManager.studentsActions[7].studentAnimation.GetAnimStateBool("vi7_Talk_ON")) gamePlayManager.studentsActions[7].studentAnimation.VI7_TalkToFriendsStop();
        gamePlayManager.studentsActions[7].LookAtWindowRoutine(Table3WindowPoint);
        // print(gamePlayManager.studentsActions[7].name + " is starting Window Look Routine");





        int probabilityOne, probabilityTwo;

        //Debug.Log(((75 / 100.0)));
        // probabilityOne = (int)((75 / 100.0) * gamePlayManager.studentsActions.Count);
        probabilityOne = (int)((60 / 100.0) * gamePlayManager.studentsActions.Count);

        probabilityTwo = gamePlayManager.studentsActions.Count - probabilityOne;

        print(probabilityTwo.ToString() + " of students are going to do something else");


        //    print("selectiing a few students to talk or look around");
        int[] sampleStudents = new int[Random.Range(2, probabilityTwo)];
        System.Random rnd = new System.Random();
        for (int i = 0; i < sampleStudents.Length; ++i)
        {
            sampleStudents[i] = rnd.Next(probabilityTwo);

        }

        for (int i = 0; i < sampleStudents.Length; i++)
        {
            var whichOne = sampleStudents[i];
            // print(whichOne.ToString());
            if (whichOne != 4 && whichOne != 7)
            {
                //   print(whichOne.ToString() + " is to start look around or talk");
                gamePlayManager.studentsActions[whichOne].LookAroundOrTalkOrWriteRoutine();
                yield return new WaitForSeconds(Random.Range(0f, 2f));
            }

        }


        //Make a few students in table 3 talk

        print("selectiing a few students to talk from table 3");

        var totatStudentsinTable3 = gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3.Count;

        sampleStudents = new int[Random.Range(2, totatStudentsinTable3)];
        rnd = new System.Random();
        for (int i = 0; i < sampleStudents.Length; ++i)
        {
            sampleStudents[i] = rnd.Next(totatStudentsinTable3);

        }
        table3KidsTalking.volume = 0.2f;
        table3KidsTalking.PlayDelayed(2f);
        for (int i = 0; i < sampleStudents.Length; i++)
        {
            var whichOne = sampleStudents[i];
            // print(whichOne.ToString());
            //   print(gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3[whichOne].name +"who is "+ whichOne.ToString()+ " is going to talk.....");
            if (whichOne != 2)
            {
                gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3[whichOne].studentAnimation.VI7_TalkToFriendsLeftAndRight();
                yield return new WaitForSeconds(Random.Range(0f, 2f));
            }
            else
            {
                gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3[whichOne].studentAnimation.VI7_TalkToFriendsStop();
                yield return new WaitForSeconds(Random.Range(0f, 2f));
            }

        }
        */
        yield return new WaitForSeconds(20.0f);
       // pd.Stop();
        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
            studentAction.scenarioStart = false;
            yield return new WaitForSeconds(Random.Range(0f, 1f));
            //            studentAction.studentAnimation.MB33_WorkOnSheets(true);
            //   studentAction.studentAnimation.MB33_WorkOnSheets(true);
        }

        yield return new WaitForSeconds(1.0f);
        print("Finished InitiailAction 1");


    }


    public void MainActionOne()
    {

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);

        //   pd.Play();
        StartCoroutine(TriggerMainActionTwo());
    }
    IEnumerator TriggerMainActionTwo()
    {
        print("Starting MainAction 1");

        CenterOfSeatsParent.SetActive(true);

        foreach (GameObject furniture in MainClassRoomFurnitureToHide)
        {
            furniture.SetActive(false);
        }
        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
            //  studentAction.scenarioStart = false;
            
            studentAction.FindMyChair(CenterChairBaseName);
            studentAction.GoToAndSitInChair(); // Sit on the new chair code
            studentAction.CarryAStudyMaterial(StudyMaterial.Book);
          //  yield return new WaitForSeconds(Random.Range(0f, 1f));
            //            studentAction.studentAnimation.MB33_WorkOnSheets(true);
            //   studentAction.studentAnimation.MB33_WorkOnSheets(true);
        }

/*
        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable1)
        {
      //      StartCoroutine(s.TakeBookFromTable(Table1AssignmentPile.transform));
        }
*/
    
        yield return new WaitForSeconds(15.0f);
        string TextToSpeak = "Guten Morgen alle miteinander. Ich sehe, jeder von euch hat ein Buch mitgebracht. Bitte legt die Bücher jetzt in die Mitte.";

        if (gamePlayManager.LOG_ENABLED) SpeechManager.Instance.StartTalking(TextToSpeak);
        yield return new WaitForSeconds(8.0f);
        //panel_P1.SetActive(true);
        //yield return new WaitForSeconds(2.0f);
        
        print("Finished MainAction 1");
        //gamePlayManager.StartPhaseFour();
        MainActionTwo();

    }



    public void MainActionTwo()
    {

      
        StartCoroutine(TriggerMainActionThree());
    }
    IEnumerator TriggerMainActionThree()
    {
        print("Entering MainAction 2");
        yield return new WaitForSeconds(2.0f);
        //panel_P1.SetActive(false);

        int bookNumber = 0;
        foreach (StudentAction s in gamePlayManager.studentsActions)
        {
           // Transform spot = CenterOfSeatsAssignmentPile.transform;
            GameObject BookToShow = CenterOfSeatsAssignmentPile.transform.GetChild(bookNumber).gameObject;
            Transform spot = BookToShow.transform;
            s.InitiateGoToSpotAndPlaceItem(spot, BookToShow);
            bookNumber++;
            yield return new WaitForSeconds(Random.Range(0.1f,2f));
        }
        
        //        animationCam.SetActive(true);

        yield return new WaitForSeconds(15.0f);

        //Teacher asks question
        string TextToSpeak = "Was ist das besondere an euren Büchern?";

        if (gamePlayManager.LOG_ENABLED)
            SpeechManager.Instance.StartTalking(TextToSpeak);
        yield return new WaitForSeconds(3.0f);

        //show P1 panel
        panel_P1.SetActive(true);
        yield return new WaitForSeconds(2.0f);

        print("MainAction 2 finished");

//        animationCam.SetActive(false);
        gamePlayManager.StartPhaseFive();


    }
    public void MainActionThree()
    {

        StartCoroutine(TriggerTeacherQuestionPhase());
        //Teacher Question Panel

    }
    IEnumerator TriggerTeacherQuestionPhase()
    {

        print("MainAction 3 starts");
        //panel_P2.SetActive(true);
        CenterOfSeatsParent.SetActive(false);

        foreach (GameObject furniture in MainClassRoomFurnitureToHide)
        {
            furniture.SetActive(true);
        }
        //foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        //{
        //
        //    studentAction.FindMyChair("Chair", studentAction.ChairNumber);
        //    studentAction.GoToAndSitInChair(); // Sit on the new chair code
        //    studentAction.CarryAStudyMaterial(StudyMaterial.Book);
        //}
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) // everyone 
        {
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].FindMyChair("Chair", gamePlayManager.studentsActions[i].ChairNumber);
            //gamePlayManager.studentsActions[i].StopLookAtSomeone();
            //gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
            if(i == 16)
            {
                gamePlayManager.studentsActions[i].chairPoint = LeonieChair;
            }
            StartCoroutine(StudentsBackToSeat(gamePlayManager.studentsActions[i]));

        }
        yield return new WaitForSeconds(2.0f);
        CenterOfSeatsAssignmentPile.SetActive(false);
        //panel_P2.SetActive(false);
        panel_P1.SetActive(false);
        yield return new WaitForSeconds(10.0f);
        print("Finished MainAction 3");
        gamePlayManager.StartPhaseSix();
      //  pd.Stop(); // stop the teacher looking through 180 degrees (approx)

    }
    IEnumerator StudentsBackToSeat(StudentAction stu)
    {
        stu.gameObject.SetActive(false);
        stu.gameObject.transform.position = stu.chairPoint.position;
        stu.gameObject.SetActive(true);
        stu.GoToAndSitInChair();
        yield return new WaitForSeconds(2f);
    }

    IEnumerator TriggerSRInitialActions(int whoCalled)
    {

        print("Initial Actions for SR starts");

        //make the scene ready for SR actions

        //switch off all the kids talking sounds
        if (table1KidsTalking.isPlaying) table1KidsTalking.Stop();
        if (table2KidsTalking.isPlaying) table2KidsTalking.Stop();
        if (table4KidsTalking.isPlaying) table4KidsTalking.Stop();

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayerToOriginalPostion(true);

        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);


        print("All students except table 3, resume working on their worksheets");
        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
            studentAction.LookAtWindowRoutineStop();
            studentAction.LookAroundOrTalkOrWriteRoutineStop();
            // studentAction.studentAnimation.ResetAllAnim();
            if (!gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3.Contains(studentAction))
            {
                if (studentAction.studentAnimation.GetAnimStateBool("vi7_Talk_ON")) studentAction.studentAnimation.VI7_TalkToFriendsStop();
                studentAction.studentAnimation.MB33_WorkOnSheets(true);
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        }
        print("selectiing a few students to talk from table 3");
        var totatStudentsinTable3 = gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3.Count;
        var sampleStudents = new int[Random.Range(2, totatStudentsinTable3)];
        var rnd = new System.Random();
        for (int i = 0; i < sampleStudents.Length; ++i)
        {
            sampleStudents[i] = rnd.Next(totatStudentsinTable3);
            //           print("selected student from table 3 to talk is " + sampleStudents[i].ToString() + " who is "+ gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3[sampleStudents[i]].name);

        }
        Table3TalkingKids.Clear();
        Table3TalkingKidsTransform.Clear();


        //if (whoCalled == 3)
        //{
        //    table1KidsTalking.volume = 0.05f; // just mumbling
        //    table2KidsTalking.volume = 0.05f; // just mumbling
        //    table4KidsTalking.volume = 0.05f; // just mumbling
        //    table1KidsTalking.PlayDelayed(Random.Range(0.5f,1f));
        //    table2KidsTalking.PlayDelayed(Random.Range(0.5f, 1f));
        //    table4KidsTalking.PlayDelayed(Random.Range(0.5f, 1f));
        //}

        if (!table3KidsTalking.isPlaying)
        {
            table3KidsTalking.volume = 0.2f;
            table3KidsTalking.PlayDelayed(1f);
        }


        for (int i = 0; i < sampleStudents.Length; i++)
        {
            // print( sampleStudents[i].ToString() + " who is " + gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3[sampleStudents[i]].name + "start to Talk");

            Table3TalkingKids.Add(gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3[i]);
            Table3TalkingKidsTransform.Add(gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3[i].transform);
            gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3[sampleStudents[i]].studentAnimation.MB33_WorkOnSheets(false);
            yield return null;
            gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3[sampleStudents[i]].studentAnimation.VI7_TalkToFriendsLeftAndRight();
            yield return new WaitForSeconds(Random.Range(0.1f, 1f));
        }

        print("Finished Initial Actions");
        yield return new WaitForSeconds(2.0f);

        switch (whoCalled)
        {
            case 1:
                StartCoroutine(StudentReactionOneInitiate());
                break;
            case 2:
                StartCoroutine(StudentReactionTwoInitiate());
                break;
            case 3:
                StartCoroutine(StudentReactionThreeInitiate());
                break;
            case 4:
                StartCoroutine(StudentReactionFourInitiate());
                break;
        }

    }


    private void StudentReactionOne()
    {
        print("SR 1 should start anytime ");
        //StartCoroutine(TriggerSRInitialActions(1));
        StartCoroutine(TriggerStudentReactionOne());
    }
    IEnumerator TriggerStudentReactionOne()
    {
        panel_P3.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        panel_P3.SetActive(false);

        //set student mumbling to low
        SetStudentMumbling(0.0f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
            if (i < 15)
            {
                StartCoroutine(RaiseHandWithDelay(gamePlayManager.studentsActions[i]));
            }
        }
    }

    IEnumerator RaiseHandWithDelay(StudentAction stu)
    {
        yield return new WaitForSeconds(Random.Range(1f, 4.0f));
        stu.RaiseHand();
    }


    IEnumerator StudentReactionOneInitiate()
    {
        print("SR 1 started");
        yield return new WaitForSeconds(2.0f);
        if (table3KidsTalking.isPlaying) table3KidsTalking.Stop();
        print("Talking students of Table 3 realise teacher is looking at them and start looking at teacher");
        foreach (StudentAction studentAction in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3)
        {
            studentAction.LookAtWindowRoutineStop();
            studentAction.LookAroundOrTalkOrWriteRoutineStop();
            // studentAction.studentAnimation.ResetAllAnim();
            if (studentAction.studentAnimation.GetAnimStateBool("vi7_Talk_ON"))
            {
                studentAction.studentAnimation.VI7_TalkToFriendsStop();
                var teacherPositon = GameObject.FindGameObjectWithTag("Player").transform;
                studentAction.LookAtSomeone(teacherPositon);
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        }
        yield return new WaitForSeconds(4.0f);
        print(" students of Table 3 looking at teacher start to work on their workhseets");
        foreach (StudentAction studentAction in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3)
        {

            if (studentAction.lookingAtSomeone)
            {
                studentAction.StopLookAtSomeone();
                if (!studentAction.studentAnimation.GetAnimStateBool("mb33_WriteOnSheet_ON")) studentAction.studentAnimation.MB33_WorkOnSheets(true);
            }
            yield return new WaitForSeconds(Random.Range(0f, 1.0f));
        }

        yield return new WaitForSeconds(4f);
        print("SR 1 finished");
    }


    //MB26_FoldHands 
    private void StudentReactionTwo()
    {
        print("SR 2 should start anytime ");
        //StartCoroutine(TriggerSRInitialActions(2));
        StartCoroutine(TriggerStudentReactionTwo());
    }
    IEnumerator TriggerStudentReactionTwo()
    {
        panel_P3.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        panel_P3.SetActive(false);

        //set student mumbling to low
        SetStudentMumbling(0.0f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
        }
        for (int i = 0; i < SR2RandomNineStudents.Count; i++)
        {
            //SR2RandomNineStudents[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
            StartCoroutine(RaiseHandWithDelay(SR2RandomNineStudents[i]));
        }
        for (int i = 0; i < SR2RestOfStudents.Count; i++)
        {
            SR2RestOfStudents[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
        }
        gamePlayManager.studentsActions[0].studentAnimation.MB26_FoldHands(true);
        gamePlayManager.studentsActions[16].studentAnimation.MB26_FoldHands(true);

        //set class' mood
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            if (i == 0) {
                //Jannik's mood
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
            } else {
                //rest of class
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
            }
        }
    }

    IEnumerator Table3StudentLooksAtTeacherAndStartWriting(StudentAction whoIs)
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        if (table3KidsTalking.isPlaying) table3KidsTalking.Stop();
        //  print(whoIs.name + "Realises that students are looking at them and looks at Teacher");
        whoIs.studentAnimation.VI7_TalkToFriendsStop();
        whoIs.studentAnimation.VI11_TalkToFriendsStop();
        whoIs.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
        yield return new WaitForSeconds(3f);
        whoIs.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(5f);
    }




    IEnumerator StudentReactionTwoInitiate()
    {

        print("SR 2 started");

        randomDisturbedStudents.Clear();
        print("Table 3 students keep talking...");
        yield return new WaitForSeconds(5f);

        int randomDisturbStudentNumber = 1;
        print("Selecting 3 random students (not from table 3) to get distrubed...");
        // we need only 3 distrubed students.
        while (randomDisturbStudentNumber < 4)
        {
            var whomTo = Random.Range(0, gamePlayManager.studentsActions.Count);
            // check if the selected student is not form Table 3
            if (!gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3.Contains(gamePlayManager.studentsActions[whomTo]))
            {
                // check if the selected student is not Already in selected list
                if (!randomDisturbedStudents.Contains(gamePlayManager.studentsActions[whomTo]))
                {
                    randomDisturbedStudents.Add(gamePlayManager.studentsActions[whomTo]);
                    randomDisturbStudentNumber++;
                }
            }
            yield return null;

        }

        print("3 random writing students are gpoing to look at the talking students as they get disturbed");
        foreach (StudentAction studentAction in randomDisturbedStudents)
        {
            studentAction.studentAnimation.MB33_WorkOnSheets(false);
            var whomToLookAt = Random.Range(0, Table3TalkingKidsTransform.Count);
            studentAction.LookAtSomeone(Table3TalkingKidsTransform[whomToLookAt]);
            StartCoroutine(Table3StudentLooksAtTeacherAndStartWriting(Table3TalkingKids[whomToLookAt]));
            yield return new WaitForSeconds(Random.Range(1f, 2f));
        }

        yield return new WaitForSeconds(2.0f);

        print("Students are back to working on sheets");
        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
            studentAction.StopLookAtSomeone();
            studentAction.studentAnimation.VI7_TalkToFriendsStop();
            studentAction.studentAnimation.VI11_TalkToFriendsStop();
            studentAction.studentAnimation.MB33_WorkOnSheets(true);
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
        }

        yield return new WaitForSeconds(4f);
        print("SR 2 finished");

    }



    private void StudentReactionThree()
    {
        print("SR 3 should start anytime ");
        //StartCoroutine(TriggerSRInitialActions(3));
        StartCoroutine(TriggerStudentReactionThree());
    }

    private void SetStudentMumbling(float volume) {
        table1KidsTalking.volume = volume;
        table1KidsTalking.Play();

        table2KidsTalking.volume = volume;
        table2KidsTalking.Play();

        table3KidsTalking.volume = volume;
        table3KidsTalking.Play();

        table4KidsTalking.volume = volume;
        table4KidsTalking.Play();
    }

    IEnumerator TriggerStudentReactionThree()
    {
        panel_P3.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        panel_P3.SetActive(false);

        //set student mumbling to low
        SetStudentMumbling(0.1f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
        }
        for (int i = 0; i < SR3RandomTwelveStudents.Count; i++)
        {
            SR3RandomTwelveStudents[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
            StartCoroutine(RaiseHandWithDelay(SR3RandomTwelveStudents[i]));
            SR3RandomTwelveStudents[i].SetMyMood(MoodIndicator.Good);

        }
        for (int i = 0; i < FourStudentsFromTable1and2.Count; i++)
        {
            FourStudentsFromTable1and2[i].studentAnimation.MB26_FoldHands(true);
            FourStudentsFromTable1and2[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
            FourStudentsFromTable1and2[i].SetMyMood(MoodIndicator.Middle);
        }
        for (int i = 0; i < SR3RestOfStudents.Count; i++)
        {
            //SR3RestOfStudents[i].LookAtWindowRoutine();

            //look out window
            SR3RestOfStudents[i].studentAnimation.ResetAllAnim();
            SR3RestOfStudents[i].LookAtSomeone(GameObject.Find("Table4WindowPoint").transform);
            SR3RestOfStudents[i].SetMyMood(MoodIndicator.Middle);
        }
        
    }


    IEnumerator StudentReactionThreeInitiate()
    {

        print("SR 3 started");
        random10WorkingStudents.Clear();
        random9whisperingStudents.Clear();
        yield return new WaitForSeconds(3f);
        print("All students stop what they are doing and look at the teacher in random timing");
        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
            studentAction.LookAtWindowRoutineStop();
            studentAction.LookAroundOrTalkOrWriteRoutineStop();
            if (studentAction.studentAnimation.GetAnimStateBool("vi7_Talk_ON")) studentAction.studentAnimation.VI7_TalkToFriendsStop();
            if (studentAction.studentAnimation.GetAnimStateBool("mb33_WriteOnSheet_ON")) studentAction.studentAnimation.MB33_WorkOnSheets(false);
            var teacherPositon = GameObject.FindGameObjectWithTag("Player").transform;
            studentAction.LookAtSomeone(teacherPositon);
            yield return new WaitForSeconds(0.5f);
        }
        print("mumbling raise for 5 seconds to 50%");

        yield return new WaitForSeconds(3f);
        print("fades back to low");


        print("Around 10 students start to work");
        print("Around 6 to 9 students start to work and whisper in between");
        var sampleStudents = new int[19];

        var rnd = new System.Random();
        for (int i = 0; i < sampleStudents.Length; ++i)
        {
            var whichOne = rnd.Next(gamePlayManager.studentsActions.Count);

            if (!Table3TalkingKids.Contains(gamePlayManager.studentsActions[whichOne]))
                if (whichOne % 2 == 0)
                {
                    gamePlayManager.studentsActions[whichOne].StopLookAtSomeone();
                    gamePlayManager.studentsActions[whichOne].studentAnimation.MB33_WorkOnSheets(true);
                    random10WorkingStudents.Add(gamePlayManager.studentsActions[whichOne]);
                    yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
                }
                else
                {
                    gamePlayManager.studentsActions[whichOne].StopLookAtSomeone();
                    gamePlayManager.studentsActions[whichOne].WisperOrWriteRoutine();
                    random9whisperingStudents.Add(gamePlayManager.studentsActions[whichOne]);
                    yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
                }
        }
        if (!table1KidsTalking.isPlaying) { table1KidsTalking.volume = 0.1f; table1KidsTalking.PlayDelayed(1f); }
        if (!table2KidsTalking.isPlaying) { table2KidsTalking.volume = 0.1f; table2KidsTalking.PlayDelayed(1f); }
        if (!table4KidsTalking.isPlaying) { table4KidsTalking.volume = 0.1f; table4KidsTalking.PlayDelayed(1f); }

        //  gamePlayManager.studentsActions[whichOne].Scenario11LookatShakeHeadsOrWriteRoutine(Table3TalkingKidsTransform[Random.Range(0, Table3TalkingKidsTransform.Count)]);

        sampleStudents = new int[Random.Range(4, 6)];
        rnd = new System.Random();
        for (int i = 0; i < sampleStudents.Length; ++i)
        {
            sampleStudents[i] = rnd.Next(random10WorkingStudents.Count);
            //           print("selected student from table 3 to talk is " + sampleStudents[i].ToString() + " who is "+ gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3[sampleStudents[i]].name);

        }
        print("Around 4 to 5 out of 10 working students gets interuppted and looks at the Table 3 talking students and shake their heads occasionally and then go to work");
        for (int i = 0; i < sampleStudents.Length; i++)
        {
            gamePlayManager.studentsActions[sampleStudents[i]].LookatShakeHeadsOrWriteRoutine(Table3TalkingKidsTransform[Random.Range(0, Table3TalkingKidsTransform.Count)]);
            yield return new WaitForSeconds(Random.Range(0.1f, 1f));
        }

        yield return new WaitForSeconds(5f);

        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
            studentAction.LookAtWindowRoutineStop();
            studentAction.LookAroundOrTalkOrWriteRoutineStop();
            studentAction.LookatShakeHeadsOrWriteRoutineStop();
            studentAction.WisperOrWriteRoutineStop();
            if (studentAction.studentAnimation.GetAnimStateBool("vi7_Talk_ON")) studentAction.studentAnimation.VI7_TalkToFriendsStop();
            if (!studentAction.studentAnimation.GetAnimStateBool("mb33_WriteOnSheet_ON")) studentAction.studentAnimation.MB33_WorkOnSheets(true);

            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
        print("SR 3 Complete");


        yield return new WaitForSeconds(0.1f);
    }


    private void StudentReactionFour()
    {
        print("SR 4 should start anytime ");
        //StartCoroutine(TriggerSRInitialActions(4));
        StartCoroutine(TriggerStudentReactionFour());
    }
    IEnumerator TriggerStudentReactionFour()
    {
        panel_P3.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        panel_P3.SetActive(false);

        //set student mumbling to low
        SetStudentMumbling(0.25f);

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
        }
        for (int i = 0; i < SR3RandomTwelveStudents.Count - 1; i++)
        {
            SR3RandomTwelveStudents[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
            StartCoroutine(RaiseHandWithDelay(SR3RandomTwelveStudents[i]));
            SR3RandomTwelveStudents[i].SetMyMood(MoodIndicator.Good);
        }
        gamePlayManager.studentsActions[16].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
        StartCoroutine(RaiseHandWithDelay(gamePlayManager.studentsActions[16]));
        gamePlayManager.studentsActions[16].SetMyMood(MoodIndicator.Good);

        for (int i = 0; i < SR4Students_2.Count; i++)
        {
            //look at support material
            SR4Students_2[i].LookAtSomeone(SR4Students_2[i].chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);
            SR4Students_2[i].SetMyMood(MoodIndicator.Middle);
        }
        for (int i = 0; i < SR4Students_3.Count; i++)
        {
            //SR4Students_3[i].LookAtWindowRoutine();

            //look out window
            SR4Students_3[i].studentAnimation.ResetAllAnim();
            SR4Students_3[i].LookAtSomeone(GameObject.Find("Table4WindowPoint").transform);
            SR4Students_3[i].SetMyMood(MoodIndicator.Middle);
        }
        for (int i = 0; i < SR4Students_4.Count; i++)
        {
            //SR4Students_4[i].LookAtWindowRoutine();

            //look at teacher
            SR4Students_4[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
            SR4Students_4[i].SetMyMood(MoodIndicator.Middle);
        }
        
        //set Maxim's, Leonie's, and Jannik's mood
        gamePlayManager.studentsActions[3].SetMyMood(MoodIndicator.Middle); //maxim
        gamePlayManager.studentsActions[16].SetMyMood(MoodIndicator.Middle); //leonie
        gamePlayManager.studentsActions[0].SetMyMood(MoodIndicator.Bad); //leonie
    }

    IEnumerator StudentReactionFourInitiate()
    {
        print("SR 4 started");
        int randomWritingStudent = Random.Range(0, gamePlayManager.studentsActions.Count);
        gamePlayManager.studentsActions[randomWritingStudent].studentAnimation.MB33_WorkOnSheets(true);



        yield return new WaitForSeconds(2f);
        print("Teacher goes to Table 3 and looks at the students there");

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table3);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table3MoveToPoint);
        if (table1KidsTalking.isPlaying) { table1KidsTalking.Stop(); }
        if (table2KidsTalking.isPlaying) { table2KidsTalking.Stop(); }
        if (table3KidsTalking.isPlaying) { table3KidsTalking.Stop(); }
        if (table4KidsTalking.isPlaying) { table4KidsTalking.Stop(); }

        print("All students except the writing one look at the teacher");
        // int table3talkingstudentsCount= 1;
        // StudentAction table3student1toTalk, table3student2toTalk, table3student3toTalk;
        // table3student1toTalk = null;
        // table3student2toTalk = null;
        // table3student3toTalk = null;
        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
            studentAction.LookAtWindowRoutineStop();
            studentAction.LookAroundOrTalkOrWriteRoutineStop();
            // studentAction.studentAnimation.ResetAllAnim();
            if (studentAction != gamePlayManager.studentsActions[randomWritingStudent])
            {
                if (studentAction.studentAnimation.GetAnimStateBool("vi7_Talk_ON")) studentAction.studentAnimation.VI7_TalkToFriendsStop();
                if (studentAction.studentAnimation.GetAnimStateBool("mb33_WriteOnSheet_ON")) studentAction.studentAnimation.MB33_WorkOnSheets(false);

                var teacherPositon = GameObject.FindGameObjectWithTag("Player").transform;
                studentAction.LookAtSomeone(teacherPositon);
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        }


        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);

        print("Teacher looks at table 3 students");
        yield return new WaitForSeconds(3f);

        print("Teacher goes back to original place");
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().l;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayerToOriginalPostion(true);
        yield return new WaitForSeconds(1.5f);

        print("Talking students at Table 3 start talking back");
        foreach (StudentAction studentAction in Table3TalkingKids)
        {
            studentAction.studentAnimation.VI7_TalkToFriendsLeftAndRight();
            yield return null;
        }
        if (!table3KidsTalking.isPlaying) { table1KidsTalking.volume = 0.2f; table3KidsTalking.PlayDelayed(1f); }
        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);


        print("Other students start to work");
        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
            studentAction.LookAtWindowRoutineStop();
            studentAction.LookAroundOrTalkOrWriteRoutineStop();
            // studentAction.studentAnimation.ResetAllAnim();
            if (studentAction != gamePlayManager.studentsActions[randomWritingStudent])
            {
                studentAction.StopLookAtSomeone();
                if (!gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3.Contains(studentAction))
                {
                    if (!Table3TalkingKids.Contains(studentAction))
                    {
                        studentAction.studentAnimation.MB33_WorkOnSheets(true);
                    }

                }
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        }
        print("14 of the students that work wispher in pairs");
        var sampleStudents = new int[Random.Range(0, 15)];
        var rnd = new System.Random();
        for (int i = 0; i < sampleStudents.Length - 1; i += 2)
        {
            sampleStudents[i] = rnd.Next(gamePlayManager.studentsAsNeighboursActions.Count);
            gamePlayManager.studentsAsNeighboursActions[sampleStudents[i]].studentAnimation.MB33_WorkOnSheets(false);
            yield return null;
            gamePlayManager.studentsAsNeighboursActions[sampleStudents[i]].studentAnimation.VI11_TalkToFriendsLeftAndRight();
            if (sampleStudents[i] + 1 < gamePlayManager.studentsAsNeighboursActions.Count)
            {
                gamePlayManager.studentsAsNeighboursActions[sampleStudents[i] + 1].studentAnimation.MB33_WorkOnSheets(false);
                yield return null;
                gamePlayManager.studentsAsNeighboursActions[sampleStudents[i] + 1].studentAnimation.VI11_TalkToFriendsLeftAndRight();
            }
            if (!table1KidsTalking.isPlaying) { table1KidsTalking.volume = 0.1f; table1KidsTalking.PlayDelayed(1f); }
            if (!table2KidsTalking.isPlaying) { table2KidsTalking.volume = 0.1f; table2KidsTalking.PlayDelayed(1f); }
            if (!table4KidsTalking.isPlaying) { table4KidsTalking.volume = 0.1f; table4KidsTalking.PlayDelayed(1f); }
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            //           print("selected student from table 3 to talk is " + sampleStudents[i].ToString() + " who is "+ gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3[sampleStudents[i]].name);

        }



        yield return new WaitForSeconds(6f);


        print("SR 4 Complete");
        yield return null;
    }


    public void Reset()
    {
        
        //Reset Anim
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayerToOriginalPostion(true);
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
        }

        /*
        StartCoroutine(TriggerInitialAction());
        
        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
            studentAction.studentAnimation.ReadingPaperOnDesk(false);
            studentAction.studentAnimation.RaiseHand(false);
            studentAction.studentAnimation.Upset(false);
        }





        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {

            studentAction.studentAnimation.SitAgitated(true);
            studentAction.studentAnimation.ReadingPaperOnDesk(true);
            
        }



        gamePlayManager.studentsActions[20].studentAnimation.ReadingPaperOnDesk(false);
        gamePlayManager.studentsActions[20].studentAnimation.SitAgitated(false);
       

        gamePlayManager.studentsActions[21].studentAnimation.ReadingPaperOnDesk(false);
        gamePlayManager.studentsActions[21].studentAnimation.SitAgitated(false);
       


        gamePlayManager.studentsActions[7].studentAnimation.ReadingPaperOnDesk(false);
        gamePlayManager.studentsActions[7].studentAnimation.SitAgitated(false);
       


        gamePlayManager.studentsActions[8].studentAnimation.ReadingPaperOnDesk(false);
        gamePlayManager.studentsActions[8].studentAnimation.SitAgitated(false);
        

        gamePlayManager.studentsActions[15].studentAnimation.ReadingPaperOnDesk(false);
        gamePlayManager.studentsActions[15].studentAnimation.SitAgitated(false);


        gamePlayManager.studentsActions[16].studentAnimation.ReadingPaperOnDesk(false);
        gamePlayManager.studentsActions[16].studentAnimation.SitAgitated(false);




        gamePlayManager.studentsActions[9].studentAnimation.ReadingPaperOnDesk(false);
        gamePlayManager.studentsActions[13].studentAnimation.ReadingPaperOnDesk(false);


        gamePlayManager.studentsActions[9].studentAnimation.RaiseHand(true);
        gamePlayManager.studentsActions[13].studentAnimation.RaiseHand(true);


    */
        gamePlayManager.StartPhaseSix();
    }

}
