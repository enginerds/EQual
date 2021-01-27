using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainObjectsManager : MonoBehaviour
{
    public Transform classmiddlePoint, Table1, Table2, Table3, Table4;
    public Transform Table1MoveToPoint, Table2MoveToPoint, Table3MoveToPoint, Table4MoveToPoint;
    public Transform BlackBoard, TeacherInFrontOfClassPoint;
    public Transform Table4WindowPoint, Table3WindowPoint;
    public List<StudentAction> studentsAtTable1, studentsAtTable2, studentsAtTable3, studentsAtTable4;
}
