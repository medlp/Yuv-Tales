using System;
using UnityEngine;
using UnityEngine.AI;

public class CompanionFollowing: MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    public GameObject ObejctToFollow;
    public Vector3 pingPosition;
    private float pingHoldTime = 3f;
    private float pingTimer;
    bool isPinged;

    private float playerDist;

    public event Action OnPingArrived;
    private bool hasArrivedAtPing;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        playerDist = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPinged)
        {
            FollowPing();
        }
        else
            FollowPlayer();
    }

    private void FollowPing()
    {
        float distance = Vector3.Distance(transform.position, pingPosition);

        if (distance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            animator.SetInteger("Speed", 0);
            Dig();

            if (!hasArrivedAtPing)
            {
                hasArrivedAtPing = true;
                OnPingArrived?.Invoke();
            }

            pingTimer -= Time.deltaTime;
            if (pingTimer <= 0f)
                isPinged = false;
        }
        else if (distance < 8)
        {
            agent.isStopped = false;
            agent.SetDestination(pingPosition);
            animator.SetInteger("Speed", 1);
            agent.speed = 3;
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(pingPosition);
            animator.SetInteger("Speed", 2);
            agent.speed = 6;
        }
    }

    private void FollowPlayer()
    {
        float distance = Vector3.Distance(transform.position, ObejctToFollow.transform.position); // get the distance 

        if (distance < playerDist)
        {
            agent.isStopped = true;
            animator.SetInteger("Speed", 0);
        }
        else if (distance >= playerDist && distance < playerDist * 2.5)
        {
            agent.isStopped = false;
            agent.SetDestination(ObejctToFollow.transform.position);

            animator.SetInteger("Speed", 1);

            agent.speed = 3;
        }
        else if (distance > playerDist * 2.5)
        {
            agent.isStopped = false;

            agent.SetDestination(ObejctToFollow.transform.position);

            animator.SetInteger("Speed", 2);

            agent.speed = 6;
        }
    }

    public void Ping(Vector3 position)
    {
        isPinged = true;
        pingTimer = pingHoldTime;
        pingPosition = position;
        hasArrivedAtPing = false;
    }

    public void Dig()
    {
        
    }
}