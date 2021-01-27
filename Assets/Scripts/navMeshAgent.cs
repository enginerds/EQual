
using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class navMeshAgent : MonoBehaviour
{

    public Transform point1;
    public Transform point2;
    public Transform point3;
    private NavMeshAgent agent;
    private Animator anim;
    public float timer;
    public int MyInt;
    private Vector3 transOffest = new Vector3(0f, -0.35f, 0f);
    public bool isSeated;
    public GameObject RightHand;


    public float time;

    public GamePlayManager gamePlayManager;


    void Start()
    {
        gamePlayManager = FindObjectOfType<GamePlayManager>();
        isSeated = true;



        if (gamePlayManager)
        {
            //gamePlayManager.navMeshAgents.Add(this);
            gamePlayManager.StartTheNavMesh += StartNavMesh;

        }


    }


    void StartNavMesh()
    {
        isSeated = false;
        agent = GetComponent<NavMeshAgent>();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        anim = GetComponent<Animator>();

        GotoPoint1();


    }

    public void GotoPoint1()
    {
        anim = GetComponent<Animator>();

        agent.destination = point1.position;

      
        GotoPoint2();

    }

    public void GotoPoint2()
    {
        agent.destination = point2.position;
        
    }

    void Update()
    {

        if (agent != null && agent.pathPending == false && agent.remainingDistance <= 0.1)
        {

            timer += Time.deltaTime * 2;
            anim.SetFloat("Horizontal", 0);
            anim.SetFloat("Vertical", timer);

            if (timer >= 1)
            {
                anim.SetFloat("Vertical", 1);

            }

            agent.velocity = Vector3.zero;
            transform.rotation = point2.transform.rotation;
            transform.position = point2.transform.position + transOffest;
         
        }
      
    }

    public void RiseHand()
    {
        anim.SetBool("AskQuestion", true);

    }

    public void GoToPoint3() {

    }


}


