using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskDetails : MonoBehaviour
{
    
    [Tooltip("Which table the chair is associated to : Table1 = 1 , Table2 = 2, Table3 = 3, Table4 = 4")]
    public int tableAssociation = 1;
    public int DeskNumber = 0;
    public bool deskOccupied;

    [Space(10)]
    [Header("All the study materials that we need to make it visible at start")]
    public List<StudyMaterial> studyMaterialsToShowAtStart;

    private StudyMaterialType myStudyMaterials;

    private void Awake()
    {
        myStudyMaterials = this.GetComponentInChildren<StudyMaterialType>();
//        print(myStudyMaterials.gameObject.name);
        if (myStudyMaterials != null) myStudyMaterials.gameObject.SetActive(true);
            if (studyMaterialsToShowAtStart == null)
        {
            if (myStudyMaterials != null) myStudyMaterials.RandomlyShowOrHideStudyMaterialsOnDesk(); // only do random hiding if study materials to show is not present for the scenario
        }
        else
        {

            if (myStudyMaterials != null)
            {
                myStudyMaterials.HideByDefaultStudyMaterials();
                if (deskOccupied)
                {
                    foreach (StudyMaterial sm in studyMaterialsToShowAtStart)
                    {
                        myStudyMaterials.SetStudyMaterialVisiblity(sm, true);
                    }
                    
                }

            }
        }
    }

    public void EnableMyItems(bool val)
    {
        myStudyMaterials.gameObject.SetActive(val);
        if (studyMaterialsToShowAtStart == null)
        {
            if (deskOccupied)
                if (myStudyMaterials != null) myStudyMaterials.RandomlyShowOrHideStudyMaterialsOnDesk(); // only do random hiding if study materials to show is not present for the scenario
        }
        else
        {

            if (myStudyMaterials != null)
            {
                myStudyMaterials.HideByDefaultStudyMaterials();
                if (deskOccupied)
                {
                    foreach (StudyMaterial sm in studyMaterialsToShowAtStart)
                    {
                        myStudyMaterials.SetStudyMaterialVisiblity(sm, val);
                    }

                }

            }
        }
    }
    public void EnableMyItem(StudyMaterial whichOne, bool val)
    {
      
        if (myStudyMaterials != null)
            {
                if (deskOccupied)
                {
                myStudyMaterials.gameObject.SetActive(val);
                myStudyMaterials.SetStudyMaterialVisiblity(whichOne, val);
  
                }

            }
    }

}
