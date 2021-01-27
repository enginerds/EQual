using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScenarioThreeManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;
    public List<AudioSource> audiosInClass;

    public List<Transform> kidsOutsideSpots;
    string questionString = "";

    public Transform JannikFightSpot, MaximFightSpot, TeacherSpotNearJannikNewChair;

    public bool allKidsReachedBreak, KidsReachedFightSpot, jannikAndMaximSat = false;
    public StudentAction kidJannik, kidMaxim, kidNextToJannikNewChair;

    public int maximOriginalChairNumber, jannikOriginalChairNumber;

    public int maximNewChairNumber, jannikNewChairNumber;

    public ChairDetails maximOriginalChair, jannikOriginalChair, maximNewChair, jannikNewChair;
    public GameObject maximLeftHandStudyMaterial, maximRightHandStudyMaterial1, maximRightHandStudyMaterial2, jannikLeftHandStudyMaterial, jannikRightHandStudyMaterial1, jannikRightHandStudyMaterial2;

    public bool firstTimeSRPlayed = false;

    public GameObject bumpingOccurredPanel;

    private void Awake()
    {
        gamePlayManager.currentScenario = "SC3";
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

        gamePlayManager.userInterfaceManager.ShowOrHideKnowYourClassPanel(true);
        gamePlayManager.audioSource.Play();
        yield return new WaitForSeconds(3.0f);

        // show the skip button for the ShowOrHideKnowYourClassPanel
        if (gamePlayManager.getToKnowSkipBtn != null) gamePlayManager.getToKnowSkipBtn.gameObject.SetActive(true);
        //Initiate Kids going for break
       
    }

    public void WhatToDoBeforeInitialSituation()
    {
        StartCoroutine(StartWhatToDoBeforeInitialSituation());
    }


    IEnumerator StartWhatToDoBeforeInitialSituation()
    {
        gamePlayManager.userInterfaceManager.ShowOrHideStudentsInBreakPanel(true);
        yield return StartCoroutine(TriggerStudentsGoingOutForBreak());
        gamePlayManager.userInterfaceManager.ShowOrHideStudentsInBreakPanel(false);
        gamePlayManager.ReadyForInitialSituation = true;

    }

    IEnumerator TriggerSRInitialActions()
    {
        // move Player to their original position and turn towards original rotation.
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);

        /*
                for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
                {
                    gamePlayManager.studentsActions[i].studentAnimation.ResetCurrentAnim();
                    if (gamePlayManager.studentsActions[i] != kidJannik && gamePlayManager.studentsActions[i] != kidMaxim) gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
                    gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
                }
          */

        kidJannik.StopLookAtSomeone();
        kidMaxim.StopLookAtSomeone();

        kidJannik.FindMyChair("Chair",jannikOriginalChairNumber);
        kidMaxim.FindMyChair("Chair", maximOriginalChairNumber);
        kidJannik.GoToAndSitInChair();
        kidMaxim.GoToAndSitInChair();
        kidJannik.studentAnimation.WalkingToStandUpIdle(false);
        kidMaxim.studentAnimation.WalkingToStandUpIdle(false);

        jannikNewChair.ShowStudyMaterial(StudyMaterial.PencilBox, false);
        jannikNewChair.ShowStudyMaterial(StudyMaterial.WorkSheet, false);
        jannikNewChair.ShowStudyMaterial(StudyMaterial.Book, false);
        maximNewChair.ShowStudyMaterial(StudyMaterial.PencilBox, false);
        maximNewChair.ShowStudyMaterial(StudyMaterial.WorkSheet, false);
        maximNewChair.ShowStudyMaterial(StudyMaterial.Book, false);

        jannikAndMaximSat = false;
        StartCoroutine(TriggerCheckJannikAndMaximSat());

        yield return new WaitUntil(() => jannikAndMaximSat);

        jannikOriginalChair.ShowStudyMaterial(StudyMaterial.PencilBox);
        jannikOriginalChair.ShowStudyMaterial(StudyMaterial.WorkSheet);
        jannikOriginalChair.ShowStudyMaterial(StudyMaterial.Book);
        maximOriginalChair.ShowStudyMaterial(StudyMaterial.PencilBox);
        maximOriginalChair.ShowStudyMaterial(StudyMaterial.WorkSheet);
        maximOriginalChair.ShowStudyMaterial(StudyMaterial.Book);


        //yield return new WaitForSeconds(1f);


        kidJannik.studentAnimation.MB26_FoldHands(true);
        kidMaxim.studentAnimation.MB26_FoldHands(true);
        kidJannik.LookAtSomeone(kidMaxim.transform);
        kidMaxim.LookAtSomeone(kidJannik.transform);
        kidJannik.SetMyMood(MoodIndicator.Bad);
        kidMaxim.SetMyMood(MoodIndicator.Bad);

        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);
        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
            if (studentAction != kidJannik && studentAction != kidMaxim) studentAction.SetMyMood(MoodIndicator.Good);
            studentAction.ShowMyMoodNow(true);
            studentAction.StartMyRandomLookingOrWrittingAnimations();
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
        yield return new WaitForSeconds(0.5f);

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
                questionString = "Alle Kinder auffordern, ihre Aufgaben weiter zu bearbeiten. Jannik (SFB LE) und Maxim (SFB ESE) für ihre bisherige Aufgabenbearbeitung loben und auffordern, bis zum Ende der Stunde eine bestimmte Anzahl an Aufgaben zu bearbeiten.";
                StudentReactionOne();
                gamePlayManager.StartPhaseSeven(15.0f);
                break;
            case "2":
                questionString = "Jannik (SFB LE) und Maxim (SFB ESE) an zwei verschiedene Tische setzen und alle Kinder auffordern, ihre Aufgaben weiter zu bearbeiten.";
                StudentReactionTwo();
                gamePlayManager.StartPhaseSeven(40.0f);
                break;
            case "3":
                questionString = " Jannik (SFB LE) an einen freien Platz an einem anderen Tisch setzen und ihm helfen, die Bearbeitung der Aufgaben wieder aufzunehmen. Währenddessen alle Kinder auffordern, ihre Aufgaben weiter zu bearbeiten.";
                StudentReactionThree();
                gamePlayManager.StartPhaseSeven(35.0f);
                break;
            case "4":
                questionString = "Maxim (SFB ESE) an einen freien Platz an einem anderen Tisch setzen, ihn für seine bisherige Aufgabenbearbeitung loben und auffordern, eine bestimmte Anzahl an Aufgaben zu bearbeiten. Anschließend alle Kinder auffordern, ihre Aufgaben weiter zu bearbeiten.";
                StudentReactionFour();
                gamePlayManager.StartPhaseSeven(65.0f);
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



    IEnumerator TriggerStudentsGoingOutForBreak()
    {
        for (int i = 0; i < gamePlayManager.studentsActions.Count;i++)
        {
            gamePlayManager.studentsActions[i].InitiateGoToSpot(kidsOutsideSpots[i],5f);
            yield return new WaitForSeconds(0.1f);
        }
        StartCoroutine(TriggerCheckStudentsReachedForBreak());
        yield return new WaitUntil(() => allKidsReachedBreak);
        yield return new WaitForSeconds(1f);
    }


    IEnumerator TriggerCheckStudentsReachedForBreak()
    {
        allKidsReachedBreak = false;
        bool[] reachedSpots = new bool[gamePlayManager.studentsActions.Count];

        while (!allKidsReachedBreak)
        {
            for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
            {
                reachedSpots[i] = gamePlayManager.studentsActions[i].reachedSpot;
                yield return new WaitForSeconds(0f);
            }
            if (reachedSpots.All(x => x)) allKidsReachedBreak = true;
        }
       
        yield return new WaitForSeconds(1f);
    }



    public void MainActionOne()
    {
        StartCoroutine(StartMainActionOne());
    }

    IEnumerator StartMainActionOne()
    {
        // move Player to their original position and turn towards original rotation.
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayerToOriginalPostion(true);
        // GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table3);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);

        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
            studentAction.SetMyMood(MoodIndicator.Good);
            studentAction.ShowMyMoodNow(true);
            studentAction.studentAnimation.WalkingToStandUpIdle(false);
          if(studentAction != kidJannik && studentAction !=kidMaxim)  studentAction.GoToAndSitInChair(); // Sit on the new chair code
        }
        // mixim and jainak stop and get upset infront of each other.

        // kidMaxim.studentNavMesh.
        KidsReachedFightSpot = false;
        kidMaxim.studentNavMesh.SetNavMeshAgentDestination(MaximFightSpot.position, onReachedFightSpot);
        kidJannik.studentNavMesh.SetNavMeshAgentDestination(JannikFightSpot.position, onReachedFightSpot);
        yield return new WaitUntil(() => KidsReachedFightSpot);
        StartCoroutine(TriggerMainActionTwo());
    }
   private void onReachedFightSpot()
    {
        if(kidMaxim.studentNavMesh.IsAgentDestinationReached)
            {
                kidMaxim.studentAnimation.WalkingToStandUpIdle(true);
                if (kidJannik.studentNavMesh.IsAgentDestinationReached)
                {
                    kidMaxim.LookAtSomeone(kidJannik.transform);
                    kidMaxim.SetMyMood(MoodIndicator.Bad);
                    kidJannik.LookAtSomeone(kidMaxim.transform);
                    kidJannik.SetMyMood(MoodIndicator.Bad);
                    KidsReachedFightSpot = true;
                }
        }
        if (kidJannik.studentNavMesh.IsAgentDestinationReached)
        {
            kidJannik.studentAnimation.WalkingToStandUpIdle(true);
            if (kidMaxim.studentNavMesh.IsAgentDestinationReached)
            {
                kidMaxim.LookAtSomeone(kidJannik.transform);
                kidMaxim.SetMyMood(MoodIndicator.Bad);
                kidJannik.LookAtSomeone(kidMaxim.transform);
                kidJannik.SetMyMood(MoodIndicator.Bad);
                KidsReachedFightSpot = true;
            }
        }

        if (KidsReachedFightSpot) {
            //display canvas informing player that bumping has occurred
            bumpingOccurredPanel.SetActive(true);
            StartCoroutine(BumpingOccurred());
        }
    }

    IEnumerator BumpingOccurred () {
        yield return new WaitForSeconds(3);
        bumpingOccurredPanel.SetActive(false);
    }

    IEnumerator TriggerMainActionTwo()
    {
        // Teacher tells he students to sit down
        string teacherTextToSpeak = "Setzt euch bitte an eure Plätze.";
        if (gamePlayManager.LOG_ENABLED)
            yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(teacherTextToSpeak));
        else
            yield return null;
        gamePlayManager.StartPhaseFour();
    }
    public void MainActionTwo()
    {
        StartCoroutine(TriggerMainActionThree());
    }



    IEnumerator TriggerCheckJannikAndMaximSat()
    {
        jannikAndMaximSat = false;
        bool[] reachedSpots = new bool[2];

        while (!jannikAndMaximSat)
        {
            reachedSpots[0] = kidJannik.studentAnimation.IsSittingNow;
            reachedSpots[1] = kidMaxim.studentAnimation.IsSittingNow;
            yield return new WaitForSeconds(0.1f);
            if (reachedSpots.All(x => x)) jannikAndMaximSat = true;
        }

        yield return new WaitForSeconds(0.1f);
    }


    IEnumerator TriggerMainActionThree()
    {
        kidJannik.StopLookAtSomeone();
        kidMaxim.StopLookAtSomeone();

        kidJannik.GoToAndSitInChair();
        kidMaxim.GoToAndSitInChair();
        kidJannik.studentAnimation.WalkingToStandUpIdle(false);
        kidMaxim.studentAnimation.WalkingToStandUpIdle(false);

        //tacher looks at Table 4
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);


        jannikAndMaximSat = false;
        StartCoroutine(TriggerCheckJannikAndMaximSat());

        yield return new WaitUntil(() => jannikAndMaximSat);

        yield return new WaitForSeconds(Random.Range(1, 2));


        kidJannik.studentAnimation.MB26_FoldHands(true);
        kidMaxim.studentAnimation.MB26_FoldHands(true);
        kidJannik.LookAtSomeone(kidMaxim.transform);
        kidMaxim.LookAtSomeone(kidJannik.transform);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);

        yield return new WaitForSeconds(6.0f);
      
        gamePlayManager.StartPhaseFive();
    }
    public void MainActionThree()
    {        
        StartCoroutine(TriggerTeacherQuestionPhase());
        //Teacher Question Panel
    }
    IEnumerator TriggerTeacherQuestionPhase()
    {
       // nothing to do here, already done all the Main Actions needed

        yield return new WaitForSeconds(1.0f);
        print("Finished MainAction 3");        
        gamePlayManager.StartPhaseSix();     
    }

    

    private void StudentReactionOne()
    {
        StartCoroutine(StudentReactionOneEnum());        
    }

    IEnumerator StudentReactionOneEnum()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);
        yield return new WaitForSeconds(4f);
        kidJannik.studentAnimation.ResetAllAnim();
        kidMaxim.studentAnimation.ResetAllAnim();
        kidMaxim.SetMyMood(MoodIndicator.Good);
        kidJannik.SetMyMood(MoodIndicator.Good);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
        yield return new WaitForSeconds(6.0f);
        firstTimeSRPlayed = true;
        print("SR 1 finished");
    }
 


    private void StudentReactionTwo()
    {        
        StartCoroutine(StudentReactionTwoEnum());

        // GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, risedStudentLcoaiton);
        // GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().rotation = risedStudentLcoaiton.rotation;

        /*
         *
         *
         *-- {2 = the teacher tells both students to sit down on other seats at different tables, causing a small delay in task-completion. the other students begin their work right away.} --
         *
         * 
         *
         *The class (except for Jannik and Maxim) starts working (not all at once!, repetitive switches between mb21+mb33 with random durations);
         * Jannik and Maxim get up (ma16) and sit down on other seats (ma9 -->
         * Jannik = sits down on empty seat on table 1 (maybe D5? chair 15),
         * Maxim = sits down on empty seat on table 3 (maybe F4? chair25)];
         * while they get up and walk [authenticity]: a few students nearby look at Jannik or Maxim (whoever is closer to them) on their way to the new seats (mb13) [but start working after they finished looking];
         * as soon as Jannik and Maxim sit down,
         * they begin working (repetitive switches between mb21+mb33 with random durations)
         * [if code allows: make Jannik and Maxim carry their working sheet, book and pencil case to the new seats,
         * otherwise they can just magically appear in front of them,
         * same for the other SRs in which someone swaps seats]
         *
         *
         * */
    }

    IEnumerator StudentReactionTwoEnum() {

        yield return new WaitForSeconds(4f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);
        kidJannik.StopLookAtSomeone();
        kidMaxim.StopLookAtSomeone();
        kidJannik.studentAnimation.ResetAllAnim();
        kidMaxim.studentAnimation.ResetAllAnim();
        kidJannik.FindMyChair("Chair", jannikNewChairNumber);
        kidMaxim.FindMyChair("Chair", maximNewChairNumber);
        kidJannik.FindMyNeighbour();
        kidMaxim.FindMyNeighbour();
        jannikOriginalChair.ShowStudyMaterial(StudyMaterial.PencilBox, false);
        jannikOriginalChair.ShowStudyMaterial(StudyMaterial.WorkSheet, false);
        jannikOriginalChair.ShowStudyMaterial(StudyMaterial.Book, false);
        maximOriginalChair.ShowStudyMaterial(StudyMaterial.PencilBox, false);
        maximOriginalChair.ShowStudyMaterial(StudyMaterial.WorkSheet, false);
        maximOriginalChair.ShowStudyMaterial(StudyMaterial.Book, false);


        kidJannik.GoToAndSitInChair();
        kidMaxim.GoToAndSitInChair();

        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable4)
        {
            if (s != kidJannik && s != kidMaxim)
            {
                s.StopMyRandomLookingAnimations();
                int whichone = Random.Range(0, 2);
                if (whichone == 0) s.LookAtSomeone(kidJannik.transform); else s.LookAtSomeone(kidMaxim.transform);
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);

        jannikLeftHandStudyMaterial.SetActive(true);
        jannikRightHandStudyMaterial1.SetActive(true);
        jannikRightHandStudyMaterial2.SetActive(true);

        maximLeftHandStudyMaterial.SetActive(true);
        maximRightHandStudyMaterial1.SetActive(true);
        maximRightHandStudyMaterial2.SetActive(true);

        kidJannik.studentAnimation.WalkingToStandUpIdle(false);
        kidMaxim.studentAnimation.WalkingToStandUpIdle(false);


        jannikAndMaximSat = false;
        StartCoroutine(TriggerCheckJannikAndMaximSat());

        yield return new WaitUntil(() => jannikAndMaximSat);

        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable4)
        {
            if (s != kidJannik && s != kidMaxim)
            {
                s.StopMyRandomLookingAnimations();
                 s.StopLookAtSomeone();
                 s.StartMyRandomLookingOrWrittingAnimations();
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }

        jannikLeftHandStudyMaterial.SetActive(false);
        jannikRightHandStudyMaterial1.SetActive(false);
        jannikRightHandStudyMaterial2.SetActive(false);

        maximLeftHandStudyMaterial.SetActive(false);
        maximRightHandStudyMaterial1.SetActive(false);
        maximRightHandStudyMaterial2.SetActive(false);


        jannikNewChair.ShowStudyMaterial(StudyMaterial.PencilBox);
        jannikNewChair.ShowStudyMaterial(StudyMaterial.WorkSheet);
        jannikNewChair.ShowStudyMaterial(StudyMaterial.Book);
        maximNewChair.ShowStudyMaterial(StudyMaterial.PencilBox);
        maximNewChair.ShowStudyMaterial(StudyMaterial.WorkSheet);
        maximNewChair.ShowStudyMaterial(StudyMaterial.Book);


        yield return new WaitForSeconds(1f);
        kidJannik.StartMyRandomLookingOrWrittingAnimations();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1);
        yield return new WaitForSeconds(3f);
        kidMaxim.StartMyRandomLookingOrWrittingAnimations();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table3);

        kidMaxim.SetMyMood(MoodIndicator.Middle);
        kidJannik.SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(3f);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
        yield return new WaitForSeconds(6f);

        firstTimeSRPlayed = true;
        print("SR 2 finished");

    }



    private void StudentReactionThree()
    {
        StartCoroutine(StudentReactionThreeEnum());

        /*
         *
         *
         *
         *
         * -- {3 = the teacher only tells Jannik to sit down on another seat,
         * which is unfair (and causes a lack in Janniks motivation).
         * then the teacher goes to Jannik to help him resume work,
         * which catches the attention of kid next to Jannik.
         * Janniks mood improves a bit but he's still a little upset due to the unfair treatment.
         * the kid next to Jannik watches the interaction and resumes working as soon as Jannik starts working.} --
         *
         *
         *
         *
         * 
         *The class (except for Jannik) starts working (not all at once!,
         * repetitive switches between mb21+mb33 with random durations),
         * Jannik gets up (ma16) and sits down on another (empty) seat (ma9 --> maybe B7?) , Sac' correction B7 is not empty, and only D5 is empty :  chair 15)
         * then crosses his arms (mb42).
         * teacher walks up to him [student next to Jannik on B6 (Sac's Edit: not B6 instead it will be D6) stops working and watches interaction between Jannik and teacher (mb14/mb12)],
         * then teacher offers him some help/says something motivational (audio) (while teacher talks,
         * Jannik looks at teacher - mb14),
         * Jannik then starts working (repetitive switches between mb21+mb33 with random durations),
         * when Jannik starts working: student next to him starts working aswell
         * (delay! repetitive switches between mb21+mb33 with random durations)
         * [[beam teacher to front of class and display scenery for additional 5 seconds]]
         *
         *
         */

    }

    IEnumerator StudentReactionThreeEnum()
    {
        Debug.Log("SR3 started ");
        yield return new WaitForSeconds(4f);
        Debug.Log("Teacher Looking at table 4 ");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);
        kidJannik.StopLookAtSomeone();
        kidMaxim.StopLookAtSomeone();
        kidJannik.studentAnimation.ResetAllAnim();
        kidMaxim.studentAnimation.ResetAllAnim();
        kidMaxim.StartMyRandomLookingOrWrittingAnimations();
        kidMaxim.SetMyMood(MoodIndicator.Middle);
        Debug.Log("Kid Jannik finds the new chair to go to ");
        kidJannik.FindMyChair("Chair", jannikNewChairNumber);
        kidJannik.FindMyNeighbour();
        yield return new WaitForSeconds(1f);

        jannikOriginalChair.ShowStudyMaterial(StudyMaterial.PencilBox, false);
        jannikOriginalChair.ShowStudyMaterial(StudyMaterial.WorkSheet, false);
        jannikOriginalChair.ShowStudyMaterial(StudyMaterial.Book, false);
        Debug.Log("Jannik starts to walk to new chair ");
        kidJannik.GoToAndSitInChair();

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
        jannikLeftHandStudyMaterial.SetActive(true);
        jannikRightHandStudyMaterial1.SetActive(true);
        jannikRightHandStudyMaterial2.SetActive(true);


        kidJannik.studentAnimation.WalkingToStandUpIdle(false);

        jannikAndMaximSat = false;
        StartCoroutine(TriggerCheckJannikAndMaximSat());

        yield return new WaitUntil(() => jannikAndMaximSat);
        Debug.Log("Jannik stat on new chair ");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1);
        jannikLeftHandStudyMaterial.SetActive(false);
        jannikRightHandStudyMaterial1.SetActive(false);
        jannikRightHandStudyMaterial2.SetActive(false);

        jannikNewChair.ShowStudyMaterial(StudyMaterial.PencilBox);
        jannikNewChair.ShowStudyMaterial(StudyMaterial.WorkSheet);
        jannikNewChair.ShowStudyMaterial(StudyMaterial.Book);
        yield return new WaitForSeconds(2f);
        Debug.Log("Jannik Folds hands ");
        kidJannik.studentAnimation.MB26_FoldHands(true);

        yield return new WaitForSeconds(3f);
        //   *teacher walks up to him 
        Debug.Log("Teacher goes to Janak ");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, kidJannik.transform);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, TeacherSpotNearJannikNewChair);
        yield return new WaitForSeconds(2f);
        // [student next to Jannik on B6  stops working and watches interaction between Jannik and teacher (mb14/mb12)],
        kidNextToJannikNewChair.StopMyRandomLookingAnimations();
        kidNextToJannikNewChair.StopLookAtSomeone();
        kidNextToJannikNewChair.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);

        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);
        yield return new WaitForSeconds(1f);
        Debug.Log("Teacher Talks to him ");
        //   * then teacher offers him some help/says something motivational (audio) (while teacher talks,
        // Teacher talks...
        string teacherTextToSpeak = "Komm Jannik, wir machen zusammen weiter";
      //  SpeechManager.Instance.StartTalking(teacherTextToSpeak);

        if (gamePlayManager.LOG_ENABLED) SpeechManager.Instance.StartTalking(teacherTextToSpeak);
        yield return new WaitForSeconds(1f);

        //   * Jannik looks at teacher - mb14)
        Debug.Log("Jannik looks at teacher ");
        kidJannik.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
        yield return new WaitForSeconds(2f);
        kidJannik.SetMyMood(MoodIndicator.Middle);
        yield return new WaitForSeconds(2f);


        //  *Jannik then starts working(repetitive switches between mb21 + mb33 with random durations),
        kidJannik.StopLookAtSomeone();
        kidJannik.StartMyRandomLookingOrWrittingAnimations();
        Debug.Log("Jannik Starts writing ");
        yield return new WaitForSeconds(1f);
        //  * when Jannik starts working: student next to him starts working aswell
        kidNextToJannikNewChair.StopLookAtSomeone();
        kidNextToJannikNewChair.StartMyRandomLookingOrWrittingAnimations();
        yield return new WaitForSeconds(2f);
        // move Player to their original position and turn towards original rotation.
        Debug.Log("Teacher is back to their place ");
        // beam teacher to front of class
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);




        yield return new WaitForSeconds(6f);
        firstTimeSRPlayed = true;
        print("SR 3 finished");
    }


    private void StudentReactionFour()
    {
        StartCoroutine(StudentReactionFourEnum());

        

        


        /*
         *
         *
         *-- {4 = the teacher only tells Maxim to sit down on another seat,
         * which is unfair (and affects his mood+motivation even more since he has SN_ESE).
         * Maxim shows his discontent by refusing to work, then the teacher goes to Maxim to help him resume work and motivate him.
         * Maxim is not impressed and only works for a short period of time.
         * then he stops and distracts his neighboured kid and talks to him/her.
         * they both end up whispering, not doing their tasks, which then disturbs the other students on their table and lowers their mood.} --
         *
         *
         * 
         *The class (except for Maxim) starts working (not all at once!, repetitive switches between mb21+mb33 with random durations),
         * Maxim gets up (ma16) and sits down on another (empty) seat (ma9 --> maybe B7?),
         * then he protests/refuses to work (mb39 + mb42),
         * the teacher walks up to him (catches attention of neighboured kid -> mb14 in between work-cycle as long as teacher talks)
         * and praises him for previous task-completion and tries to motivate him (audio).
         * Maxim looks at his paper (mb16),
         * then starts working (repetitive switches between mb21+mb33 with random durations) for a bit.
         * [[teacher beams to front of class, with good sight of Maxim and his neighbour]]
         * but after some time (about 6 seconds),
         * he tips on the shoulder of kid next to him and starts whispering with him/her (mb28+vi11).
         * students on the same table as Maxim occasionally stop working and look at the talking students mb12)
         * [display for additional 5 seconds]
         *
         */

    }


    IEnumerator StudentReactionFourEnum()
    {
        //  GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, risedStudentLcoaiton);
        //    GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().rotation = risedStudentLcoaiton.rotation;

        Debug.Log("SR4 started ");
        yield return new WaitForSeconds(2f);

        
        //all students in class except Maxim begin to write (mb21 or mb33)
        print("All students resume working on their worksheets");
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {
            if (!gamePlayManager.studentsActions[i].name.Contains("Maxim")) {
                gamePlayManager.studentsActions[i].StopLookAtSomeone();
                gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
                gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();

                gamePlayManager.studentsActions[i].StartMyRandomLookingOrWrittingAnimations();
            } 
        }

        //yield return new WaitForSeconds(2f);


        Debug.Log("Teacher Looking at table 4 ");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table4);
        yield return new WaitForSeconds(2f);
        kidJannik.StopLookAtSomeone();
        kidMaxim.StopLookAtSomeone();
        kidJannik.studentAnimation.ResetAllAnim();
        kidMaxim.studentAnimation.ResetAllAnim();
        kidJannik.StartMyRandomLookingOrWrittingAnimations();
        kidJannik.SetMyMood(MoodIndicator.Middle);
        Debug.Log("Janak mood is middle ");

        //using same Jainnk's chair so we can be in the same 
        kidMaxim.FindMyChair("Chair", jannikNewChairNumber);
        kidMaxim.FindMyNeighbour();
        Debug.Log("Maxim finds the new chair");
        yield return new WaitForSeconds(1f);
        maximOriginalChair.ShowStudyMaterial(StudyMaterial.PencilBox, false);
        maximOriginalChair.ShowStudyMaterial(StudyMaterial.WorkSheet, false);
        maximOriginalChair.ShowStudyMaterial(StudyMaterial.Book, false);
        Debug.Log("Maxim goes to the new chair");
        kidMaxim.GoToAndSitInChair();
        yield return new WaitForSeconds(2f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);

        maximLeftHandStudyMaterial.SetActive(true);
        maximRightHandStudyMaterial1.SetActive(true);
        maximRightHandStudyMaterial2.SetActive(true);


        kidMaxim.studentAnimation.WalkingToStandUpIdle(false);

        jannikAndMaximSat = false;
        StartCoroutine(TriggerCheckJannikAndMaximSat());

        yield return new WaitUntil(() => jannikAndMaximSat);
        Debug.Log("Maxim sits on the new chair");
        yield return new WaitForSeconds(2f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().Table1);

        //set everyone's mood at the table to Middle
        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable1) {
            s.SetMyMood(MoodIndicator.Middle);
            int whichone = Random.Range(0, 2);
            if (whichone == 0)
                s.LookAtSomeone(kidMaxim.myNeighbourStudent.transform);
            else
                s.LookAtSomeone(kidMaxim.transform);
        }

        maximLeftHandStudyMaterial.SetActive(false);
        maximRightHandStudyMaterial1.SetActive(false);
        maximRightHandStudyMaterial2.SetActive(false);

        jannikNewChair.ShowStudyMaterial(StudyMaterial.PencilBox);
        jannikNewChair.ShowStudyMaterial(StudyMaterial.WorkSheet);
        jannikNewChair.ShowStudyMaterial(StudyMaterial.Book);

        yield return new WaitForSeconds(1f);

        Debug.Log("Maxim shakes his head");
        // *then he protests/ refuses to work(mb39 +mb42),
        // kidMaxim.studentAnimation.MB39_ShakesHead(true); // mb 42 is not there in our library

        // As per JIRA EQ-57, we will use MB19 instead of MB39
        kidMaxim.studentAnimation.MB19_Protest(true);

        yield return new WaitForSeconds(3f);
        //   *the teacher walks up to him
        Debug.Log("Teacher walks upto Maxim");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, kidMaxim.transform);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, TeacherSpotNearJannikNewChair);

        yield return new WaitForSeconds(2f);
        // catches attention of neighboured kid->mb14 in between work - cycle as long as teacher talks)
        kidNextToJannikNewChair.StopMyRandomLookingAnimations();
        kidNextToJannikNewChair.StopLookAtSomeone();
        kidNextToJannikNewChair.LookAtSomeone(GameObject.FindGameObjectWithTag("Player").transform);
        yield return new WaitForSeconds(2f);
        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);


        // * and praises him for previous task-completion and tries to motivate him(audio).
        yield return new WaitForSeconds(2f);
        // teacher speaks here..
        string teacherTextToSpeak = "Guck mal Maxim, du hast schon zwei Aufgaben gelöst. Da schaffst du doch auch noch ein paar mehr in dieser Stunde";
        if (gamePlayManager.LOG_ENABLED) SpeechManager.Instance.StartTalking(teacherTextToSpeak);

        yield return new WaitForSeconds(1f);

        //  *Maxim looks at his paper(mb16),
        Debug.Log("Maxim looks at his worrksheet");
        kidMaxim.LookAtSomeone(kidMaxim.chairPoint.gameObject.GetComponentInChildren<StudyMaterialType>().GetStudyMaterial(StudyMaterial.WorkSheet).transform);
        yield return new WaitForSeconds(2f);

        Debug.Log("Maxim starts to work on his worksheet");
        //    *then starts working(repetitive switches between mb21+mb33 with random durations) for a bit.
        kidMaxim.StartMyRandomLookingOrWrittingAnimations();
        yield return new WaitForSeconds(5f);

        kidNextToJannikNewChair.StopLookAtSomeone();
        kidNextToJannikNewChair.StartMyRandomLookingOrWrittingAnimations();
        Debug.Log("kid next to Maxim looks at his worrksheet");
        yield return new WaitForSeconds(2f);
        // * [[teacher beams to front of class, with good sight of Maxim and his neighbour]]

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().MovePlayer(true, gamePlayManager.GetComponent<MainObjectsManager>().TeacherInFrontOfClassPoint);
        Debug.Log("Teacher goes back to original position.");

        yield return new WaitUntil(() => !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isMove);
 


        // * but after some time (about 6 seconds),
        //  *he tips on the shoulder of kid next to him and starts whispering with him/ her(mb28 + vi11).
         yield return new WaitForSeconds(6.0f);

        kidMaxim.studentAnimation.MB28_TapOnNeighbour(true);
        yield return new WaitForSeconds(2f);

        kidMaxim.studentAnimation.VI11_TalkToFriendsLeftAndRight();
        kidMaxim.SetMyMood(MoodIndicator.Middle);
        kidMaxim.myNeighbourStudent.studentAnimation.VI7_TalkToFriendsLeftAndRight();
        // * students on the same table as Maxim occasionally stop working and look at the talking students mb12)

        foreach (StudentAction s in gamePlayManager.GetComponent<MainObjectsManager>().studentsAtTable1)
        {
            s.SetMyMood(MoodIndicator.Middle);
            int whichone = Random.Range(0, 2);
            if (whichone == 0) s.LookAtSomeone(kidMaxim.myNeighbourStudent.transform); else s.LookAtSomeone(kidMaxim.transform);
        }

     

        yield return new WaitForSeconds(16.0f);
        firstTimeSRPlayed = true;
        print("SR 4 finished");
    }


    public void Reset()
    {
        // what is the Student Mood BEFORE Reest?


        // get the background students back to their random looking and writing sequence.
        if (firstTimeSRPlayed) StartCoroutine(TriggerSRInitialActions());

        foreach (StudentAction studentAction in gamePlayManager.studentsActions)
        {
            Debug.LogFormat("{0} \t Mood BEFORE Reset: {1}  =>  SAVED Mood: {2}", studentAction.gameObject.name, studentAction.myCurrentMood, studentAction.mySavedMood);
            studentAction.SetMyMood(studentAction.mySavedMood); // Reset Moods
        }

        //Reset Anim

        gamePlayManager.StartPhaseSix();
    }

}
