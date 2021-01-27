using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassRoomCellTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //   print("objected entered trigger" + other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerMovement>().SetPlayerAtCell(this.gameObject.name);
        }
        if (other.gameObject.CompareTag("child"))
        {
            other.gameObject.GetComponent<StudentAction>().SetPlayerAtCell(this.gameObject.name);
        }
    }

}
