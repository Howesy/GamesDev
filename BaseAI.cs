using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private Transform player;

    [SerializeField] private LayerMask whatIsGround, whatIsPlayer, obstructionMask;

    [SerializeField] private float walkPointRange;

    private Vector3 _walkPoint;

    private bool walkPointSet;

    public float sightRange, attackRange;

    public float radius;
    [Range(0, 360)]
    public float angle;

    public GameObject playerRef;

    public bool playerInSightRange, playerInAttackRange,
        canSeePlayer, chasePlayer;



    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

    }

    void Start()
    {
        StartCoroutine(FOVRoutine());

    }

    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        MainAILogic();

    }
    private void MainAILogic()
    {
        if (chasePlayer && !playerInAttackRange)
        {
            agent.speed = 4.5f;
            ChasePlayer();
        }

        if (chasePlayer && playerInAttackRange)
        {
            AttackPlayer();
        }
        if (playerInAttackRange)
        {
            chasePlayer = true;
        }

        if (!chasePlayer && !playerInAttackRange)
        {
            Patrol();
            agent.speed = 3.5f;
        }

        if (!canSeePlayer && !playerInSightRange)
        {

            chasePlayer = false;
        }

    }


    private IEnumerator FOVRoutine()
    {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, whatIsPlayer);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;

            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                    chasePlayer = true;
                }


            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }

    private void ChasePlayer()
    {
        //guard destination will always be player's position to chase.
        agent.SetDestination(player.position);

    }

    private void AttackPlayer()
    {
        //Once in attack range, guard will stop
        agent.SetDestination(transform.position);
        agent.speed = 0;
        var lookPos = player.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 15f);
        
    }

    private void Patrol()
    {
        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(_walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - _walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in given range to patrol
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        _walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(_walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;

    }


}
