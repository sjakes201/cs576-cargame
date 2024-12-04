// TrafficAI.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrafficAI : MonoBehaviour
{
    [Header("Vehicle Settings")]
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float accelerationSpeed = 10f;
    [SerializeField] private float brakingSpeed = 15f;
    [SerializeField] private float steeringSpeed = 30f;
    [SerializeField] private float detectionDistance = 20f;
    [SerializeField] private LayerMask detectionLayers;

    [Header("Path Settings")]
    [SerializeField] private Transform[] pathPoints;
    private int currentPathIndex = 0;

    private Rigidbody rb;
    private float currentSpeed = 0f;
    private bool isBraking = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        if (pathPoints.Length == 0) return;

        // get the target point
        Vector3 targetPoint = pathPoints[currentPathIndex].position;
        
        // check for obstacles
        CheckObstacles();

        // update movement
        UpdateMovement(targetPoint);

        // check waypoint distance
        CheckWaypointDistance(targetPoint);
    }

    private void CheckObstacles()
    {
        RaycastHit hit;
        // forward raycast detection
        if (Physics.Raycast(transform.position, transform.forward, out hit, detectionDistance, detectionLayers))
        {
            // calculate the distance ratio
            float distanceRatio = hit.distance / detectionDistance;
            isBraking = true;
            currentSpeed = Mathf.Lerp(0, maxSpeed, distanceRatio);
        }
        else
        {
            isBraking = false;
        }

        // right raycast detection
        Vector3 rightOffset = transform.right * 2f;
        if (Physics.Raycast(transform.position + rightOffset, transform.forward, detectionDistance, detectionLayers))
        {
            // To Do: Implement lane changing logic
        }
    }

    private void UpdateMovement(Vector3 targetPoint)
    {
        // calculate the direction to the target
        Vector3 directionToTarget = (targetPoint - transform.position).normalized;
        
        // calculate the angle to turn
        float targetAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
        float currentAngle = transform.eulerAngles.y;
        float angleToTurn = Mathf.MoveTowardsAngle(currentAngle, targetAngle, steeringSpeed * Time.fixedDeltaTime);

        // apply rotation
        transform.rotation = Quaternion.Euler(0, angleToTurn, 0);

        // calculate speed
        if (!isBraking)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, accelerationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, brakingSpeed * Time.fixedDeltaTime);
        }

        // apply Movement
        rb.velocity = transform.forward * currentSpeed;
    }

    private void CheckWaypointDistance(Vector3 targetPoint)
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPoint);
        
        if (distanceToTarget < 2f)
        {
            // update the current path index
            currentPathIndex = (currentPathIndex + 1) % pathPoints.Length;
        }
    }

    // Called by the traffic light
    public void OnTrafficSignal(bool shouldStop)
    {
        isBraking = shouldStop;
    }

    private void OnDrawGizmos()
    {
        // draw detection distance
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * detectionDistance);
    }
}