using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{



    public Transform TeacherDeskPoint,LookAtPoint, OriginalPosition;


    [Space(10)]
    [Header("Game Managers")]
    [SerializeField]
    private GamePlayManager gamePlayManager;

    public Camera mainCamera;
    public bool isMove,isLookAround, turnPlayertoOriginalRotation;
    public float speed,additionalSpeed =0f;
    public float rotSpeed = 10.0f;
    public Quaternion originalCameraRotation;
    public string myPreviousCell ="" ,myCurrentCell ="";

    private Quaternion m_CharacterTargetRot;

    public GameObject currentLookAtTarget;
    
    private const float ANIMATION_DURATION_IN_SECONDS = 5f;

    private bool updateMoveToLog, updateLookToLog;

    public float rangeTocheckForTarget = 10, timeCount;


    public Animator myHandsAnimator;

    public bool isGivingWorkSheet = false;
    public float currentAnimationTime = 1f;

    public bool IsPlayerInOriginalPotion { get; set; } = true;

    public GameObject teacherHands,WorksheetHoldingHandAttachementPoint, WorksheetGivingHandAttachementPoint, myFloorObject;

    Vector3 intiPos;

    private bool isStartedMoveToTimeout = false;

    void Start()
    {
        if(OriginalPosition == null) OriginalPosition = this.transform;
        intiPos = transform.position;
        originalCameraRotation = transform.rotation;
        m_CharacterTargetRot = this.transform.localRotation;
        if(myHandsAnimator !=null) myHandsAnimator = GetComponentInChildren<Animator>();
        if(myHandsAnimator !=null) myHandsAnimator.SetBool("WorkSheetHandIdle",true);
        if (teacherHands != null) teacherHands.SetActive(false);
        if (WorksheetHoldingHandAttachementPoint != null) WorksheetHoldingHandAttachementPoint.SetActive(false);
        if (WorksheetGivingHandAttachementPoint != null) WorksheetGivingHandAttachementPoint.SetActive(false);

    }

    
    void Update()
    {
        if (isMove) {
           if( !isStartedMoveToTimeout )
            {
                isStartedMoveToTimeout = true;
                StartCoroutine(MoveToTimeout());
            }
            
            transform.position = Vector3.Lerp(transform.position, TeacherDeskPoint.position, Time.deltaTime * (speed+ additionalSpeed));
          //  if(transform.rotation != TeacherDeskPoint.rotation) transform.rotation = TeacherDeskPoint.rotation;

            //Debug.Log(Vector3.Distance(transform.position, TeacherDeskPoint.position));

            if (Vector3.Distance(transform.position, TeacherDeskPoint.position) <= 0.3f) {
                // originalCameraRotation = transform.rotation;
                if (Vector3.Distance(transform.position, OriginalPosition.position) <= 0.3f) IsPlayerInOriginalPotion = true; else IsPlayerInOriginalPotion = false;
                isMove = false;
                // if there is any additional speed used, then reset it to 0
                additionalSpeed = 0f;
                if (turnPlayertoOriginalRotation)
                {
                    transform.rotation = OriginalPosition.rotation;
                    turnPlayertoOriginalRotation = false;
                }
            }
        }
        if (myCurrentCell != myPreviousCell)
        {
//            print("currentCEll = " + myCurrentCell + " Previous Cell = " + myPreviousCell);
            updateMoveToLog = true;
        }
        if (isLookAround)
        {
            // Determine which direction to rotate towards
            Vector3 targetDirection = LookAtPoint.position - transform.position;

            // The step size is equal to speed times frame time.
            float singleStep = 0.5f * Time.deltaTime;

            // Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            // Draw a ray pointing at our target in
            Debug.DrawRay(transform.position, newDirection, Color.red);

            // Calculate a rotation a step closer to the target and applies rotation to this object
            transform.rotation = Quaternion.LookRotation(newDirection);

          //  transform.LookAt(LookAtPoint,Vector3.up);
            
            if (currentLookAtTarget != LookAtPoint.gameObject)
            {
                currentLookAtTarget = LookAtPoint.gameObject;
              
                m_CharacterTargetRot = this.transform.localRotation;
                // player moved to a new cell, log it
                updateLookToLog = true;
            }
            if (transform.rotation == LookAtPoint.rotation) { isLookAround = false; }
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
                        currentLookAtTarget = hit.transform.gameObject;
                        m_CharacterTargetRot = this.transform.localRotation;
                        updateLookToLog = true;
                    }
                }

               
            }
        }
    }

    private IEnumerator MoveToTimeout()
    {
        float timeout = 3f;

        // while (Vector3.Distance(transform.position, TeacherDeskPoint.position) > 0.3f)
        while(isMove && timeout > 0)
        {
            yield return new WaitForSeconds(1f);
            timeout--;
        }
        if (isMove)
        {
            Debug.LogFormat("(PlayerMovement) | MoveToTimeout | Forcing Player to postion: {0}", TeacherDeskPoint.position);
            transform.position = TeacherDeskPoint.position;
        }
        
        isStartedMoveToTimeout = false;
    }

    void LateUpdate()
    {
        if(updateMoveToLog)
        {
            Vector3 currentPos = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));

            // player moved to a new cell, log it
            PlayerActionsMovedToData tempObj = new PlayerActionsMovedToData
            {
                FromCell = myPreviousCell,
                CurrentCell = myCurrentCell,
                time = gamePlayManager.GetCurrentTime(),
                CurrentActualPos = currentPos
            };
            if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetPlayerActionsMovedToData(tempObj);
            updateMoveToLog = false;

            myPreviousCell = myCurrentCell; // set previous cell to current cell so we dont need to update the log with duplicates
        }
        if (updateLookToLog)
        {

            Vector3 localLookAtAngle = new Vector3(Mathf.Round(mainCamera.transform.eulerAngles.x), Mathf.Round(m_CharacterTargetRot.eulerAngles.y), Mathf.Round(m_CharacterTargetRot.eulerAngles.z));
            Vector3 currentPos = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));

            // player moved to a new cell, log it
            PlayerActionsLookingAtData tempObj = new PlayerActionsLookingAtData
            {
                target = (currentLookAtTarget != null) ? currentLookAtTarget.name : "",
                CurrentCell = myCurrentCell,
                time = gamePlayManager.GetCurrentTime(),
                CurrentActualPos = currentPos,
                CurrentActualLookingDirection = localLookAtAngle
            };
            if (gamePlayManager.LOG_ENABLED) LogDB.instance.SetPlayerActionsLookingAtData(tempObj);
            updateLookToLog = false;
        }
    }

    public void ElevateCamera(float addedHeight) {
        //This will elevate the main camera to provide a higher vantage point when viewing the classroom
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + addedHeight, mainCamera.transform.position.z);
    }

    public void SetPlayerAtCell(string whichCell)
    {
        if (whichCell != null & whichCell != "")
        {
            this.myPreviousCell = this.myCurrentCell;
            this.myCurrentCell = whichCell;
        }
    }
    public void SetPlayerAtCell(string whichCell, GameObject floorObject = null)
    {
        if (whichCell != null & whichCell != "")
        {
            this.myPreviousCell = this.myCurrentCell;
            this.myCurrentCell = whichCell;
            if (floorObject != null) myFloorObject = floorObject;
        }
    }

    public void MovePlayer(bool isMove) {
        this.isMove = isMove; 
    }

    public void MovePlayer(bool isMove, Transform whereTo)
    {
        this.isMove = isMove;
        TeacherDeskPoint = whereTo;
    }

    public void TeleportPlayer(Transform whereTo)
    {
        TeacherDeskPoint = whereTo;
        this.isMove = false;
     //   transform.position = TeacherDeskPoint.position;
        transform.SetPositionAndRotation(TeacherDeskPoint.position, TeacherDeskPoint.rotation);

    }
    public void MovePlayerToOriginalPostion(bool isMove, float addedSpeed=0)
    {
        // if (TeacherDeskPoint != OriginalPosition)
        // {
            additionalSpeed = addedSpeed;
            this.isMove = isMove;
            TeacherDeskPoint = OriginalPosition;
            turnPlayertoOriginalRotation = true;
        Debug.Log("player going to onriginal postion at speed" + addedSpeed.ToString());
        Debug.Log("current position = " + transform.position.ToString() + " , original position = " + OriginalPosition.position.ToString());
        //  }
    }

    public void LookToPlace(bool isLookAt, Transform whereTo)
    {
        this.isLookAround = isLookAt;
        // originalCameraRotation = transform.rotation;
        if (whereTo != null)
        {
            LookAtPoint = whereTo;
            // print(whereTo.rotation.ToString());
            //    StartCoroutine(SmoothLookAround(LookAtPoint));
        }
    }

    public void LookToPlayer(bool isLookAt, Transform whereTo)
    {
        this.isLookAround = isLookAt;
       // originalCameraRotation = transform.rotation;
        if (whereTo != null)
        {
            LookAtPoint = whereTo;
           // print(whereTo.rotation.ToString());
        //    StartCoroutine(SmoothLookAround(LookAtPoint));
        }
     }

    public void LookToPlayer(bool isLookAt)
    {
        this.isLookAround = isLookAt;
        transform.rotation = originalCameraRotation ;
    }


    private IEnumerator SmoothLookAround(Transform lookAtTransform)
    {
        float currentDelta = 0;

        while (currentDelta <= 1f)
        {
            currentDelta += Time.deltaTime / ANIMATION_DURATION_IN_SECONDS;

            if (lookAtTransform != null)
            {
                // HACK Trick: 
                // We want to rotate the camera transform at 'lookAtTransform', so we do that.
                // On every frame we rotate the camera to that direction. And to have a smooth effect we use lerp.
                // The trick is even when we know where to rotate the camera, we are going to override the rotation 
                // change caused by `transform.lookAt` with the rotation given by the lerp operation. 
              //  transform.LookAt(lookAtTransform);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookAtTransform.rotation, currentDelta);
            }
            else
            {
                Debug.LogWarning("SmoothLookAround :  There is no rotation data defined!");
                transform.rotation = originalCameraRotation;
            }

            yield return null;
        }

    }


    public void ShowOrHideTeachersHands(bool val)
    {
        if (teacherHands != null) teacherHands.SetActive(val);

    }

    public void ShowHoldingHandWorkSheets(bool val)
    {
        if (teacherHands != null) teacherHands.SetActive(true);
        if (WorksheetHoldingHandAttachementPoint != null) WorksheetHoldingHandAttachementPoint.SetActive(val);
       
    }

    public void ShowGivingHandWorkSheets(bool val)
    {
        if (WorksheetGivingHandAttachementPoint != null) WorksheetGivingHandAttachementPoint.SetActive(val);
    }

    public void PlayWorksheetPlacingOnTableAnimation()
    {
        ShowGivingHandWorkSheets(true);
        if (myHandsAnimator != null) myHandsAnimator.SetBool("PlaceWorksheetOnTable",true);
        if (myHandsAnimator != null) myHandsAnimator.SetBool("WorkSheetHandIdle", false);
        currentAnimationTime = 4f;
        isGivingWorkSheet = true;
        
        StartCoroutine(WaitTillPlacingWorksheetAnimationGetsCompleted());
       
    }
    IEnumerator WaitTillPlacingWorksheetAnimationGetsCompleted()
    {
        Debug.Log(myHandsAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        yield return null;
        Debug.Log(myHandsAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        yield return new WaitForSeconds(currentAnimationTime);
        if (myHandsAnimator != null) myHandsAnimator.SetBool("PlaceWorksheetOnTable", false);
        if (myHandsAnimator != null) myHandsAnimator.SetBool("WorkSheetHandIdle", true);
        Debug.Log(myHandsAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        ShowGivingHandWorkSheets(false);
        isGivingWorkSheet = false;
    }


}
