using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScenarioSixteenManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    public Transform teacherPosAtCubbyBox, leonieCubbyBoxPos, leoniePosAtCubbyBox, leonieTurnAwayPosAtCubbyBox, leonieLookAtTeacherPos;
    public AudioSource audioInClass, ripPaperAudio;
    public Transform SRLookAt;  //where to have the player look at when the SRs begin

    public StudentAction kidLeonie, kidAtB6;
    public int[] randomStudentsInAction;
    string questionString = "";

    private bool randomKidsSatBackInChairs = false;

    private void Awake()
    {
       
        if (gamePlayManager)
        {
            gamePlayManager.currentScenario = "SC16";
            gamePlayManager.StartPrepForStudentReaction += PrepForSRs;
        }
    }
        private void Start()
    {

        randomStudentsInAction = new int[2];
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
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true,gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
        kidLeonie.chairPoint.gameObject.GetComponent<ChairDetails>().ShowStudyMaterial(StudyMaterial.WorkSheet, false);
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

        // get the background students back to their random looking and writing sequence.
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].studentAnimation.ResetCurrentAnim();
            if (i != 16 && i!=19) // except for Leonie and tom
            {
                gamePlayManager.studentsActions[i].StartMyRandomLookingOrWrittingAnimations();
            }
        }
        //      GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        //      GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform.rotation = gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint.rotation;
        //       GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);

        

        switch (value)
        {

            case "1":
                questionString = "... sie daran erinnern, dass mit Lernmaterial sorgfältig umgegangen werden soll. Ihr anbieten, gemeinsam ein neues Arbeitsblatt auszusuchen.";

                StudentReactionOne();
                gamePlayManager.StartPhaseSeven(30.0f);
                break;
            case "2":
                questionString = "... sie daran erinnern, dass mit Lernmaterial sorgfältig umgegangen werden soll. Sie freundlich auffordern, sich ein neues Arbeitsblatt auszusuchen.";

                StudentReactionTwo();
                gamePlayManager.StartPhaseSeven(30.0f);
                break;
            case "3":
                questionString = "... sie an die Regel erinnern, dass Lernmaterialien nicht zerstört werden dürfen. Sie freundlich auffordern, sich ein neues Arbeitsblatt auszusuchen.";

                StudentReactionThree();
                gamePlayManager.StartPhaseSeven(30.0f);
                break;
            case "4":
                questionString = ".. sie freundlich fragen, welche Regel sie nicht beachtet hat und sie auffordern, mit ihrem Nachbarkind zusammen zu arbeiten.";
                StudentReactionFour();
                gamePlayManager.StartPhaseSeven(20.0f);

                break;
        }

        //showTempText();
        gamePlayManager.playerActionDataHandler.PlayerAction = questionString;
       

    }

    void showTempText() {

     //   gamePlayManager.userInterfaceManager.ShowOrHideTempPanel(true);
     //   gamePlayManager.userInterfaceManager.SetTempText(questionString);


    }

    // SAc: Edit, We have removed Julia from the group of kids as she is not in the 9 year old list as per configuration, so all the index numbers will have to be modified as per this change where it is needed



    public void MainActionOne()
    {
       
        kidLeonie.SetMyMood(MoodIndicator.Middle);
        kidLeonie.ShowMyMoodNow(true);
        StartCoroutine(ActivateMainActionOne());
      
    }

    IEnumerator ActivateMainActionOne()
    {
        yield return StartCoroutine(MainActionOneSequence());
        yield return null;
        gamePlayManager.StartPhaseFour();

    }

    IEnumerator MainActionOneSequence()
    {
       
        yield return new WaitForSeconds(1.0f);
        foreach (StudentAction sa in gamePlayManager.studentsActions)
        {
            sa.StopLookAtSomeone();
            sa.StopMyRandomLookingAnimations(); // stop the initial intro animations for all kids
            sa.scenarioStart = false;
            if(sa != kidLeonie)sa.StartMyRandomLookingOrWrittingAnimations();
            yield return new WaitForSeconds(Random.Range(0.1f,0.4f));
        }
        kidLeonie.studentAnimation.MB9_LookAround(true);
        randomStudentsInAction[0] = Random.Range(0,gamePlayManager.studentsActions.Count);
        randomStudentsInAction[0] = (randomStudentsInAction[0] == 16) ? 5 : (randomStudentsInAction[0] == 19) ? 5 : randomStudentsInAction[0];
        randomStudentsInAction[1] = Random.Range(0, gamePlayManager.studentsActions.Count);

        if(randomStudentsInAction[0] == randomStudentsInAction[1])
        {
            randomStudentsInAction[1] = (randomStudentsInAction[0] >= gamePlayManager.studentsActions.Count - 1) ? randomStudentsInAction[0] - 1 : (randomStudentsInAction[0] <= 1) ? randomStudentsInAction[0] + 1 : randomStudentsInAction[1];
            randomStudentsInAction[1] = (randomStudentsInAction[1] == 16) ? 10 : (randomStudentsInAction[1] == 19) ? 10 : randomStudentsInAction[1];
        }
        // triple check that we are not taking 16 or 19
        if (randomStudentsInAction[0] == 16 || randomStudentsInAction[0] == 19) randomStudentsInAction[0] = 5;
        if (randomStudentsInAction[1] == 16 || randomStudentsInAction[1] == 19) randomStudentsInAction[1] = 10;
        yield return new WaitForSeconds(2.5f);

        gamePlayManager.studentsActions[randomStudentsInAction[0]].StopLookAtSomeone();
        gamePlayManager.studentsActions[randomStudentsInAction[0]].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[randomStudentsInAction[0]].studentAnimation.ResetAllAnim();

        gamePlayManager.studentsActions[randomStudentsInAction[0]].InitiateGoToSpot(gamePlayManager.studentsActions[randomStudentsInAction[0]].myCubbyBoxPos.transform);
//        Debug.Log("Student 0 started to walk");
        yield return new WaitForSeconds(2.5f);
        gamePlayManager.studentsActions[randomStudentsInAction[1]].StopLookAtSomeone();
        gamePlayManager.studentsActions[randomStudentsInAction[1]].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[randomStudentsInAction[1]].studentAnimation.ResetAllAnim();
        gamePlayManager.studentsActions[randomStudentsInAction[1]].InitiateGoToSpot(gamePlayManager.studentsActions[randomStudentsInAction[1]].myCubbyBoxPos.transform);
   //     Debug.Log("Student 1 started to walk");


        yield return new WaitUntil(() => gamePlayManager.studentsActions[randomStudentsInAction[0]].reachedSpot);
    //    Debug.Log("Student 0 reached the cubby spot");
        yield return new WaitForSeconds(2.5f);
        gamePlayManager.studentsActions[randomStudentsInAction[0]].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        gamePlayManager.studentsActions[randomStudentsInAction[0]].GoToAndSitInChair();

        yield return new WaitUntil(() => gamePlayManager.studentsActions[randomStudentsInAction[1]].reachedSpot);

    //    Debug.Log("Student 1 reached the cubby spot");
        yield return new WaitForSeconds(2.5f);
        
        gamePlayManager.studentsActions[randomStudentsInAction[1]].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        gamePlayManager.studentsActions[randomStudentsInAction[1]].GoToAndSitInChair();

        randomKidsSatBackInChairs = false;
        StartCoroutine(TriggerCheckRandomKidsSat());

        yield return new WaitUntil(() => randomKidsSatBackInChairs);

        yield return new WaitForSeconds(Random.Range(1, 2));

        gamePlayManager.studentsActions[randomStudentsInAction[0]].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, false);
        gamePlayManager.studentsActions[randomStudentsInAction[1]].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, false);


        yield return new WaitForSeconds(Random.Range(1, 2));


        gamePlayManager.studentsActions[randomStudentsInAction[0]].StartMyRandomLookingOrWrittingAnimations();
        gamePlayManager.studentsActions[randomStudentsInAction[1]].StartMyRandomLookingOrWrittingAnimations();
    }

    IEnumerator TriggerCheckRandomKidsSat()
    {
        randomKidsSatBackInChairs = false;
        bool[] reachedSpots = new bool[2];

        while (!randomKidsSatBackInChairs)
        {
            reachedSpots[0] = gamePlayManager.studentsActions[randomStudentsInAction[0]].studentAnimation.IsSittingNow;
            reachedSpots[1] = gamePlayManager.studentsActions[randomStudentsInAction[1]].studentAnimation.IsSittingNow;
            yield return new WaitForSeconds(0.1f);
            if (reachedSpots.All(x => x)) randomKidsSatBackInChairs = true;
        }

        yield return new WaitForSeconds(0.1f);
    }


    IEnumerator TriggerMainActionTwo()
    {
        yield return null;

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4MoveToPoint);

        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);

        yield return new WaitForSeconds(1.0f);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, kidLeonie.gameObject.transform);
        yield return new WaitForSeconds(2.0f);

        string textToTalk = "Komm Leonie, suche dir auch eine Aufgabe raus.";
        if(gamePlayManager.LOG_ENABLED)SpeechManager.Instance.StartTalking(textToTalk);

        yield return new WaitForSeconds(3.0f);
        kidLeonie.SetMyMood(MoodIndicator.Bad);

        yield return new WaitForSeconds(2.0f);

        //Leonie talks back
        textToTalk = "Ich habe aber keine Lust.";
        if (gamePlayManager.LOG_ENABLED)
            yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(KidAtB6GoToCubbyBox());
    
        //yield return new WaitForSeconds(4.0f);

        gamePlayManager.StartPhaseFive();

    }

    IEnumerator KidAtB6GoToCubbyBox() {
        if (kidAtB6 != null) {
            kidAtB6.StopLookAtSomeone();
            kidAtB6.StopMyRandomLookingAnimations();
            kidAtB6.studentAnimation.ResetAllAnim();

            kidAtB6.InitiateGoToSpot(kidAtB6.myCubbyBoxPos.transform);
            Debug.Log("Student at B6 started to walk to his cubby box");
            yield return new WaitUntil(() => kidAtB6.reachedSpot);
            yield return new WaitForSeconds(2.5f);
            kidAtB6.GoToAndSitInChair();
            kidAtB6.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
            yield return new WaitUntil(() => kidAtB6.studentAnimation.IsSittingNow);
            kidAtB6.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, false);
            yield return new WaitForSeconds(1f);
            kidAtB6.StartMyRandomLookingOrWrittingAnimations();
        }
    }


    public void MainActionTwo()
    {
        StartCoroutine(TriggerMainActionTwo());
    }



    IEnumerator TriggerMainActionThree()
    {
        yield return new WaitForSeconds(1.0f); // timeframe is already covered for nearly 10 seconds


        ////Leonie talks back - moved from here to MainActionTwo
        //string textToTalk = "Ich habe aber keine Lust.";
        //if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));
        //yield return new WaitForSeconds(2.0f);


        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, false);
        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, false);
        kidLeonie.SetMyLeftHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, false);
        kidLeonie.StopLookAtSomeone();
        kidLeonie.StopMyRandomLookingAnimations();
        kidLeonie.studentAnimation.ResetAllAnim();

        kidLeonie.InitiateGoToSpot(leoniePosAtCubbyBox);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, kidLeonie.myCubbyBoxPos.transform);
        Debug.Log("Leonie started to walk to her cubby box");
        yield return new WaitForSeconds(4f);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, teacherPosAtCubbyBox);
        yield return new WaitForSeconds(2f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, kidLeonie.gameObject.transform);
        yield return new WaitUntil(() => kidLeonie.reachedSpot);
       // GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, kidLeonie.gameObject.transform);
        yield return new WaitForSeconds(1.5f);

        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        yield return new WaitForSeconds(1.5f);
        if (!ripPaperAudio.isPlaying) ripPaperAudio.Play();
        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, false);

        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, true);
        kidLeonie.SetMyLeftHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, true);

        foreach (StudentAction sa in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2)
        {
            sa.StopLookAtSomeone();
            sa.StopMyRandomLookingAnimations();
            sa.LookAtSomeone(kidLeonie.gameObject.transform);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
        }

        yield return new WaitForSeconds(1.5f);

        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, false);
        kidLeonie.SetMyLeftHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, false);
        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        if (ripPaperAudio.isPlaying) ripPaperAudio.Stop();
        yield return new WaitForSeconds(1.5f);
        
        if (!ripPaperAudio.isPlaying) ripPaperAudio.Play();
        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, false);
        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, true);
        kidLeonie.SetMyLeftHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, true);

        foreach (StudentAction sa in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2)
        {
            sa.StopLookAtSomeone();
            sa.StartLookBetweenTwoPeopoleRoutine(GameObject.FindGameObjectWithTag("Player").transform, kidLeonie.gameObject.transform,2f,4f);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
        }
        yield return new WaitForSeconds(3f);
        Debug.Log("MA3 finished");

        //Teacher Question Panel 
        StartCoroutine(TriggerTeacherQuestionPhase());

    }

    public void MainActionThree()
    {
        //gamePlayManager.userInterfaceManager.SetTempText("Student with ESE gives her a pen (iwo_28+iwo_7) + two random students (table 2 and table 3) are raising their hands (mb_20)");
        StartCoroutine(TriggerMainActionThree());
       
    }

    IEnumerator TriggerTeacherQuestionPhase() {

        yield return new WaitForSeconds(1.0f);
        gamePlayManager.StartPhaseSix();
    }


    private void StudentReactionOne()
    {

        /*
         * 
         * [rest of class (except kids on Table 2 who watch situation and student standing at the shelf) is still working,
         * cycle of mb21 /random duration per kid: 2.5-4 seconds + mb33 /random duration per kid: 2.5-5 seconds]
         * [teacher stands with Leonie at her box] 
         *  Sac: All the above already done is preseting of the SR
         *  
         * Leonie looks into her box (mb41: 2.5 seconds),
         * then at teacher (mb14: 2.5 seconds), 
         * then takes another working-sheet out of box (iwo32: working-sheet/box),
         * returns to her seat (ma7) and 
         * starts working (cycle of mb21 /random duration per kid: 2.5-4 seconds + mb33 /random duration per kid: 2.5-5 seconds); 
         * as soon as Leonie sits down: 
         * observing kids resume working (cycle of mb21 /random duration per kid: 2.5-4 seconds + mb33 /random duration per kid: 2.5-5 seconds)
         * 
         * 
         * 
         * Leonie, as soon as returning to seat = bad --> middle --> good (within 3 seconds)

         * */

        //student mumbling - off
        audioInClass.volume = 0.0f;
        audioInClass.Play();

        // start wait and other aspects of SR1
        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, false);
        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, false);
        kidLeonie.SetMyLeftHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, false);
        StartCoroutine(TriggerStudentReactioOne());
    }

    IEnumerator TriggerStudentReactioOne()
    {
        //set mood of table2 to Middle
        foreach (StudentAction sa in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2) {
            sa.SetMyMood(MoodIndicator.Middle);
        }

        //Leonie looks into her box (mb41: 2.5 seconds),
        yield return new WaitForSeconds(2.5f);
        //then at teacher (mb14: 2.5 seconds), 
        kidLeonie.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform);
        yield return new WaitForSeconds(2.5f);
        // then takes another working-sheet out of box (iwo32: working-sheet/box),
        // IWO32 animation is not there, so just attaching the Worksheet to the kid's hand.
        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        yield return new WaitForSeconds(2.0f);
        //returns to her seat (ma7)
        kidLeonie.TurnToPlayerOrObject(false);
        kidLeonie.StopLookAtSomeone();

        kidLeonie.GoToAndSitInChair();
        yield return new WaitForSeconds(1.0f);
        kidLeonie.StopLookAtSomeone();
        yield return new WaitUntil(() => kidLeonie.studentAnimation.IsSittingNow);

        kidLeonie.chairPoint.gameObject.GetComponent<ChairDetails>().ShowStudyMaterial(StudyMaterial.WorkSheet, true);
        yield return new WaitForSeconds(1f);
        //Leonie, as soon as returning to seat = bad --> middle 
        kidLeonie.SetMyMood(MoodIndicator.Middle);
        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, false);
        yield return new WaitForSeconds(2f);
        kidLeonie.StartMyRandomLookingOrWrittingAnimations(2.5f, 4f, 2.5f, 5f);
        yield return new WaitForSeconds(1f);
        //when starting work = middle --> good (within 3 seconds)
        kidLeonie.SetMyMood(MoodIndicator.Good);
        //as soon as Leonie sits down: 
        //observing kids resume working(cycle of mb21 / random duration per kid: 2.5 - 4 seconds + mb33 / random duration per kid: 2.5 - 5 seconds)
        foreach (StudentAction sa in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2)
        {
            sa.StopLookAtSomeone();
            sa.StopLookBetweenTwoPeopoleRoutine();
            sa.StartMyRandomLookingOrWrittingAnimations(2.5f, 4f, 2.5f, 5f);
            
            //set mood back to Good
            sa.SetMyMood(MoodIndicator.Good);

            yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
        }

        yield return new WaitForSeconds(6.0f);

    }
    private void StudentReactionTwo()
    {

        /*
         * 
         * [rest of class (except kids on Table 2 who watch situation and student standing at the shelf) is still working, 
         * cycle of mb21 /random duration per kid: 2.5-4 seconds + mb33 /random duration per kid: 2.5-5 seconds]
         * [teacher stands with Leonie at her box]
         * Sac: All the above already done is preseting of the SR
         * 
         * Leonie stands in front of box and looks at floor (mb35) for 3.5 seconds, 
         * then rummages in her box for a while (iwo26: box instead of school-bag / 6 seconds),
         * gets another working-sheet (iwo32: working-sheet/box),
         * returns to her seat (ma7) and 
         * starts working (cycle of mb21 /random duration per kid: 2.5-4 seconds + mb33 /random duration per kid: 2.5-5 seconds); 
         * as soon as Leonie sits down: 
         * observing kids resume working (cycle of mb21 /random duration per kid: 2.5-4 seconds + mb33 /random duration per kid: 2.5-5 seconds)



           Leonie, as soon as returning to seat = bad --> middle / when starting work = middle --> good (within 3 seconds)
         * 
         * 
         */

        //student mumbling - low
        //student mumbling - middle
        audioInClass.volume = 0.1f;
        audioInClass.Play();


        // Leonie Stands infront of her box and looks at the floor for 3.5 seconds
        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, false);
        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, false);
        kidLeonie.SetMyLeftHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, false);
        kidLeonie.LookAtSomeone(leonieTurnAwayPosAtCubbyBox);
       
        // start wait and other aspects of SR2
        StartCoroutine(TriggerStudentReactioTwo());

    }

    IEnumerator TriggerStudentReactioTwo()
    {
        //set mood of table2 to Middle
        foreach (StudentAction sa in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2) {
            sa.SetMyMood(MoodIndicator.Middle);
        }

        yield return new WaitForSeconds(3.5f);
        kidLeonie.StopLookAtSomeone();
        // IWO26 animation is not there
        // skipping to next step
        // IWO32 animation is not there, so just attaching the Worksheet to the kid's hand.
        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        yield return new WaitForSeconds(2.0f);
        //returns to her seat (ma7)
        kidLeonie.TurnToPlayerOrObject(false);
        kidLeonie.StopLookAtSomeone();
        kidLeonie.GoToAndSitInChair();

        //set mood of table2 to Good
        foreach (StudentAction sa in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2) {
            sa.SetMyMood(MoodIndicator.Good);
        }

        yield return new WaitUntil(() => kidLeonie.studentAnimation.IsSittingNow);
        yield return new WaitForSeconds(1f);
        //Leonie, as soon as returning to seat = bad --> middle 
        kidLeonie.SetMyMood(MoodIndicator.Middle);
        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, false);
        kidLeonie.chairPoint.gameObject.GetComponent<ChairDetails>().ShowStudyMaterial(StudyMaterial.WorkSheet, true);
        yield return new WaitForSeconds(2f);
        kidLeonie.StartMyRandomLookingOrWrittingAnimations(2.5f,4f,2.5f,5f);
        yield return new WaitForSeconds(1f);
        //when starting work = middle --> good (within 3 seconds)
        kidLeonie.SetMyMood(MoodIndicator.Good);
        //as soon as Leonie sits down: 
        //observing kids resume working(cycle of mb21 / random duration per kid: 2.5 - 4 seconds + mb33 / random duration per kid: 2.5 - 5 seconds)
        foreach (StudentAction sa in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2)
        {
            sa.StopLookAtSomeone();
            sa.StopLookBetweenTwoPeopoleRoutine();
            sa.StartMyRandomLookingOrWrittingAnimations(2.5f, 4f, 2.5f, 5f);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
        }
     
        yield return new WaitForSeconds(6.0f);

        

    }



    private void StudentReactionThree()
    {

        /*
         * 
         * [rest of class (except kids on Table 2 who watch situation) is still working,
         * cycle of mb21 /random duration per kid: 2.5-4 seconds + mb33 /random duration per kid: 2.5-5 seconds]
         * [teacher stands with Leonie at her box]
         * Sac: All the above already done is preseting of the SR
         * 
         * Leonie crosses arms while standing (mb42) and looks at her box (mb41: box) for 7 seconds, 
         * then takes a new working-sheet out of her box (iwo32: working-sheet/box) and returns to seat (ma7),
         * [observing kids resume working when Leonie sits down],
         * at seat: Leonie looks out of window (mb17 / 5 seconds), 
         * then resumes working (cycle of mb21 /random duration per kid: 2.5-4 seconds + mb33 /random duration per kid: 2.5-5 seconds)
         * 
         * 
         * Leonie (beginning to end) = bad (so unchanged)
         * 
         * 
         */

        //student mumbling - middle
        audioInClass.volume = 0.2f;
        audioInClass.Play();

        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, false);
        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, false);
        kidLeonie.SetMyLeftHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, false);
        //// Leonie crosses arms
        //kidLeonie.studentAnimation.MB42_StandCrossArms(true);

        StartCoroutine(TriggerStudentReactioThree());

    }

    IEnumerator TriggerStudentReactioThree()
    {
        // Leonie crosses arms and looks at her box(mb41: box) for 7 seconds
        kidLeonie.studentAnimation.MB42_StandCrossArms(true);

        //set mood of table2 to Middle
        foreach (StudentAction sa in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2) {
            sa.SetMyMood(MoodIndicator.Middle);
        }

        yield return new WaitForSeconds(7f);

        // IWO32 animation is not there, so just attaching the Worksheet to the kid's hand.

        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        yield return new WaitForSeconds(1.0f);
        //returns to her seat (ma7)
        kidLeonie.studentAnimation.MB42_StandCrossArms(false);
        kidLeonie.TurnToPlayerOrObject(false);
        kidLeonie.StopLookAtSomeone();
        kidLeonie.GoToAndSitInChair();


        yield return new WaitUntil(() => kidLeonie.studentAnimation.IsSittingNow);
       
        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, false);
        kidLeonie.chairPoint.gameObject.GetComponent<ChairDetails>().ShowStudyMaterial(StudyMaterial.WorkSheet, true);
        yield return new WaitForSeconds(1f);
        //[observing kids resume working when Leonie sits down],
        foreach (StudentAction sa in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2)
        {
            sa.StopLookAtSomeone();
            sa.StopLookBetweenTwoPeopoleRoutine();
            sa.StartMyRandomLookingOrWrittingAnimations(2.5f, 4f, 2.5f, 5f);
            sa.SetMyMood(MoodIndicator.Good);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
        }
        yield return new WaitForSeconds(1f);

        // Leonie looks out to window

        kidLeonie.LookAtWindowRoutine();

        yield return new WaitForSeconds(5f);

        kidLeonie.LookAtWindowRoutineStop();
        yield return new WaitForSeconds(1f);

        kidLeonie.StartMyRandomLookingOrWrittingAnimations(2.5f, 4f, 2.5f, 5f);
        yield return new WaitForSeconds(5f);

    }

    private void StudentReactionFour()
    {

        /*
         * 
         * 
         * [rest of class (except kids on Table 2 who watch situation) is still working, 
         * cycle of mb21 /random duration per kid: 2.5-4 seconds + mb33 /random duration per kid: 2.5-5 seconds]
         * [teacher stands with Leonie at her box]
         * Leonie stands in front of boxes and looks at teacher (mb14 - 3 seconds), 
         * then crosses arms (mb42), 
         * turns her back on teacher, 
         * looks at floor (mb35) and shakes head (mb39 - 2-3 seconds)
         * 
         * 
         * 
         * as in SR3 (bad for the whole time)
         * 
         * 
         */

        //student mumbling - middle
        audioInClass.volume = 0.2f;
        audioInClass.Play();
        

        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, false);
        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, false);
        kidLeonie.SetMyLeftHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, false);

        kidLeonie.TurnToPlayerOrObject(true, leonieLookAtTeacherPos,5f);
        kidLeonie.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().transform);

        StartCoroutine(TriggerStudentReactionFour());
    }

    IEnumerator TriggerStudentReactionFour()
    {

        //set mood of table2 to Middle
        foreach (StudentAction sa in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2) {
            sa.SetMyMood(MoodIndicator.Middle);
        }

        yield return new WaitForSeconds(3.0f);

        // Leonie crosses arms
        kidLeonie.studentAnimation.MB42_StandCrossArms(true);
        yield return new WaitForSeconds(2.0f);
        kidLeonie.TurnToPlayerOrObject(true, leonieTurnAwayPosAtCubbyBox,5f);
        yield return new WaitForSeconds(3.0f);
        // Leonie looks down and shakes head
        kidLeonie.studentAnimation.MB39_StandingShakesHead(true);
       yield return new WaitForSeconds(3.0f);

    }


    IEnumerator InitiateResetOfSRs()
    {
        gamePlayManager.StartPhaseSix();

        

        kidLeonie.SetMyMood(MoodIndicator.Bad);
        kidLeonie.StopLookAtSomeone();
        kidLeonie.StopSittingIdleLookAroundAnything();
        kidLeonie.studentAnimation.ResetAllAnim();
        kidLeonie.StopMyRandomLookingAnimations();

        kidLeonie.InitiateGoToSpot(leoniePosAtCubbyBox, 2.5f);
        kidLeonie.TurnToPlayerOrObject(true, leonieCubbyBoxPos, 5f);

        kidLeonie.chairPoint.gameObject.GetComponent<ChairDetails>().ShowStudyMaterial(StudyMaterial.WorkSheet, false);

        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, false);
        kidLeonie.SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, false);
        kidLeonie.SetMyLeftHandStudyMaterialsVisibilty(StudyMaterial.RippedPaper, false);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().TeleportPlayer(teacherPosAtCubbyBox);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, kidLeonie.gameObject.transform);

      

        Debug.Log("Leonie started to walk to her cubby box");
        foreach (StudentAction sa in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable2)
        {
            sa.StopLookAtSomeone();
            sa.StopSittingIdleLookAroundAnything(); // stop the initial intro animations for all kids
            sa.StopMyRandomLookingAnimations();
            sa.StartLookBetweenTwoPeopoleRoutine(GameObject.FindGameObjectWithTag("Player").transform, kidLeonie.gameObject.transform, 2f, 4f);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.25f));
        }



        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, kidLeonie.gameObject.transform);

        yield return new WaitForSeconds(1f);
        
       


    }

    public void PrepForSRs()
    {
        Debug.Log("Prep for SR called.........");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().TeleportPlayer(teacherPosAtCubbyBox);

        //orient Player to see more of table 2
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().TeleportPlayer(SRLookAt);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, kidLeonie.gameObject.transform);
        
        

    }

    public void Reset()
    {
        Debug.Log("Reset.........");
        StartCoroutine(InitiateResetOfSRs());
       
    }

}
