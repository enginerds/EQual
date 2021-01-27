using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HighlightListner : MonoBehaviour
{
    public Behaviour Halo; 
    GamePlayManager gamePlayManager;
    public IKControl IKControlScript;
    public StudentAction studentAction;
    public Camera playerCamera;

    private bool updateStudentSelectedData = false;
    private string studentSelectedTimStamp = "";
    private string studentSelectedDuration = "";
    private TimeSpan studentSelectedTimeSpan;
    private void Start()
    {
        Halo.enabled = false;
        gamePlayManager = FindObjectOfType<GamePlayManager>();
        IKControlScript = GetComponentInParent<IKControl>();
        studentAction = GetComponentInParent<StudentAction>();
        playerCamera = Camera.main;
        updateStudentSelectedData = false;
    }

    
    private void OnMouseEnter()
    {
        
        if (Halo)
        {
            Halo.enabled = true;
        }
    //    print("entering " + gameObject.name);
        if (gamePlayManager)
        {
            studentSelectedTimeSpan = gamePlayManager.GetCurrentTimeSpan();
            studentSelectedTimStamp = string.Format("{0:D2}:{1:D2} ", studentSelectedTimeSpan.Minutes, studentSelectedTimeSpan.Seconds);
        }
        if (gamePlayManager)
        {
            gamePlayManager.CurrentlySelectedStudent(gameObject, studentSelectedTimStamp,true);
        }
     //   print(studentSelectedTimeSpan);
        if (studentAction)
        {
            studentAction.TeacherSelectedMeUsingMouse = true;
        }
        //commented below as per latest JIRA request EQ-92
       // HeadOrientation(true);
    }

    private void OnMouseExit()
    {
        if (Halo)
        {
            Halo.enabled = false;

        }

        var tempDuration =  gamePlayManager.GetCurrentTimeSpan()- studentSelectedTimeSpan;
        studentSelectedDuration = string.Format("{0:D2}:{1:D2} ", tempDuration.Minutes, tempDuration.Seconds);
      //  print("exiting " + gameObject.name);
        if (gamePlayManager)
        {
            gamePlayManager.CurrentlySelectedStudent(gameObject, studentSelectedDuration);
        }
        if (studentAction)
        {
            studentAction.TeacherSelectedMeUsingMouse = false;
        }
        //commented below as per latest JIRA request EQ-92
        //   HeadOrientation(false);
    }

    public void HeadOrientation(bool lookAtPlayer)
    {
        if (lookAtPlayer == true)
        {
            IKControlScript.lookObj = playerCamera.transform;
            IKControlScript.ikActive = true;
        }
        else
        {
            IKControlScript.ikActive = false;
        }

        
    }
    public void HeadOrientation(bool lookAtPlayer,Transform whomToLookAt)
    {
        if (lookAtPlayer == true)
        {
            if (whomToLookAt != null)
            {
                IKControlScript.lookObj = whomToLookAt;
                IKControlScript.ikActive = true;
            }
        }
        else
        {
            IKControlScript.ikActive = false;
        }


    }


}
