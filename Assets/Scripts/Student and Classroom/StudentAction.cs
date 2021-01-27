using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StudentAction : MonoBehaviour
{
    [Space(10)]
    [Header("NavMesh Target Positions")]
    public Transform classEnterPoint;
    public Transform chairPoint, deskPoint;
    public Transform TeacherDeskPoint, TeacherMainPoint, turnToPoint;
    public int ChairNumber;
    public string myPreviousCell = "", myCurrentCell = "";

    [Space(10)]
    [Header("Student Managers")]
    [SerializeField]
    public StudentAnimation studentAnimation;
    public StudentNavMeshAgent studentNavMesh;

    [Space(10)]
    [Header("Game Managers")]
    [SerializeField]
    private GamePlayManager gamePlayManager;

    [Space(10)]
    [Header("Student Scenario Specific Values")]
    public StudentScenarioSpecificValues studentScenarioValues;

    private bool isStudentInteractable;

    [Space(10)]
    [Header("Selected Study Material in Hand")]
    [SerializeField]
    public GameObject inHandSelectedStudyMaterial;
    [Space(10)]
    [Header("Selected Study Material on Child's Desk")]
    [SerializeField]
    public GameObject onHandSelectedStudyMaterial;
    [Space(10)]
    [Header("Selected Study Material on Common Place on Table")]
    [SerializeField]
    public GameObject onDeskCommonStudyMaterialHolder;


    [Space(10)]
    [Header("IK Control script")]
    public IKControl IKControlScript;


    [Space(10)]
    [Header("Is this student an ESE or LE student")]
    public bool ESEStudent, LEStudent;

    [Space(10)]
    [Header("Set programatically...")]
    private bool resetScenario;

    [Space(10)]
    public Vector3 sitPos;

    [Space(10)]
    [Header("Student's Cubby Box number")]
    public int CubbyBoxNumber;

    public StudyMaterialType rightHandStudyMaterials, leftHandStudyMaterials;

    [Space(10)]
    [Header("Set programatically...")]
    public bool tookAssignmentAndBackToSit;
    public bool lookingAtSomeone, scenarioStart, randomLookAtOrWriteScequenceStart, sittingIdleLookAroundAnything, RandomTalkAndLaughActive, RandomLookAtShakeHeadAndWriteActive;
    public bool updateMoveToLog, updateLookToLog;
    public StudentAction myNeighbourStudent;
    public GameObject myHiddenBag;
    public GameObject myCubbyBox, myCubbyBoxPos, myFloorObject;
    public Transform PlaceToGoTo, currentLookAtTarget;
    public bool reachedSpot;

    public bool TeacherSelectedMeUsingMouse;

    public bool activelyInvolvedInAction; // this is set when this student should be actively controled programmatically or else it can keep doing the normal other stuff that it is asked to do.


    [Space(10)]
    [Header("Notes pickup from Teacher's desk point")]
    public Vector3 teacherNotesPickupPos;

    //These functions are called when student's Mood changes.
    public event System.Action<MoodIndicator> MyMoodChange;
    public event System.Action<bool> ShowMyMood;
    public MoodIndicator mySavedMood { get; private set; }
    public MoodIndicator myCurrentMood { get; private set; }

    private bool isTurnAround = false;

    private Quaternion m_CharacterTargetRot;
    public GameObject currentTurnToTarget;

    public float rangeTocheckForTarget = 10;

    private IEnumerator activeSequence;

    private float turnSpeed = 0.8f;
    private float additionalTurnSpeed = 0f;

    private void Awake()
    {
        InitVariables();

        if (gamePlayManager)
        {
            //gamePlayManager.studentsActions.Add(this);
            gamePlayManager.StartTheNavMesh += EnterClassRoom;
            gamePlayManager.StartWithSitOnDesk += StartWithSitting;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        teacherNotesPickupPos = new Vector3((Random.Range(TeacherDeskPoint.position.x, 3.8f)), TeacherDeskPoint.position.y, TeacherDeskPoint.position.z);
        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        IKControlScript = this.gameObject.GetComponentInChildren<IKControl>();
        tookAssignmentAndBackToSit = false;
        MyMoodChange(MoodIndicator.Good);
        ShowMyMood(false);
        if (rightHandStudyMaterials != null) rightHandStudyMaterials.HideByDefaultStudyMaterials();
        if (leftHandStudyMaterials != null) leftHandStudyMaterials.HideByDefaultStudyMaterials();
        
    }


    public void ShowMyMoodNow(bool val)
    {
        ShowMyMood(val);        
    }


    public void SetMyMood(MoodIndicator val)
    {
        MyMoodChange(val);
        myCurrentMood = val;
    }
    public void SaveMyMood()
    {
        mySavedMood = myCurrentMood;
        // Debug.LogFormat("mySavedMood: {0}", mySavedMood);
    }

    public void StopMyInitialAnimations()
    {
        scenarioStart = false;
        if (lookingAtSomeone) StopLookAtSomeone();
        studentAnimation.ResetCurrentAnim();
    }

    public void StopMyRandomLookingAnimations()
    {
        //        print("Stopping Random Animation for" + this.gameObject.name);
        if (this.gameObject.name == "Leonie") Debug.Log(this.gameObject.name + " StopMyRandomLookingAnimations ");
        if (lookingAtSomeone) StopLookAtSomeone();
        studentAnimation.ResetCurrentAnim();
        randomLookAtOrWriteScequenceStart = false;
        StopSittingIdleLookAroundAnything();
    }

    public void StartMyRandomLookingOrWrittingAnimations(float minMB21 = 2f, float MaxMB21 = 4f, float minMB33 = 3f, float maxMB33 = 5f)
    {
            randomLookAtOrWriteScequenceStart = true;
            StartSittingIdleLookAroundAnything();
        if (this.gameObject.name == "Leonie") Debug.Log(this.gameObject.name + " InitiateRandomLookingOrWritingScequenceStart ");
        StartCoroutine(InitiateRandomLookingOrWritingScequenceStart(minMB21,MaxMB21,minMB33,maxMB33));
    }



    public void StartMyTalkOrLaughAnimations(bool eseLaughs = true)
    {
        RandomTalkAndLaughActive = true;

        StartCoroutine(InitiateTalkOrLaughScequenceStart(eseLaughs));
    }

    public void StopMyTalkOrLaughAnimations()
    {
        RandomTalkAndLaughActive = false;
    }

    public void StartMyLookAtShakeHeadAndWriteAnimations(Transform whomTo, bool peepAtWoorkSheetOrWriteOwn = true)
    {
        RandomLookAtShakeHeadAndWriteActive = true;
        StartCoroutine(InitiateLookAtSomeoneShakeHeadsAndWriteScequenceStart(whomTo, peepAtWoorkSheetOrWriteOwn));
    }

    public void StopMyLookAtShakeHeadAndWriteAnimations()
    {
        RandomLookAtShakeHeadAndWriteActive = false;
    }





    public void StopSittingIdleLookAroundAnything()
    {
        sittingIdleLookAroundAnything = false;
    }
    public void StartSittingIdleLookAroundAnything()
    {
        sittingIdleLookAroundAnything = true;
    }

    public void FindMyChair()
    {
        if (chairPoint == null)
        {
            var mychair = "Chair";
            if (ChairNumber > 0) mychair += ChairNumber.ToString();
            var theChair = GameObject.Find(mychair);
            if (theChair != null)
            {
                chairPoint = theChair.transform;
               if(theChair.GetComponent<ChairDetails>()!=null) theChair.GetComponent<ChairDetails>().StudentSittingOnMe = this;
            }
        }
    }
    public void FindMyChair(string chairBaseName, int chairnumber = -1)
    {
        // becareful, this has to be used only when you are asking the student to go to another chair which was not initially assigned to them,
        // moving to this chair will remove their actual chair from their mind, unless you call it again with original chair' Base name
        // if there is a old chair where the kid is sitting, then remove that kid reference fro that chair.
        if (this.chairPoint) this.chairPoint.gameObject.GetComponent<ChairDetails>().StudentSittingOnMe = null;

        var mychair = chairBaseName;
        if (chairnumber == -1) {
            if (ChairNumber > 0) mychair += ChairNumber.ToString();
        }
        else
            mychair += chairnumber.ToString();
            var theChair = GameObject.Find(mychair);
            if (theChair != null)
            {
                chairPoint = theChair.transform;
                if (theChair.GetComponent<ChairDetails>() != null) theChair.GetComponent<ChairDetails>().StudentSittingOnMe = this;
            }
        
    }

    public void FindMyCubbyBox()
    {
        if (myCubbyBox == null)
        {
            var myBox = "Kid_Box";
            if (CubbyBoxNumber > 0) myBox += CubbyBoxNumber.ToString();
            var theBox = GameObject.Find(myBox);
            if (theBox != null)
            {
                myCubbyBox = theBox;
               if(myCubbyBox.GetComponentInChildren<TextMeshPro>()!=null) myCubbyBox.GetComponentInChildren<TextMeshPro>().text = this.gameObject.name;
            }
            var theBoxPos = GameObject.Find(myBox+"Pos");
            if (theBoxPos != null)
            {
                myCubbyBoxPos = theBoxPos;
            }
        }
    }

    public void FindMyDesk()
    {
        if (deskPoint == null)
        {
            var mydesk = "Desk_0";

            if (chairPoint == null) FindMyChair();
            int deskNumber = 0;
            if (chairPoint.gameObject.GetComponent<ChairDetails>() != null)
            {
                var chairdetails = chairPoint.gameObject.GetComponent<ChairDetails>();
                if (chairdetails != null)
                    deskNumber = chairdetails.DeskNumber;
                if (deskNumber > 0) mydesk += deskNumber.ToString();
                var theDesk = GameObject.Find(mydesk);
                if (theDesk != null) deskPoint = theDesk.transform;
            }
        }
    }


    public void FindMyNeighbour()
    {
        if (myNeighbourStudent == null)
        {
            var mychair = "Chair";
            if (chairPoint.GetComponent<ChairDetails>() != null)
            {
                var neighbourChairNumber = chairPoint.GetComponent<ChairDetails>().MyNeighbourChair;
                if (neighbourChairNumber > 0) mychair += neighbourChairNumber.ToString();
                var theChair = GameObject.Find(mychair);
                if (theChair != null)
                {

                    myNeighbourStudent = theChair.GetComponent<ChairDetails>().StudentSittingOnMe;
                }

            }
            
        }
    }


    public bool SetMyRightHandStudyMaterialsVisibilty(StudyMaterial sm, bool visibleOrNot)
    {
        if (rightHandStudyMaterials != null)
        {
            return rightHandStudyMaterials.SetStudyMaterialVisiblityAndRetrun(sm, visibleOrNot);
        }
        else
            return false;
    }
    public bool SetMyLeftHandStudyMaterialsVisibilty(StudyMaterial sm, bool visibleOrNot)
    {
        if (leftHandStudyMaterials != null)
        {
            return leftHandStudyMaterials.SetStudyMaterialVisiblityAndRetrun(sm, visibleOrNot);
        }
        else
            return false;
    }

    public void SetTeacherSeatedPoint(Transform whereTo)
    {
        if (whereTo != null)
        {
            TeacherMainPoint = whereTo;
        }
    }

    public void SetTeacherNotesPickupPoint(Transform whereTo)
    {
        if (whereTo != null)
        {
            TeacherDeskPoint = whereTo;
            teacherNotesPickupPos = whereTo.position;
        }
    }

    public void StartWithSitting()
    {

        // GoToDesk();
        scenarioStart = true;
        SetMyMood(MoodIndicator.Good);
        ShowMyMoodNow(true);
        StartCoroutine(InitialGoToDesk());
    }
    public void GoToAndSitInChair()
    {
        StartCoroutine(InitiateGoToChair());
    }

    public void ResetImmediatelyToChair() {
        StartCoroutine(SitOnChair());
    }


    void InitVariables()
    {
        studentAnimation = GetComponent<StudentAnimation>();
        studentNavMesh = GetComponent<StudentNavMeshAgent>();
        gamePlayManager = FindObjectOfType<GamePlayManager>();
        studentScenarioValues = GetComponent<StudentScenarioSpecificValues>();
        FindMyChair();
        FindMyDesk();
        FindMyCubbyBox();
        FindMyNeighbour();


        /*
                print("Student Name = " + this.gameObject.name);
                print("Chair number = " + ChairNumber.ToString());
                print("Desk number = " + deskPoint.gameObject.name);
                print("CubbyBox number = " + CubbyBoxNumber.ToString());
                if(myNeighbourStudent!=null) print("Neighbour Student Name = " + myNeighbourStudent.gameObject.name);
          */
        if (gamePlayManager.LOG_ENABLED)
        {
            //logging student setup data to LogDB
            StudentSetupData tempdata = new StudentSetupData
            {
                StudentName = this.gameObject.name,
                Chair = ChairNumber,
                Desk = deskPoint.gameObject.name,
                CubbyBox = CubbyBoxNumber,
                NeighbourStudent = (myNeighbourStudent != null) ? myNeighbourStudent.gameObject.name : ""

            };
            LogDB.instance.SetStudentSetupData(tempdata);
        }

    }

    void EnterClassRoom()
    {
        studentNavMesh.SetNavMeshAgentDestination(classEnterPoint.position, OnEnteringRoomComplete);
    }

    void OnEnteringRoomComplete()
    {

        GoToDesk();
    }

    public void ResetRoom()
    {
        resetScenario = true;
        studentAnimation.RaiseHand(false);
        studentAnimation.Read(false);
        if(lookingAtSomeone)StopLookAtSomeone();
        if (inHandSelectedStudyMaterial != null)
        {
            inHandSelectedStudyMaterial.SetActive(false);
        }

        if (onHandSelectedStudyMaterial != null)
        {
            onHandSelectedStudyMaterial.SetActive(false);
        }
        if (studentScenarioValues.scenario1SR3ReadAndThenLookAround) studentScenarioValues.scenario1SR3ReadAndThenLookAround = false;
        GoToDesk();
    }

    public void TeleportKid(Transform whereTo, bool StandOrSit = false)
    {
        Transform tempPos = whereTo;
        tempPos.position = new Vector3(whereTo.position.x,transform.position.y,whereTo.position.z);
        this.studentNavMesh.GetNavMeshAgent().enabled = false;
        //   transform.position = TeacherDeskPoint.position;
        transform.SetPositionAndRotation(tempPos.position, tempPos.rotation);
        if(StandOrSit)
        {
            this.studentAnimation.ResetAllAnimAndWalk();
            this.studentAnimation.WalkingToStandUpIdle(true);
        }
        this.studentNavMesh.GetNavMeshAgent().enabled = true;

    }

    private IEnumerator InitiateInitialScequenceStart()
    {
        IEnumerator initiSequene = InitialScequenceStart();
        yield return StartCoroutine(initiSequene);
        while ( sittingIdleLookAroundAnything && scenarioStart)
        {
            yield return null;
        }
        //        print("Kill Init Sequence called");
        if (lookingAtSomeone) StopLookAtSomeone();
        studentAnimation.MB33_WorkOnSheets(false);
        StopCoroutine(initiSequene);
    }

    private IEnumerator InitialScequenceStart()
    {
        while (scenarioStart)
        {
            yield return new WaitForSeconds(Random.Range(1f, 2f));
            studentAnimation.MB33_WorkOnSheets(false);
            int myActionToDo = Random.Range(5, 25) % 5;
            //   print(myActionToDo);
            switch (myActionToDo)
            {
                case 1:
                case 2:
                case 5:
                    // do nothing
                    if (lookingAtSomeone) StopLookAtSomeone();
                    break;
                
                case 3:
                    // look at the neighbour
             //       if (gameObject.name == "Tom") print("Init Tom Looking at Neighbour");
            //        if (gameObject.name == "Leonie") print("Init Leonie Looking at Neighbour");
                    LookAtNeighbourRoutine();
                    break;
                case 4:
                    // look out of window
             //       if (gameObject.name == "Tom") print("Init Tom Looking at Window");
             //       if (gameObject.name == "Leonie") print("Init Leonie Looking at Window");
                    LookAtWindowRoutine();
                    break;
                default: // case 0 or 5 above
                    //look around the class
              //      if (gameObject.name == "Tom") print("Init Tom Looking at Interesting spots");
              //      if (gameObject.name == "Leonie") print("Init Leonie Looking at Interesting spots");
                    LookAtInterestingSpotsRoutine();
                    break;

            }
            yield return new WaitForSeconds(Random.Range(2f, 4f));

            // stop the action that was been done and restart till their is an interuption
            switch (myActionToDo)
            {
                case 1:
                case 2:
                case 5:
                    // do nothing
                    if (lookingAtSomeone) StopLookAtSomeone();
                    break;
                
                case 3:
                    // look at the neighbour
                //    if (gameObject.name == "Tom") print("Init Tom  Stopped Looking at Neighbour");
                //    if (gameObject.name == "Leonie") print("Init Leonie  Stopped Looking at Neighbour");
                    LookAtNeighbourStop();
                    break;
                case 4:
                    // look out of window
                    // this will be automatically handled
                //    if (gameObject.name == "Tom") print("Inint Tom Stopped Looking at Window");
                //    if (gameObject.name == "Leonie") print("Inint Leonie Stopped Looking at Window");
                    LookAtWindowRoutineStop();
                    break;
                default: // case 0 and above 5

                    //look around the class
               //     if (gameObject.name == "Tom") print("Init Tom Stopped Looking at Intersting spots");
               //     if(gameObject.name == "Leonie") print("Init Leonie Stopped Looking at Intersting spots");
                    LookAtInterestingSpotsRoutineStop();
                    break;

            }
            if (lookingAtSomeone) StopLookAtSomeone();
            //  print("Inity still going on");
            yield return null;
        }
    }


    private IEnumerator InitiateRandomLookingOrWritingScequenceStart(float minMB21 = 2f, float MaxMB21 = 4f, float minMB33 = 3f, float maxMB33 = 5f)
    {
        IEnumerator initiSequene = RandomLookingOrWritingScequenceStart(minMB21, MaxMB21, minMB33, maxMB33);
       if(this.gameObject.name == "Leonie") Debug.Log(this.gameObject.name + " Random Looking or writing scequence about to start");
        yield return StartCoroutine(initiSequene);
        if (this.gameObject.name == "Leonie") Debug.Log(this.gameObject.name + " Finished Random Looking or writing scequence ");
        while (sittingIdleLookAroundAnything && randomLookAtOrWriteScequenceStart)
        {
            // do nothing
            yield return null;
        }
        if (this.gameObject.name == "Leonie") Debug.Log(this.gameObject.name + " Finished Do nothing in Random Looking or writing scequence ");
        if (lookingAtSomeone) StopLookAtSomeone();
        studentAnimation.MB33_WorkOnSheets(false);
        if (this.gameObject.name == "Leonie") Debug.Log(this.gameObject.name + " Wroking is stopped in Random Looking or writing scequence ");
        StopCoroutine(initiSequene);
        if (this.gameObject.name == "Leonie") Debug.Log(this.gameObject.name + " Scequence killed is stopped in Random Looking or writing scequence ");
        yield return null;
    }

    private IEnumerator RandomLookingOrWritingScequenceStart(float minMB21=2f,float MaxMB21=4f,float minMB33=3f,float maxMB33=5f)
    {
        while (randomLookAtOrWriteScequenceStart)
        {
                yield return new WaitForSeconds(0.2f);
           
                // studentAnimation.MB33_WorkOnSheets(false);
                int myActionToDo = Random.Range(5, 25) % 5;
            if (this.gameObject.name == "Leonie") Debug.Log(this.gameObject.name + " doing Random Looking or writing scequence : " + myActionToDo.ToString());
            //   print(myActionToDo);
            switch (myActionToDo)
                {
                    case 1:
                    case 2:
                    case 5:
                        // stop looking and write
                        if (lookingAtSomeone) StopLookAtSomeone();
                        studentAnimation.MB33_WorkOnSheets(true);
                        break;
                    
                    case 3:
                    // look at the neighbour
               //     if (gameObject.name == "Tom") print("Tom Looking at Neighbour");
                        LookAtNeighbourRoutine();
                        break;
                    case 4:
                    // look out of window
                 //   if (gameObject.name == "Tom") print("Tom Looking at Window");
                    LookAtWindowRoutine();
                        break;
                    default: // case 0 or 5 above
                             //look around the class
                   // if (gameObject.name == "Tom") print("Tom Looking at Interesting spots");
                    LookAtInterestingSpotsRoutine();
                        break;

                }
            if(myActionToDo == 1 || myActionToDo == 2 || myActionToDo == 5)
                yield return new WaitForSeconds(Random.Range(minMB33, maxMB33));
            else
                yield return new WaitForSeconds(Random.Range(minMB21, MaxMB21));

            if (this.gameObject.name == "Leonie") Debug.Log(this.gameObject.name + " getting back to normal doing Random Looking or writing scequence : " + myActionToDo.ToString());
            // stop the action that was been done and restart till their is an interuption
            switch (myActionToDo)
                {
                    case 1:
                    case 2:
                    case 5:
                    // do nothing
                    if (lookingAtSomeone) StopLookAtSomeone();
                    break;
                    case 3:
                    // look at the neighbour
               //     if (gameObject.name == "Tom") print("Tom  Stopped Looking at Neighbour");
                    LookAtNeighbourStop();
                        break;
                    case 4:
                    // look out of window
                    // this will be automatically handled
                //    if (gameObject.name == "Tom") print("Tom Stopped Looking at Window");
                    LookAtWindowRoutineStop();
                        break;
                    default: // case 0 and above 5

                    //look around the class
                //    if (gameObject.name == "Tom") print("Tom Stopped Looking at Intersting spots");
                    LookAtInterestingSpotsRoutineStop();
                        break;

                }
            if (lookingAtSomeone) StopLookAtSomeone();
            yield return null;
            if (this.gameObject.name == "Leonie") Debug.Log(this.gameObject.name + " getting into next cycle Random Looking or writing scequence : " + myActionToDo.ToString());
        }
      
    }



    private IEnumerator InitiateTalkOrLaughScequenceStart(bool OnlyESELaughs)
    {
        IEnumerator initiSequene = RandomTalkOrLaughScequenceStart(OnlyESELaughs);
        StartCoroutine(initiSequene);
        while (RandomTalkAndLaughActive)
        {
            // do nothing
            yield return null;
        }
        if (lookingAtSomeone) StopLookAtSomeone();
        studentAnimation.ResetCurrentAnim();
        StopCoroutine(initiSequene);
        yield return null;
    }



    private IEnumerator RandomTalkOrLaughScequenceStart(bool OnlyESELaughs)
    {
        while (RandomTalkAndLaughActive)
        {
            
            int myActionToDo =0;
                // Only ESE student can laugh in between 
                if (this.ESEStudent)
                    myActionToDo = Random.Range(0, 15) % 5;
            if (myActionToDo >= 3) studentAnimation.EE4_Laughs(true);
            else
                studentAnimation.VI7_TalkToFriendsLeftAndRight(true);

//            if (this.ESEStudent) print(myActionToDo);
            yield return new WaitForSeconds(Random.Range(3f, 5f));
           
        }
    }




    private IEnumerator InitiateLookAtSomeoneShakeHeadsAndWriteScequenceStart(Transform whomToLookAt, bool peepAtWorkSheetOrWrite)
    {
        IEnumerator initiSequene = RandomLookAtSomeoneShakeHeadsAndWriteScequenceStart(whomToLookAt, peepAtWorkSheetOrWrite);
        StartCoroutine(initiSequene);
        while (RandomLookAtShakeHeadAndWriteActive)
        {
            // do nothing
            yield return null;
        }
        if (lookingAtSomeone) StopLookAtSomeone();
        studentAnimation.ResetCurrentAnim();
        StopCoroutine(initiSequene);
        yield return null;
    }


    private IEnumerator RandomLookAtSomeoneShakeHeadsAndWriteScequenceStart(Transform whomToLookAt, bool peepAtWorkSheetOrWrite)
    {
        while (RandomLookAtShakeHeadAndWriteActive)
        {
            yield return new WaitForSeconds(Random.Range(1f, 2f));
            int myActionToDo = Random.Range(5, 25) % 5;
            //   print(myActionToDo);
            switch (myActionToDo)
            {
                case 1:
                case 2:
                case 5:
                    // stop looking and write
                    if (lookingAtSomeone) StopLookAtSomeone();
                    if (peepAtWorkSheetOrWrite) studentAnimation.MB30_PeepToSideLeftOrRight(true);
                    else
                        studentAnimation.MB33_WorkOnSheets(true);
                    break;
                case 3:
                case 4:
                default: // case 0 or 5 above
                         // look at the target
                    IKControlScript.lookobjHeightOffset = 1f;
                    LookAtSomeone(whomToLookAt);
                    yield return new WaitForSeconds(Random.Range(1f, 1.5f));
                    // Shake Head
                    studentAnimation.MB39_ShakesHead(true);
                    break;

            }
            yield return new WaitForSeconds(Random.Range(3f, 4f));
            yield return null;
        }
    }



    public void StartLookingAtTabletAndWrittingAnimations()
    {
        randomLookAtOrWriteScequenceStart = true;
        //StartSittingIdleLookAroundAnything();
        StartCoroutine(InitiateLookingAtTabletAndOrWritingSequenceStart());
    }

    public void StopLookingAtTabletAndWrittingAnimations()
    {
        randomLookAtOrWriteScequenceStart = false;
    }
    private IEnumerator InitiateLookingAtTabletAndOrWritingSequenceStart()
    {
        IEnumerator initiSequene = LookingAtTabletAndWritingSequenceStart();
        StartCoroutine(initiSequene);
        while (sittingIdleLookAroundAnything && randomLookAtOrWriteScequenceStart)
        {
            // do nothing
            yield return null;
        }
        if (lookingAtSomeone) StopLookAtSomeone();
        studentAnimation.MB33_WorkOnSheets(false);
        StopCoroutine(initiSequene);
        yield return null;
    }

    private IEnumerator LookingAtTabletAndWritingSequenceStart()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            studentAnimation.MB33_WorkOnSheets(true);
            yield return new WaitForSeconds(Random.Range(1f, 2f));
            studentAnimation.MB33_WorkOnSheets(false);
            yield return new WaitForSeconds(1f);
            LookAtSomeone(chairPoint.gameObject.transform.GetComponentInChildren<StudyMaterialType>().Tablet.transform);


        }

    }



    #region Look at and Whisper actions

    public void LookAtSomeone(Transform WhomTo)
    {
        if (WhomTo != null)
        {
            Transform tempObjectToLookAt;
            currentLookAtTarget = WhomTo;
            lookingAtSomeone = true;
            if (TeacherSelectedMeUsingMouse)
                tempObjectToLookAt = Camera.main.transform;
            else
                tempObjectToLookAt = WhomTo;
            if (tempObjectToLookAt.gameObject.name != "Player" || tempObjectToLookAt.gameObject.name != "MainCamera")
                IKControlScript.lookObj = tempObjectToLookAt;
            else
            {
                Transform haloTransform = tempObjectToLookAt.Find("Halo").transform; // find the Halo potion of the kid
                IKControlScript.lookObj = haloTransform;
            }
            IKControlScript.ikActive = true;
            updateLookToLog = true;
        }
    }

    public void LookAtTransformXZ(Transform lookAt)
    {
        Transform tempLook = lookAt;
        tempLook.position = new Vector3(lookAt.position.x, 0f, lookAt.position.z);
        transform.LookAt(tempLook);
    }


    public void ForceStopWalk()
    {
        this.studentNavMesh.GetNavMeshAgent().isStopped = true;
        this.reachedSpot = true;
    }



    public void StopLookAtSomeone()
    {
        IKControlScript.ikActive = false;
        IKControlScript.lookobjHeightOffset = 0f;
        currentLookAtTarget = null;
        lookingAtSomeone = false;
    }





    public void LookAtNeighbourRoutine()
    {
        StartCoroutine(LookAtNeighbourStart());
    }
    public void LookAtNeighbourRoutineStop()
    {
        StartCoroutine(LookAtNeighbourStop());
    }


    private IEnumerator LookAtNeighbourStart()
    {
        //   sc11LookatWindowActive = true;
        //    print(name + " going to look out of window......");
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        studentAnimation.MB33_WorkOnSheets(false);
        if (myNeighbourStudent != null)
        {
            IKControlScript.lookobjHeightOffset = 1f;
            LookAtSomeone(myNeighbourStudent.transform);
        }
        yield return null;
    }

    private IEnumerator LookAtNeighbourStop()
    {
        //   sc11LookatWindowActive = false;
        //   yield return new WaitForSeconds(Random.Range(1f, 2f));
        if (lookingAtSomeone) StopLookAtSomeone();

        yield return null;
    }




    public void LookAtFloorRoutine()
    {
        StartCoroutine(LookAtFloorStart());
    }
    public void LookAtFloorRoutineStop()
    {
        StartCoroutine(LookAtFloorStop());
    }


    private IEnumerator LookAtFloorStart()
    {
        //   sc11LookatWindowActive = true;
        //    print(name + " going to look out of window......");
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        studentAnimation.MB33_WorkOnSheets(false);
        if (myNeighbourStudent != null)
        {
            IKControlScript.lookobjHeightOffset = 1f;
            if(myFloorObject !=null)
                LookAtSomeone(myFloorObject.transform);
            else
            {
                Debug.Log("Finding my current Floor point " + myCurrentCell);
                myFloorObject = GameObject.Find(myCurrentCell);
                if (myFloorObject != null)
                    LookAtSomeone(myFloorObject.transform);
                else
                    Debug.LogAssertion("Floor not found to look at it");
            }
        }
        yield return null;
    }

    private IEnumerator LookAtFloorStop()
    {
        //   sc11LookatWindowActive = false;
        //   yield return new WaitForSeconds(Random.Range(1f, 2f));
        if (lookingAtSomeone) StopLookAtSomeone();

        yield return null;
    }





    public void LookAtInterestingSpotsRoutine()
    {
        StartCoroutine(LookAtInterestingSpotsStart());
    }
    public void LookAtInterestingSpotsRoutineStop()
    {
        StartCoroutine(LookAtInterestingSpotsStop());
    }


    private IEnumerator LookAtInterestingSpotsStart()
    {
        //   sc11LookatWindowActive = true;
        //    print(name + " going to look out of window......");
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        studentAnimation.MB33_WorkOnSheets(false);

        Transform whichOneToLook = chairPoint.gameObject.GetComponent<ChairDetails>().GetAInterestingSpotToLookAt();
        if (whichOneToLook != null)
        {
            LookAtSomeone(whichOneToLook);
        }
        yield return null;

    }

    private IEnumerator LookAtInterestingSpotsStop()
    {
        //   sc11LookatWindowActive = false;
       // yield return new WaitForSeconds(Random.Range(1f, 2f));
        if (lookingAtSomeone) StopLookAtSomeone();
        yield return null;
    }


    public void LookAtBlackboardRoutine(Transform BlackBoardPoint)
    {
        studentScenarioValues.LookatBlackboardRoutineEnabler = true;
        //  studentAnimation.ResetAllAnim();
        studentScenarioValues.LookatWindowPoint = BlackBoardPoint;
        StartCoroutine(LookAtBlackboardStart());

    }
    public void LookAtBlackboardRoutineStop()
    {
        studentScenarioValues.LookatBlackboardRoutineEnabler = false;
        StartCoroutine(LookAtBlackboardStop());

    }


    private IEnumerator LookAtBlackboardStart()
    {
        //   sc11LookatWindowActive = true;
        //    print(name + " going to look out of window......");
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        studentAnimation.MB33_WorkOnSheets(false);
        LookAtSomeone(studentScenarioValues.LookatBlackboardPoint);
        yield return null;
    }

    private IEnumerator LookAtBlackboardStop()
    {
        //   sc11LookatWindowActive = false;
       // yield return new WaitForSeconds(Random.Range(1f, 2f));
        if (lookingAtSomeone) StopLookAtSomeone();
        yield return null;
    }




    private IEnumerator LookAtBlackboardStopAndContinueWorksheet()
    {
        //   sc11LookatWindowActive = false;
        StartCoroutine(LookAtBlackboardStop());
        studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(Random.Range(2f, 4f));
        if (studentScenarioValues.LookatBlackboardRoutineEnabler) StartCoroutine(LookAtBlackboardStart());
    }


    // Make sure that the LookAtWindowPoint is set already before calling this
    public void LookAtWindowRoutine()
    {
      //  studentScenarioValues.LookatWindowRoutineEnabler = true;
        //  studentAnimation.ResetAllAnim();
        StartCoroutine(LookAtWindowStart());

    }

    public void LookAtWindowRoutine(Transform TableWindowPoint)
    {
    //    studentScenarioValues.LookatWindowRoutineEnabler = true;
        //  studentAnimation.ResetAllAnim();
        studentScenarioValues.LookatWindowPoint = TableWindowPoint;
        StartCoroutine(LookAtWindowStart(TableWindowPoint));

    }
    public void LookAtWindowRoutineStop()
    {
       // studentScenarioValues.LookatWindowRoutineEnabler = false;
        StartCoroutine(LookAtWindowStop());
    }
    public void LookAtWindowRoutineStopAndWorkOnSheet()
    {
        studentScenarioValues.LookatWindowRoutineEnabler = false;
        StartCoroutine(LookAtWindowStopAndContinueWorksheet());
    }


    private IEnumerator LookAtWindowStop()
    {
        //   sc11LookatWindowActive = false;
        // yield return new WaitForSeconds(Random.Range(1f, 2f));
        if (lookingAtSomeone) StopLookAtSomeone();
        yield return null;
    }



    private IEnumerator LookAtWindowStart()
    {
        //   sc11LookatWindowActive = true;
        //    print(name + " going to look out of window......");
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        studentAnimation.MB33_WorkOnSheets(false);
        LookAtSomeone(chairPoint.gameObject.GetComponent<ChairDetails>().GetAWindowToLookAt());
        yield return null;

    }
    private IEnumerator LookAtWindowStart(Transform whichWindow)
    {
        //   sc11LookatWindowActive = true;
        //    print(name + " going to look out of window......");
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        studentAnimation.MB33_WorkOnSheets(false);
        LookAtSomeone(whichWindow);
        yield return null;
    }


        private IEnumerator LookAtWindowStopAndContinueWorksheet()
    {
        //   sc11LookatWindowActive = false;
        StartCoroutine(LookAtWindowStop());
        studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(Random.Range(2f, 4f));
        if (studentScenarioValues.LookatWindowRoutineEnabler) StartCoroutine(LookAtWindowStart());
    }

  

    public void LookAroundOrTalkOrWriteRoutine()
    {
        studentScenarioValues.LookAroundOrTalkOrWriteRoutineEnabler = true;

        StartCoroutine(LookAtAroundOrTalk());

    }
    public void LookAroundOrTalkOrWriteRoutineStop()
    {
        studentScenarioValues.LookAroundOrTalkOrWriteRoutineEnabler = false;

    }

    public void WisperOrWriteRoutine()
    {
        studentScenarioValues.WisperOrWriteRoutineEnabler = true;

        StartCoroutine(WhisperRoutineStopAndContinueWorksheet());

    }
    public void WisperOrWriteRoutineStop()
    {
        studentScenarioValues.WisperOrWriteRoutineEnabler = false;

    }

    public void LookatShakeHeadsOrWriteRoutine(Transform LookAtPersonPoint)
    {
        studentScenarioValues.LookatShakeHeadsOrWriteRoutineEnabler = true;
        studentScenarioValues.LookatWindowPoint = LookAtPersonPoint;
        StartCoroutine(LookatAndShakeHeadRoutine());

    }
    public void LookatShakeHeadsOrWriteRoutineStop()
    {
        studentScenarioValues.LookatShakeHeadsOrWriteRoutineEnabler = false;

    }

    public void StartLookBetweenTwoPeopoleRoutine(Transform person1, Transform person2, float lowerTimeGap = 0.5f, float higherTimeGap = 5f)
    {
        studentScenarioValues.LookatBetweenTwoPeopleRoutineEnabler = true;
        if(activeSequence != null)
        {
            StopCoroutine(activeSequence);
            activeSequence = null;
        }
        activeSequence = LookatBetweenTwoPeopleRoutine(person1, person2, lowerTimeGap, higherTimeGap);
        StartCoroutine(activeSequence);

    }
    public void StopLookBetweenTwoPeopoleRoutine()
    {
        studentScenarioValues.LookatBetweenTwoPeopleRoutineEnabler  = false;
        if(activeSequence !=null)StopCoroutine(activeSequence);
        activeSequence = null;
    }


    private IEnumerator LookAtAroundOrTalk()
    {
        //   sc11LookatWindowActive = true;
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        studentAnimation.MB33_WorkOnSheets(false);
        int LookAroundOrTalk = Random.Range(0, 2);
        if (LookAroundOrTalk == 0) studentAnimation.MB9_LookAround(true);
        else studentAnimation.VI7_TalkToFriendsLeftAndRight();
        yield return new WaitForSeconds(Random.Range(2.5f, 4f));
        StartCoroutine(LookAtAroundOrTalkStopAndContinueWorksheet());

    }
    private IEnumerator LookAtAroundOrTalkStopAndContinueWorksheet()
    {
        //   sc11LookatWindowActive = false;
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        if (studentAnimation.GetAnimStateBool("vi7_Talk_ON")) studentAnimation.VI7_TalkToFriendsStop();
        if (studentAnimation.GetAnimStateBool("mb9_LookAround_ON")) studentAnimation.MB9_LookAround(false);
        studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(Random.Range(2.5f, 4f));
        if (studentScenarioValues.LookAroundOrTalkOrWriteRoutineEnabler) StartCoroutine(LookAtAroundOrTalk());
    }


    private IEnumerator WhisperRoutine()
    {
        //   sc11LookatWindowActive = true;
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        studentAnimation.MB33_WorkOnSheets(false);
        studentAnimation.VI11_TalkToFriendsLeftAndRight();
        yield return new WaitForSeconds(Random.Range(2.5f, 4f));
        StartCoroutine(WhisperRoutineStopAndContinueWorksheet());

    }
    private IEnumerator WhisperRoutineStopAndContinueWorksheet()
    {
        //   sc11LookatWindowActive = false;
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        if (studentAnimation.GetAnimStateBool("vi11_Wispher_ON")) studentAnimation.VI11_TalkToFriendsStop();
        studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(Random.Range(2.5f, 4f));
        if (studentScenarioValues.WisperOrWriteRoutineEnabler) StartCoroutine(WhisperRoutine());
    }

    private IEnumerator LookatAndShakeHeadRoutine()
    {
        //   sc11LookatWindowActive = true;
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        studentAnimation.MB33_WorkOnSheets(false);
        LookAtSomeone(studentScenarioValues.LookatWindowPoint);
        yield return new WaitForSeconds(Random.Range(2f, 4f));
        studentAnimation.MB9_LookAroundFast(true);
        yield return new WaitForSeconds(Random.Range(2f, 4f));
        StartCoroutine(LookatAndShakeHeadAndContinueWorksheet());

    }
    private IEnumerator LookatAndShakeHeadAndContinueWorksheet()
    {
        //   sc11LookatWindowActive = false;
        if (studentAnimation.GetAnimStateBool("mb9_LookAround_ON")) studentAnimation.MB9_LookAroundFast(false);
        studentAnimation.MB33_WorkOnSheets(true);
        yield return new WaitForSeconds(Random.Range(4f, 6f));
        if (studentScenarioValues.WisperOrWriteRoutineEnabler) StartCoroutine(LookatAndShakeHeadRoutine());
    }



    private IEnumerator LookatBetweenTwoPeopleRoutine(Transform person1, Transform person2,float lowerTimeGap = 0.5f, float higherTimeGap = 5f)
    {
        //   sc11LookatWindowActive = true;
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        while (studentScenarioValues.LookatBetweenTwoPeopleRoutineEnabler)
        {
            if(person1 !=null)LookAtSomeone(person1); else { Debug.LogWarning("Cant look at person 1 as it is empty, exiting the routine"); studentScenarioValues.LookatBetweenTwoPeopleRoutineEnabler = false; }
            yield return new WaitForSeconds(Random.Range(lowerTimeGap, higherTimeGap));
            if (person2 != null) LookAtSomeone(person2); else { Debug.LogWarning("Cant look at person 2 as it is empty, exiting the routine"); studentScenarioValues.LookatBetweenTwoPeopleRoutineEnabler = false; }
            yield return new WaitForSeconds(Random.Range(lowerTimeGap, higherTimeGap));
        }
    }
   

    #endregion




    private IEnumerator InitiateGoToChair()
    {
        //yield return new WaitForSeconds(Random.Range(0.0f, 1f));
        GoToChair();
        yield return new WaitForEndOfFrame();
    }



    void GoToChair()
    {
        // this.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        Debug.Log("GO TO CHAIR");
        studentAnimation.WalkingToStandUpIdle(false);
        studentAnimation.Sitting(false);
        // studentAnimation.Walk_Sit_Pickup_Aniamtion(0.0f, 0.0f);
//        print("Chair point is" + chairPoint.name);
        transform.LookAt(chairPoint);
       
        studentNavMesh.SetNavMeshAgentDestination(chairPoint.position, OnGoToChairComplete);
    }

    void OnGoToChairComplete()
    {
        StartCoroutine(SitOnChair());

    }

    private IEnumerator SitOnChair()
    {

        float sitValue = 0;
        //  if(this.gameObject.GetComponent<CapsuleCollider>().enabled)this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        /*
          while (sitValue <= 1.1f)
          {
              yield return new WaitForSeconds(0.01f);

              studentAnimation.Walk_Sit_Pickup_Aniamtion(sitValue, 0.0f);

              transform.rotation = chairPoint.transform.rotation;
              transform.position = chairPoint.transform.position + new Vector3(0f, -0.35f, 0f);      

              sitValue += (Time.deltaTime * 2);

          }
          */
        // sitting code
        yield return new WaitForSeconds(0.01f);

        studentAnimation.Sitting(true);

        transform.rotation = chairPoint.transform.rotation;
        transform.position = chairPoint.transform.position + new Vector3(0f, -0.35f, 0f);

        yield return null;

    }


   public void InitiateGoToSpotAndPlaceItem(Transform spot, GameObject ItemToPlaceOnGroundAndShow)
    {
        StartCoroutine(WalkToASpotAndPlaceItem(spot.position, ItemToPlaceOnGroundAndShow));
    }

    IEnumerator WalkToASpotAndPlaceItem(Vector3 spot,GameObject itemToShowOnGroundAfterPlaced)
    {
      
        yield return null;
        StartCoroutine(WalkToASpot(spot));
        yield return new WaitUntil(() => reachedSpot);
       // yield return new WaitForSeconds(1.0f);
        PlaceItemOnGround(); // just animate the placing of the book, not actual placing which will be done below
        yield return new WaitForSeconds(2.0f);
        
        if (itemToShowOnGroundAfterPlaced != null) itemToShowOnGroundAfterPlaced.SetActive(true);// show the item on ground
        if (inHandSelectedStudyMaterial != null) inHandSelectedStudyMaterial.SetActive(false); // hide the item in hand
        StartCoroutine(GoToSeatAfterReachingSpot());
        //studentAnimation.Walk_Sit_Pickup_Aniamtion(0.0f, 0.0f);
    }


    public void InitiateGoToSpot(Transform spot, float newSpeed=0f)
    {
        StartCoroutine(WalkToASpotAndStandIdle(spot.position,newSpeed));
    }
    IEnumerator WalkToASpotAndStandIdle(Vector3 spot, float newSpeed = 0f)
    {

        yield return new WaitForEndOfFrame();
        StartCoroutine(WalkToASpot(spot, newSpeed));
    //    Debug.Log("Reached spot at start is " + reachedSpot.ToString());
        yield return new WaitUntil(() => reachedSpot);
        studentAnimation.WalkingToStandUpIdle(true);
        yield return new WaitForSeconds(2.0f);
        //studentAnimation.Walk_Sit_Pickup_Aniamtion(0.0f, 0.0f);
    }

    IEnumerator WalkToASpot(Vector3 spot, float newspeed = 0)
    {
        reachedSpot = false;
        yield return new WaitForEndOfFrame();
        studentAnimation.Sitting(false);
        studentAnimation.WalkingToStandUpIdle(false);
        reachedSpot = false;
        Debug.Log("Reached spot inside WalkToSpot is " + reachedSpot.ToString());
        yield return null;
        transform.LookAt(spot);
        if (newspeed != 0) studentNavMesh.SetNavigationSpeed(newspeed);
//        Debug.Log("Reached spot at walking is " + reachedSpot.ToString());
        studentNavMesh.SetNavMeshAgentDestination(spot, OnWalkToSpotComplete);
//        Debug.Log("Reached spot at WalkToASpot complted " + reachedSpot.ToString());
        yield return new WaitForSeconds(1.0f);
        //studentAnimation.Walk_Sit_Pickup_Aniamtion(0.0f, 0.0f);
    }

    private void OnWalkToSpotComplete()
    {
        reachedSpot = true;
        Debug.Log("Reached spot at finish is " + reachedSpot.ToString());
        studentNavMesh.ResetNavigationSpeed();
        // transform.LookAt(teacherNotesPickupPos);
        //    transform.LookAt(PlaceToGoTo);
        // do nothing as what to do next will be handled by the initiating function
    }
    private void PlaceItemOnGround()
    {
        studentAnimation.PlaceItemOnGround(true);

    }

    IEnumerator GoToSeatAfterReachingSpot()
    {
        yield return new WaitForSeconds(1.0f);
        //  inHandSelectedStudyMaterial.SetActive(false);
        //  yield return new WaitForSeconds(studentAnimation.takingItemAnimationtime);
        GoToAndSitInChair(); // to the currently assigned Chair point
    }








    public void LogMyAnimation(string animName)
    {
        // Student look changed, log it
        StudentActionsAnimationsData tempObj = new StudentActionsAnimationsData
        {
            StudentName = gameObject.name,
            AnimationName = animName,
            time = gamePlayManager.GetCurrentTime()
//            CurrentCell = myCurrentCell
        };
        if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetStudentActionsAnimationsData(tempObj);
    }



    void LateUpdate()
    {


      



        if (updateLookToLog)
        {

            /*
            print("Student Name = " + this.gameObject.name);
            print("target = " + currentLookAtTarget.gameObject.name);
            print("CurrentCell = " + myCurrentCell);
            print("time = " + gamePlayManager.GetCurrentTime());
            print("CurrentActualPos = " + transform.position.ToString());
            */
            Vector3 currentPos = new Vector3(Mathf.Round(this.transform.position.x), Mathf.Round(this.transform.position.y), Mathf.Round(this.transform.position.z));


            // Student look changed, log it
            StudentActionsLookingAtData tempObj = new StudentActionsLookingAtData
            {
                StudentName = this.gameObject.name,
                target = (currentLookAtTarget != null) ? currentLookAtTarget.gameObject.name : "",
            //    CurrentCell = myCurrentCell,
                time = gamePlayManager.GetCurrentTime(),
                CurrentActualPos = currentPos

            };
            if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetStudentActionsLookingAtData(tempObj);
            updateLookToLog = false;
        }

    }

    public void SetPlayerAtCell(string whichCell)
    {
        if (whichCell != null & whichCell != "")
        {
            this.myPreviousCell = this.myCurrentCell;
            this.myCurrentCell = whichCell;
        }
    }
    










    private IEnumerator InitialGoToDesk()
    {
        yield return new WaitForSeconds(Random.Range(0.0f, 1f));
        GoToDesk();
    }



    void GoToDesk()
    {
        // this.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        studentAnimation.TakeItem(false);
        TeacherDeskPoint.gameObject.GetComponent<NavPointStatus>().occupied = false;
        studentAnimation.Sitting(false);
        // studentAnimation.Walk_Sit_Pickup_Aniamtion(0.0f, 0.0f);
        transform.LookAt(chairPoint);
        studentNavMesh.SetNavMeshAgentDestination(chairPoint.position, OnGoToDeskComplete);
    }

    void OnGoToDeskComplete()
    {
        if (inHandSelectedStudyMaterial != null)
        {
            inHandSelectedStudyMaterial.SetActive(false);
        }

        if (onHandSelectedStudyMaterial != null)
        {
            onHandSelectedStudyMaterial.SetActive(true);
            tookAssignmentAndBackToSit = true;
        }

        StartCoroutine(SitOnDesk());

    }

    private IEnumerator SitOnDesk()
    {

        float sitValue = 0;
        //  if(this.gameObject.GetComponent<CapsuleCollider>().enabled)this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        /*
          while (sitValue <= 1.1f)
          {
              yield return new WaitForSeconds(0.01f);

              studentAnimation.Walk_Sit_Pickup_Aniamtion(sitValue, 0.0f);

              transform.rotation = chairPoint.transform.rotation;
              transform.position = chairPoint.transform.position + new Vector3(0f, -0.35f, 0f);      

              sitValue += (Time.deltaTime * 2);

          }
          */
        // sitting code
        yield return new WaitForSeconds(0.01f);

        studentAnimation.Sitting(true);

        transform.rotation = chairPoint.transform.rotation;
        transform.position = chairPoint.transform.position + new Vector3(0f, -0.35f, 0f);

        // if it is start of the scenario
        // this should not be called more than once per start of the scenario  , so make sure to call the StopMyInitialAnimations function before going to any other phase of the scenario
        if (scenarioStart)
        {

            sitPos = transform.position;

            // if the starting scenario has same start sequences
            if( gamePlayManager.initialActionForScenarioIsCommon)
            {
                sittingIdleLookAroundAnything = true;
                StartCoroutine(InitiateInitialScequenceStart());

            }
           
        }
        else
        {
            // if the scenario is already in progress, as sitOnDesk can be called from inside scenario also

            if (!studentScenarioValues.scenario1SR3ReadAndThenLookAround) // if the character is not sitting after SR3 of Scenario1
            {
                yield return new WaitForSeconds(Random.Range(1.0f, 2.5f));

                sitPos = transform.position;

                //   studentAnimation.Talk(true);
                //   studentAnimation.TalkRightLeft(1.0f);

                if (!resetScenario)
                {
                    int ReadOrTalk = Random.Range(0, 3);
                    if (ReadOrTalk == 1) studentAnimation.VI7_TalkToFriendsLeftAndRight();
                    if (ReadOrTalk == 0) studentAnimation.Sitting(true);
                }
                if (studentScenarioValues.scenario11Start && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("T11"))
                {
                    if (!tookAssignmentAndBackToSit)
                    {
                        int ReadOrTalk = Random.Range(0, 3);
                        if (ReadOrTalk == 0 || (ReadOrTalk == 2)) studentAnimation.MB33_WorkOnSheets(true);
                        yield return new WaitForSeconds(2.0f);
                    }

                }

                if (!studentScenarioValues.scenario11Start && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("T11"))
                {

                    yield return new WaitForSeconds(20.0f);
                    //studentAnimation.Talk(false);
                    studentAnimation.ResetAllAnim();
                }

                // Debug.Log("Talk Finished");
            }
            else
            {
                yield return new WaitForSeconds(2.0f);
                sitPos = transform.position;
                studentAnimation.Read(true);
                yield return new WaitForSeconds(Random.Range(4.0f, 6.0f));
                studentAnimation.Read(false);
                //studentAnimation.LookAround(true);
                studentAnimation.MB9_LookAround(true);
                //  studentAnimation.LiftUp
                // Student has to look around

            }
        }

    }

    public void CarryAStudyMaterial(StudyMaterial studyMaterial)
    {


        Transform[] parent = this.GetComponentsInChildren<Transform>();

        foreach (Transform child in parent)
        {
            // if (child.name == "StudyMaterial")
            if (child.name.Contains("StudyMaterial"))
            {

                inHandSelectedStudyMaterial = child.GetComponent<StudyMaterialType>().GetStudyMaterial(studyMaterial);
                inHandSelectedStudyMaterial.SetActive(true);

            }
        }

    }
    public void UnCarryAStudyMaterial(StudyMaterial studyMaterial)
    {

        if (inHandSelectedStudyMaterial != null)
        {
            inHandSelectedStudyMaterial.SetActive(false);
        }
    }

    public void SetSelectedStudyMaterial(StudyMaterial studyMaterial)
    {

        Transform[] parent = chairPoint.GetComponentsInChildren<Transform>();
        foreach (Transform child in parent)
        {


            // if (child.name == "StudyMaterial")
            if (child.name.Contains("StudyMaterial"))
            {

                onHandSelectedStudyMaterial = child.GetComponent<StudyMaterialType>().GetStudyMaterial(studyMaterial);

            }
        }

        parent = this.GetComponentsInChildren<Transform>();

        foreach (Transform child in parent)
        {
            // if (child.name == "StudyMaterial")
            if (child.name.Contains("StudyMaterial"))
            {

                inHandSelectedStudyMaterial = child.GetComponent<StudyMaterialType>().GetStudyMaterial(studyMaterial);

            }
        }

    }

    private bool isF9 = false;
    private bool isF8 = false;

    public void Update()
    {

        if (!isF9 && Input.GetKeyDown(KeyCode.F9))
        {
            isF9 = true;
            PlayNeil(true);
        }
        if (!isF8 && Input.GetKeyDown(KeyCode.F8))
        {
            isF8 = true;
            PlayNeil(false);
        }

        //   print(name + " playing the animation"+ studentAnimation.GetAnimStateName(0));

            if (isTurnAround)
            {
               // turnToPoint.position = new Vector3(turnToPoint.position.x, 0, turnToPoint.position.z);
                // Determine which direction to rotate towards
                Vector3 targetDirection = turnToPoint.position - transform.position;

                // The step size is equal to speed times frame time.
                float singleStep = (turnSpeed + additionalTurnSpeed) * Time.deltaTime;

                // Rotate the forward vector towards the target direction by one step
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

                // Draw a ray pointing at our target in
                Debug.DrawRay(transform.position, newDirection, Color.red);

         

            // Calculate a rotation a step closer to the target and applies rotation to this object
            transform.rotation = Quaternion.LookRotation(newDirection);

                //  transform.LookAt(LookAtPoint,Vector3.up);

                if (currentTurnToTarget != turnToPoint.gameObject)
                {
                    currentTurnToTarget = turnToPoint.gameObject;

                    m_CharacterTargetRot = this.transform.localRotation;
                    // player moved to a new cell, log it
                }
                if (transform.rotation == turnToPoint.rotation) { additionalTurnSpeed = 0f;  isTurnAround = false; }
                //   transform.rotation = Quaternion.RotateTowards(transform.rotation, LookAtPoint.rotation, rotSpeed * Time.deltaTime);
            }
            else
            {
                // player is not made to look at some place, so check if the player has turned atonamusly
                if (this.transform.localRotation != m_CharacterTargetRot)
                {
                    // player has turned around in the class

                    // store the current turnned angle, and try to find if there is any new target on the sight
                    // if changes are there then record them in the Log


                    Ray ray = new Ray(this.transform.position, this.transform.forward);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit) && (hit.distance < rangeTocheckForTarget))
                    {
                        // We did hit something, is it the same object than the last frame?, this means there is a significance turn, so log it
                        if (currentLookAtTarget != hit.transform.gameObject)
                        {
                            currentTurnToTarget = hit.transform.gameObject;
                            m_CharacterTargetRot = this.transform.localRotation;
                        }
                    }


                }
            }


    }



    public void TurnToPlayerOrObject(bool isLookAt, Transform whereTo = null, float turningSpeed = 0f)
    {
        
        this.isTurnAround = isLookAt;
        // originalCameraRotation = transform.rotation;
        if (whereTo != null)
        {
            turnToPoint = whereTo;
           // transform.LookAt(whereTo);
            additionalTurnSpeed = turningSpeed;
            // print(whereTo.rotation.ToString());
            //    StartCoroutine(SmoothLookAround(LookAtPoint));
        }
    }





    public void PlayNeil(bool isNeil)
    {
        studentAnimation.Talk(false);
        studentAnimation.PlayNeil(isNeil);
        isF8 = isF9 = false;
    }

    public void RaiseHand()
    {
        studentAnimation.RaiseHand(true);
    }

    public void RaiseHand(float lowerDelay)
    {
        studentAnimation.RaiseHand(true);
        StartCoroutine(LowerHand(lowerDelay));
    }

    public void SetStudentBag(bool val)
    {
        myHiddenBag.SetActive(val);
    }

    private IEnumerator LowerHand(float delay)
    {
        yield return new WaitForSeconds(delay);
        studentAnimation.RaiseHand(false);
    }
    //Scenario One Student Reaction

    #region Scenario One Student Reaction

    public void ScenarioOneStudentReaction(string response, StudentAction neighbourStudent)
    {

        // studentAnimation.ResetAllAnim();
        // studentAnimation.RaiseHand(false);
        studentScenarioValues.scenario1SR3ReadAndThenLookAround = false;

        if (onHandSelectedStudyMaterial != null)
        {
            onHandSelectedStudyMaterial.SetActive(false);
        }
        if (inHandSelectedStudyMaterial != null)
        {
            inHandSelectedStudyMaterial.SetActive(false);
        }
        tookAssignmentAndBackToSit = false;
        studentAnimation.ResetAllAnimAndWalk();
        switch (response)
        {
            case "1":
                //studentAnimation.Talk(false);
                //studentAnimation.LookAround(false);
                StartCoroutine(WalkToTeacherDeskPickupText());

                break;
            case "2":
                //studentAnimation.Talk(false);
                //studentAnimation.LookAround(false);
                StartCoroutine(WalkToTeacherDeskPickupText());

                break;
            case "3":
                //studentAnimation.Talk(false);
                //studentAnimation.LookAround(false);
                if (neighbourStudent != null) myNeighbourStudent = neighbourStudent;
                StartCoroutine(WalkToTeacherDeskAndTalkText());

                break;
            case "4":
                //studentAnimation.Talk(false);
                //studentAnimation.LookAround(false);
                if (neighbourStudent != null) myNeighbourStudent = neighbourStudent;
                StartCoroutine(WalkToTeacherDeskAndRaiseHandText());

                break;
        }
    }

    //Response 1 , 2
    IEnumerator WalkToTeacherDeskPickupText()
    {
        yield return null;
        studentAnimation.Sitting(false);
        yield return null;
        studentNavMesh.SetNavMeshAgentDestination(teacherNotesPickupPos, OnWalkToTeacherDeskPickupTextComplete);
        transform.LookAt(teacherNotesPickupPos);
        yield return new WaitForSeconds(1.0f);
        //studentAnimation.Walk_Sit_Pickup_Aniamtion(0.0f, 0.0f);
    }

    private void OnWalkToTeacherDeskPickupTextComplete()
    {
        TeacherDeskPoint.gameObject.GetComponent<NavPointStatus>().occupied = true;
        // transform.LookAt(teacherNotesPickupPos);
        transform.LookAt(TeacherMainPoint);
        studentAnimation.TakeItem(true);
        StartCoroutine(GoToDeskAfterTakingTheItem());

    }


    IEnumerator GoToDeskAfterTakingTheItem()
    {
        yield return new WaitForSeconds(1.0f);
        inHandSelectedStudyMaterial.SetActive(true);
        //  yield return new WaitForSeconds(studentAnimation.takingItemAnimationtime);
        yield return new WaitForSeconds(2.0f);
        GoToDesk();
    }





    //Response 3
    IEnumerator WalkToTeacherDeskAndTalkText()
    {
        yield return null;
        studentAnimation.Sitting(false);
        yield return null;
        studentNavMesh.SetNavMeshAgentDestination(TeacherDeskPoint.position, OnWalkToTeacherDeskAndTalkComplete);
        // transform.LookAt(TeacherDeskPoint);
        transform.LookAt(TeacherMainPoint);
        yield return new WaitForSeconds(1.0f);
        //studentAnimation.Walk_Sit_Pickup_Aniamtion(0.0f, 0.0f);
    }

    private void OnWalkToTeacherDeskAndTalkComplete()
    {
        if (myNeighbourStudent != null)
        {
            IKControlScript.lookobjHeightOffset = 1.2f;
            LookAtSomeone(myNeighbourStudent.transform);
        }

        StartCoroutine(PickUpAfterTakingToNeighbour());

        //studentAnimation.Walk_Sit_Pickup_Aniamtion(0.0f, 1.0f, OnTalkComplete);
    }

    IEnumerator PickUpAfterTakingToNeighbour()
    {
        TeacherDeskPoint.gameObject.GetComponent<NavPointStatus>().occupied = true;
        yield return new WaitForSeconds(2.0f);
        StopLookAtSomeone();

        transform.LookAt(TeacherMainPoint);
        studentAnimation.TakeItem(true);
        studentScenarioValues.scenario1SR3ReadAndThenLookAround = true;
        StartCoroutine(GoToDeskAfterTakingTheItem());
    }

    /*private void OnTalkComplete()
    {
        studentAnimation.TakeItem(true, GoToDesk);
    }*/

    /*  IEnumerator GoToDeskAfterTakingTheItem()
      {
          yield return new WaitForSeconds(1.0f);
          inHandSelectedStudyMaterial.SetActive(true);
          yield return new WaitForSeconds(1.0f);
          GoToDesk();
      }
      */





    //Response 4
    IEnumerator WalkToTeacherDeskAndRaiseHandText()
    {
        yield return null;
        studentAnimation.Sitting(false);
        yield return null;
        studentNavMesh.SetNavMeshAgentDestination(TeacherDeskPoint.position, OnWalkToTeacherDeskAndRaiseHandComplete);
        transform.LookAt(TeacherDeskPoint);
        yield return new WaitForSeconds(1.0f);
        //studentAnimation.Walk_Sit_Pickup_Aniamtion(0.0f, 0.0f);
    }

    private void OnWalkToTeacherDeskAndRaiseHandComplete()
    {
        TeacherDeskPoint.gameObject.GetComponent<NavPointStatus>().occupied = true;
        transform.LookAt(TeacherMainPoint);
        studentAnimation.TakeItem(true);
        StartCoroutine(GoToDeskAndRaiseHand());

    }


    IEnumerator GoToDeskAndRaiseHand()
    {
        yield return new WaitForSeconds(1.0f);
        inHandSelectedStudyMaterial.SetActive(true);

        // yield return new WaitForSeconds(1.0f);

        studentAnimation.TakeItem(false);
        studentAnimation.Sitting(false);
        //studentAnimation.Walk_Sit_Pickup_Aniamtion(0.0f, 0.0f);
        transform.LookAt(chairPoint);
        studentNavMesh.SetNavMeshAgentDestination(chairPoint.position, OnGoToDeskAndRaiseHandComplete);
    }

    void OnGoToDeskAndRaiseHandComplete()
    {
        if (inHandSelectedStudyMaterial != null)
        {
            inHandSelectedStudyMaterial.SetActive(false);
        }

        if (onHandSelectedStudyMaterial != null)
        {

            onHandSelectedStudyMaterial.SetActive(true);
        }
        tookAssignmentAndBackToSit = true;

        StartCoroutine(SitOnDeskAndRaiseHand());
    }

    private IEnumerator SitOnDeskAndRaiseHand()
    {

        float sitValue = 0;
        /*
        while (sitValue <= 1.1f)
        {
            yield return new WaitForSeconds(0.01f);

            studentAnimation.Walk_Sit_Pickup_Aniamtion(sitValue, 0.0f);

            transform.rotation = chairPoint.transform.rotation;
            transform.position = chairPoint.transform.position + new Vector3(0f, -0.35f, 0f);

            sitValue += (Time.deltaTime * 2);

        }
        */
        studentAnimation.Sitting(true);
        transform.rotation = chairPoint.transform.rotation;
        transform.position = chairPoint.transform.position + new Vector3(0f, -0.35f, 0f);


        yield return new WaitForSeconds(1.0f);
        studentAnimation.Read(true);

        yield return new WaitForSeconds(Random.Range(4.0f, 8.0f));
        studentAnimation.Read(false);


        yield return new WaitForSeconds(1.0f);

        studentAnimation.RaiseHand(true);

        //   yield return new WaitForSeconds(1.0f);
        //  studentAnimation.RaiseHand(false);


        //  yield return new WaitForSeconds(1.0f);
        //  studentAnimation.RaiseHand(true);

    }

    #endregion


    #region Scenario Two Student Reaction






    #endregion


    #region Scenario Eleven Student Reaction
    public IEnumerator TakeBookFromTable(Transform bookPile)
    {

        // this current implemetnation is changed now, now all the kids will work on sheets after they receive it. they dont look at someone or whisper as before.
        /*
        yield return null;
        tookAssignmentAndBackToSit = false;
       
        bool v11Wispher = false;
        bool workOnSheets = false;
        if (studentAnimation.GetAnimStateBool("mb33_WriteOnSheet_ON")) { workOnSheets = true; studentAnimation.MB33_WorkOnSheets(false); }
        
        if (studentAnimation.GetAnimStateBool("vi11_Wispher_ON")) { v11Wispher = true; studentAnimation.VI11_TalkToFriendsStop(); }
        transform.LookAt(bookPile);
        yield return new WaitForSeconds(Random.Range(0.5f, 1.4f));
        studentAnimation.TakeItem(true);
        yield return new WaitForSeconds(3f);
        SetSelectedStudyMaterial(StudyMaterial.WorkSheet);
        inHandSelectedStudyMaterial.SetActive(true);
        yield return new WaitForSeconds(2f);
        studentAnimation.TakeItem(false);
        tookAssignmentAndBackToSit = false;
        if (workOnSheets && !studentAnimation.GetAnimStateBool("mb33_WriteOnSheet_ON")) studentAnimation.MB33_WorkOnSheets(true);
        if (v11Wispher && !studentAnimation.GetAnimStateBool("vi11_Wispher_ON")) studentAnimation.VI7_TalkToFriendsLeftAndRight();
        inHandSelectedStudyMaterial.SetActive(false);
        onHandSelectedStudyMaterial.SetActive(true);
        //yield return new WaitForSeconds(2f);
        //ResetAndWorkOnSheets();
        */

        yield return null;
        tookAssignmentAndBackToSit = false;
      
        if (studentAnimation.GetAnimStateBool("mb33_WriteOnSheet_ON")) {studentAnimation.MB33_WorkOnSheets(false); }

        if (studentAnimation.GetAnimStateBool("vi11_Wispher_ON")) { studentAnimation.VI11_TalkToFriendsStop(); }
        transform.LookAt(bookPile);
        yield return new WaitForSeconds(Random.Range(0.5f, 1.4f));
        studentAnimation.TakeItem(true);
        yield return new WaitForSeconds(3f);
        SetSelectedStudyMaterial(StudyMaterial.WorkSheet);
        inHandSelectedStudyMaterial.SetActive(true);
        yield return new WaitForSeconds(2f);
        studentAnimation.TakeItem(false);
        tookAssignmentAndBackToSit = false;
        studentAnimation.MB33_WorkOnSheets(true);
        inHandSelectedStudyMaterial.SetActive(false);
        onHandSelectedStudyMaterial.SetActive(true);
        yield return null;
        //yield return new WaitForSeconds(2f);


    }
    public void ResetAndWorkOnSheets()
    {
        StartCoroutine(ResetAndStartWorkOnSheets());
    }

    public IEnumerator ResetAndStartWorkOnSheets()
    {
        studentAnimation.ResetAllAnim();
        yield return null;
        studentAnimation.MB33_WorkOnSheets(true);
        yield return null;
    }


        #endregion


    }
