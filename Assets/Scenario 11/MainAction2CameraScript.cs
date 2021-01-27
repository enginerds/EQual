using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAction2CameraScript : MonoBehaviour
{
    public List <GameObject> DayShedules;


    public void TriggerBook1()
    {
       if (DayShedules[0] !=null) DayShedules[0].SetActive(true);
    }
    public void TriggerBook2()
    {
        if (DayShedules[1] != null) DayShedules[1].SetActive(true);
    }
    public void TriggerBook3()
    {
        if (DayShedules[2] != null) DayShedules[2].SetActive(true);
    }
    public void TriggerBook4()
    {
        if (DayShedules[3] != null) DayShedules[3].SetActive(true);
    }
    public void TriggerBook5()
    {
        if (DayShedules[4] != null) DayShedules[4].SetActive(true);
    }
    public void TriggerBook6()
    {
        if (DayShedules[5] != null) DayShedules[5].SetActive(true);
    }



    public void TriggerNext2()
    {
        Debug.Log("call main action 3");
    }
}
