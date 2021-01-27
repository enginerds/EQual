using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScenarioElevenManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    public Transform risedStudentLcoaiton;
    string questionString = "";


    private List<StudentAction> randomDisturbedStudents, random10WorkingStudents, random9whisperingStudents;
    private List<StudentAction> Table3TalkingKids;
    private List<Transform> Table3TalkingKidsTransform;

    public StudentAction kidLeonie,kidJannik;

    public List<StudentAction> table3DisturbedKids;

    public bool sc11LookatWindowActive;
    Transform sc11LookatWindowPoint;

    public AudioSource table1KidsTalking, table2KidsTalking, table3KidsTalking, table4KidsTalking;


    public GameObject Table1AssignmentPile;
    public GameObject Table2AssignmentPile;
    public GameObject Table3AssignmentPile;
    public GameObject Table4AssignmentPile;

    public Animator pencilCaseAnim;

    private void Start()
    {
        randomDisturbedStudents = new List<StudentAction>();
        random10WorkingStudents = new List<StudentAction>();
        random9whisperingStudents = new List<StudentAction>();
        Table3TalkingKids = new List<StudentAction>();
        Table3TalkingKidsTransform = new List<Transform>();
        gamePlayManager.userInterfaceManager.ShowOrHideInitLoadingPanel(true);
        gamePlayManager.currentScenario = "SC11";
        gamePlayManager.initialActionForScenarioIsCommon = true;
        Table1AssignmentPile.SetActive(false);
        Table2AssignmentPile.SetActive(false);
        Table3AssignmentPile.SetActive(false);
        Table4AssignmentPile.SetActive(false);

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

        yield return new WaitForSeconds(2.0f);
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




        /*
        StartCoroutine(TriggerInitialAction());
        yield return new WaitForSeconds(3.0f);
        gamePlayManager.userInterfaceManager.ShowOrHideInitLoadingPanel(false);
        yield return new WaitForSeconds(0.01f);
        gamePlayManager.userInterfaceManager.ShowOrHideKnowYourClassPanel(true);
        yield return new WaitForSeconds(3.0f);
        if (table1KidsTalking.isPlaying) table1KidsTalking.Pause();
        if (table2KidsTalking.isPlaying) table2KidsTalking.Pause();
        if (table3KidsTalking.isPlaying) table3KidsTalking.Pause();
        if (table4KidsTalking.isPlaying) table4KidsTalking.Pause();
        // show the skip button for the ShowOrHideKnowYourClassPanel
        if (gamePlayManager.getToKnowSkipBtn != null) gamePlayManager.getToKnowSkipBtn.gameObject.SetActive(true);
        */
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
                questionString = "Die Organisation des Tagesplans an der Tafel fortsetzen und gleichzeitig Leonie (SFB ESE) und Jannik (SFB LE) persönlich ansprechen und mit kurzem Schulterblick freundlich um Ruhe bitten.";
                print("Question asked = " + questionString);
                StudentReactionOne();
                gamePlayManager.playerActionDataHandler.PlayerAction = questionString;
                gamePlayManager.StartPhaseSeven(33.0f); // originally 30.0f, but it is not enough for registering all the action that happens
                break;
            case "2":
                questionString = "Die Organisation des Tagesplans an der Tafel kurz unterbrechen und sich der Klasse zuwenden. Die abgelenkten Schülerinnen und Schüler anschauen und durch eine non-verbale Ermahnung zum Weiterarbeiten auffordern.";
                print("Question asked = " + questionString);
                StudentReactionTwo();
                gamePlayManager.playerActionDataHandler.PlayerAction = questionString;
                gamePlayManager.StartPhaseSeven(33.0f); // originally 30.0f, but it is not enough for registering all the action that happens
                break;
            case "3":
                questionString = "Die Organisation des Tagesplans an der Tafel fortsetzen und die gesamte Klasse freundlich auffordern, leise weiter zu arbeiten.";
                print("Question asked = " + questionString);
                StudentReactionThree();
                gamePlayManager.playerActionDataHandler.PlayerAction = questionString;
                gamePlayManager.StartPhaseSeven(40.0f); // originally 30.0f, but it is not enough for registering all the action that happens
                break;
            case "4":
                questionString = "Die Organisation des Tagesplans an der Tafel kurz unterbrechen und den störenden Kindern Punkte im (von Ihnen langfristig etablierten) Belohnungssystem an der Tafel abziehen, sodass die anderen Kinder ungestört weiterarbeiten können.";
                print("Question asked = " + questionString);
                StudentReactionFour();
                gamePlayManager.playerActionDataHandler.PlayerAction = questionString;
                gamePlayManager.StartPhaseSeven(60.0f); // originally 30.0f, but it is not enough for registering all the action that happens

                break;
        }


        //showTempText();

    }

    void showTempText()
    {

     //   gamePlayManager.userInterfaceManager.ShowOrHideTempPanel(true);
    //    gamePlayManager.userInterfaceManager.SetTempText(questionString);


    }










//    Students sit at their tables and look around at random locations(mb25+mb9/mb15/mb17),
//    7 pairs of students(random locations) quietly whisper with each other(vi11) [soft background mumbling]
    IEnumerator TriggerInitialAction()
    {
        print("Starting InitialAction");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
 

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
        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
            studentAction.scenarioStart = false;
            //            studentAction.studentAnimation.MB33_WorkOnSheets(true);
            //   studentAction.studentAnimation.MB33_WorkOnSheets(true);
        }
     
        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);

        yield return new WaitForSeconds(1.0f);
        print("Finished InitiailAction 1");


    }


    public void MainActionOne()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
    
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().ShowHoldingHandWorkSheets(true);
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().myHandsAnimator != null)
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().myHandsAnimator.SetBool("WorkSheetHandIdle", true);
        StartCoroutine(TriggerMainActionTwo());
    }
    IEnumerator TriggerMainActionTwo()
    {
        print("Starting MainAction 1");

        
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1MoveToPoint);
        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().PlayWorksheetPlacingOnTableAnimation();
        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isGivingWorkSheet);
        Table1AssignmentPile.SetActive(true);

        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable1)
        {
            StartCoroutine(s.TakeBookFromTable(Table1AssignmentPile.transform));
        }



        yield return new WaitForSeconds(4f);
        Table1AssignmentPile.SetActive(false);
        yield return new WaitForSeconds(2f);
        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable1)
        {
            s.StopLookAtSomeone();
            s.StopSittingIdleLookAroundAnything();
            s.scenarioStart = false;
            s.ResetAndWorkOnSheets();
            s.SetMyMood(MoodIndicator.Good);
            s.ShowMyMoodNow(true);
        }
        yield return new WaitForSeconds(2f);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table2);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table2MoveToPoint);
        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().PlayWorksheetPlacingOnTableAnimation();
        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isGivingWorkSheet);

        Table2AssignmentPile.SetActive(true);
        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2)
        {
            StartCoroutine(s.TakeBookFromTable(Table2AssignmentPile.transform));
        }
        yield return new WaitForSeconds(4f);
        Table2AssignmentPile.SetActive(false);
        yield return new WaitForSeconds(2f);
        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2)
        {
            s.StopLookAtSomeone();
            s.StopSittingIdleLookAroundAnything();
            s.scenarioStart = false;
            s.ResetAndWorkOnSheets();
            s.SetMyMood(MoodIndicator.Good);
            s.ShowMyMoodNow(true);
        }
        yield return new WaitForSeconds(2f);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table3);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table3MoveToPoint);
        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().PlayWorksheetPlacingOnTableAnimation();
        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isGivingWorkSheet);

        Table3AssignmentPile.SetActive(true);
        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3)
        {
            StartCoroutine(s.TakeBookFromTable(Table3AssignmentPile.transform));
        }
        yield return new WaitForSeconds(4f);
        Table3AssignmentPile.SetActive(false);
        yield return new WaitForSeconds(2f);
        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable3)
        {
            s.StopLookAtSomeone();
            s.StopSittingIdleLookAroundAnything();
            s.scenarioStart = false;
            s.ResetAndWorkOnSheets();
            s.SetMyMood(MoodIndicator.Good);
            s.ShowMyMoodNow(true);
        }
        yield return new WaitForSeconds(2f);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4MoveToPoint);
        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().PlayWorksheetPlacingOnTableAnimation();
        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isGivingWorkSheet);

        Table4AssignmentPile.SetActive(true);
        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable4)
        {
            StartCoroutine(s.TakeBookFromTable(Table4AssignmentPile.transform));
        }
        yield return new WaitForSeconds(4f);
        Table4AssignmentPile.SetActive(false);
        yield return new WaitForSeconds(2f);

        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable4)
        {
            s.StopLookAtSomeone();
            s.StopSittingIdleLookAroundAnything();
            s.scenarioStart = false;
            s.ResetAndWorkOnSheets();
            s.SetMyMood(MoodIndicator.Good);
            s.ShowMyMoodNow(true);
        }
        yield return new WaitForSeconds(2f);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayerToOriginalPostion(true);
        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(false);
        //yield return new WaitForSeconds(10.0f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().ShowOrHideTeachersHands(false);
        print("Finished MainAction 1");
        //gamePlayManager.StartPhaseFour();
        MainActionTwo();

    }

    public GameObject animationCam;



    public void MainActionTwo()
    {

        StartCoroutine(TriggerMainActionThree());
    }
    IEnumerator TriggerMainActionThree()
    {
        print("Entering MainAction 2");

        animationCam.SetActive(true);

        yield return new WaitForSeconds(6.0f);
        // Leonie and Jannik will start talking and laughing.
        kidLeonie.StartMyTalkOrLaughAnimations(true);
        kidJannik.StartMyTalkOrLaughAnimations(true);
        if (!table2KidsTalking.isPlaying)
        {
            table2KidsTalking.volume = 0.02f;
            table2KidsTalking.PlayDelayed(1f);
        }
        // Increase the audio over span of 7 seconds.
        print("MA3: Increasing volume over 7 seconds");
        StartCoroutine(GradualVolumeChange(7, table2KidsTalking, 0.2f));

        // other students in table 2 changes mood from good to middle
        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2)
        {
            if(s != kidJannik && s != kidLeonie)
                s.SetMyMood(MoodIndicator.Middle);
        }
        yield return new WaitForSeconds(16.0f);
        print("MainAction 2 finished");

        animationCam.SetActive(false);
        gamePlayManager.StartPhaseFive();

    }
    public void MainActionThree()
    {
        //Teacher Question Panel
        StartCoroutine(TriggerTeacherQuestionPhase());


    }
    IEnumerator TriggerTeacherQuestionPhase()
    {
        print("MainAction 3 starts");
        yield return new WaitForSeconds(10.0f);
        print("Finished MainAction 3");        
        gamePlayManager.StartPhaseSix();
    }

    IEnumerator GradualVolumeChange(int numSeconds, AudioSource audioSource, float endVolume) {

        float increment = ((endVolume - audioSource.volume) / numSeconds);

        for (int i = 0; i <= numSeconds; i++) {
            yield return new WaitForSeconds(1.0f);
            audioSource.volume += increment;
        }
    }

    IEnumerator GradualVolumeRiseAndFall(int numSeconds, AudioSource audioSource, float midVolume, float endVolume) {
        //This will increment the volume up to the midVolume level by the mid-point of the duration, then decrease the volume to the endVolume from the mid-point to the end of the duration.
        float increment1 = ((midVolume - audioSource.volume) / (numSeconds/2));
        float increment2 = ((endVolume - midVolume) / (numSeconds / 2));

        for (int i = 0; i <= numSeconds; i++) {
            yield return new WaitForSeconds(1.0f);
            if (i < (numSeconds/2)) {
                audioSource.volume += increment1;
            } else {
                audioSource.volume += increment2;
            }
        }
    }

    /* this implementation is changed now
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

*/




    /* New Implementation is here */
    IEnumerator TriggerSRInitialActions(int whoCalled)
    {

        print("Initial Actions for SR starts");

        //make the scene ready for SR actions


        if (kidJannik.studentAnimation.GetAnimStateBool("EE2_Stand_Upset")) kidJannik.studentAnimation.EE2_StandUpset(false);
        if (kidJannik.studentAnimation.GetAnimStateBool("MA16_Sitting_Stand_Idle")) kidJannik.studentAnimation.MA16_SittingToStandUpIdle(false);

        if (kidLeonie.studentAnimation.GetAnimStateBool("EE2_Stand_Upset")) kidLeonie.studentAnimation.EE2_StandUpset(false);
        if (kidLeonie.studentAnimation.GetAnimStateBool("MA16_Sitting_Stand_Idle")) kidLeonie.studentAnimation.MA16_SittingToStandUpIdle(false);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayerToOriginalPostion(true);

        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);


        print("All students except leonie and Jainnk, resume working on their worksheets");
        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
            bool setMoodMiddle = false;
            studentAction.LookAtWindowRoutineStop();
            studentAction.LookAroundOrTalkOrWriteRoutineStop();
            // studentAction.studentAnimation.ResetAllAnim();
            if (studentAction != kidLeonie && studentAction !=kidJannik )
            {
                if (studentAction.studentAnimation.GetAnimStateBool("vi7_Talk_ON")) studentAction.studentAnimation.VI7_TalkToFriendsStop();
                studentAction.studentAnimation.MB33_WorkOnSheets(true);

                foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2) {
                    if (studentAction.name == s.name) {
                        setMoodMiddle = true;
                    }
                }

                if (setMoodMiddle) {
                    studentAction.SetMyMood(MoodIndicator.Middle);
                } else {
                    studentAction.SetMyMood(MoodIndicator.Good);
                }

                setMoodMiddle = false;
                
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        }

        // Leonie and Jannik will start talking and laughing.
        kidLeonie.StartMyTalkOrLaughAnimations(true);
        kidJannik.StartMyTalkOrLaughAnimations(true);

        // other students in table 2 changes mood from good to middle
        //foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2)
        //{
        //    if (s != kidJannik && s != kidLeonie)
        //        s.SetMyMood(MoodIndicator.Middle);
        //}
        //if (whoCalled == 3)
        //{
        //    table1KidsTalking.volume = 0.05f; // just mumbling
        //    table2KidsTalking.volume = 0.05f; // just mumbling
        //    table4KidsTalking.volume = 0.05f; // just mumbling
        //    table1KidsTalking.PlayDelayed(Random.Range(0.5f,1f));
        //    table2KidsTalking.PlayDelayed(Random.Range(0.5f, 1f));
        //    table4KidsTalking.PlayDelayed(Random.Range(0.5f, 1f));
        //}

        if (!table2KidsTalking.isPlaying)
        {
            table2KidsTalking.volume = 0.2f;
            table2KidsTalking.PlayDelayed(1f);
        } else {
            table2KidsTalking.volume = 0.2f;
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
        StartCoroutine(TriggerSRInitialActions(1));
    }

    IEnumerator StudentReactionOneInitiate()
    {
        print("SR 1 started");
        yield return new WaitForSeconds(2.0f);
        //if (table2KidsTalking.isPlaying) table2KidsTalking.Stop();
        print("Talking students of Table 2 realise teacher is looking at them");

        //gradual reduction in student mumbling over 5 seconds
        if (table2KidsTalking) {
            StartCoroutine(GradualVolumeChange(5, table2KidsTalking, 0f));
        }

        kidJannik.StopMyTalkOrLaughAnimations();
        yield return new WaitForSeconds(1.5f);
        kidLeonie.StopMyTalkOrLaughAnimations();

        print("Other students of Table 2  becomes good mood as soon as both the students stop talking");
        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2)
        {
            if (s != kidJannik && s != kidLeonie)
                s.SetMyMood(MoodIndicator.Good);
        }

        var teacherPositon = GameObject.FindGameObjectWithTag("Player").transform;
        print("Talking students of Table 2 start looking at teacher");
        kidJannik.LookAtSomeone(teacherPositon);
        yield return new WaitForSeconds(1.5f);
        kidLeonie.LookAtSomeone(teacherPositon);
        yield return new WaitForSeconds(1.5f);
        kidJannik.StopLookAtSomeone();
        print("Talking students of Table 2 stop looking at teacher and start working on worksheets");
        if (!kidJannik.studentAnimation.GetAnimStateBool("mb33_WriteOnSheet_ON")) kidJannik.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(1.5f);
        kidLeonie.StopLookAtSomeone();
        if (!kidLeonie.studentAnimation.GetAnimStateBool("mb33_WriteOnSheet_ON")) kidLeonie.studentAnimation.MB33_WorkOnSheets(true);

        // more to do, occationally few kids loo out side the window for 2-3 seconds



        /* no more valid

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
        */
        yield return new WaitForSeconds(4f);
        print("SR 1 finished");
    }



    private void StudentReactionTwo()
    {
        print("SR 2 should start anytime ");
        StartCoroutine(TriggerSRInitialActions(2));
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

        print("Table 2 students keep talking...");
        yield return new WaitForSeconds(Random.Range(1f, 2f));

        print("Table 2 other students stop working and look at teacher or the talking students...");
        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2)
        {
            if (s != kidJannik && s != kidLeonie)
            {
                s.SetMyMood(MoodIndicator.Middle);
                s.studentAnimation.MB33_WorkOnSheets(false);
                var whomToLookAt = Random.Range(0, 3);
                switch (whomToLookAt)
                {
                    case 1:
                        s.LookAtSomeone(kidLeonie.gameObject.transform);
                        break;
                    case 2:
                        s.LookAtSomeone(kidJannik.gameObject.transform);
                        break;
                    default:
                        s.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
                        break;
                }
            }
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        }
        print("Table 3  few students stop working and look at teacher or the talking students...");
        foreach (StudentAction s in table3DisturbedKids)
        {
            s.SetMyMood(MoodIndicator.Middle);
            s.studentAnimation.MB33_WorkOnSheets(false);
            var whomToLookAt = Random.Range(0, 3);
            switch (whomToLookAt)
            {
                case 1:
                    s.LookAtSomeone(kidLeonie.gameObject.transform);
                    break;
                case 2:
                    s.LookAtSomeone(kidJannik.gameObject.transform);
                    break;
                default:
                    s.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
                    break;
            }
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        }


        yield return new WaitForSeconds(2f);


        //if (table2KidsTalking.isPlaying) table2KidsTalking.Stop();
        print("Talking students of Table 2 realise teacher is looking at them");

        //gradual reduction in student mumbling over 10 seconds
        if (table2KidsTalking) {
            StartCoroutine(GradualVolumeChange(10, table2KidsTalking, 0f));
        }

        kidJannik.StopMyTalkOrLaughAnimations();
        yield return new WaitForSeconds(1.5f);
        kidLeonie.StopMyTalkOrLaughAnimations();

        print("Other students of Table 2  becomes good mood as soon as both the students stop talking and start working");
        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2)
        {
            if (s != kidJannik && s != kidLeonie)
            {
                s.StopLookAtSomeone();
                s.SetMyMood(MoodIndicator.Good);
                s.studentAnimation.MB33_WorkOnSheets(true);
                yield return new WaitForSeconds(Random.Range(0.5f, 1f));
            }
        }
        var teacherPositon = GameObject.FindGameObjectWithTag("Player").transform;
        print("Talking students of Table 2 start looking at teacher");
        kidJannik.LookAtSomeone(teacherPositon);
        yield return new WaitForSeconds(1.5f);
        kidLeonie.LookAtSomeone(teacherPositon);
        yield return new WaitForSeconds(1.5f);
        kidJannik.StopLookAtSomeone();
        print("Talking students of Table 2 stop looking at teacher and start working on worksheets");
        if (!kidJannik.studentAnimation.GetAnimStateBool("mb33_WriteOnSheet_ON")) kidJannik.studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(1.5f);
        kidLeonie.StopLookAtSomeone();
        if (!kidLeonie.studentAnimation.GetAnimStateBool("mb33_WriteOnSheet_ON")) kidLeonie.studentAnimation.MB33_WorkOnSheets(true);

        print("students of Table 3  becomes good mood as soon as both the students stop talking and start working");

        foreach (StudentAction s in table3DisturbedKids)
        {
            s.StopLookAtSomeone();
            s.SetMyMood(MoodIndicator.Good);
            s.studentAnimation.MB33_WorkOnSheets(true);
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        }


        yield return new WaitForSeconds(4f);
        print("SR 2 finished");






        /* No more correct, changed the scenario
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
        */


    }



    private void StudentReactionThree()
    {
        print("SR 3 should start anytime ");
        StartCoroutine(TriggerSRInitialActions(3));

    }

    IEnumerator StudentReactionThreeInitiate()
    {

        print("SR 3 started");

        // New Implementation

        // for simplicity sake I'm using the old Lists, so dont mind the naming of the lists.
        random10WorkingStudents.Clear();
        random9whisperingStudents.Clear();
        yield return new WaitForSeconds(2f);

        foreach (StudentAction s in gamePlayManager.studentsActions)
        {
            s.StopMyTalkOrLaughAnimations();
            s.LookAtWindowRoutineStop();
            s.LookAroundOrTalkOrWriteRoutineStop();
            if (s.studentAnimation.GetAnimStateBool("vi7_Talk_ON")) s.studentAnimation.VI7_TalkToFriendsStop();
            if (s.studentAnimation.GetAnimStateBool("mb33_WriteOnSheet_ON")) s.studentAnimation.MB33_WorkOnSheets(false);
            s.SetMyMood(MoodIndicator.Middle);
            var teacherPositon = GameObject.FindGameObjectWithTag("Player").transform;
            s.LookAtSomeone(teacherPositon);
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(Random.Range(1.5f,2.5f));

        print("Around 14 students start to whisper in between");

        var sampleStudents = new int[14];

        var rnd = new System.Random();
        int i = 0;
        while(i < sampleStudents.Length - 2)
        {
            var whichOne = rnd.Next(gamePlayManager.studentsActions.Count);
            if(gamePlayManager.studentsActions[whichOne] != kidLeonie && gamePlayManager.studentsActions[whichOne] != kidJannik)
            {
                gamePlayManager.studentsActions[whichOne].StopLookAtSomeone();
                gamePlayManager.studentsActions[whichOne].studentAnimation.VI11_TalkToFriendsLeftAndRight();
                random9whisperingStudents.Add(gamePlayManager.studentsActions[whichOne]);
                yield return new WaitForSeconds(Random.Range(0.5f, 1f));
                i++;
            }
        }

        //if (!table1KidsTalking.isPlaying) { table1KidsTalking.volume = 0.2f; table1KidsTalking.PlayDelayed(1f); }
        //if (!table2KidsTalking.isPlaying) { table2KidsTalking.volume = 0.2f; table2KidsTalking.PlayDelayed(1f); }
        //if (!table3KidsTalking.isPlaying) { table3KidsTalking.volume = 0.2f; table3KidsTalking.PlayDelayed(1f); }
        //if (!table4KidsTalking.isPlaying) { table4KidsTalking.volume = 0.2f; table4KidsTalking.PlayDelayed(1f); }

        //Adjust student volume of talking from medium -> loud -> low over the course of 10 seconds
        StartCoroutine(GradualVolumeRiseAndFall(10, table2KidsTalking, 0.8f, 0.05f));


        // Add Jannik and Leonie into this wishphering list
        kidJannik.StopLookAtSomeone();
        kidJannik.studentAnimation.VI11_TalkToFriendsLeftAndRight();
        random9whisperingStudents.Add(kidJannik);
        yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        kidLeonie.StopLookAtSomeone();
        kidLeonie.studentAnimation.VI11_TalkToFriendsLeftAndRight();
        random9whisperingStudents.Add(kidLeonie);
        yield return new WaitForSeconds(Random.Range(0.5f, 1f));


        print("Other 7 students work on their worksheets");


        foreach (StudentAction s in gamePlayManager.studentsActions)
        {
            if (!random9whisperingStudents.Contains(s))
            {
                s.StopMyTalkOrLaughAnimations();
                s.LookAtWindowRoutineStop();
                s.LookAroundOrTalkOrWriteRoutineStop();
                if (s.studentAnimation.GetAnimStateBool("vi11_Wispher_ON")) s.studentAnimation.VI11_TalkToFriendsStop();
                if (s.studentAnimation.GetAnimStateBool("vi7_Talk_ON")) s.studentAnimation.VI7_TalkToFriendsStop();
                s.studentAnimation.MB33_WorkOnSheets(true);
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            }
        }

        yield return new WaitForSeconds(2f);

        // Alls tudents start working except the two in Table 2

        foreach (StudentAction s in gamePlayManager.studentsActions)
        {
            if (s != kidJannik && s != kidLeonie)
            {
                s.StopMyTalkOrLaughAnimations();
                s.LookAtWindowRoutineStop();
                s.LookAroundOrTalkOrWriteRoutineStop();
                if (s.studentAnimation.GetAnimStateBool("vi11_Wispher_ON")) s.studentAnimation.VI11_TalkToFriendsStop();
                if (s.studentAnimation.GetAnimStateBool("vi7_Talk_ON")) s.studentAnimation.VI7_TalkToFriendsStop();
                s.studentAnimation.MB33_WorkOnSheets(true);
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            }
        }

        //if (table1KidsTalking.isPlaying) {  table1KidsTalking.Stop(); }
        //if (!table2KidsTalking.isPlaying) { table2KidsTalking.volume = 0.1f; table2KidsTalking.PlayDelayed(1f); }
        //if (table3KidsTalking.isPlaying) { table3KidsTalking.Stop(); }
        //if (table4KidsTalking.isPlaying) { table4KidsTalking.Stop(); }


        yield return new WaitForSeconds(4f);

        print("SR 3 Complete");

        /* These are no more valid


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

         */
    }


    private void StudentReactionFour()
    {
        print("SR 4 should start anytime ");
        StartCoroutine(TriggerSRInitialActions(4));
    }


    IEnumerator StudentReactionFourInitiate()
    {
        print("SR 4 started");


        yield return new WaitForSeconds(2f);


        //if (table2KidsTalking.isPlaying) table2KidsTalking.Stop();
        print("Talking students of Table 2 realise teacher is looking at them");

        //gradual reduction in student mumbling over 5 seconds
        if (table2KidsTalking) {
            StartCoroutine(GradualVolumeChange(5, table2KidsTalking, 0f));
        }

        kidJannik.StopMyTalkOrLaughAnimations();
        yield return new WaitForSeconds(1.5f);
        kidLeonie.StopMyTalkOrLaughAnimations();
        yield return new WaitForSeconds(1.5f);
        var teacherPositon = GameObject.FindGameObjectWithTag("Player").transform;
        print("Talking students of Table 2 start looking at teacher");
        kidJannik.LookAtSomeone(teacherPositon);
        yield return new WaitForSeconds(1.5f);
        kidLeonie.LookAtSomeone(teacherPositon);
        yield return new WaitForSeconds(1.5f);

        kidLeonie.SetMyMood(MoodIndicator.Bad);
        kidJannik.SetMyMood(MoodIndicator.Bad);
        if (kidLeonie.studentAnimation.GetAnimStateBool("vi11_Wispher_ON")) kidLeonie.studentAnimation.VI11_TalkToFriendsStop();
        if (kidLeonie.studentAnimation.GetAnimStateBool("vi7_Talk_ON")) kidLeonie.studentAnimation.VI7_TalkToFriendsStop();

        if (kidJannik.studentAnimation.GetAnimStateBool("vi11_Wispher_ON")) kidJannik.studentAnimation.VI11_TalkToFriendsStop();
        if (kidJannik.studentAnimation.GetAnimStateBool("vi7_Talk_ON")) kidJannik.studentAnimation.VI7_TalkToFriendsStop();

        print("Leonie Clomps her feet");
        // Leonie clomps with her feet (MB4) 2-3 seconds
        kidLeonie.studentAnimation.MB4_SitClompFeetMultipleTimes();
        yield return new WaitForSeconds(1f);
        print("All students mood becomes middle because of Leonie Clomps her feet");
        // as soon as Leonie clomps her feet all students with good mood becomes middle
        foreach (StudentAction s in gamePlayManager.studentsActions)
        {
            if (s != kidJannik && s != kidLeonie)
            {
                s.SetMyMood(MoodIndicator.Middle);
                yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
            }
        }
        print("Leonie throws her pencile box");
        // Leonie Throws down her pencil case (IWO14) and crosses her arms (MB26)
        kidLeonie.studentAnimation.ThrowPen(true);
        pencilCaseAnim.SetBool("isThrow", true);
        yield return new WaitForSeconds(3.0f);
        //kidLeonie.studentAnimation.IWO14_MB26_ThrowPencilBoxAndFoldHands();
        //yield return new WaitForSeconds(5f); // the above animation is for 4.9 seconds

        /* 
         * swapping per Jira task EQ-73
         * 
        print("Leonie  folds her hands");
        kidLeonie.studentAnimation.MB26_FoldHands(true);
        */
        // then gets up(ma16) and then gets upset(ee2 --> stays that way),
        print("Jannik Gets up and stands");
        kidLeonie.studentAnimation.MA16_SittingToStandUpIdle(true);
        yield return new WaitForSeconds(2f);
        print("Jannik Gets upset ");
        kidLeonie.studentAnimation.EE2_StandUpset(true);
        yield return new WaitForSeconds(2f);


        // Jannik clomps with his feet(MB4),
        print("Jannik Clomps his feet");
        kidJannik.studentAnimation.MB4_SitClompFeetMultipleTimes();
        yield return new WaitForSeconds(1f);
        // then gets up(ma16) and then gets upset(ee2 --> stays that way),
        print("Jannik Gets up and stands");
        kidJannik.studentAnimation.MA16_SittingToStandUpIdle(true);
        yield return new WaitForSeconds(2f);
        print("Jannik Gets upset ");
        kidJannik.studentAnimation.EE2_StandUpset(true);
        yield return new WaitForSeconds(2f);
        // the 4 other students on table 2 and all students from table 1 stop working(not all at once!)
        // and look at blackboard or teacher(mb11/ mb14 // not all at once! delay: 1-2 seconds)

        yield return new WaitForSeconds(2f);

        print("other students of Table 2 start looking at teacher");
        // other students in table 2 look at teacher
        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2)
        {
            if (s != kidJannik && s != kidLeonie)
            {
                s.StopLookAtSomeone();
                s.studentAnimation.MB33_WorkOnSheets(false);
                teacherPositon = GameObject.FindGameObjectWithTag("Player").transform;
               
                s.LookAtSomeone(teacherPositon);
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            }
        }

        // other students in table 1 look at teacher
        print("other students of Table 1 start looking at teacher");
        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable1)
        {
                s.StopLookAtSomeone();
                s.studentAnimation.MB33_WorkOnSheets(false);
                teacherPositon = GameObject.FindGameObjectWithTag("Player").transform;
                
                s.LookAtSomeone(teacherPositon);
                yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        }

        yield return new WaitForSeconds(6f);


        print("SR 4 Complete");

        /* not correct anymore

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

        */


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

        StartCoroutine(TriggerInitialAction());
        
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
