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

        // 确保角色在 NavMesh 上
        StartCoroutine(InitializeNavMeshAgent());
    }

    private IEnumerator InitializeNavMeshAgent()
    {
        // 等待一帧确保所有组件都初始化完成
        yield return new WaitForEndOfFrame();

        // 尝试将角色放置到最近的 NavMesh 上
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            
            // 现在可以安全地设置目标点
            if (waypoints != null && waypoints.Length > 0)
            {
                SetNextDestination();
            }
            else
            {
                Debug.LogWarning("No waypoints assigned to " + gameObject.name);
            }
        }
        else
        {
            Debug.LogError("Could not find valid NavMesh position near " + gameObject.name);
        }
    }

    private void Update()
    {
        if (!agent.isOnNavMesh)
            return;

        if (agent.pathStatus == NavMeshPathStatus.PathComplete && isWaiting)
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
        if (!agent.isOnNavMesh)
            yield break;

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

    private IEnumerator WaitAtWaypoint()
    {
        if (!agent.isOnNavMesh)
            yield break;

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
        if (!agent.isOnNavMesh || waypoints == null || waypoints.Length == 0) 
            return;

        if (waypoints[currentWaypointIndex] == null)
        {
            Debug.LogError("Waypoint at index " + currentWaypointIndex + " is null!");
            return;
        }

        // Find valid position on NavMesh near waypoint
        NavMeshHit hit;
        Vector3 targetPos = waypoints[currentWaypointIndex].position;
        if (NavMesh.SamplePosition(targetPos, out hit, 5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            Debug.LogWarning("Could not find valid NavMesh position for waypoint " + currentWaypointIndex);
        }

        // Update the waypoint index
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
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

    // 辅助方法：检查是否在 NavMesh 上
    public bool IsOnNavMesh()
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(transform.position, out hit, 0.1f, NavMesh.AllAreas);
    }

    private void OnDrawGizmos()
    {
        // Draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw waypoint connections
        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    Vector3 current = waypoints[i].position;
                    Vector3 next = waypoints[(i + 1) % waypoints.Length].position;
                    Gizmos.DrawLine(current, next);
                }
            }
        }
    }
}