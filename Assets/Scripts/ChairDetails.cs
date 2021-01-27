using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairDetails : MonoBehaviour
{
    [Tooltip("Student talk direction in this chair : 0 for left and 1 for right")]
    public int studentTalkOrientation; // 0f for left and 1f for right
    [Tooltip("Which table the chair is associated to : Table1 = 1 , Table2 = 2, Table3 = 3, Table4 = 4")]
    public int tableAssociation = 1;
    public int DeskNumber = 0;
    public int MyNeighbourChair;

 
    [Space(10)]
    [Header("Progamattically set")]
    public StudentAction StudentSittingOnMe;

    [Space(10)]
    [Header("All the study materials that we need to make it visible at start")]
    public List<StudyMaterial> studyMaterialsToShowAtStart;

    [Space(10)]
    [Header("All the interseting spots the student of this chair can look at")]
    public List<Transform> thisChairBasedInterestingSpots;
    [Header("All the Windows the student of this chair can look at")]
    public List<Transform> thisChairBasedWindowsToLookAt;

    private void Start()
    {
        StudyMaterialType myStudyMaterialtype = this.GetComponentInChildren<StudyMaterialType>();
        if (studyMaterialsToShowAtStart == null)
        {
            if (myStudyMaterialtype != null) myStudyMaterialtype.RandomlyShowOrHideStudyMaterialsOnDesk(); // only do random hiding if study materials to show is not present for the scenario
        }
        else
        {
            
            if (myStudyMaterialtype != null)
            {
                myStudyMaterialtype.HideByDefaultStudyMaterials();
                if (StudentSittingOnMe != null)
                {
                    foreach (StudyMaterial sm in studyMaterialsToShowAtStart)
                    {
                        myStudyMaterialtype.SetStudyMaterialVisiblity(sm, true);
                    }
                }
               
            }
        }
    }

    public void ShowStudyMaterial(StudyMaterial whichone, bool showOrHide=true)
    {
        StudyMaterialType myStudyMaterialtype = this.GetComponentInChildren<StudyMaterialType>();
        if (myStudyMaterialtype != null) myStudyMaterialtype.SetStudyMaterialVisiblity(whichone, showOrHide);
    }

    public Transform GetAInterestingSpotToLookAt()
    {
        if (thisChairBasedInterestingSpots != null && thisChairBasedInterestingSpots.Count > 0) return thisChairBasedInterestingSpots[Random.Range(0, thisChairBasedInterestingSpots.Count)];
        else return null;
    }

    public Transform GetAWindowToLookAt()
    {
        // if there is a window for the student to look at from here, return that spot, or else return one o the interesting spots
        if (thisChairBasedWindowsToLookAt != null && thisChairBasedWindowsToLookAt.Count > 0) return thisChairBasedWindowsToLookAt[Random.Range(0, thisChairBasedWindowsToLookAt.Count)];
        else return GetAInterestingSpotToLookAt();
    }

}