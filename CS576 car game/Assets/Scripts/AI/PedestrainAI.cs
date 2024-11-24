// PedestrianAI.cs
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class PedestrianAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float crossingSpeed = 1.2f;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float detectionRadius = 5f;

    [Header("Path Settings")]
    [SerializeField] private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    
    private NavMeshAgent agent;
    private Animator animator;
    private bool isCrossing = false;
    private bool isWaiting = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        // Set initial speed
        agent.speed = walkSpeed;
        
        // Set initial destination
        if (waypoints.Length > 0)
        {
            SetNextDestination();
        }
    }

    private void Update()
    {
        if (!agent.pathStatus == NavMeshPathStatus.PathComplete && !isWaiting)
        {
            // check if the agent has reached the destination
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                StartCoroutine(WaitAtWaypoint());
            }
        }

        // Update animator speed parameter
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }

        // Check for vehicles when crossing
        if (isCrossing)
        {
            CheckForVehicles();
        }
    }

    private IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        agent.isStopped = true;

        // Wait for a few seconds
        yield return new WaitForSeconds(waitTime);

        // Continue to the next waypoint
        SetNextDestination();
        agent.isStopped = false;
        isWaiting = false;
    }

    private void SetNextDestination()
    {
        if (waypoints.Length == 0) return;

        // Set the next waypoint as the destination
        agent.SetDestination(waypoints[currentWaypointIndex].position);

        // Update the waypoint index
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    private void CheckForVehicles()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("PlayerVehicle"))
            {
                // Get the vehicle's rigidbody
                Rigidbody vehicleRb = col.GetComponent<Rigidbody>();
                if (vehicleRb != null && vehicleRb.velocity.magnitude > 0.1f)
                {
                    // Stop and wait for the vehicle to pass
                    StopAndWait();
                }
            }
        }
    }

    private void StopAndWait()
    {
        if (!isWaiting)
        {
            StartCoroutine(WaitForVehicle());
        }
    }

    private IEnumerator WaitForVehicle()
    {
        isWaiting = true;
        agent.isStopped = true;

        // Wait until the vehicle is no longer nearby
        while (IsVehicleNearby())
        {
            yield return new WaitForSeconds(0.5f);
        }

        agent.isStopped = false;
        isWaiting = false;
    }

    private bool IsVehicleNearby()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("PlayerVehicle"))
            {
                return true;
            }
        }
        return false;
    }

    // Called when the pedestrian starts crossing the road
    public void StartCrossing()
    {
        isCrossing = true;
        agent.speed = crossingSpeed;
    }

    // Called when the pedestrian finishes crossing the road
    public void EndCrossing()
    {
        isCrossing = false;
        agent.speed = walkSpeed;
    }

    private void OnDrawGizmos()
    {
        // Draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}