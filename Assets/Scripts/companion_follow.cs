using UnityEngine;
using UnityEngine.AI;

public class companion_follow : MonoBehaviour
{
    /*// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    */

    public NavMeshAgent agent;
    public Transform playerTransform;
    public Animator anim;
    Vector3 dest;

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Distance : " + agent.remainingDistance + " | Pending : " + agent.pathPending);

        dest = playerTransform.position;
        agent.SetDestination(dest);


        dest = playerTransform.position;
        agent.destination = dest;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            anim.ResetTrigger("walking");
            anim.SetTrigger("idle");
            
        }else
        {
            anim.ResetTrigger("idle");
            anim.SetTrigger("walking");
        }
    }
}
