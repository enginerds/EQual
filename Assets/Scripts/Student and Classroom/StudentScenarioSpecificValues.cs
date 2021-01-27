using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentScenarioSpecificValues : MonoBehaviour
{


    [Space(10)]
    [Header("General Values")]
    public bool LookatWindowNeighbourEnabler, LookatBetweenTwoPeopleRoutineEnabler, LookatBlackboardRoutineEnabler, LookatWindowRoutineEnabler, LookAroundOrTalkOrWriteRoutineEnabler, LookatWindowActive, WisperOrWriteRoutineEnabler, LookatShakeHeadsOrWriteRoutineEnabler;
    public Transform LookatWindowPoint, LookatBlackboardPoint;


    [Space(10)]
    [Header("Scenario 1 Values")]
    public bool scenario1SR3ReadAndThenLookAround;

    [Space(10)]
    [Header("Scenario 11 Values")]
    public bool scenario11Start;
    

}
