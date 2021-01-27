using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentData
{
     
    private string name, gender, age, ethnicity, specialNeed, levelOfAchiement;


    public string Name {
        get { return name; }
        set { name = value; }
    }


    public string Gender
    {
        get { return gender; }
        set { gender = value; }
    }

    public string Age
    {
        get { return age; }
        set { age = value; }
    }

    public string Ethnicity
    {
        get { return ethnicity; }
        set { ethnicity = value; }
    }

    public string SpecialNeed
    {
        get { return specialNeed; }
        set { specialNeed = value; }
    }

    public string LevelOfAchiement {
        get { return levelOfAchiement; }
        set { levelOfAchiement = value; }
    }

}
