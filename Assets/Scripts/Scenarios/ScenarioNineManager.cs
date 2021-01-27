using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioNineManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayManager gamePlayManager;

    public PlayerMovement playerMovement;

    public Transform tableOnePoint;
    public List<AudioSource> audiosInClass;

    string questionString = "";

    public Animator pencilAnim;
    public Animator pencilCaseAnim;

    public GameObject inHandPencil, inLeftHandPencil;
    public GameObject inHandPencil_Tom;

    public GameObject bumpingOccurredPanel;
    public GameObject howDoYouReactdPanel;
    private bool alreadyBeenHere = false;

    private void Awake()
    {
        if (gamePlayManager)
        {
            gamePlayManager.currentScenario = "SC9";
            gamePlayManager.StartPrepForStudentReaction += PrepForSRs;
        }
    }
        private void Start()
    {
        this.PlayBGAudio();
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
    }

    public void WhatToDoBeforeInitialSituation()
    {
        // nothing to do, let us continue to Initial Situation
        gamePlayManager.ReadyForInitialSituation = true;
    }

    public void StudentReaction(string teacherReactionIndex)
    {

        //display pop-up message only once (the first time through here)
        if (alreadyBeenHere == false) {
            alreadyBeenHere = true;
            howDoYouReactdPanel.SetActive(true);
            StartCoroutine(HowDoYouReact());
        }
        

        string value = teacherReactionIndex;
        gamePlayManager.CurrentPhase = "SR-" + value;
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TeacherActionSelected", value,true);
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TASQuestion", gamePlayManager.userInterfaceManager.teacherSelectedQuestion);
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetTeacherActionData("TASTimeStarted", gamePlayManager.GetCurrentTime());

        // get the background students back to their random looking and writing sequence.
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++) {

            //return each student to their seat
            gamePlayManager.studentsActions[i].ResetImmediatelyToChair();
            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);

            gamePlayManager.studentsActions[i].studentAnimation.ResetCurrentAnim();
            if (i != 16 && i!=19) // except for Leonie and tom
            {
                gamePlayManager.studentsActions[i].StartMyRandomLookingOrWrittingAnimations();
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
        gamePlayManager.StartPhaseSeven(35.0f);

    }

    void showTempText() {

     //   gamePlayManager.userInterfaceManager.ShowOrHideTempPanel(true);
     //   gamePlayManager.userInterfaceManager.SetTempText(questionString);


    }

    // SAc: Edit, We have removed Julia from the group of kids as she is not in the 9 year old list as per configuration, so all the index numbers will have to be modified as per this change where it is needed



    public void MainActionOne()
    {
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
            //gamePlayManager.studentsActions[i].studentAnimation.MB33_WorkOnSheets(true);


        }


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


        StartCoroutine(MainActionTwoRoutine());

    }


    IEnumerator MainActionTwoRoutine()
    {

        


        string textToTalk = "So, wir kommen langsam zum Ende der ersten Stunde. Wer noch eine neue Aufgabe braucht," +
                            " kann sich jetzt noch ein neues Arbeitsblatt hier vorne aussuchen und nach der Pause daran arbeiten.";
        if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));

        yield return new WaitForSeconds(1.0f); // timeframe is already covered for nearly 10 seconds

        

        playerMovement.LookToPlayer(true, gamePlayManager.studentsActions[1].transform);
        Debug.Log("GOING TOOOO Cubby");

        //        gamePlayManager.studentsActions[1].studentAnimation.Sitting(false);
        //        gamePlayManager.studentsActions[1].studentAnimation.MB33_WorkOnSheets(false);
        gamePlayManager.studentsActions[1].StopLookAtSomeone();
        gamePlayManager.studentsActions[1].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[1].studentAnimation.ResetAllAnim();

        gamePlayManager.studentsActions[1].InitiateGoToSpot((GameObject.Find("Kid_Box3Pos").transform));

        //        gamePlayManager.studentsActions[3].studentAnimation.Sitting(false);
        //        gamePlayManager.studentsActions[3].studentAnimation.MB33_WorkOnSheets(false);

        gamePlayManager.studentsActions[3].StopLookAtSomeone();
        gamePlayManager.studentsActions[3].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[3].studentAnimation.ResetAllAnim();
        gamePlayManager.studentsActions[3].InitiateGoToSpot((GameObject.Find("Kid_Box5Pos").transform));

        //        gamePlayManager.studentsActions[4].studentAnimation.Sitting(false);

        gamePlayManager.studentsActions[4].StopLookAtSomeone();
        gamePlayManager.studentsActions[4].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[4].studentAnimation.ResetAllAnim();
        gamePlayManager.studentsActions[4].InitiateGoToSpot((GameObject.Find("Kid_Box7Pos").transform));

        yield return new WaitUntil(() => gamePlayManager.studentsActions[1].reachedSpot);

        

        yield return new WaitForSeconds(1f);

        gamePlayManager.StartPhaseFive();


    }

    public void MainActionThree()
    {
        StartCoroutine(MainActionThreeRoutine());               
    }

    IEnumerator MainActionThreeRoutine()
    {
        gamePlayManager.studentsActions[1].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        gamePlayManager.studentsActions[1].GoToAndSitInChair();

        yield return new WaitUntil(() => gamePlayManager.studentsActions[3].reachedSpot);
        yield return new WaitUntil(() => gamePlayManager.studentsActions[4].reachedSpot);
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[3].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        gamePlayManager.studentsActions[3].StopLookAtSomeone();
        gamePlayManager.studentsActions[3].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[3].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[3].InitiateGoToSpot(GameObject.Find("E5").transform);

        gamePlayManager.studentsActions[4].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        gamePlayManager.studentsActions[4].StopLookAtSomeone();
        gamePlayManager.studentsActions[4].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[4].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[4].reachedSpot = false;
        gamePlayManager.studentsActions[4].InitiateGoToSpot(GameObject.Find("F5").transform);

        playerMovement.LookToPlayer(true, gamePlayManager.studentsActions[4].transform);

        yield return new WaitUntil(() => gamePlayManager.studentsActions[3].reachedSpot);
        yield return new WaitUntil(() => gamePlayManager.studentsActions[4].reachedSpot);

        Debug.Log("REACHED SPOOOOT");

        //Display pop-up indicating the kids bumped into each other and got mad
        bumpingOccurredPanel.SetActive(true);
        StartCoroutine(BumpingOccurred());


        Transform t3 = gamePlayManager.studentsActions[3].transform;
        Transform t4 = gamePlayManager.studentsActions[4].transform;
        //t3.position = new Vector3(t3.position.x, t3.position.y + 1f, t3.position.z);
        //t4.position = new Vector3(t4.position.x, t4.position.y + 1f, t4.position.z);

        gamePlayManager.studentsActions[3].LookAtTransformXZ(t4);
        gamePlayManager.studentsActions[4].LookAtTransformXZ(t3);

        //set moods of two arguing kids to Bad
        gamePlayManager.studentsActions[3].SetMyMood(MoodIndicator.Bad);
        gamePlayManager.studentsActions[4].SetMyMood(MoodIndicator.Bad);

        gamePlayManager.studentsActions[3].studentAnimation.EE2_StandUpset(true);
        gamePlayManager.studentsActions[4].studentAnimation.EE2_StandUpset(true);

        playerMovement.LookToPlayer(false, gamePlayManager.studentsActions[4].transform);
        playerMovement.MovePlayerToOriginalPostion(true);


        yield return new WaitForSeconds(8.0f);

        ////display pop-up message
        //howDoYouReactdPanel.SetActive(true);
        //StartCoroutine(HowDoYouReact());

        StartCoroutine(TriggerTeacherQuestionPhase());
    }

    IEnumerator BumpingOccurred() {
        yield return new WaitForSeconds(4);
        bumpingOccurredPanel.SetActive(false);
    }

    IEnumerator HowDoYouReact() {
        yield return new WaitForSeconds(5);
        howDoYouReactdPanel.SetActive(false);
    }

    IEnumerator TriggerTeacherQuestionPhase() {

        //Tom is back to do his worksheets / random look arounds
        //yield return new WaitForSeconds(5.0f);

        this.StopBGAudio();

        //string textToTalk = "Wie reagieren Sie auf diese Situation? Beachten Sie, dass Sie diese Klasse gerade erst übernommen haben. Sie unterbrechen die Arbeitsphase und";
        //if (gamePlayManager.LOG_ENABLED) yield return StartCoroutine(SpeechManager.Instance.WaitForTalking(textToTalk));

        

        yield return new WaitForSeconds(10.0f);

        print("Tom is no more aggitated");
        gamePlayManager.studentsActions[3].studentAnimation.EE2_StandUpset(false);
        gamePlayManager.studentsActions[4].studentAnimation.EE2_StandUpset(false);

        //set moods of two arguing kids back to Good
        gamePlayManager.studentsActions[3].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[4].SetMyMood(MoodIndicator.Good);

        gamePlayManager.studentsActions[3].StopLookAtSomeone();
        gamePlayManager.studentsActions[3].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[3].studentAnimation.ResetAllAnim();
        gamePlayManager.studentsActions[3].GoToAndSitInChair();

        gamePlayManager.studentsActions[4].StopLookAtSomeone();
        gamePlayManager.studentsActions[4].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[4].studentAnimation.ResetAllAnim();
        gamePlayManager.studentsActions[4].GoToAndSitInChair();

        


        // inLeftHandPencil.SetActive(false);
        gamePlayManager.StartPhaseSix();
        //gamePlayManager.userInterfaceManager.ShowOrHideTempPanel(false);

    }


    private void StudentReactionOne()
    {

        Debug.Log("Student Reaction One ");
        StartCoroutine(SROneRoutine());
    }

    IEnumerator SROneRoutine()
    {

        playerMovement.LookToPlace(true, GameObject.Find("Camera_SR3").transform);

        gamePlayManager.studentsActions[2].StopLookAtSomeone();
        gamePlayManager.studentsActions[2].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[2].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[2].InitiateGoToSpot(GameObject.Find("StationQueue1").transform);

        yield return new WaitForSeconds(1f);
        gamePlayManager.studentsActions[5].StopLookAtSomeone();
        gamePlayManager.studentsActions[5].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[5].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[5].InitiateGoToSpot(GameObject.Find("StationQueue2").transform);
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[6].StopLookAtSomeone();
        gamePlayManager.studentsActions[6].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[6].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[6].InitiateGoToSpot(GameObject.Find("StationQueue3").transform);
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[7].StopLookAtSomeone();
        gamePlayManager.studentsActions[7].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[7].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[7].InitiateGoToSpot(GameObject.Find("StationQueue4").transform);
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[8].StopLookAtSomeone();
        gamePlayManager.studentsActions[8].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[8].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[8].InitiateGoToSpot(GameObject.Find("StationQueue5").transform);

        yield return new WaitUntil((() => gamePlayManager.studentsActions[2].reachedSpot));
        gamePlayManager.studentsActions[2].transform.position = GameObject.Find("StationQueue1").transform.position;
        gamePlayManager.studentsActions[2].LookAtTransformXZ(GameObject.Find("PaperStand").transform);

        yield return new WaitUntil((() => gamePlayManager.studentsActions[5].reachedSpot));
        gamePlayManager.studentsActions[5].transform.position = GameObject.Find("StationQueue2").transform.position;
        gamePlayManager.studentsActions[5].LookAtTransformXZ(GameObject.Find("PaperStand").transform);

        yield return new WaitUntil((() => gamePlayManager.studentsActions[6].reachedSpot));
        gamePlayManager.studentsActions[6].transform.position = GameObject.Find("StationQueue3").transform.position;
        gamePlayManager.studentsActions[6].LookAtTransformXZ(GameObject.Find("PaperStand").transform);

        yield return new WaitUntil((() => gamePlayManager.studentsActions[7].reachedSpot));
        gamePlayManager.studentsActions[7].transform.position = GameObject.Find("StationQueue4").transform.position;
        gamePlayManager.studentsActions[7].LookAtTransformXZ(GameObject.Find("PaperStand").transform);

        yield return new WaitUntil((() => gamePlayManager.studentsActions[8].reachedSpot));
        gamePlayManager.studentsActions[8].transform.position = GameObject.Find("StationQueue5").transform.position;
        gamePlayManager.studentsActions[8].LookAtTransformXZ(GameObject.Find("PaperStand").transform);


        gamePlayManager.studentsActions[2].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        gamePlayManager.studentsActions[2].GoToAndSitInChair();
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[5].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        gamePlayManager.studentsActions[5].GoToAndSitInChair();
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[6].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        gamePlayManager.studentsActions[6].GoToAndSitInChair();
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[7].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        gamePlayManager.studentsActions[7].GoToAndSitInChair();
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[8].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        gamePlayManager.studentsActions[8].GoToAndSitInChair();
        yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(20f);

        yield return new WaitForEndOfFrame();
    }

    private void StudentReactionTwo()
    {
        StartCoroutine(SRTwoRoutine());
    }

    IEnumerator SRTwoRoutine()
    {

        playerMovement.LookToPlace(true, GameObject.Find("Camera_SR3").transform);

        gamePlayManager.studentsActions[2].StopLookAtSomeone();
        gamePlayManager.studentsActions[2].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[2].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[2].InitiateGoToSpot(GameObject.Find("StationQueue1").transform);

        // Modify Speed so that every student reaches at the same time
        Vector2 currentPos2 = new Vector2(gamePlayManager.studentsActions[8].transform.position.x, gamePlayManager.studentsActions[8].transform.position.z);
        Vector2 destPos2 = new Vector2(GameObject.Find("StationQueue1").transform.position.x, GameObject.Find("StationQueue1").transform.position.z);
        float dst2 = Vector2.Distance(currentPos2, destPos2);
        gamePlayManager.studentsActions[2].studentNavMesh.GetNavMeshAgent().speed = dst2 / 4f;

        //yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[5].StopLookAtSomeone();
        gamePlayManager.studentsActions[5].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[5].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[5].InitiateGoToSpot(GameObject.Find("StationQueue2").transform);

        // Modify Speed so that every student reaches at the same time
        Vector2 currentPos5 = new Vector2(gamePlayManager.studentsActions[8].transform.position.x, gamePlayManager.studentsActions[8].transform.position.z);
        Vector2 destPos5 = new Vector2(GameObject.Find("StationQueue2").transform.position.x, GameObject.Find("StationQueue2").transform.position.z);
        float dst5 = Vector2.Distance(currentPos5, destPos5);
        gamePlayManager.studentsActions[5].studentNavMesh.GetNavMeshAgent().speed = 5 / 4f;

        //yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[6].StopLookAtSomeone();
        gamePlayManager.studentsActions[6].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[6].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[6].InitiateGoToSpot(GameObject.Find("StationQueue3").transform);

        // Modify Speed so that every student reaches at the same time
        Vector2 currentPos6 = new Vector2(gamePlayManager.studentsActions[8].transform.position.x, gamePlayManager.studentsActions[8].transform.position.z);
        Vector2 destPos6 = new Vector2(GameObject.Find("StationQueue3").transform.position.x, GameObject.Find("StationQueue3").transform.position.z);
        float dst6 = Vector2.Distance(currentPos6, destPos6);
        gamePlayManager.studentsActions[6].studentNavMesh.GetNavMeshAgent().speed = dst6 / 4f;

        //yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[7].StopLookAtSomeone();
        gamePlayManager.studentsActions[7].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[7].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[7].InitiateGoToSpot(GameObject.Find("StationQueue4").transform);

        // Modify Speed so that every student reaches at the same time
        Vector2 currentPos7 = new Vector2(gamePlayManager.studentsActions[8].transform.position.x, gamePlayManager.studentsActions[8].transform.position.z);
        Vector2 destPos7 = new Vector2(GameObject.Find("StationQueue4").transform.position.x, GameObject.Find("StationQueue4").transform.position.z);
        float dst7 = Vector2.Distance(currentPos7, destPos7);
        gamePlayManager.studentsActions[7].studentNavMesh.GetNavMeshAgent().speed = dst7 / 4f;

        //yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[8].StopLookAtSomeone();
        gamePlayManager.studentsActions[8].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[8].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[8].InitiateGoToSpot(GameObject.Find("StationQueue5").transform);

        // Modify Speed so that every student reaches at the same time
        Vector2 currentPos8 = new Vector2(gamePlayManager.studentsActions[8].transform.position.x, gamePlayManager.studentsActions[8].transform.position.z);
        Vector2 destPos8 = new Vector2(GameObject.Find("StationQueue5").transform.position.x, GameObject.Find("StationQueue5").transform.position.z);
        float dst8 = Vector2.Distance(currentPos8, destPos8);
        gamePlayManager.studentsActions[8].studentNavMesh.GetNavMeshAgent().speed = dst8 / 4f;

        yield return new WaitUntil((() => gamePlayManager.studentsActions[2].reachedSpot));


        yield return new WaitUntil((() => gamePlayManager.studentsActions[5].reachedSpot));


        yield return new WaitUntil((() => gamePlayManager.studentsActions[6].reachedSpot));


        yield return new WaitUntil((() => gamePlayManager.studentsActions[7].reachedSpot));


        yield return new WaitUntil((() => gamePlayManager.studentsActions[8].reachedSpot));

        //Add student mumbling
        audiosInClass[0].volume = 0.35f;
        audiosInClass[0].Play();
        StartCoroutine(GradualVolumeChange(12, audiosInClass[0], 0));


        gamePlayManager.studentsActions[2].studentAnimation.EE2_StandUpset(true);
        yield return new WaitForSeconds(Random.Range(0.1f, 0.25f));

        gamePlayManager.studentsActions[5].studentAnimation.EE2_StandUpset(true);
        yield return new WaitForSeconds(Random.Range(0.1f, 0.25f));

        gamePlayManager.studentsActions[6].studentAnimation.EE2_StandUpset(true);
        yield return new WaitForSeconds(Random.Range(0.1f, 0.25f));

        gamePlayManager.studentsActions[7].studentAnimation.EE2_StandUpset(true);
        yield return new WaitForSeconds(Random.Range(0.1f, 0.25f));

        gamePlayManager.studentsActions[8].studentAnimation.EE2_StandUpset(true);

        gamePlayManager.studentsActions[2].SetMyMood(MoodIndicator.Middle);
        gamePlayManager.studentsActions[5].SetMyMood(MoodIndicator.Middle);
        gamePlayManager.studentsActions[6].SetMyMood(MoodIndicator.Middle);
        gamePlayManager.studentsActions[7].SetMyMood(MoodIndicator.Middle);
        gamePlayManager.studentsActions[8].SetMyMood(MoodIndicator.Middle);

        yield return new WaitForSeconds(6f);

        gamePlayManager.studentsActions[2].studentAnimation.EE2_StandUpset(false);
        gamePlayManager.studentsActions[5].studentAnimation.EE2_StandUpset(false);
        gamePlayManager.studentsActions[6].studentAnimation.EE2_StandUpset(false);
        gamePlayManager.studentsActions[7].studentAnimation.EE2_StandUpset(false);
        gamePlayManager.studentsActions[8].studentAnimation.EE2_StandUpset(false);

        gamePlayManager.studentsActions[2].transform.position = GameObject.Find("StationQueue1").transform.position;
        gamePlayManager.studentsActions[2].LookAtTransformXZ(GameObject.Find("PaperStand").transform);

        gamePlayManager.studentsActions[5].transform.position = GameObject.Find("StationQueue2").transform.position;
        gamePlayManager.studentsActions[5].LookAtTransformXZ(GameObject.Find("PaperStand").transform);

        gamePlayManager.studentsActions[7].transform.position = GameObject.Find("StationQueue4").transform.position;
        gamePlayManager.studentsActions[7].LookAtTransformXZ(GameObject.Find("PaperStand").transform);


        gamePlayManager.studentsActions[8].transform.position = GameObject.Find("StationQueue5").transform.position;
        gamePlayManager.studentsActions[8].LookAtTransformXZ(GameObject.Find("PaperStand").transform);

        gamePlayManager.studentsActions[6].transform.position = GameObject.Find("StationQueue3").transform.position;
        gamePlayManager.studentsActions[6].LookAtTransformXZ(GameObject.Find("PaperStand").transform);

        yield return new WaitForSeconds(2f);
        // Go and Sit in the chairs

        gamePlayManager.studentsActions[2].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        gamePlayManager.studentsActions[2].GoToAndSitInChair();
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[5].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        gamePlayManager.studentsActions[5].GoToAndSitInChair();
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[6].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        gamePlayManager.studentsActions[6].GoToAndSitInChair();
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[7].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        gamePlayManager.studentsActions[7].GoToAndSitInChair();
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[8].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        gamePlayManager.studentsActions[8].GoToAndSitInChair();
        yield return new WaitForSeconds(1f);


        gamePlayManager.studentsActions[2].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[5].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[6].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[7].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[8].SetMyMood(MoodIndicator.Good);

        yield return new WaitForSeconds(20f);

    }

    IEnumerator GradualVolumeChange(int numSeconds, AudioSource audioSource, float endVolume) {

        float increment = ((endVolume - audioSource.volume) / numSeconds);

        for (int i = 0; i <= numSeconds; i++) {
            yield return new WaitForSeconds(1.0f);
            audioSource.volume += increment;
        }
    }

    private void StudentReactionThree()
    {
        StartCoroutine(SRThreeRoutine());
    }

    IEnumerator SRThreeRoutine()
    {
        //LEON // The Special one
        gamePlayManager.studentsActions[9].StopLookAtSomeone();
        gamePlayManager.studentsActions[9].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[9].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[9].InitiateGoToSpot(GameObject.Find("StationQueue1").transform);
        gamePlayManager.studentsActions[9].studentNavMesh.GetNavMeshAgent().speed = 3f;
        gamePlayManager.studentsActions[9].studentAnimation.SetAnimationSpeed(3f);

        playerMovement.LookToPlayer(true, gamePlayManager.studentsActions[9].transform);
        // OTHER KIDDOSSS


        gamePlayManager.studentsActions[5].StopLookAtSomeone();
        gamePlayManager.studentsActions[5].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[5].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[5].InitiateGoToSpot(GameObject.Find("StationQueue2").transform);
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[6].StopLookAtSomeone();
        gamePlayManager.studentsActions[6].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[6].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[6].InitiateGoToSpot(GameObject.Find("StationQueue3").transform);
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[7].StopLookAtSomeone();
        gamePlayManager.studentsActions[7].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[7].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[7].InitiateGoToSpot(GameObject.Find("StationQueue4").transform);
        yield return new WaitForSeconds(1f);

        gamePlayManager.studentsActions[8].StopLookAtSomeone();
        gamePlayManager.studentsActions[8].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[8].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[8].InitiateGoToSpot(GameObject.Find("StationQueue5").transform);

        // Special Love for LEON
        yield return new WaitUntil(() => gamePlayManager.studentsActions[9].reachedSpot);


        gamePlayManager.studentsActions[9].studentAnimation.SetAnimationSpeed(1f);
        gamePlayManager.studentsActions[9].transform.position = GameObject.Find("StationQueue1").transform.position;
        gamePlayManager.studentsActions[9].LookAtTransformXZ(GameObject.Find("PaperStand").transform);
        gamePlayManager.studentsActions[9].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);


        gamePlayManager.studentsActions[5].SetMyMood(MoodIndicator.Bad);
        gamePlayManager.studentsActions[6].SetMyMood(MoodIndicator.Bad);
        gamePlayManager.studentsActions[7].SetMyMood(MoodIndicator.Bad);
        gamePlayManager.studentsActions[8].SetMyMood(MoodIndicator.Bad);

        yield return new WaitForSeconds(1f);
        gamePlayManager.studentsActions[9].GoToAndSitInChair();

        playerMovement.LookToPlace(true, GameObject.Find("Camera_SR3").transform);



        // Others Kiddos let them reach first
        yield return new WaitUntil((() => gamePlayManager.studentsActions[5].reachedSpot));
        yield return new WaitUntil((() => gamePlayManager.studentsActions[6].reachedSpot));
        yield return new WaitUntil((() => gamePlayManager.studentsActions[7].reachedSpot));
        yield return new WaitUntil((() => gamePlayManager.studentsActions[8].reachedSpot));

        gamePlayManager.studentsActions[5].LookAtTransformXZ(gamePlayManager.studentsActions[9].transform);
        gamePlayManager.studentsActions[6].LookAtTransformXZ(gamePlayManager.studentsActions[9].transform);
        gamePlayManager.studentsActions[7].LookAtTransformXZ(gamePlayManager.studentsActions[9].transform);
        gamePlayManager.studentsActions[8].LookAtTransformXZ(gamePlayManager.studentsActions[9].transform);

        gamePlayManager.studentsActions[5].studentAnimation.EE2_StandUpset(true);
        yield return new WaitForSeconds(Random.Range(0.1f, 0.25f));
        gamePlayManager.studentsActions[6].studentAnimation.EE2_StandUpset(true);
        yield return new WaitForSeconds(Random.Range(0.1f, 0.25f));

        gamePlayManager.studentsActions[7].studentAnimation.EE2_StandUpset(true);
        yield return new WaitForSeconds(Random.Range(0.1f, 0.25f));

        gamePlayManager.studentsActions[8].studentAnimation.EE2_StandUpset(true);

        //Add student mumbling
        audiosInClass[0].volume = 0.0f;
        audiosInClass[0].Play();
        StartCoroutine(GradualVolumeChange(2, audiosInClass[0], 0.4f));


        yield return new WaitForSeconds(6f);


        gamePlayManager.studentsActions[5].studentAnimation.EE2_StandUpset(false);
        gamePlayManager.studentsActions[6].studentAnimation.EE2_StandUpset(false);
        gamePlayManager.studentsActions[7].studentAnimation.EE2_StandUpset(false);
        gamePlayManager.studentsActions[8].studentAnimation.EE2_StandUpset(false);

        StartCoroutine(GradualVolumeChange(3, audiosInClass[0], 0.18f));

        gamePlayManager.studentsActions[5].transform.position = GameObject.Find("StationQueue1").transform.position;
        gamePlayManager.studentsActions[5].LookAtTransformXZ(GameObject.Find("PaperStand").transform);
        gamePlayManager.studentsActions[5].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        yield return new WaitForSeconds(1f);
        gamePlayManager.studentsActions[5].GoToAndSitInChair();
        yield return new WaitForSeconds(1f);
        gamePlayManager.studentsActions[5].StartMyRandomLookingOrWrittingAnimations();


        gamePlayManager.studentsActions[6].transform.position = GameObject.Find("StationQueue1").transform.position;
        gamePlayManager.studentsActions[6].LookAtTransformXZ(GameObject.Find("PaperStand").transform);
        gamePlayManager.studentsActions[6].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        yield return new WaitForSeconds(1f);
        gamePlayManager.studentsActions[6].GoToAndSitInChair();
        yield return new WaitForSeconds(1f);
        gamePlayManager.studentsActions[6].StartMyRandomLookingOrWrittingAnimations();

        gamePlayManager.studentsActions[7].transform.position = GameObject.Find("StationQueue1").transform.position;
        gamePlayManager.studentsActions[7].LookAtTransformXZ(GameObject.Find("PaperStand").transform);
        gamePlayManager.studentsActions[7].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        yield return new WaitForSeconds(1f);
        gamePlayManager.studentsActions[7].GoToAndSitInChair();
        yield return new WaitForSeconds(1f);
        gamePlayManager.studentsActions[7].StartMyRandomLookingOrWrittingAnimations();

        gamePlayManager.studentsActions[8].transform.position = GameObject.Find("StationQueue1").transform.position;
        gamePlayManager.studentsActions[8].LookAtTransformXZ(GameObject.Find("PaperStand").transform);
        gamePlayManager.studentsActions[8].SetMyRightHandStudyMaterialsVisibilty(StudyMaterial.WorkSheet, true);
        yield return new WaitForSeconds(1f);
        gamePlayManager.studentsActions[8].GoToAndSitInChair();
        yield return new WaitForSeconds(1f);
        gamePlayManager.studentsActions[8].StartMyRandomLookingOrWrittingAnimations();

        yield return new WaitForSeconds(3f);
        gamePlayManager.studentsActions[5].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[6].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[7].SetMyMood(MoodIndicator.Good);
        gamePlayManager.studentsActions[8].SetMyMood(MoodIndicator.Good);

    }

    private void StudentReactionFour()
    {
        StartCoroutine(SRFourRoutine());
    }

    IEnumerator SRFourRoutine()
    {
        gamePlayManager.studentsActions[9].SetMyMood(MoodIndicator.Middle);
        //4 , 7, 9
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Niko"), LayerMask.NameToLayer("Leon"));
        gamePlayManager.studentsActions[15].StopLookAtSomeone();
        gamePlayManager.studentsActions[15].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[15].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[15].InitiateGoToSpot(GameObject.Find("StationQueue1").transform);

        yield return new WaitForSeconds(1.5f);

        gamePlayManager.studentsActions[4].StopLookAtSomeone();
        gamePlayManager.studentsActions[4].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[4].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[4].InitiateGoToSpot(GameObject.Find("StationQueue1").transform);
        float s = gamePlayManager.studentsActions[4].studentNavMesh.GetNavMeshAgent().speed;
        gamePlayManager.studentsActions[4].studentNavMesh.GetNavMeshAgent().speed = s/2;

        yield return new WaitForSeconds(3f);
        //Maxim = studentsActions[9]
        gamePlayManager.studentsActions[9].StopLookAtSomeone();
        gamePlayManager.studentsActions[9].StopMyRandomLookingAnimations();
        gamePlayManager.studentsActions[9].studentAnimation.ResetAllAnim();
        //gamePlayManager.studentsActions[3].reachedSpot = false;
        gamePlayManager.studentsActions[9].InitiateGoToSpot(GameObject.Find("StationQueue1").transform);
        gamePlayManager.studentsActions[9].studentNavMesh.GetNavMeshAgent().speed = 2f;
        gamePlayManager.studentsActions[9].studentAnimation.SetAnimationSpeed(2f);

        playerMovement.LookToPlayer(true, gamePlayManager.studentsActions[9].transform);
        //playerMovement.MovePlayerToOriginalPostion(true);

        float dst = Vector3.Distance(gamePlayManager.studentsActions[9].transform.position, gamePlayManager.studentsActions[15].transform.position);

        gamePlayManager.studentsActions[15].studentNavMesh.GetNavMeshAgent().radius = 0.01f;

        yield return new WaitForSeconds(1f);



        gamePlayManager.studentsActions[15].studentNavMesh.GetNavMeshAgent().radius = 0.3f;
        gamePlayManager.studentsActions[15].ForceStopWalk();
        gamePlayManager.studentsActions[15].SetMyMood(MoodIndicator.Bad);
        gamePlayManager.studentsActions[15].studentAnimation.EE2_StandUpset(true);

        yield return new WaitForSeconds(0.5f);

        //Add student mumbling
        audiosInClass[0].volume = 0.0f;
        audiosInClass[0].Play();
        StartCoroutine(GradualVolumeChange(2, audiosInClass[0], 0.7f));

        gamePlayManager.studentsActions[9].ForceStopWalk();
        gamePlayManager.studentsActions[9].SetMyMood(MoodIndicator.Bad);
        gamePlayManager.studentsActions[9].studentAnimation.SetAnimationSpeed(1f);
        gamePlayManager.studentsActions[9].studentAnimation.EE2_StandUpset(true);
        gamePlayManager.studentsActions[9].LookAtTransformXZ(gamePlayManager.studentsActions[15].transform);
        gamePlayManager.studentsActions[15].LookAtTransformXZ(gamePlayManager.studentsActions[9].transform);

        gamePlayManager.studentsActions[4].ForceStopWalk();
        gamePlayManager.studentsActions[4].LookAtTransformXZ(gamePlayManager.studentsActions[9].transform);
        gamePlayManager.studentsActions[4].SetMyMood(MoodIndicator.Bad);

        

        for (int i = 0; i < gamePlayManager.studentsActions.Count;  i++)
        {
            if (i == 15 || i == 9 || i == 4) continue;

            gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Good);
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].LookAtSomeone(gamePlayManager.studentsActions[15].transform);
            if (i == 0 | i == 1 | i == 3) {
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Bad);
            } else {
                gamePlayManager.studentsActions[i].SetMyMood(MoodIndicator.Middle);
            }
            
        }


        playerMovement.LookToPlayer(false, gamePlayManager.studentsActions[9].transform);
        yield return null;
    }


    public void Reset()
    {

        StartCoroutine(ResetRoutine());
    }

    IEnumerator ResetRoutine()
    {
        for (int i = 0; i < gamePlayManager.studentsActions.Count; i++)
        {
            gamePlayManager.studentsActions[i].StopLookAtSomeone();
            gamePlayManager.studentsActions[i].StopMyRandomLookingAnimations();
            gamePlayManager.studentsActions[i].studentAnimation.ResetAllAnim();
            gamePlayManager.studentsActions[i].GoToAndSitInChair();

            if (i == 9 ||  i == 15)
            {
                gamePlayManager.studentsActions[i].studentNavMesh.GetNavMeshAgent().isStopped = false;
            }

        }


        gamePlayManager.StartPhaseSix();
        yield return new WaitForSeconds(2f);


    }

    public void PrepForSRs()
    {
        Debug.Log("Prep for SR called.........");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().LookToPlace(true, gamePlayManager.GetComponent<MainObjectsManager>().classmiddlePoint);

    }


    public void PlayBGAudio()
    {
        for (int i = 0; i < audiosInClass.Count; i++)
        {
            audiosInClass[i].Play();
        }

    }

    public void StopBGAudio()
    {
        for (int i = 0; i < audiosInClass.Count; i++)
        {
            audiosInClass[i].Stop();
        }
    }
}
