using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MoodIndicator
{
    Bad,
    Middle,
    Good
}
public class StudentMoodIndicator : MonoBehaviour
{
    public SpriteRenderer myMoodIcon;
    public StudentAction studentAction;
    public Camera playerCamera;

    public Color[] MoodColor = { Color.red, Color.yellow, Color.green };
    // Start is called before the first frame update
    void Start()
    {
      //  if (myMoodIcon != null) myMoodIcon.gameObject.SetActive(false);
        playerCamera = Camera.main;
    }

    private void Awake()
    {
        studentAction = GetComponentInParent<StudentAction>();
        if (studentAction)
        {
            //gamePlayManager.studentsActions.Add(this);
            studentAction.MyMoodChange += SetMoodIndicator;
            studentAction.ShowMyMood += SetMoodIndicatorVisible;
        }
    }

    private void Update()
    {
        if (myMoodIcon != null) if (myMoodIcon.gameObject.activeInHierarchy) myMoodIcon.gameObject.transform.LookAt(playerCamera.transform);
    }

    void SetMoodIndicatorVisible(bool val)
    {
      if(myMoodIcon !=null)  myMoodIcon.gameObject.SetActive(val);
    }

    void SetMoodIndicator(MoodIndicator mood)
    {
        switch (mood)
        {
            case MoodIndicator.Bad:
                if (myMoodIcon != null) myMoodIcon.color = MoodColor[(int)MoodIndicator.Bad];
                break;
            case MoodIndicator.Middle:
                if (myMoodIcon != null) myMoodIcon.color = MoodColor[(int)MoodIndicator.Middle];
                break;
            default: // is good
                if (myMoodIcon != null) myMoodIcon.color = MoodColor[(int)MoodIndicator.Good];
                break;

        }
    }
}
