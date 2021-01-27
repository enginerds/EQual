using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    public delegate void OnAnimationComplete();
    protected OnAnimationComplete onAnimationComplete;

    public string animClipTriggerName, previousAnimStateName;

    private Vector3 initialPos;
    public bool raisedHand, takingItem, NavPointStatusNeeded;
    public float takingItemAnimationtime;

    public bool IsSittingNow { get; set; } = false;

    StudentAction studentAction;


    


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        studentAction = GetComponent<StudentAction>();
        previousAnimStateName = "";

    }

    public bool GetAnimStateBool(string whichOne)
    {
        return anim.GetBool(whichOne);
    }


    public string GetAnimStateName(int layerIndex)
    {
        AnimatorClipInfo[] nameArr = anim.GetCurrentAnimatorClipInfo(layerIndex);
      //  print("Current Animation clip array length is "+nameArr.Length.ToString());
        return nameArr[0].clip.name;
    }

    private void Update()
    {

//        if (studentAction.gameObject.name == "Tom" && previousAnimStateName !="")
 //           print(previousAnimStateName);
        if (animClipTriggerName == "vi7_Talk_ON")
        {
            if ((anim.GetCurrentAnimatorStateInfo(0).normalizedTime) % 1 > 0.99f)
            {
               // print("Talking time " + anim.GetCurrentAnimatorStateInfo(0).normalizedTime.ToString() + " and its mod is " + ((anim.GetCurrentAnimatorStateInfo(0).normalizedTime) % 1 < 0.99f).ToString());
                VI7_TalkToFriendsStop();
                VI7_TalkToFriendsLeftAndRight();
            }
        }

        if(anim!=null)
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
            {
                if(NavPointStatusNeeded) // only needed for any scenario where kids walk to a nav point on the map, which can be occupied by other kids
                    if (studentAction.TeacherDeskPoint.gameObject.GetComponent<NavPointStatus>().occupied)
                        studentAction.studentNavMesh.enabled = false;
                    else
                        studentAction.studentNavMesh.enabled = true;
            }

    }
    /** WalkSitPickup Animation Without Callback */
    public void Walk_Sit_Pickup_Aniamtion(float verticalValue, float horizontalValue) {
        anim.SetFloat("Vertical", verticalValue);
        anim.SetFloat("Horizontal", horizontalValue);
        if(verticalValue !=0 || horizontalValue !=0) LogMyAnimation("Walking");
    }

    /** WalkSitPickup Animation With Callback */
    public void Walk_Sit_Pickup_Aniamtion(float verticalValue, float horizontalValue, OnAnimationComplete onAnimationComplete)
    {
        //Debug.Log("Sit Called");
        anim.SetFloat("Vertical", verticalValue);
        anim.SetFloat("Horizontal", horizontalValue);
        if (verticalValue != 0 || horizontalValue != 0) LogMyAnimation("Walking");
        this.onAnimationComplete = onAnimationComplete;

        StartCoroutine(WaitTillAnimationGetsCompleted());

    }

    /** Raise hand animation */
    public void RaiseHand(bool isRaise) {

        //ResetPos();   

        //ResetAnim();

        if (isRaise) {

            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") {
                anim.SetBool(previousAnimStateName, false);
            }

            previousAnimStateName = "AskQuestion";
        }// stop previous animation state, and set previousAnimState to current animation
        raisedHand = isRaise;
        animClipTriggerName = "AskQuestion";
        anim.SetBool("AskQuestion", isRaise);
        if (isRaise) LogMyAnimation("Ask Question - Raise Hand");
    }

    public void Sitting(bool isSitting)
    {

        //ResetPos();   

        //ResetAnim();

/*
        if (isSitting)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") anim.SetBool(previousAnimStateName, false); // stop previous animation state
            animClipTriggerName = "isSitting";
        }
        */
        anim.SetBool("isSitting", isSitting);
        IsSittingNow = isSitting;
        if (isSitting) LogMyAnimation("Is Sitting");
    }

    public void Sitting(bool isSitting,OnAnimationComplete onAnimationComplete)
    {

        //ResetPos();   

        //ResetAnim();

/*
        if (isSitting)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") anim.SetBool(previousAnimStateName, false); // stop previous animation state
            animClipTriggerName = "isSitting";
        }
        */
        anim.SetBool("isSitting", isSitting);
        if (isSitting) LogMyAnimation("Is Sitting");
        IsSittingNow = isSitting;
        this.onAnimationComplete = onAnimationComplete;

        StartCoroutine(WaitTillAnimationGetsCompleted());

    }


    public void MA16_SittingToStandUpIdle(bool val)
    {
        anim.SetBool("MA16_Sitting_Stand_Idle", val);
        if (val) LogMyAnimation("Sitting to Stand Idle");
    }


    public void WalkingToStandUpIdle(bool val)
    {
        anim.SetBool("Walking_Stand_Idle", val);
        if (val) LogMyAnimation("Walking to Stand Idle");
    }

    public void EE2_StandUpset(bool val)
    {
        anim.SetBool("EE2_Stand_Upset", val);
        if (val) LogMyAnimation("Stand Upset");
    }

    public void EE2_GetUpsetSeated(bool val) {
        anim.SetBool("ee2_getUpset_ON", val);
        if (val) LogMyAnimation("Stand Upset");
    }

    public void MB42_StandCrossArms(bool val)
    {
        anim.SetBool("MB42_Stand_CrossARMS_ON", val);
        if (val) LogMyAnimation("Stand Cross Arms ");
    }

    public void MB37_StandJumpingJacks(bool val)
    {
        anim.SetBool("MB37_StandingJumpingJacks_ON", val);
        if (val) LogMyAnimation("Stand do Jumping Jacks ");
    }

    public void MB4_SitClompFeetMultipleTimes()
    {
        anim.SetTrigger("MB4_Sit_Clomp_Feet_Multiple_Times");
        LogMyAnimation("MB4 Sit Clomp Feet Multiple Times");
    }


    public void MB4_SitClompFeetSingleTime()
    {
        anim.SetTrigger("MB4_Sit_Clomp_Feet_Single_Time");
        LogMyAnimation("MB4 Sit Clomp Feet Single Time");
    }

    public void IWO14_MB26_ThrowPencilBoxAndFoldHands()
    {
        anim.SetTrigger("IWO14_MB26_ThrowPencilBox_And_FoldHands");
        LogMyAnimation("IWO14+MB26 Throw Pencil Box And Fold Hands");

    }

    public void LookAround(bool isLookAround)
    {

        //ResetPos();   

        //ResetAnim();

        if (isLookAround) {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated")  anim.SetBool(previousAnimStateName, false);
            previousAnimStateName = "isLookAround"; }// stop previous animation state, and set previousAnimState to current animation

        animClipTriggerName = "isLookAround";
        anim.SetBool("isLookAround", isLookAround);
        if (isLookAround) LogMyAnimation("Is Looking Around");
    }


    public void TalkRightLeft(float value) {        
        anim.SetFloat("Horizontal", value);
        LogMyAnimation("Talk Right Left");
    }

    public void Talk(bool isTalk) {
        ResetPos();


       // ResetAnim();
        animClipTriggerName = "Istalking";
        if (isTalk) {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated")  anim.SetBool(previousAnimStateName, false);
                previousAnimStateName = "Istalking"; }// stop previous animation state, and set previousAnimState to current animation

        anim.SetBool("Istalking", isTalk);
        if (isTalk) LogMyAnimation("Is Talking");
    }

    public void Read(bool isReading) {

        ResetPos();

        //ResetAnim();
        animClipTriggerName = "IsReading";

        if (isReading) {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated")  anim.SetBool(previousAnimStateName, false);
                previousAnimStateName = "IsReading"; }// stop previous animation state, and set previousAnimState to current animation

        anim.SetBool("IsReading", isReading);
        if (isReading) LogMyAnimation("Is Reading");
    }

    /** Waits Till Animation Get Completed */
    IEnumerator WaitTillAnimationGetsCompleted() {
        Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name);

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("taking_item"))
        {
            Debug.Log((anim.GetCurrentAnimatorStateInfo(0).length + anim.GetCurrentAnimatorStateInfo(0).normalizedTime));
            if (takingItem) takingItem = false;
            //   yield return new WaitForSeconds(3.0f);
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);

        }
        else {

            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }

        onAnimationComplete();
    }

    public void SetAnimationSpeed(float value)
    {
        anim.speed = value;
    }


    ///
    /// Neil
    /// 
    public void PlayNeil(bool isNeil)
    {
        Debug.LogFormat("PlayNeil( {0} )", isNeil);
        anim.SetBool("IsNeil", isNeil);
        if (isNeil) LogMyAnimation("Is Neil");
    }



    public void SearchForPencil(bool isSeachr)
    {
        animClipTriggerName = "searchPencil";

        anim.SetTrigger("searchPencil");
        if (isSeachr) LogMyAnimation("Search Pencil");
    }

    public void TakeItem(bool isSeachr)
    {
        if (isSeachr)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") anim.SetBool(previousAnimStateName, false);  // stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "isTakeITem";
        }
        anim.SetBool("isTakeITem", isSeachr);
        if (isSeachr) LogMyAnimation("Take Item");
    }



    public void TakeItem(bool isTake, OnAnimationComplete onAnimationComplete)
    {

        animClipTriggerName = "AskForPencil";
        if (isTake)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "isTakeITem";
        }
        anim.SetBool("isTakeITem", isTake);
        if (isTake) LogMyAnimation("Take Item");
        takingItem = isTake;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("taking_item"))
            takingItemAnimationtime = anim.GetCurrentAnimatorStateInfo(0).length + anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        else
            takingItemAnimationtime = 1.0f;
        this.onAnimationComplete = onAnimationComplete;

        StartCoroutine(WaitTillAnimationGetsCompleted());

    }

    public void SearchForPencil(bool isSeachr, OnAnimationComplete onAnimationComplete)
    {
        animClipTriggerName = "searchPencil";

        anim.SetTrigger("searchPencil");
        if (isSeachr) LogMyAnimation("Search Pencil");
        this.onAnimationComplete = onAnimationComplete;

        StartCoroutine(WaitTillAnimationGetsCompleted());

    }

    public void PlaceItemOnGround(bool isplaceItem)
    {

        animClipTriggerName = "PlaceItem";
        anim.SetTrigger("isPlaceItemOnGround");
        if (isplaceItem) LogMyAnimation("Place Item On Ground");

    }
    public void TapAndAskAskPen(bool isAskPen)
    {


        //   ResetAnim();
        animClipTriggerName = "MB28_TapOnNeighbour&AskPen";
        float value = 0.5f;
        if (isAskPen)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "";
            var chairdetails = studentAction.chairPoint.gameObject.GetComponent<ChairDetails>();

            if (chairdetails != null)
            {
                value = (float)chairdetails.studentTalkOrientation;
            }
            else
                value = (Random.Range(0, 2) == 1) ? 1.0f : 0.0f;
        }
        anim.SetFloat("Horizontal", value);
        anim.SetTrigger("TapAndAskForPencil");

        if (isAskPen) LogMyAnimation("MB28 - Tap On Neighbour & Ask Pen");
    }

    public void AskPen(bool isAskPen)
    {

        animClipTriggerName = "AskForPencil";
        if (isAskPen)
        {
            //        if (previousAnimStateName != "" && previousAnimStateName != "isSitting") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "";
        }

        anim.SetTrigger("AskForPencil");
        if (isAskPen) LogMyAnimation("ASk for Pencil");

    }

    public void ReceivePencil()
    {
        animClipTriggerName = "receivePencil";
        anim.SetTrigger("receivePencil");
       LogMyAnimation("Receive Pencil");
    }

    public void AskPen(bool isAskPen, OnAnimationComplete onAnimationComplete)
    {

        animClipTriggerName = "AskForPencil";

        if (isAskPen)
        {
            //        if (previousAnimStateName != "" && previousAnimStateName != "isSitting") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "";
        }

        anim.SetTrigger("AskForPencil");
        if (isAskPen) LogMyAnimation("Ask for Pencil");

        this.onAnimationComplete = onAnimationComplete;

        StartCoroutine(WaitTillAnimationGetsCompleted());

    }


    public void GivePen(bool isAskPen)
    {
        animClipTriggerName = "passPencil";

        if (isAskPen) LogMyAnimation("Pass Pencil");
        anim.SetTrigger("passPencil");
    }

    public void GivePen(bool isAskPen, OnAnimationComplete onAnimationComplete)
    {

        animClipTriggerName = "passPencil";


        anim.SetTrigger("passPencil");
        if (isAskPen) LogMyAnimation("Pass Pencil");
        this.onAnimationComplete = onAnimationComplete;

        StartCoroutine(WaitTillAnimationGetsCompleted());

    }


    public void ThrowPen(bool isAskPen)
    {

        animClipTriggerName = "throwPencilLayHead";
        if (isAskPen)
        {
            //        if (previousAnimStateName != "" && previousAnimStateName != "isSitting") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "throwPencilLayHead";
        }
        anim.SetBool("throwPencilLayHead", isAskPen);
        if (isAskPen) LogMyAnimation("Throw Pencil Lay Head");
    }

    public void ThrowPen(bool isAskPen, OnAnimationComplete onAnimationComplete)
    {
        animClipTriggerName = "throwPencilLayHead";

        if (isAskPen)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "throwPencilLayHead";
        }
        anim.SetBool("throwPencilLayHead", isAskPen);
        if (isAskPen) LogMyAnimation("Throw Pencil Lay Head");

        this.onAnimationComplete = onAnimationComplete;

        StartCoroutine(WaitTillAnimationGetsCompleted());

    }



    public void LaughAndWork(bool isAskPen)
    {
        animClipTriggerName = "laughingAndreading";
        if (isAskPen)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "laughingAndreading";
        }
        anim.SetBool("laughingAndreading", isAskPen);
        if (isAskPen) LogMyAnimation("Laughing and Reading");
    }

    public void LaughAndWork(bool isAskPen, OnAnimationComplete onAnimationComplete)
    {
        animClipTriggerName = "laughingAndreading";
        if (isAskPen)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "laughingAndreading";
        }
        anim.SetBool("laughingAndreading", isAskPen);
        if (isAskPen) LogMyAnimation("Laughing and Reading");

        this.onAnimationComplete = onAnimationComplete;

        StartCoroutine(WaitTillAnimationGetsCompleted());

    }

    private void ResetAnim() {

        if (animClipTriggerName != null){

            //anim.SetBool(animClipTriggerName, false);

        }
        
    }

    public void ResetCurrentAnim()
    {

        if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
        previousAnimStateName = "";

    }

    public void SitAgitated(bool isAskPen)
    {
        animClipTriggerName = "SitAgitated";
        /*
        if (isAskPen)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "SitAgitated";
        }
        */
        anim.SetBool("SitAgitated", isAskPen);
        if (isAskPen) LogMyAnimation("Sit Agitated");
    }

    public void ReadingPaperOnDesk(bool isAskPen)
    {

        animClipTriggerName = "ReadingPaperOnDesk";
        if (isAskPen)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "ReadingPaperOnDesk";
        }
        anim.SetBool("ReadingPaperOnDesk", isAskPen);
        if (isAskPen) LogMyAnimation("Reading Paper On Desk");
    }

    public void WritePaperOnDesk(bool isAskPen)
    {
        animClipTriggerName = "workOnSheet";
        if (isAskPen)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "workOnSheet";
        }
        anim.SetBool("workOnSheet", isAskPen);
        if (isAskPen) LogMyAnimation("Write Paper On Desk");
    }




    public void EE4_Laughs(bool islaugh)
    {
        animClipTriggerName = "EE4_Laugh";
        if (islaugh)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "EE4_Laugh_ON";
        }
        anim.SetBool("EE4_Laugh_ON", islaugh);
        if (islaugh) LogMyAnimation("EE4 - Laughing");
    }





    public void IWO29_FidgetsInChair(bool shallWe)
    {
        //  LiftUp();

        //   ResetAnim();
        animClipTriggerName = "iwo29_FidgetsInChair";
        if (shallWe)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "iwo29_FidgetsInChair_ON";
        }
        anim.SetBool("iwo29_FidgetsInChair_ON", shallWe);
        if (shallWe) LogMyAnimation("IWO29 - Fidgets In Chair");
    }

    public void MB39_ShakesHead(bool shallWe)
    {
        //  LiftUp();

        //   ResetAnim();
        animClipTriggerName = "MB39_ShakesHead";
        if (shallWe)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "MB39_ShakesHead_ON";
        }
        anim.SetBool("MB39_ShakesHead_ON", shallWe);
        if (shallWe) LogMyAnimation("MB39 - Shakes Head");
    }

    public void MB39_StandingShakesHead(bool shallWe)
    {
        anim.SetBool("MB39_StandingShakesHead_ON", shallWe);
        if (shallWe) LogMyAnimation("MB39 - Standing and Shakes Head");
    }

    public void MB18_NodHead(bool shallWe)
    {
        //  LiftUp();

        //   ResetAnim();
        animClipTriggerName = "MB18_NodHead";
        if (shallWe)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "MB18_Nod_ON";
        }
        anim.SetBool("MB18_Nod_ON", shallWe);
        if (shallWe) LogMyAnimation("MB18 - Nods Head");
    }

    public void MB26_FoldHands(bool shallWe)
    {
        //  LiftUp();

        //   ResetAnim();
        animClipTriggerName = "MB26_FoldHands";
        if (shallWe)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "MB26_FoldHands_ON";
        }
        anim.SetBool("MB26_FoldHands_ON", shallWe);
        if (shallWe) LogMyAnimation("MB26 - Fold Hands");
    }
    

    public void MB21_readTextBook(bool shallWe)
    {
        //  LiftUp();

        //   ResetAnim();
        animClipTriggerName = "mb21_readTextBook";
        if (shallWe)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "mb21_readTextBook_ON";
        }
        anim.SetBool("mb21_readTextBook_ON", shallWe);
        if (shallWe) LogMyAnimation("mb21 readTextBook ");
    }



    public void MB28_TapOnNeighbour(bool shallWe)
    {
        //  LiftUp();

        //   ResetAnim();
        animClipTriggerName = "MB28_TapOnNeighbour";
        float value = 0.5f;
        if (shallWe)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "MB28_TapOnNeighbour_ON";
            var chairdetails = studentAction.chairPoint.gameObject.GetComponent<ChairDetails>();

            if (chairdetails != null)
            {
                value = (float)chairdetails.studentTalkOrientation;
            }
            else
                value = (Random.Range(0, 2) == 1) ? 1.0f : 0.0f;
        }
        anim.SetFloat("Horizontal", value);
        anim.SetBool("MB28_TapOnNeighbour_ON", shallWe);
        
        if (shallWe) LogMyAnimation("MB28 - Tap On Neighbour");
    }
    public void MB28_TapOnNeighbour(bool shallWe, float SideDirection = 0.5f)
    {
        //  LiftUp();

        //   ResetAnim();
        animClipTriggerName = "MB28_TapOnNeighbour";
        float value = 0.5f;
        if (shallWe)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "MB28_TapOnNeighbour_ON";
            var chairdetails = studentAction.chairPoint.gameObject.GetComponent<ChairDetails>();

            if (chairdetails != null)
            {
                value = (float)chairdetails.studentTalkOrientation;
            }
            else
                value = (Random.Range(0, 2) == 1) ? 1.0f : 0.0f;
        }
        anim.SetFloat("Horizontal", SideDirection);
        anim.SetBool("MB28_TapOnNeighbour_ON", shallWe);

        if (shallWe) if (SideDirection < 0.5f) LogMyAnimation("MB28 - Tap On Neighbour Left"); else if (SideDirection > 0.5f) LogMyAnimation("MB28 - Tap On Neighbour Right");
    }


    public void MB30_PeepToSideLeftOrRight(bool shallWe)
    {
        //  LiftUp();

        //   ResetAnim();
        animClipTriggerName = "MB30_PeepToSide_LeftOrRight";
        float value = 0.5f;
        if (shallWe)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "MB30_PeepToSide_ON";
            var chairdetails = studentAction.chairPoint.gameObject.GetComponent<ChairDetails>();
            
            if (chairdetails != null)
            {
                value = (float)chairdetails.studentTalkOrientation;
            }
            else
                value = (Random.Range(0, 2) == 1) ? 1.0f : 0.0f;         
        }
        anim.SetBool("MB30_PeepToSide_ON", shallWe);
        anim.SetFloat("Horizontal", value);
        if (shallWe) LogMyAnimation("MB30 - Peep To Side - Left Or Right");
    }
    public void MB30_PeepToSide(bool shallWe, float SideDirection=0.5f)
    {
        //  LiftUp();

        //   ResetAnim();
        animClipTriggerName = "MB30_PeepToSide";
        if (shallWe)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "MB30_PeepToSide_ON";
        }
        anim.SetFloat("Horizontal", SideDirection);
        anim.SetBool("MB30_PeepToSide_ON", shallWe);
        if (shallWe) if (SideDirection < 0.5f) LogMyAnimation("MB30 - Peep ToS ide Left"); else if (SideDirection > 0.5f) LogMyAnimation("MB30 - Peep To Side Right");
    }


    public void MB33_WorkOnSheets(bool shallWe)
    {
        //  LiftUp();

        //   ResetAnim();
        animClipTriggerName = "mb33_WriteOnSheet_ON";
        if (shallWe)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "mb33_WriteOnSheet_ON";
        }
//        print("WorkSheet Writting animation started");
        anim.SetBool("mb33_WriteOnSheet_ON", shallWe);
        if (shallWe) LogMyAnimation("MB33 - Work On Sheets");
    }

    public void MB9_LookAround(bool shallWe)
    {
        //  LiftUp();

        //   ResetAnim();
        animClipTriggerName = "mb9_LookAround";
        if (shallWe)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "mb9_LookAround_ON";
        }
        anim.SetBool("mb9_LookAround_ON", shallWe);
        if (shallWe) LogMyAnimation("MB9 - Look Around");
    }

    public void MB9_LookAroundFast(bool shallWe)
    {
        //  LiftUp();

        //   ResetAnim();
        animClipTriggerName = "mb9_LookAround";
        if (shallWe)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "mb9_LookAround_ON";
        }
        anim.SetBool("mb9_LookAround_ON", shallWe);
        anim.speed *= (shallWe)?1.5f:1f;
        if(shallWe) LogMyAnimation("MB9 - Look Around Fast");
    }

    public void IWO29_FidgetingOnChair(bool shallWe)
    {
        //  LiftUp();

        //   ResetAnim();
        animClipTriggerName = "iwo29_FidgetsInChair_ON";
        //if (shallWe)
        //{
        //    if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
        //    previousAnimStateName = "mb33_WriteOnSheet_ON";
        //}
        //        print("WorkSheet Writting animation started");
        anim.SetBool("iwo29_FidgetsInChair_ON", shallWe);
        if (shallWe) LogMyAnimation("iwo29_FidgetsInChair_ON");
    }

    public void MB34_ShrugShoulders(bool shallWe)
    {
        //  LiftUp();

        //   ResetAnim();
        animClipTriggerName = "mb34_ShrugShoulders";
        if (shallWe)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "mb34_ShrugShoulders_ON";
        }
        anim.SetBool("mb34_ShrugShoulders_ON", shallWe);
        if (shallWe) LogMyAnimation("mb34_ShrugShoulders_ON");
    }

    public void VI11_TalkToFriendsLeftAndRight(float value)
    {
        animClipTriggerName = "vi11_Wispher_ON";
         value = (value == 1f) ? 0.0f : 1.0f; // quick hack
        if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false);  }// stop previous animation state, and set previousAnimState to current animation
        previousAnimStateName = "vi11_Wispher_ON";
        anim.SetBool("vi11_Wispher_ON", true);
        anim.SetFloat("Horizontal", value);
        LogMyAnimation("VI11 - Wispher Right Or Left");
    }


    public void VI11_TalkToFriendsLeftAndRight() // use the kid's default chair based orientation
    {
        animClipTriggerName = "vi11_Wispher_ON";
        var chairdetails = studentAction.chairPoint.gameObject.GetComponent<ChairDetails>();
        float value = 0f;
        if (chairdetails != null)
        { value = (float)chairdetails.studentTalkOrientation;
       //     print("VI11 Value found for " + studentAction.name + " is " + value.ToString());
        }
        else
            value = (Random.Range(0, 2) == 1) ? 1.0f : 0.0f;
        if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false);  }// stop previous animation state, and set previousAnimState to current animation
        previousAnimStateName = "vi11_Wispher_ON";
        anim.SetBool("vi11_Wispher_ON", true);
        anim.SetFloat("Horizontal", value);
        LogMyAnimation("VI11 - Wispher Right Or Left");
    }
    public void VI11_TalkToFriendsStop() // reset their talking
    {
        animClipTriggerName = "";
        anim.SetFloat("Horizontal", 0.5f);
        if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") previousAnimStateName = "";
        anim.SetBool("vi11_Wispher_ON", false);
        
    }

    public void VI7_TalkToFriendsLeftAndRight(float value)
    {
        animClipTriggerName = "vi7_Talk_ON";
        // value = (value == 1) ? 0.0f : 1.0f; // quick hack
        if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false);  }// stop previous animation state, and set previousAnimState to current animation
        previousAnimStateName = "vi7_Talk_ON";
        anim.SetBool("vi7_Talk_ON", true);
        anim.SetFloat("Horizontal", value);
        LogMyAnimation("VI7 - Talk Right Or Left");
    }


    public void VI7_TalkToFriendsLeftAndRight()// use the kid's default chair based orientation
    {
        animClipTriggerName = "vi7_Talk_ON";
        var chairdetails = studentAction.chairPoint.gameObject.GetComponent<ChairDetails>();
        float value = 0f;
        if (chairdetails != null)
            value = (float)chairdetails.studentTalkOrientation;
        else
            value = (Random.Range(0, 2) == 1) ? 1.0f : 0.0f;

        if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false);  }// stop previous animation state, and set previousAnimState to current animation
        previousAnimStateName = "vi7_Talk_ON";
        anim.SetBool("vi7_Talk_ON", true);
        anim.SetFloat("Horizontal", value);
        LogMyAnimation("VI7 - Talk Right or Left");
    }
    public void VI7_TalkToFriendsLeftAndRight(bool talkOrNot)
    {

        if (talkOrNot)
        {
            animClipTriggerName = "vi7_Talk_ON";
            var chairdetails = studentAction.chairPoint.gameObject.GetComponent<ChairDetails>();
            float value = 0f;
            if (chairdetails != null)
                value = (float)chairdetails.studentTalkOrientation;
            else
                value = (Random.Range(0, 2) == 1) ? 1.0f : 0.0f;
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false);  }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "vi7_Talk_ON";
            anim.SetBool("vi7_Talk_ON", true);
            anim.SetFloat("Horizontal", value);
            if (talkOrNot) LogMyAnimation("VI7 - Talk");
        }
        else
        {
            animClipTriggerName = "";
            anim.SetFloat("Horizontal", 0.5f);
            anim.SetBool("vi7_Talk_ON", false);
           // Sitting(true);
        }
    }

    public void VI7_TalkToFriendsStop() // reset their talking
    {
        animClipTriggerName = "";
        anim.SetFloat("Horizontal", 0.5f);
        if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") previousAnimStateName = "";
        anim.SetBool("vi7_Talk_ON", false);
      //  Sitting(true);

    }


    public void EE2_Upset(bool isUpset)
    {
     //   LiftUp();

     //   ResetAnim();
        animClipTriggerName = "ee2_getUpset";
        if (isUpset)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "ee2_getUpset_ON";
        }
        anim.SetBool("ee2_getUpset_ON", isUpset);
       if(isUpset) LogMyAnimation("EE2 - Get Upset");
    }

    public void MB19_Protest(bool isProtest)
    {
        //   LiftUp();

        //   ResetAnim();
        animClipTriggerName = "mb19_Protest";
        if (isProtest)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "mb19_Protest_ON";
        }
        anim.SetBool("mb19_Protest_ON", isProtest);
        if(isProtest)LogMyAnimation("MB19 - Protest");
    }


    /** Raise hand animation */
    public void RaiseHandAndKeep(bool isRaise)
    {

        //ResetPos();   

        //ResetAnim();


        raisedHand = isRaise;
        animClipTriggerName = "RaiseHandAndKeep";
        if (isRaise)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") anim.SetBool(previousAnimStateName, false);// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "RaisedHandAndKeep_ON";
        }
        anim.SetBool("RaisedHandAndKeep_ON", isRaise);
        if(isRaise)LogMyAnimation("Raised Hands And Keep");
    }




    public void Upset(bool isAskPen)
    {
        LiftUp();

        ResetAnim();
        animClipTriggerName = "laughingAndreading";
        if (isAskPen)
        {
            if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") anim.SetBool(previousAnimStateName, false);// stop previous animation state, and set previousAnimState to current animation
            previousAnimStateName = "isUpset";
        }
        anim.SetBool("isUpset", isAskPen);
        if(isAskPen)LogMyAnimation("UpSet");
    }


    public void ResetAllAnim()
    {
        //    print("resetting animations of this character " + this.name);
        anim.SetBool("EE2_Stand_Upset", false);
        anim.SetBool("AskQuestion", false);
        anim.SetBool("mb19_Protest_ON", false);
        anim.SetBool("ee2_getUpset_ON", false);
        anim.SetFloat("Horizontal", 0.5f);
        anim.SetBool("vi11_Wispher_ON", false);
        anim.SetBool("vi7_Talk_ON", false);
        anim.SetBool("mb33_WriteOnSheet_ON", false);
        anim.SetBool("isTakeITem", false);
        anim.SetBool("IsReading", false);
        anim.SetBool("Istalking", false);
        anim.SetBool("isLookAround", false);
        anim.SetBool("mb9_LookAround_ON", false);
        anim.SetBool("mb20_RaiseHands_ON", false);
        anim.SetBool("RaisedHandAndKeep_ON", false);
        anim.SetBool("MB26_FoldHands_ON", false);
        anim.SetBool("MB18_Nod_ON", false);
        anim.SetBool("MB39_ShakesHead_ON", false);
        anim.SetBool("MB39_StandingShakesHead_ON", false);
        anim.SetBool("MB42_Stand_CrossARMS_ON", false);
        anim.SetBool("MB37_StandingJumpingJacks_ON", false);
        
        animClipTriggerName = "isSitting";
        if (previousAnimStateName != "" && previousAnimStateName != "isSitting" && previousAnimStateName != "SitAgitated") { anim.SetBool(previousAnimStateName, false); }// stop previous animation state, and set previousAnimState to current animation
        previousAnimStateName = "";
        anim.SetBool("isSitting", true);

    }
    public void ResetAllAnimAndWalk()
    {
      //  print("resetting animations of this character " + this.name);
        anim.SetBool("AskQuestion", false);
        anim.SetBool("mb19_Protest_ON", false);
        anim.SetBool("ee2_getUpset_ON", false);
        anim.SetFloat("Horizontal", 0.5f);
        anim.SetBool("vi11_Wispher_ON", false);
        anim.SetBool("vi7_Talk_ON", false);
        anim.SetBool("mb33_WriteOnSheet_ON", false);
        anim.SetBool("isTakeITem", false);
        anim.SetBool("IsReading", false);
        anim.SetBool("Istalking", false);
        anim.SetBool("isLookAround", false);
        anim.SetBool("mb9_LookAround_ON", false);
        anim.SetBool("RaisedHandAndKeep_ON", false);
        anim.SetBool("MB26_FoldHands_ON", false);
        anim.SetBool("MB18_Nod_ON", false);
        anim.SetBool("MB39_ShakesHead_ON", false);
        anim.SetBool("MB39_StandingShakesHead_ON", false);
        anim.SetBool("MB42_Stand_CrossARMS_ON", false);
        anim.SetBool("MB37_StandingJumpingJacks_ON", false);
        animClipTriggerName = "Walking";
        anim.SetBool("isSitting", false);
        anim.SetBool("EE2_Stand_Upset", false);
    }


    void LogMyAnimation(string animName)
    {
        if (animName!= null && animName != "")
            this.studentAction.LogMyAnimation(animName);
    }

    public void ResetPos() {

        //transform.position = studentAction.sitPos;

    }

    public void LiftUp() {
        //transform.position = new Vector3(studentAction.sitPos.x, -0.128f, studentAction.sitPos.z);
    }

}
