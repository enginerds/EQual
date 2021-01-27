using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioTwoManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    public Transform tableOnePoint;
    public AudioSource audioInClass;

    string questionString = "";

    public Animator pencilAnim;
    public Animator pencilCaseAnim;

    public GameObject inHandPencil, inLeftHandPencil;
    public GameObject inHandPencil_Tom;


    private void Awake()
    {
        gamePlayManager.currentScenario = "SC2";
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
        audioInClass.Pause();

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

        // get the background students back to their random looking and writing sequence. and reset all students mood back to what it is to be
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].studentAnimation.ResetCurrentAnim();
            if (i != 16 && i!=19) // except for Leonie and Shirin
            {
                gamePlayManager.studentsActions[i].StartMyRandomLookingOrWrittingAnimations();
            }
            if(i != 16)
            {
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
                gamePlayManager.studentsActions[i].ShowMyMoodNow(true);
            }
            else
            {
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
                gamePlayManager.studentsActions[i].ShowMyMoodNow(true);
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
        gamePlayManager.StartPhaseSeven(20.0f);

    }

    void showTempText() {

     //   gamePlayManager.userInterfaceManager.ShowOrHideTempPanel(true);
     //   gamePlayManager.userInterfaceManager.SetTempText(questionString);


    }

    // SAc: Edit, We have removed Julia from the group of kids as she is not in the 9 year old list as per configuration, so all the index numbers will have to be modified as per this change where it is needed



    public void MainActionOne()
    {
        audioInClass.Play();

        //gamePlayManager.userInterfaceManager.ShowOrHideTempPanel(true);
        //gamePlayManager.userInterfaceManager.SetTempText("Student searches for a pen in her pencil case (iwo 27) and shrugs shoulders(mb_34)");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, tableOnePoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = tableOnePoint.rotation;

        // All kids Except Tom is working on their WorkSheets, every 2-3 seconds : random student(other than tom) ,

        //looks around class - intersting spots, or Outside window for 2- 3 seconds

        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopSittingIdleLookAroundAnything(); // stop the initial intro animations for all kids
            gamePlayManager.studentsActions[i].scenarioStart = false;
            gamePlayManager.studentsActions[i].StartMyRandomLookingOrWrittingAnimations();
  
        }

        // Tom searches for Pen in box (IWO27) and shrigs shoulders (mb34)

        // index as 20, changed it to 19

//        print("tom is aggitated");
        gamePlayManager.studentsActions[19].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[16].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[19].StopLookAtSomeone();
        gamePlayManager.studentsActions[16].StopLookAtSomeone();

        gamePlayManager.studentsActions[19].studentAnimation.SitAgitated(true); //Tom
        gamePlayManager.studentsActions[16].studentAnimation.SitAgitated(true); //Leonie

        gamePlayManager.studentsActions[16].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[16].ShowMyMoodNow(true);
        gamePlayManager.studentsActions[19].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[19].ShowMyMoodNow(true);

        //      print("tom is searching for Pencil");
        gamePlayManager.studentsActions[19].studentAnimation.SearchForPencil(true);

        // below , this is no more the case, so dont animate these
        // gamePlayManager.studentsActions[8].studentAnimation.RaiseHand(true); //Hannah
        //  gamePlayManager.studentsActions[14].studentAnimation.RaiseHand(true); //Shirin

        StartCoroutine(TriggerMainActionTwo());

    }

    IEnumerator TriggerMainActionTwo()
    {
        yield return null;
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
        }
        yield return new WaitForSeconds(8.0f);
 //       gamePlayManager.studentsActions[16].StopMyRandomLookingAnimations();
        gamePlayManager.StartPhaseFour();
        

    }



    public void MainActionTwo()
    {
        //gamePlayManager.userInterfaceManager.SetTempText("Student asks her neigbor with ESE for a pen (vi_11)");
        //gamePlayManager.MainActionTwoDelay = 10.0f;
        // Tom tips on Leonie shoulder (mb 28)
        // Tom asks Leonie for pen (mb 28)
        // Other students (Ivan(20) and Hannah(8) ) one by one on table 1 stop writing and look at Tom and Leonie (mb10)
        // Leonie nods (mb18)
        // Tom and Leonie whisper (vi11 for 3 seconds)


        StartCoroutine(TriggerMainActionTwoTomAndLeonieActions());

    }


    IEnumerator TriggerMainActionTwoTomAndLeonieActions()
    {

        yield return StartCoroutine(TriggerMainActionTwoTomAsks());
        yield return StartCoroutine(TriggerMainActionTwoLeonieNods());
        yield return StartCoroutine(TriggerMainActionTwoTomAndLeonieWisphers());
 //       yield return new WaitForSeconds(2f);
        StartCoroutine(TriggerMainActionThree());
    }


    IEnumerator TriggerMainActionTwoTomAsks()
    {


        // Tom tips on Leonie shoulder (mb 28)
        // Tom asks Leonie for pen (mb 28)
  //      print("tom Taps on Leonie");
  //      print("tom Asks Pen to Leonie");
        gamePlayManager.studentsActions[19].studentAnimation.TapAndAskAskPen(true); //Tom
        yield return new WaitForSeconds(2f);
    }

    IEnumerator TriggerMainActionTwoLeonieNods()
    {
  //      print("Leonie Nods");
        // Leonie nods (mb18)
        gamePlayManager.studentsActions[16].studentAnimation.MB18_NodHead(true);

        // Other students (Ivan(20) and Hannah(8) ) one by one on table 1 stop writing and look at Tom and Leonie (mb10)

        gamePlayManager.studentsActions[20].StopMyRandomLookingAnimations();
   //     print("Ivan looks at Tom");
        gamePlayManager.studentsActions[20].LookAtSomeone(gamePlayManager.studentsActions[19].gameObject.transform); //Ivan looks at Tom

        gamePlayManager.studentsActions[8].StopMyRandomLookingAnimations();
   //     print("Hannah looks at Leonie");
        gamePlayManager.studentsActions[8].LookAtSomeone(gamePlayManager.studentsActions[16].gameObject.transform); //Hannah looks at Leonie

        yield return new WaitForSeconds(1f);
    }

    IEnumerator TriggerMainActionTwoTomAndLeonieWisphers()
    {
        // Tom and Leonie whispers
  //      print("tom and Leonie wishphers");
        gamePlayManager.studentsActions[19].studentAnimation.VI11_TalkToFriendsLeftAndRight();
        gamePlayManager.studentsActions[16].studentAnimation.VI11_TalkToFriendsLeftAndRight();

        //Zoe raises her hand and her mood changes from good to medium
        gamePlayManager.studentsActions[18].SetMyMood(MoodIndicator.Middle);
        gamePlayManager.studentsActions[18].StopLookingAtTabletAndWrittingAnimations();
        gamePlayManager.studentsActions[18].RaiseHand(9000);

        yield return new WaitForSeconds(3f);
    }


    IEnumerator TriggerMainActionThree()
    {
        yield return new WaitForSeconds(1.0f); // timeframe is already covered for nearly 10 seconds
  //      print("tom and Leonie wishphers stops");
        gamePlayManager.studentsActions[19].studentAnimation.VI11_TalkToFriendsStop();
        gamePlayManager.studentsActions[16].studentAnimation.VI11_TalkToFriendsStop();
        gamePlayManager.StartPhaseFive();


    }

    public void MainActionThree()
    {
        //gamePlayManager.userInterfaceManager.SetTempText("Student with ESE gives her a pen (iwo_28+iwo_7) + two random students (table 2 and table 3) are raising their hands (mb_20)");

        gamePlayManager.studentsActions[20].StopLookAtSomeone();
        gamePlayManager.studentsActions[20].StartMyRandomLookingOrWrittingAnimations();

     
        gamePlayManager.studentsActions[8].StopLookAtSomeone();
        gamePlayManager.studentsActions[8].StartMyRandomLookingOrWrittingAnimations();


        // Leonie gives pen (iwo27+iwo28+iwo07) at the same time 2 random students form table 2 raises hands (mb20)

        // initially Index was to point at Ivan (20), now client has asked to change it to Leonie so index 16
  //      print("Leonie gets aggitated");
 //       gamePlayManager.studentsActions[16].studentAnimation.SitAgitated(true);
 //       print("Leonie gives pen");
 //       inLeftHandPencil.SetActive(true);
        gamePlayManager.studentsActions[16].studentAnimation.GivePen(true);

        
        StartCoroutine(GivePencilAnimAfterDelay("isGive",2.0f));
        
        
       StartCoroutine(TriggerTeacherQuestionPhase());


        // at the same time 2 random students form table 2 raises hands (mb20)

        int student1OfTable2 = Random.Range(0, gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2.Count);
        int student2OfTable2 = Random.Range(0, gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2.Count);
        // if student2 is same as student 1 then check if student 2 is last of the list of table 2, if not add 1 to student 2 or subtract q from student 2
        student2OfTable2 = (student2OfTable2 == student1OfTable2) ? ((student2OfTable2  < gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2.Count -2) ? student2OfTable2 + 1 : student2OfTable2 - 1): student2OfTable2;

  //      print("Selected student1 to raise hand " + gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2[student1OfTable2].gameObject.name);
  //      print("Selected student2 to raise hand " + gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2[student2OfTable2].gameObject.name);
        // StartCoroutine(receivePencilDelay("receivePencil", 1.5f));
        gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2[student1OfTable2].StopMyRandomLookingAnimations();
        gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2[student2OfTable2].StopMyRandomLookingAnimations();

        gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2[student1OfTable2].studentAnimation.RaiseHand(true); 
        gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2[student2OfTable2].studentAnimation.RaiseHand(true); 




        //Teacher Question Panel
       
    }

    IEnumerator TriggerTeacherQuestionPhase() {
        //Tom is back to do his worksheets / random look arounds
        yield return new WaitForSeconds(2.0f);
       
        print("Tom is no more aggitated");
        gamePlayManager.studentsActions[19].studentAnimation.SitAgitated(true);

        gamePlayManager.studentsActions[16].studentAnimation.SitAgitated(true);
        gamePlayManager.studentsActions[19].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[19].ShowMyMoodNow(true);

        // inLeftHandPencil.SetActive(false);
        yield return new WaitForSeconds(10.0f);
        audioInClass.Pause();
        gamePlayManager.StartPhaseSix();
        //gamePlayManager.userInterfaceManager.ShowOrHideTempPanel(false);

    }


    private void StudentReactionOne()
    {

        // Leonie laughs (EE04) and continues working mb_33,
        // rest of the class quiently works mb 33,
        // every 2-3 seconds : random student looks around class or outside of window (2-3 seconds)
        gamePlayManager.studentsActions[16].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[16].StopLookAtSomeone();
        gamePlayManager.studentsActions[16].studentAnimation.LaughAndWork(true);
        gamePlayManager.studentsActions[16].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[16].ShowMyMoodNow(true);
        pencilAnim.gameObject.SetActive(false);
        inHandPencil.SetActive(true);

        //rest of class begins writing
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
        }

        //gamePlayManager.studentsActions[0].studentAnimation.Talk(true);
        //gamePlayManager.studentsActions[0].studentAnimation.TalkRightLeft(1.0f);

    }
    private void StudentReactionTwo()
    {

        // Leonie(16) looks at teacher, looks around the class and continues working mb_33,
        // rest of the class quiently works mb 33,
        // every 2-3 seconds : random student looks around class or outside of window (2-3 seconds)
        gamePlayManager.studentsActions[16].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[16].StopLookAtSomeone();
        gamePlayManager.studentsActions[16].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[16].ShowMyMoodNow(true);
        StartCoroutine(TriggerLeonieLookAtTeacherAndLookAroundClassAndContinuesWorking());

        //rest of class begins writing
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
        }

        // index as 21, changed it to 20

        //StartCoroutine(startPencilAnimAfterDelay("isWrite", 1.5f));

    }

    IEnumerator TriggerLeonieLookAtTeacherAndLookAroundClassAndContinuesWorking()
    {

        // look at teacher for few seconds
        yield return new WaitForSeconds(0.5f);
        gamePlayManager.studentsActions[16].LookAtSomeone(GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform);
        yield return new WaitForSeconds(3.0f);
        gamePlayManager.studentsActions[16].StopLookAtSomeone();
        // look around class for few seconds
        gamePlayManager.studentsActions[16].studentAnimation.MB9_LookAround(true);
        yield return new WaitForSeconds(11.0f); // initially it was for 1 cycle, added more seconds  for having a 2 cycle animation as per JIRA EQ-30
        // look at teacher for few seconds
        gamePlayManager.studentsActions[16].studentAnimation.MB33_WorkOnSheets(true);
     //   yield return new WaitForSeconds(24.0f);
    }



    private void StudentReactionThree()
    {
        gamePlayManager.studentsActions[16].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[16].StopLookAtSomeone();
        gamePlayManager.studentsActions[16].SetMyMood(MoodIndicator.Middle);
        gamePlayManager.studentsActions[16].ShowMyMoodNow(true);

        //rest of class begins writing
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
        }

        // Leonie(16) looks around the class , fidgeds around in chair (iwo 29),
        // rest of the class quiently works mb 33,
        // every 2-3 seconds : random student looks around class or outside of window (2-3 seconds)
        StartCoroutine(TriggerLeonieLookAroundClassAndFidgetInChair());

        // index as 21, changed it to 20

    }

    IEnumerator TriggerLeonieLookAroundClassAndFidgetInChair()
    {
        yield return new WaitForSeconds(0.5f);
        // look around class for few seconds
        gamePlayManager.studentsActions[16].studentAnimation.MB9_LookAround(true);
        yield return new WaitForSeconds(4.0f);
        // look at teacher for few seconds
        gamePlayManager.studentsActions[16].studentAnimation.IWO29_FidgetsInChair(true);
        yield return new WaitForSeconds(1.0f);
        //   yield return new WaitForSeconds(24.0f);
    }

    private void StudentReactionFour()
    {

        // Leonie(16) throws pencil box on the floor (iwo_14) , and lays head on the table (mb 8)
        // other students on table 1 keep looking at Leonie for 2-3 seconds and continue to work mb 33,
        // Tom refuses to work (mb39, mb26)
        // rest of the class quiently works mb 33,
        // every 2-3 seconds : random student looks around class or outside of window (2-3 seconds)


        //rest of class begins writing
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);
        }

        foreach (StudentAction s in gamePlayManager.studentsActions)
        {
            s.SetMyMood(MoodIndicator.Middle);
            s.ShowMyMoodNow(true);
        }

        gamePlayManager.studentsActions[16].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[19].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[16].StopLookAtSomeone();
        gamePlayManager.studentsActions[19].StopLookAtSomeone();
        gamePlayManager.studentsActions[16].SetMyMood(MoodIndicator.Bad);
        gamePlayManager.studentsActions[16].ShowMyMoodNow(true);
        gamePlayManager.studentsActions[19].SetMyMood(MoodIndicator.Bad);
        gamePlayManager.studentsActions[19].ShowMyMoodNow(true);


      

        StartCoroutine(TriggerStudentReactionFourOtherStudentsReaction());

        // index as 21, changed it to 20
        //    gamePlayManager.studentsActions[20].studentAnimation.WritePaperOnDesk(true);
        //    pencilAnim.gameObject.SetActive(false);
        //    inHandPencil.SetActive(true);
        //pencilAnim.SetBool("isWrite", true);
    }

    IEnumerator TriggerLeonieThrowPencilBox()
    {
        // Leonie(16) throws pencil box on the floor (iwo_14) , and lays head on the table (mb 8)
        gamePlayManager.studentsActions[16].studentAnimation.ThrowPen(true);
      //  print("Leonie Penbox thrown");
        pencilCaseAnim.SetBool("isThrow", true);
        yield return new WaitForSeconds(1.0f);
        //   yield return new WaitForSeconds(24.0f);
    }
    IEnumerator TriggerTomRefuseToWork()
    {
        // Tom refuses to work (mb39, mb26)
        yield return StartCoroutine(TriggerLeonieThrowPencilBox());
        yield return new WaitForSeconds(2.0f);
        // mb 39 - Shake head
        gamePlayManager.studentsActions[19].studentAnimation.MB39_ShakesHead(true);
        // mb 26 - folds hands and sits
        gamePlayManager.studentsActions[19].studentAnimation.MB26_FoldHands(true);
        yield return new WaitForSeconds(5.0f);
        //   yield return new WaitForSeconds(24.0f);
    }

    IEnumerator TriggerStudentReactionFourOtherStudentsReaction()
    {

        yield return StartCoroutine(TriggerTomRefuseToWork());
        // Other students (Ivan(20) and Hannah(8) ) one by one on table 1 stop writing and look at Leonie (mb10)
        yield return new WaitForSeconds(3.0f);
        gamePlayManager.studentsActions[20].StopMyRandomLookingAnimations();

        gamePlayManager.studentsActions[20].LookAtSomeone(gamePlayManager.studentsActions[19].gameObject.transform); //Ivan looks at Tom
        gamePlayManager.studentsActions[8].StopMyRandomLookingAnimations();

        gamePlayManager.studentsActions[8].LookAtSomeone(gamePlayManager.studentsActions[16].gameObject.transform); //Hannah looks at Leonie
        yield return new WaitForSeconds(Random.Range(2.0f,4.0f));
        gamePlayManager.studentsActions[20].studentAnimation.MB33_WorkOnSheets(true); //Ivan continues to work again
        yield return new WaitForSeconds(Random.Range(2.0f, 4.0f));
        gamePlayManager.studentsActions[8].studentAnimation.MB33_WorkOnSheets(true); //Hannah continues to work again
        yield return new WaitForSeconds(3.0f);

    }





    public void Reset()
    {
        //gamePlayManager.studentsActions[12].studentAnimation.RaiseHand(false);
        //gamePlayManager.studentsActions[15].studentAnimation.RaiseHand(false);
        inHandPencil.SetActive(false);
        inHandPencil_Tom.SetActive(false);
        pencilAnim.gameObject.SetActive(true);
        // initially  20 now 16
        gamePlayManager.studentsActions[16].studentAnimation.LaughAndWork(false);
        gamePlayManager.studentsActions[16].studentAnimation.ThrowPen(false);
        gamePlayManager.studentsActions[16].studentAnimation.WritePaperOnDesk(false);
        gamePlayManager.StartPhaseSix();

        pencilAnim.SetBool("isThrow", false);
        pencilCaseAnim.SetBool("isThrow", false);
        pencilAnim.SetBool("isWrite", false);
        pencilAnim.SetBool("isReceive", false);

    }

    IEnumerator GivePencilAnimAfterDelay(string boolString, float delay)
    {
        yield return new WaitForSeconds(delay);
   //     print("Leonie Pen giving animation");
        // pencilAnim.SetBool(boolString, true);
        pencilAnim.SetBool(boolString, true);
    }

    IEnumerator startPencilAnimAfterDelay(string boolString, float delay) {
        yield return new WaitForSeconds(delay);
  //      print("Leonie Penbox thrown");
        // pencilAnim.SetBool(boolString, true);
        pencilCaseAnim.SetBool(boolString, true);
    }

    IEnumerator receivePencilDelay(string triggerString, float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("calling ReceivePencil...");
  //      pencilAnim.SetBool("isGive", false);
        // index as 20, changed it to 19
        gamePlayManager.studentsActions[19].studentAnimation.ReceivePencil();        
        pencilAnim.SetBool("isReceive", true);        
    }
}
