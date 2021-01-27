using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StudentNavMeshAgent : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    public delegate void OnDestinationReached();
    protected OnDestinationReached onDestinationReached;
    private float orignalSpeed = 1;
    public bool IsAgentDestinationReached { get; set; } = false;
    Vector3 target;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.autoBraking = false;
        orignalSpeed = navMeshAgent.speed;
    }

    public void SetNavMeshAgentDestination(Vector3 targetDestination)
    {
        navMeshAgent.enabled = true;
        navMeshAgent.destination = targetDestination;
        IsAgentDestinationReached = false;
    }

    public void SetNavMeshAgentDestination(Vector3 targetDestination, OnDestinationReached onDestinationReached)
    {
        navMeshAgent.enabled = true;
        target = targetDestination;        
        navMeshAgent.destination = targetDestination;
        this.onDestinationReached = onDestinationReached;
        IsAgentDestinationReached = false;
        StartCoroutine(WaitTillAgentTeachsToDestination());
    }

    IEnumerator WaitTillAgentTeachsToDestination()
    {

        Vector3 distance = transform.position - target;
        float remaining = Vector3.SqrMagnitude(distance);

        
        while (remaining > 0.8f)
        {
            yield return new WaitForSeconds(0.1f);
            distance = transform.position - target;
            remaining = Vector3.SqrMagnitude(distance);
            //Debug.Log(remaining);
        }

     
        if (remaining <= 0.8)
        {
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.enabled = false;
            IsAgentDestinationReached = true;
            onDestinationReached();            
            
        }


        //Debug.Log("out of loop");

        //yield return null;
    }

    public NavMeshAgent GetNavMeshAgent() {
        return navMeshAgent;
    }

    public void SetNavigationSpeed( float val)
    {
        navMeshAgent.speed = val;
    }
    public void ResetNavigationSpeed()
    {
        navMeshAgent.speed = orignalSpeed;
    }

}
