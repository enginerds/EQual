using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOVController : MonoBehaviour
{


    public int targetFOV;
    bool isSet;
    private int orignalFOV;
    // Start is called before the first frame update
    void Start()
    {
        orignalFOV = Mathf.RoundToInt(Camera.main.fieldOfView);
    }

    // Update is called once per frame
    void Update()
    {
        if (isSet) {

            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * 1f);

            if (Camera.main.fieldOfView == targetFOV) {
                isSet = false;
            }

        }
    }

    public void ReSetFOV()
    {
        targetFOV = orignalFOV;
        isSet = true;


    }
    public void SetFOV(int TargetValue) {
        targetFOV = TargetValue;
        isSet = true;
       

    }

}
