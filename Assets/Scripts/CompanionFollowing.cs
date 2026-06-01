using UnityEngine;
using UnityEngine.AI;

public class FollowAI : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    public GameObject ObejctToFollow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, ObejctToFollow.transform.position); // get the distance 

        if (distance < 3)
        {
            agent.isStopped = true;
            animator.SetInteger("Speed", 0);
        }
        else if (distance >= 3 && distance < 8)
        {
            agent.isStopped = false;
            agent.SetDestination(ObejctToFollow.transform.position);

            animator.SetInteger("Speed", 1);

            agent.speed = 3;
        }
        else if (distance > 8)
        {
            agent.isStopped = false;

            agent.SetDestination(ObejctToFollow.transform.position);

            animator.SetInteger("Speed", 2);

            agent.speed = 6;
        }
    }
}