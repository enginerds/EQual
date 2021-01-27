using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PencilEvents : MonoBehaviour
{
    public ScenarioTwoManager mySceneTwoManager;

    public void EndGiveEvent()
    {
        Debug.Log("End Receive Animation Event");
        mySceneTwoManager.pencilAnim.gameObject.SetActive(false);
        mySceneTwoManager.inHandPencil_Tom.SetActive(true);
    }
}
