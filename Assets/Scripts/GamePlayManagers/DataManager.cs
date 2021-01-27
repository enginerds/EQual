using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;


public class DataManager : MonoBehaviour
{

    public TextAsset studentsDataTextFile;
    private string studentsdata;

    private void Start()
    {
        studentsdata = studentsDataTextFile.text;
    }


    public StudentData GetStudentData(string studentName)
    {
        StudentData studentData = new StudentData();
        if (!studentName.Equals("") && studentName != null)
        {

            JSONNode data = JSON.Parse(studentsdata);

            JSONNode student = data[studentName];
            studentData.Name = studentName;
            studentData.Age = "" + student["age"];

            studentData.Gender = student["gender"];
            studentData.Ethnicity = student["ethnicity"];
            studentData.SpecialNeed = student["special need"];
            studentData.LevelOfAchiement = student["Level of achievement"];

        }
        else
        {
            studentData.Name = "";
            studentData.Age = "";
            studentData.Gender = "";
            studentData.Ethnicity = "";
            studentData.SpecialNeed = "";
            studentData.LevelOfAchiement = "";
        }


        return studentData;
    }


    public string GetStudentAcheivement(string studentName)
    {
        StudentData studentData = new StudentData();
        JSONNode data = JSON.Parse(studentsdata);

        switch ((string)data[studentName]["Level of achievement"])
        {
            case "hoch":
                return "High";


            case "niedrig":
                return "Low";


            case "mittel":
                return "Middle";


            default:
                return "default";
        }
    }
}
