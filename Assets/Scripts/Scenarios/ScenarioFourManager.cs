using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class ScenarioFourManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    public Transform risedStudentLcoaiton;

    public List<DeskDetails> desksInClass;

    public AudioSource audioInClass;

    string questionString = "";


    public List<int> studentsThatTalkInTable3, studentsThatTalkInTable4, studentsThatLookAtTalkingStudentsInTable3, studentsThatLookAtTalkingStudentsInTable4;

    private List<StudentAction> studentsToLookAtTable1, studentsToLookAtTable2, studentsToLookAtTable3, studentsToLookAtTable4;
    private List<Transform> SelectedTablePointsToGo;

    private bool audioVolumeChange, firstTimeSREnterance;
    private float targetVolume;
    private void Awake()
    {
        gamePlayManager.currentScenario = "SC4";
    }

    private void Start()
    {
        gamePlayManager.initialActionForScenarioIsCommon = true;
        studentsToLookAtTable1 = new List<StudentAction>();
        studentsToLookAtTable2 = new List<StudentAction>();
        studentsToLookAtTable3 = new List<StudentAction>();
        studentsToLookAtTable4 = new List<StudentAction>();
        SelectedTablePointsToGo = new List<Transform>();
        gamePlayManager.userInterfaceManager.ShowOrHideInitLoadingPanel(true);
        StartCoroutine(StartTheScene());
    }


    private void Update()
    {
        if(audioVolumeChange)
        {
            if (!audioInClass.isPlaying) audioInClass.Play();
            if(audioInClass.volume < targetVolume )
                audioInClass.volume = Mathf.Lerp(audioInClass.volume, targetVolume, 0.1f * Time.deltaTime);
            else
                audioInClass.volume = Mathf.Lerp(targetVolume, audioInClass.volume, 0.1f * Time.deltaTime);
            if (audioInClass.volume >= targetVolume) audioVolumeChange = false;
        }
        
    }


    IEnumerator StartTheScene()
    {
        yield return new WaitForSeconds(1.0f);
        gamePlayManager.StartWithSittingPos();

        firstTimeSREnterance = true;
        yield return new WaitForSeconds(3.0f);
        gamePlayManager.userInterfaceManager.ShowOrHideInitLoadingPanel(false);

        gamePlayManager.userInterfaceManager.ShowOrHideIntroPanel(true);
        yield return new WaitUntil(() => gamePlayManager.InitialIntroComplete);

        gamePlayManager.userInterfaceManager.ShowOrHideKnowYourClassPanel(true);
        gamePlayManager.audioSource.Play();
        yield return new WaitForSeconds(3.0f);
        if (audioInClass.isPlaying) audioInClass.Pause();
        // show the skip button for the ShowOrHideKnowYourClassPanel
        if (gamePlayManager.getToKnowSkipBtn != null) gamePlayManager.getToKnowSkipBtn.gameObject.SetActive(true);
       
        // SpeechManager.Instance.StartTalking("jetzt ist die Zeit", SpeechManager.Instance.SpeakingFinished); // test speech
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

        print(value);
        switch (value)
        {

            case "1":
                questionString = "In Richtung der sich meldenden Kinder gehen. Auf dem Weg durch die Klasse in der Nähe der störenden Kinder langsamer werden und diese mit Hilfe von Blickkontakt ermahnen.";
                print("Question asked = " + questionString);
                break;
            case "2":
                questionString = "Die störenden Kinder anschauen. Dabei den Blick so lange auf diese richten, bis sie den Blick erwidern, um ihnen non-verbal eine Verwarnung zu vermitteln und sie an ihre Aufgaben zu erinnern. Anschließend zu den sich meldenden Kindern hingehen.";
                print("Question asked = " + questionString);
                 break;
            case "3":
                questionString = "Auf direktem Weg zu den störenden Kindern hingehen, diese individuell ermahnen und an die Arbeitsphase erinnern. Anschließend zu den sich meldenden Kindern gehen.";
                print("Question asked = " + questionString);
                  break;
            case "4":
                questionString = "Die störenden Kinder mündlich ermahnen sich zu konzentrieren und dabei auch die gesamte Klasse an die Klassenregel erinnern, leise zu arbeiten. Danach zu den sich meldenden Kindern gehen.";
                print("Question asked = " + questionString);
                 break;
        }


        // start initial trigger action and then go to each SRs
        StartCoroutine(TriggerInitialAction(value));

      
       

        //showTempText();
        gamePlayManager.playerActionDataHandler.PlayerAction = questionString;


    }

    void showTempText()
    {

    //    gamePlayManager.userInterfaceManager.ShowOrHideTempPanel(true);
    //    gamePlayManager.userInterfaceManager.SetTempText(questionString);


    }






    IEnumerator TriggerInitialAction(string value)
    {
      
        GameObject.FindObjectOfType<CameraFOVController>().ReSetFOV();

        if (!firstTimeSREnterance)
        {
            print("Starting InitialAction");
            yield return StartCoroutine(TriggerInitialActionStart());
            print("Finished InitiailAction ");
        }

        switch (value)
        {

            case "1":
                   yield return StartCoroutine(StudentReactionOneEnum());
                //StudentReactionOne();
                break;
            case "2":
                yield return StartCoroutine(StudentReactionTwoEnum());
                // StudentReactionTwo();
                break;
            case "3":
                yield return StartCoroutine(StudentReactionThreeEnum());
                //StudentReactionThree();
                break;
            case "4":
                yield return StartCoroutine(StudentReactionFourEnum());
               // StudentReactionFour();

                break;
        }

        firstTimeSREnterance = false;
        yield return new WaitForSeconds(5.0f);
        
        gamePlayManager.StartPhaseSeven(0.1f);

    }



    IEnumerator TriggerInitialActionStart()
    {


        GameObject.FindGameObjectWithTag("Player").GetComponent<RigidbodyFirstPersonController>().MoveLocked = true;
        GameObject.FindObjectOfType<CameraFOVController>().ReSetFOV();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1MoveToPoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().Table1MoveToPoint.rotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1);

        foreach (StudentAction s in gamePlayManager.studentsActions)
        {
            s.StopLookAtSomeone();
            s.SetMyMood(MoodIndicator.Good);
            s.ShowMyMoodNow(true);
            s.studentAnimation.MB30_PeepToSideLeftOrRight(true);
            yield return new WaitForSeconds(Random.Range(0f, 0.1f));
            s.StopLookAtSomeone();
        }
        gamePlayManager.studentsActions[20].StartMyTalkOrLaughAnimations();
        gamePlayManager.studentsActions[20].myNeighbourStudent.StartMyTalkOrLaughAnimations();
        gamePlayManager.studentsActions[4].StartMyTalkOrLaughAnimations();
        gamePlayManager.studentsActions[3].StartMyTalkOrLaughAnimations();

        gamePlayManager.studentsActions[10].studentAnimation.RaiseHand(true);
        gamePlayManager.studentsActions[12].studentAnimation.RaiseHand(true);

        for (int i = 0; i < studentsThatLookAtTalkingStudentsInTable3.Count; i++)
        {
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable3[i]].StartMyLookAtShakeHeadAndWriteAnimations(gamePlayManager.studentsActions[studentsThatTalkInTable3[i]].transform);
            yield return new WaitForSeconds(Random.Range(0f, 0.1f));
        }

        for (int i = 0; i < studentsThatLookAtTalkingStudentsInTable4.Count; i++)
        {
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable4[i]].StartMyLookAtShakeHeadAndWriteAnimations(gamePlayManager.studentsActions[studentsThatTalkInTable4[i]].transform);
            yield return new WaitForSeconds(Random.Range(0f, 0.1f));
        }
        targetVolume = 0.65f; // 25 percent
        audioVolumeChange = true;


        yield return null;

    }








    public void MainActionOne()
    {


        if (!audioInClass.isPlaying) audioInClass.Play();
        // Students are working on their working sheet with their partner MB30 , Sticking heads togeahter and quitely whispering
        // Teacher standing at table 1 B7 and can look around , no movement

        // start the desk itens to be visible
        if (desksInClass != null && desksInClass.Count > 0)
            foreach (DeskDetails dsk in desksInClass)
                dsk.EnableMyItems(true);

            StartCoroutine(TriggerMainActionOne());
    }
    IEnumerator TriggerMainActionOne()
    {
        print("Starting MainAction 1");
        // lock movement of the teacher
        GameObject.FindGameObjectWithTag("Player").GetComponent<RigidbodyFirstPersonController>().MoveLocked = true;
        
       GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1MoveToPoint);
      //  GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().Table1MoveToPoint.rotation;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1);
        

        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
           
            studentAction.StopSittingIdleLookAroundAnything(); // stop the initial intro animations for all kids
            studentAction.scenarioStart = false;
            studentAction.StopLookAtSomeone();
            studentAction.studentAnimation.MB30_PeepToSideLeftOrRight(true);
            yield return new WaitForSeconds(Random.Range(0f, 0.5f));
            studentAction.StopLookAtSomeone();
        }
        
        yield return new WaitForSeconds(10.0f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isLookAround = false;
        print("Finished MainAction 1");

       gamePlayManager.StartPhaseFour();


    }

    public void MainActionTwo()
    {
        // Teacher looks at tables 3 and 4 one by one
        // Teacher standing at table 1 B7
        // Teacher control is restricted, so only Guided camera movements

        // One pair of students among them an SN-ESE , in field of view stops working and talk to each other loudly Vi7 + ee4
        // Audio volume slowly increasing to 50%,
        // All students in table 4 talk to each other

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1);

        StartCoroutine(TriggerMainActionTwo());
    }


    IEnumerator TriggerMainActionTwo()
    {
        print("Entering MainAction 2");
      //  var totalWaittime = 0.0f;

        yield return new WaitForSeconds(Random.Range(0f, 1.5f));

        print("Player looks at Table3");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table3);
        GameObject.FindObjectOfType<CameraFOVController>().SetFOV(20);
        yield return new WaitForSeconds(Random.Range(2f, 4f));
        print("ESE Student Evan and his neighbour starts talking");
        // both ESE student(Ivan) and his neighbour in Table 3 starts talking and laughin (only ESE)
        gamePlayManager.studentsActions[20].StartMyTalkOrLaughAnimations();
        gamePlayManager.studentsActions[20].myNeighbourStudent.StartMyTalkOrLaughAnimations();

        // add both talking students into a list so later we can catch them for any work
        studentsThatTalkInTable3.Add(20);
        studentsThatTalkInTable3.Add(gamePlayManager.studentsActions.FindIndex(sa => sa == gamePlayManager.studentsActions[20].myNeighbourStudent));


        // as time is not there to do random students selection properly, i am randomly selecting which student will act what.
        studentsThatLookAtTalkingStudentsInTable3.Add(6); // Elias as they are sitting close to the above students
        studentsThatLookAtTalkingStudentsInTable3.Add(14); // Shirin as they are sitting close to the above students

        print("Volume increases to 25%");
        targetVolume = 0.25f; // 25 percent
        audioVolumeChange = true;

       

        yield return new WaitForSeconds(Random.Range(5f, 7f));

        // selecting Julian and Niklas to talk in table 4

        print("Player looks at Table4");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);
        print("Students Juian and Niklas of table 4 starts talking");

        yield return new WaitForSeconds(Random.Range(1f, 2f));

        gamePlayManager.studentsActions[4].StartMyTalkOrLaughAnimations();
        gamePlayManager.studentsActions[3].StartMyTalkOrLaughAnimations();
        studentsThatTalkInTable4.Add(4);
        studentsThatTalkInTable4.Add(3);

        studentsThatLookAtTalkingStudentsInTable4.Add(0); // Jannik as they are sitting close to the above students
        studentsThatLookAtTalkingStudentsInTable4.Add(2); // Sofie as they are sitting close to the above students

        yield return new WaitForSeconds(Random.Range(0f, 1.5f));

        print("Volume increases to 50%");
        targetVolume = 0.5f; // 50 percent
        audioVolumeChange = true;


        yield return new WaitForSeconds(Random.Range(5f, 7f));
        GameObject.FindObjectOfType<CameraFOVController>().ReSetFOV();
     
        print("MainAction 2 finished");
        gamePlayManager.StartPhaseFive();


    }
    public void MainActionThree()
    {

        // Noce increased to another 20%
        // at table 2 , two random kids raise hands , in delay of around 2 seconds
        // two kids  sit and chat loudly
        // 4 random students including Jannik (SN-LE) stop work and look at the two chatting students and shake their head - MB39 every 2 to 4 seconds


        StartCoroutine(TriggerMainActionThree());


    }
    IEnumerator TriggerMainActionThree()
    {

        yield return new WaitForSeconds(1f);
        print("MainAction 3 starts");

        print("Volume increases by 10%");
        targetVolume = 0.6f; // 60 percent
        audioVolumeChange = true;

        yield return new WaitForSeconds(Random.Range(2f, 4f));
        print("Player looks at Table2");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table2);
        GameObject.FindObjectOfType<CameraFOVController>().SetFOV(20);


        // Selin and Lena lifts their hands
        yield return new WaitForSeconds(2f);
        gamePlayManager.studentsActions[13].studentAnimation.RaiseHand(true);
        yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));
        gamePlayManager.studentsActions[12].studentAnimation.RaiseHand(true);

        yield return new WaitForSeconds(Random.Range(3f, 5f));

        // Jannik and other students get disturbed and look at the talking students.
  
        print("Player looks at Table3");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table3);
        yield return new WaitForSeconds(Random.Range(0f, 1.5f));


        for (int i=0;i<studentsThatLookAtTalkingStudentsInTable3.Count;i++)
        {
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable3[i]].StartMyLookAtShakeHeadAndWriteAnimations(gamePlayManager.studentsActions[studentsThatTalkInTable3[i]].transform);
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable3[i]].SetMyMood(MoodIndicator.Middle);
            yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));
        }

        yield return new WaitForSeconds(Random.Range(5f, 8f));

        print("Player looks at Table4");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);
        yield return new WaitForSeconds(Random.Range(0f, 1.5f));


        for (int i = 0; i < studentsThatLookAtTalkingStudentsInTable4.Count; i++)
        {
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable4[i]].StartMyLookAtShakeHeadAndWriteAnimations(gamePlayManager.studentsActions[studentsThatTalkInTable4[i]].transform);
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable4[i]].SetMyMood(MoodIndicator.Middle);
            yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));
        }

        yield return new WaitForSeconds(10.0f);
        GameObject.FindObjectOfType<CameraFOVController>().ReSetFOV();
        print("Player looks at Table1");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1);
        
        print("Finished MainAction 3");       

        //Teacher Question Panel
        StartCoroutine(TriggerTeacherQuestionPhase());

    }
    IEnumerator TriggerTeacherQuestionPhase()
    {
        yield return new WaitForSeconds(5f); // extra five seconds is been asked by the client to give before showing the popup.
        gamePlayManager.StartPhaseSix();
    }

    
    private void StudentReactionOne()
    {
        print("SR 1 should start anytime ");

        // Teacher goes to Table 2 but after going to Table 4 and table 3 and slowing there for sometime till ...

        // Teacher walks to table 4 , (somewhere around G9 to F2),
        // Table 4 talking students look at teacher approching and stop talking and resume their work.

        // audio volume reduces 25%
        //table 3  , teacher goes to F5 - G4, while looking at table 3, students talking there will stop talking and start working
        // audio volume reduced by 25%

        // when teacher arrives at table 2, raised hands kids drop their hands and start working,
        // students who were occationally looking at talking students also start working. 
       
       
 

        StartCoroutine(StudentReactionOneEnum());
    }

    IEnumerator StudentReactionOneEnum()
    {
        print("SR 1 started");

        print("Volume decreases to 25%");
        targetVolume = 0.03f; // reduces to 25 percent
        audioVolumeChange = true;

        //set previously disturbed students' mood back to Good
        for (int i = 0; i < studentsThatLookAtTalkingStudentsInTable3.Count; i++) {
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable3[i]].SetMyMood(MoodIndicator.Good);
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
        }

        for (int i = 0; i < studentsThatLookAtTalkingStudentsInTable4.Count; i++) {
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable4[i]].SetMyMood(MoodIndicator.Good);
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
        }

        yield return new WaitForSeconds(0.5f);
        print("Player looks at Table4 and goes to Table4");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4MoveToPoint);

        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);


        yield return new WaitForSeconds(2.0f);
        print("Students at Table4 Looks at Teacher and stops talking and starts working");
        for (int i=0;i<studentsThatTalkInTable4.Count;i++)
        {
            gamePlayManager.studentsActions[studentsThatTalkInTable4[i]].StopMyTalkOrLaughAnimations();
            gamePlayManager.studentsActions[studentsThatTalkInTable4[i]].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
            yield return new WaitForSeconds(2.0f);
            gamePlayManager.studentsActions[studentsThatTalkInTable4[i]].StopLookAtSomeone();
            gamePlayManager.studentsActions[studentsThatTalkInTable4[i]].studentAnimation.MB30_PeepToSideLeftOrRight(true);
        }
        //print("Volume decreases by 25%");
        //targetVolume = 0.03f; // reduces to 25 percent
        //audioVolumeChange = true;

        yield return new WaitForSeconds(4.0f);



        print("Player looks at Table3 and goes to Table3");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table3);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table3MoveToPoint);


        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);


        yield return new WaitForSeconds(2.0f);
        print("Students at Table3 Looks at Teacher and stops talking and starts working");
        for (int i = 0; i < studentsThatTalkInTable3.Count; i++)
        {
            gamePlayManager.studentsActions[studentsThatTalkInTable3[i]].StopMyTalkOrLaughAnimations();
            gamePlayManager.studentsActions[studentsThatTalkInTable3[i]].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
            yield return new WaitForSeconds(2.0f);
            gamePlayManager.studentsActions[studentsThatTalkInTable3[i]].StopLookAtSomeone();
            gamePlayManager.studentsActions[studentsThatTalkInTable3[i]].studentAnimation.MB30_PeepToSideLeftOrRight(true);
        }
        print("Volume decreases by 25%");
        targetVolume = 0.0f; // reduces to 25 percent
        audioVolumeChange = true;

        yield return new WaitForSeconds(4.0f);





        print("Player looks at Table2 and goes to Table2");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table2);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table2MoveToPoint);


        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);


        yield return new WaitForSeconds(2.0f);
      

        // Mia and Lena drops hands and starts to do their work - UPDATE: only one of these students lowers their hand.
        gamePlayManager.studentsActions[13].studentAnimation.MB30_PeepToSideLeftOrRight(true);
        //yield return new WaitForSeconds(Random.Range(1.5f, 2f));
        //gamePlayManager.studentsActions[12].studentAnimation.MB30_PeepToSideLeftOrRight(true);



        yield return new WaitForSeconds(4.0f);

        print("Player looks at Table3 ");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table3);

        // students at table 3 and table 4 who were looking occactionaly stops and starts working
        for (int i = 0; i < studentsThatLookAtTalkingStudentsInTable3.Count; i++)
        {
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable3[i]].StopMyLookAtShakeHeadAndWriteAnimations();
            yield return new WaitForSeconds(0.5f);
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable3[i]].StopLookAtSomeone();
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable3[i]].studentAnimation.MB30_PeepToSideLeftOrRight(true);
        }

        yield return new WaitForSeconds(4.0f);

        print("Player looks at Table4 ");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);

        for (int i = 0; i < studentsThatLookAtTalkingStudentsInTable4.Count; i++)
        {
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable4[i]].StopMyLookAtShakeHeadAndWriteAnimations();
            yield return new WaitForSeconds(0.5f);
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable4[i]].StopLookAtSomeone();
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable4[i]].studentAnimation.MB30_PeepToSideLeftOrRight(true);
        }

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isLookAround = false;

        yield return new WaitForSeconds(5.0f);

        print("SR 1 Stopped");
       
    }



    private void StudentReactionTwo()
    {
        print("SR 2 should start anytime ");

        // teach stays at table 1 till all students resume work and then go to table 2

        // teacher looks at talking students on table 4, after 5 seconds students notice teacher(look at teacher) and stop talking and go back to their sheets.  volume - 25%

        // teacher looks at talking students on table 3, after 4 seconds students notice teacher(look at teacher) and stop talking and go back to their sheets.  volume - 25%

        // students who were occationally looking at talking students also start working.

        // teacher goes to table 2 , students who raised hands put down as soon as teacher arrives


        StartCoroutine(StudentReactionTwoEnum());
    }

    IEnumerator StudentReactionTwoEnum() {

        print("SR 2 started");



        foreach (StudentAction s in gamePlayManager.studentsActions)
        {
            s.SetMyMood(MoodIndicator.Middle);
            s.ShowMyMoodNow(true);
        }
        for (int i = 0; i < studentsThatTalkInTable4.Count; i++)
        {
            gamePlayManager.studentsActions[studentsThatTalkInTable4[i]].SetMyMood(MoodIndicator.Good);
        }
        for (int i = 0; i < studentsThatTalkInTable3.Count; i++)
        {
            gamePlayManager.studentsActions[studentsThatTalkInTable3[i]].SetMyMood(MoodIndicator.Good);
        }

        yield return new WaitForSeconds(1f);
 

        yield return new WaitForSeconds(Random.Range(2f, 4f));
        print("Player looks at Table4");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);
        GameObject.FindObjectOfType<CameraFOVController>().SetFOV(20);

        yield return new WaitForSeconds(5.0f);
        print("Students at Table4 Looks at Teacher and stops talking and starts working");
        for (int i = 0; i < studentsThatTalkInTable4.Count; i++)
        {
            gamePlayManager.studentsActions[studentsThatTalkInTable4[i]].StopMyTalkOrLaughAnimations();
            gamePlayManager.studentsActions[studentsThatTalkInTable4[i]].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
            yield return new WaitForSeconds(0.5f);
            yield return new WaitForSeconds(2.0f);
            gamePlayManager.studentsActions[studentsThatTalkInTable4[i]].StopLookAtSomeone();
            gamePlayManager.studentsActions[studentsThatTalkInTable4[i]].studentAnimation.MB30_PeepToSideLeftOrRight(true);
        }
        print("Volume decreases by 25%");
        targetVolume = 0.35f; // reduces to 25 percent
        audioVolumeChange = true;

        yield return new WaitForSeconds(4.0f);




        print("Player looks at Table3");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table3);

        yield return new WaitForSeconds(2.0f);
        print("Students at Table3 Looks at Teacher and stops talking and starts working");
        for (int i = 0; i < studentsThatTalkInTable3.Count; i++)
        {
            gamePlayManager.studentsActions[studentsThatTalkInTable3[i]].StopMyTalkOrLaughAnimations();
            gamePlayManager.studentsActions[studentsThatTalkInTable3[i]].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
            yield return new WaitForSeconds(0.5f);
            yield return new WaitForSeconds(2.0f);
            gamePlayManager.studentsActions[studentsThatTalkInTable3[i]].StopLookAtSomeone();
            gamePlayManager.studentsActions[studentsThatTalkInTable3[i]].studentAnimation.MB30_PeepToSideLeftOrRight(true);
        }
        //print("Volume decreases by 25%");
        //targetVolume = 0.05f; // reduces to 25 percent
        print("Volume decreases to zero");
        targetVolume = 0f;
        audioVolumeChange = true;

        yield return new WaitForSeconds(4.0f);


        for (int i = 0; i < studentsThatLookAtTalkingStudentsInTable3.Count; i++)
        {
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable3[i]].StopMyLookAtShakeHeadAndWriteAnimations();
            yield return new WaitForSeconds(0.5f);
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable3[i]].StopLookAtSomeone();
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable3[i]].studentAnimation.MB30_PeepToSideLeftOrRight(true);
        }


        for (int i = 0; i < studentsThatLookAtTalkingStudentsInTable4.Count; i++)
        {
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable4[i]].StopMyLookAtShakeHeadAndWriteAnimations();
            yield return new WaitForSeconds(0.5f);
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable4[i]].StopLookAtSomeone();
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable4[i]].studentAnimation.MB30_PeepToSideLeftOrRight(true);
        }


        foreach (StudentAction s in gamePlayManager.studentsActions)
        {
            s.SetMyMood(MoodIndicator.Good);
            s.ShowMyMoodNow(true);
        }


        yield return new WaitForSeconds(4.0f);

        print("Player looks at Table2 and goes to Table2");

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table2);
        GameObject.FindObjectOfType<CameraFOVController>().ReSetFOV();

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);


        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);


        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table2MoveToPoint);
       

        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);


        yield return new WaitForSeconds(2.0f);


        // Mia and Lena lifts drops hands and starts to do their work
        gamePlayManager.studentsActions[10].studentAnimation.MB30_PeepToSideLeftOrRight(true);
        yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));
        gamePlayManager.studentsActions[12].studentAnimation.MB30_PeepToSideLeftOrRight(true);



        yield return new WaitForSeconds(4.0f);

        print("Player looks at Table4 ");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);
        // students at table 3 and table 4 who were looking occactionaly stops and starts working


        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isLookAround = false;

        yield return new WaitForSeconds(5.0f);

        print("SR 2 Stopped");


    }



    private void StudentReactionThree()
    {
        print("SR 3 should start anytime ");



        // teach goes to table 4 infront of talking students, then goes to table 3 talking students, and then to table 2 and then finally goes to front of class

        // teacher goes to table 4 and stands infront of talking students , after 3 seconds students notice teacher(look at teacher) and stop talking and go back to their sheets.  volume - 25%
        /* [Teacher goes to talking pair on table 4 and stands in front of them for 3 seconds] talking students stop talking and look at teacher(mb14: 2 seconds), then start working again(mb30),
        [random students who occasionally looked at talking students while shaking head(see Main Action III) resume working(mb30)],
        2 random students on the same table[must be in field of view] look at teacher aswell(mb14, delay-span: 1-2 seconds, duration: 1-2 seconds)
        then start talking to each other for 5 seconds(vi7: 5 seconds),
        The mumbling gets louder(another +10%); on table 3, 2 and 1, one random pair(except for children who already do something else than working)
        each starts talking to each other(vi7, not all at once, delay: 2 seconds) [volume: +20%],
        [Teacher goes to table 3 and stands in front of talking students(seats: G2 + H2) for 3 seconds],
        students on seats G2+H2 stop talking and look at teacher(mb14: 2 seconds), then resume working(mb30),
        [random students who occasionally looked at talking students while shaking head(see Main Action III) resume working(mb30)] ,
        [teacher goes to students, who raised their hands and stands in front of them for 2 seconds],
        students who raised hands lower their hands again and resume working[mb30, delay: 1 second],
        [teacher then returns to front of class and slowly looks at each table], pairs stop talking within 5 seconds(delay, pair by pair) and resume working(mb30)
        [reduce volume as pairs stop talking until default]

         */
       

        StartCoroutine(StudentReactionThreeEnum());

    }

    IEnumerator StudentReactionThreeEnum()
    {

        print("SR 3 started");


        foreach (StudentAction s in gamePlayManager.studentsActions)
        {
            s.SetMyMood(MoodIndicator.Middle);
            s.ShowMyMoodNow(true);
        }


        print("Player looks at Table4 and goes to Table4");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);
        GameObject.FindObjectOfType<CameraFOVController>().ReSetFOV();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4MoveToPoint);

        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);


        yield return new WaitForSeconds(2.0f);
        print("Students at Table4 Looks at Teacher and stops talking and starts working");
        for (int i = 0; i < studentsThatTalkInTable4.Count; i++)
        {
            gamePlayManager.studentsActions[studentsThatTalkInTable4[i]].StopMyTalkOrLaughAnimations();
            gamePlayManager.studentsActions[studentsThatTalkInTable4[i]].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
            yield return new WaitForSeconds(Random.Range(1.5f,2.5f));
            gamePlayManager.studentsActions[studentsThatTalkInTable4[i]].StopLookAtSomeone();
            gamePlayManager.studentsActions[studentsThatTalkInTable4[i]].studentAnimation.MB30_PeepToSideLeftOrRight(true);
        }
        print("Students at Table4 watching the other kids start to work and also look at Teacher and  starts working");
        for (int i = 0; i < studentsThatLookAtTalkingStudentsInTable4.Count; i++)
        {
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable4[i]].StopMyLookAtShakeHeadAndWriteAnimations();
            yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable4[i]].StopLookAtSomeone();
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable4[i]].studentAnimation.MB30_PeepToSideLeftOrRight(true);
        }
        print("Volume decreases by 25%");
        targetVolume = 0.35f; // reduces to 25 percent
        audioVolumeChange = true;
        yield return new WaitForSeconds(2.0f);
        // Students look at teacher again and start talking 
        for (int i = 0; i < studentsThatLookAtTalkingStudentsInTable4.Count; i++)
        {
            gamePlayManager.studentsActions[studentsThatTalkInTable4[i]].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
            yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable4[i]].StopLookAtSomeone();
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable4[i]].StartMyTalkOrLaughAnimations(true);
        }

        print("Volume increased by 10%");
        targetVolume = 0.45f; // reduces to 25 percent
        audioVolumeChange = true;
        yield return new WaitForSeconds(4.0f);


        gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable1[2].StartMyTalkOrLaughAnimations();
        gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable1[3].StartMyTalkOrLaughAnimations();
        yield return new WaitForSeconds(1.0f);

        // other than the kids who have raised their hands
        gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2[0].StartMyTalkOrLaughAnimations();
        gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2[3].StartMyTalkOrLaughAnimations();

        yield return new WaitForSeconds(1.0f);

        gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3[2].StartMyTalkOrLaughAnimations();
        gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3[3].StartMyTalkOrLaughAnimations();

        print("Volume increased by 35%");
        targetVolume = 0.65f; 
        audioVolumeChange = true;



        print("Player looks at Table3 and goes to Table3");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table3);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table3MoveToPoint);


        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);


        yield return new WaitForSeconds(2.0f);
        print("Students at Table3 Looks at Teacher and stops talking and starts working");
        for (int i = 0; i < studentsThatTalkInTable3.Count; i++)
        {
            gamePlayManager.studentsActions[studentsThatTalkInTable3[i]].StopMyTalkOrLaughAnimations();
            gamePlayManager.studentsActions[studentsThatTalkInTable3[i]].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
            yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));
            gamePlayManager.studentsActions[studentsThatTalkInTable3[i]].StopLookAtSomeone();
            gamePlayManager.studentsActions[studentsThatTalkInTable3[i]].studentAnimation.MB30_PeepToSideLeftOrRight(true);
        }

        // students at table 3 and table 4 who were looking occactionaly stops and starts working
        for (int i = 0; i < studentsThatLookAtTalkingStudentsInTable3.Count; i++)
        {
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable3[i]].StopMyLookAtShakeHeadAndWriteAnimations();
            yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable3[i]].StopLookAtSomeone();
            gamePlayManager.studentsActions[studentsThatLookAtTalkingStudentsInTable3[i]].studentAnimation.MB30_PeepToSideLeftOrRight(true);
        }

        print("Volume decreases by 25%");
        targetVolume = 0.45f; // reduces to 25 percent
        audioVolumeChange = true;

        yield return new WaitForSeconds(4.0f);


        print("Player looks at Table2 and goes to Table2");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table2);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table2MoveToPoint);


        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);


        yield return new WaitForSeconds(2.0f);


        // Mia and Lena drops hands and starts to do their work - UPDATE: only one of these students lowers their hand.
        //gamePlayManager.studentsActions[10].studentAnimation.MB30_PeepToSideLeftOrRight(true);
        //yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));
        gamePlayManager.studentsActions[12].studentAnimation.MB30_PeepToSideLeftOrRight(true);



        yield return new WaitForSeconds(4.0f);

        print("Player goes to front of the class and looks at centre of the class ");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().BlackBoard);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);


        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
        yield return new WaitForSeconds(2.0f);

        // Start reducing over all volumes from now...
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);

        foreach(StudentAction sa in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable4)
        {
            sa.StopMyTalkOrLaughAnimations();
            sa.StopMyLookAtShakeHeadAndWriteAnimations();
            sa.studentAnimation.MB30_PeepToSideLeftOrRight(true);
            yield return new WaitForSeconds(0.2f);

        }
        print("Volume decreases by 10%");
        targetVolume = 0.35f; // reduces to 25 percent
        audioVolumeChange = true;

        yield return new WaitForSeconds(2.0f);


        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table3);

        foreach (StudentAction sa in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3)
        {
            sa.StopMyTalkOrLaughAnimations();
            sa.StopMyLookAtShakeHeadAndWriteAnimations();
            sa.studentAnimation.MB30_PeepToSideLeftOrRight(true);
            yield return new WaitForSeconds(0.2f);

        }
        print("Volume decreases by 10%");
        targetVolume = 0.25f; // reduces to 25 percent
        audioVolumeChange = true;

        yield return new WaitForSeconds(2.0f);


        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table2);

        foreach (StudentAction sa in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2)
        {
            sa.StopMyTalkOrLaughAnimations();
            sa.StopMyLookAtShakeHeadAndWriteAnimations();
            sa.studentAnimation.MB30_PeepToSideLeftOrRight(true);
            yield return new WaitForSeconds(0.2f);

        }
        print("Volume decreases by 10%");
        targetVolume = 0.15f; // reduces to 25 percent
        audioVolumeChange = true;


        yield return new WaitForSeconds(2.0f);


        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1);

        foreach (StudentAction sa in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable1)
        {
            sa.StopMyTalkOrLaughAnimations();
            sa.StopMyLookAtShakeHeadAndWriteAnimations();
            sa.studentAnimation.MB30_PeepToSideLeftOrRight(true);
            yield return new WaitForSeconds(0.2f);

        }
        print("Volume decreases by 10%");
        targetVolume = 0.05f; // reduces to 25 percent
        audioVolumeChange = true;

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isLookAround = false;

        yield return new WaitForSeconds(5.0f);

        print("SR 3 Stopped");

        yield return new WaitForSeconds(0.1f);
    }


    private void StudentReactionFour()
    {
        print("SR 4 should start anytime ");


        /*
         *
         * Teacher stands at Table 1
         * then goes to table 2 and then comes to front of class
         * 
         *talking students stop talking, look at teacher (mb14: 2-3 seconds),
         * then resume talking (vi7);
         * parallel: 90% of working students (among them two SN_ESE and one SN_LE) interrupt their work
         * and look at teacher (mb14 / within 3 seconds, not all at once, random durations: 1-3 seconds),
         * then protest (mb19: not all at once / random durations: 2-5 seconds),
         * 
         * [Teacher walks to students who raised hands on table 2, stays in front of them for 2 seconds (students lower hands), then returns to front of class];
         *
         * in the meantime: protesting students start talking in groups or pairs (vi5, not all at once, delay: about 2 seconds / durations about 7-11 seconds)
         *
         * [volume rising another 30/40% to 100%],
         *
         * 50% of protesting students resume working one by one (mb30, within 3-4 seconds, not all at once)
         *
         * [reduce volume as students stop talking],
         *
         * other 50% (among them one girl SN_ESE [Leonie]) look out of the windows or around class (mb17/mb9)
         *
         *
         */

        foreach (StudentAction s in gamePlayManager.studentsActions)
        {
            s.SetMyMood(MoodIndicator.Bad);
            s.ShowMyMoodNow(true);
        }

        StartCoroutine(StudentReactionFourEnum());
    }


    IEnumerator StudentReactionFourEnum()
    {
        print("SR 4 started");

        IEnumerator parallelRoutine;

        parallelRoutine = StudentReactionFourEnumParellelRoutine();


        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);
        yield return new WaitForSeconds(2.0f);

        print("Talking students stop talking");
        targetVolume = 0;
        audioVolumeChange = true;

        gamePlayManager.studentsActions[20].StopMyTalkOrLaughAnimations();

        gamePlayManager.studentsActions[20].myNeighbourStudent.StopMyTalkOrLaughAnimations();
        gamePlayManager.studentsActions[4].StopMyTalkOrLaughAnimations();
        gamePlayManager.studentsActions[3].StopMyTalkOrLaughAnimations();

        //students' moods turn bad
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Bad);
        }

            print("They look at teacher for few seconds");
        yield return new WaitForSeconds(Random.Range(0.5f,1.5f));
        gamePlayManager.studentsActions[20].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        gamePlayManager.studentsActions[4].myNeighbourStudent.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        gamePlayManager.studentsActions[3].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);

        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        gamePlayManager.studentsActions[20].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
        yield return new WaitForSeconds(Random.Range(2f, 4f));

        // then they start talking again
        targetVolume = 0.3f;
        audioVolumeChange = true;

        StartCoroutine(parallelRoutine);

        

        gamePlayManager.studentsActions[20].StartMyTalkOrLaughAnimations();
        gamePlayManager.studentsActions[20].myNeighbourStudent.StartMyTalkOrLaughAnimations();
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        gamePlayManager.studentsActions[4].StartMyTalkOrLaughAnimations();
        gamePlayManager.studentsActions[3].StartMyTalkOrLaughAnimations();

        yield return new WaitForSeconds(6f);
        print("Player goes to table two ");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table2);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);


        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table2MoveToPoint);


        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);

        yield return new WaitForSeconds(2f);


        // Mia and Lena drops hands and starts to do their work - UPDATE: only one of these students lowers their hand.
        //gamePlayManager.studentsActions[10].studentAnimation.MB30_PeepToSideLeftOrRight(true);
        //yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));
        gamePlayManager.studentsActions[12].studentAnimation.MB30_PeepToSideLeftOrRight(true);

        yield return new WaitForSeconds(2f);


        print("Player goes to front of the class and looks at centre of the class ");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().BlackBoard);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);


        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);


        yield return new WaitForSeconds(5f);

        StopCoroutine(parallelRoutine);
        parallelRoutine = StudentReactionFourEnumParellelRoutine2();
        StartCoroutine(parallelRoutine);
        yield return new WaitForSeconds(5f);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);

        yield return new WaitForSeconds(5f);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table2);

        yield return new WaitForSeconds(5f);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1);

        yield return new WaitForSeconds(5f);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);

        yield return new WaitForSeconds(5f);


        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);


        StopCoroutine(parallelRoutine);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isLookAround = false;
        yield return new WaitForSeconds(5f);

        print("SR 4 Complete");
        yield return new WaitForSeconds(0.1f);
    }


    IEnumerator StudentReactionFourEnumParellelRoutine()
    {
        List<int> activePeople = new List<int>();
        activePeople.Add(20); activePeople.Add(14); activePeople.Add(4); activePeople.Add(3); activePeople.Add(12); activePeople.Add(10);
        // students who are involved in main routines will not participate here..
        // 20, 14, 4, 3, 10,12
        for (int i= 0; i<gamePlayManager.studentsActions.Count;i++)
        {
            if(!activePeople.Contains(i))
            {
                gamePlayManager.studentsActions[i].StopMyLookAtShakeHeadAndWriteAnimations();
                yield return new WaitForSeconds(Random.Range(0.5f, 1f));
                gamePlayManager.studentsActions[i].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
                yield return new WaitForSeconds(Random.Range(1f, 4f));
                gamePlayManager.studentsActions[i].studentAnimation.MB19_Protest(true);
            }
        }
        yield return new WaitForSeconds(Random.Range(4f, 6f));

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            if (!activePeople.Contains(i))
            {
                gamePlayManager.studentsActions[i].StartMyTalkOrLaughAnimations();
                yield return new WaitForSeconds(Random.Range(0.5f, 1f)); print("Volume increase by 10%");
                targetVolume += 0.1f; // increases  by 10 percent
                targetVolume = (targetVolume>1.0f)?1.0f:targetVolume;
                audioVolumeChange = true;
            }
        }

        while (true)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }



    IEnumerator StudentReactionFourEnumParellelRoutine2()
    {
        List<int> activePeople = new List<int>();
        activePeople.Add(20); activePeople.Add(14); activePeople.Add(4); activePeople.Add(3); activePeople.Add(12); activePeople.Add(10);

        // 50% class gets back to normal.
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {

            if (!activePeople.Contains(i))
            {
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
            }


                if (i%2 != 0)
            {
                gamePlayManager.studentsActions[i].studentAnimation.MB30_PeepToSideLeftOrRight(true);
                print("Volume decrease by 10%");
                targetVolume -= 0.1f; // increases  by 10 percent
                targetVolume = (targetVolume < 0.5f) ? 0.5f : targetVolume;
                yield return new WaitForSeconds(Random.Range(2f, 4f));
               
            }
            else
            {
                gamePlayManager.studentsActions[i].studentAnimation.ResetCurrentAnim();
                gamePlayManager.studentsActions[i].LookAtWindowRoutine();
                yield return new WaitForSeconds(Random.Range(0.5f, 1f));
            }
        }

        while (true)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }


    public void Reset()
    {
        // what is the Student Mood BEFORE Reest?        
        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
            Debug.LogFormat("{0} \t Mood BEFORE Reset: {1}  =>  SAVED Mood: {2}", studentAction.gameObject.name, studentAction.myCurrentMood, studentAction.mySavedMood);

        //Reset Anim
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayerToOriginalPostion(true);
        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
            studentAction.SetMyMood(studentAction.mySavedMood); // Reset Moods
            studentAction.studentAnimation.ResetAllAnim();            
        }

        /*
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
