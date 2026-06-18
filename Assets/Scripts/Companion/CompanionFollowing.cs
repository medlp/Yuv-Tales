using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CompanionFollowing: MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    [Header("Target")]
    public GameObject ObjectToFollow;

    [Header("Ping")]
    public Vector3 pingPosition;
    bool isPinged;
    private float playerDist;

    public event Action OnPingArrived;
    private bool hasArrivedAtPing;

    private bool isDigging = false;
    private bool hasPendingPing = false;

    private float pingDistance = 1;

    [Header("Digging")]
    [SerializeField] private float digDuration = 3f;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        playerDist = agent.stoppingDistance;
    }

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
        if (isDigging) return;

        agent.stoppingDistance = pingDistance;

        float distance = Vector3.Distance(transform.position, pingPosition);

        if (distance <= agent.stoppingDistance)
        {
            Dig();

            if (!hasArrivedAtPing)
            {
                hasArrivedAtPing = true;
                OnPingArrived?.Invoke();
            }
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
        agent.stoppingDistance = playerDist;

        float distance = Vector3.Distance(transform.position, ObjectToFollow.transform.position); 

        if (distance < playerDist)
        {
            agent.isStopped = true;
            animator.SetInteger("Speed", 0);
        }
        else if (distance >= playerDist && distance < playerDist * 2.5)
        {
            agent.isStopped = false;
            agent.SetDestination(ObjectToFollow.transform.position);

            animator.SetInteger("Speed", 1);

            agent.speed = 3;
        }
        else if (distance > playerDist * 2.5)
        {
            agent.isStopped = false;

            agent.SetDestination(ObjectToFollow.transform.position);

            animator.SetInteger("Speed", 2);

            agent.speed = 6;
        }
    }

    public void Ping(Vector3 position)
    {
        pingPosition = position;
        hasArrivedAtPing = false;

        if (isDigging)
        {

            hasPendingPing = true;
        }
        else
        {
            isPinged = true;
        }
    }

    public void Dig()
    {
        if (isDigging) return;

        StartCoroutine(DigRoutine());
    }

    private IEnumerator DigRoutine()
    {
        isDigging = true;
        agent.isStopped = true;
        animator.SetInteger("Speed", 0);

        DigZone currentZone = null;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent<DigZone>(out DigZone zone))
            {
                if (!zone.IsAlreadyDug)
                {
                    currentZone = zone;
                    break;
                }
            }
        }

        float elapsed = 0f;
        float rotationSpeed = 360f;

        while (elapsed < digDuration)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (currentZone != null)
        {
            currentZone.OnDigComplete(); 
        }

        isDigging = false;

        if (hasPendingPing)
        {
            hasPendingPing = false; 
            isPinged = true;        
        }
        else
        {
            isPinged = false;
        }
    }
}