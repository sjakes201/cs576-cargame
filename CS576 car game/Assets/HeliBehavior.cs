using UnityEngine;

public class HelicopterFollow : MonoBehaviour
{
    public Transform player;             // The player's transform (Car)
    public float followHeight = 20f;     // Height to follow at
    public float followSpeed = 3f;       // Base speed while following
    public float wanderSpeed = 3f;       // Speed while wandering
    public Vector3 offset;               // Positional offset
    public float wanderRadius = 30f;     // Radius for wandering
    public float raycastDistance = 1000f; // Max distance to check for line of sight
    public float lostPlayerTimeout = 5f; // Time to continue in last known direction

    private Vector3 wanderTarget;
    private Vector3 previousPosition;
    private Vector3 lastKnownPosition;
    private bool hasSeenPlayer = false;  // Track if the player has ever been seen
    private float timeSinceLastSeen = 0f;
    private float currentFollowSpeed;
    private float fixedY; // Fixed Y-coordinate to lock movement height

    void Start()
    {
        fixedY = transform.position.y; // Store the initial Y position

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Car");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player with tag 'Car' not found!");
            }
        }

        SetRandomWanderTarget();
        previousPosition = transform.position;
        currentFollowSpeed = followSpeed;
    }

    void Update()
    {
        if (player == null) return;

        bool seesPlayer = CanSeePlayer();

        if (seesPlayer)
        {
            hasSeenPlayer = true; // Mark that the player has been seen at least once
            FollowPlayer();
            TiltDownSmoothly();
            currentFollowSpeed = followSpeed * 1.25f;
            timeSinceLastSeen = 0f;
            lastKnownPosition = player.position; // Store player's position
        }
        else if (hasSeenPlayer && timeSinceLastSeen < lostPlayerTimeout) // Only move to lastKnown if player has been seen
        {
            ContinueInLastKnownDirection();
            TiltDownSmoothly();
            timeSinceLastSeen += Time.deltaTime;
        }
        else
        {
            WanderAround();
            ResetTiltSmoothly();
            currentFollowSpeed = followSpeed;
        }

        RotateTowardsMovement();
    }

    bool CanSeePlayer()
    {
        Vector3 rayOrigin = transform.position - new Vector3(0f, 1f, 0f);
        Vector3 directionToPlayer = (player.position - rayOrigin).normalized;
        float distanceToPlayer = Vector3.Distance(rayOrigin, player.position);

        Debug.DrawRay(rayOrigin, directionToPlayer * distanceToPlayer, Color.yellow);

        if (Physics.Raycast(rayOrigin, directionToPlayer, out RaycastHit hit, raycastDistance))
        {
            if (hit.collider.CompareTag("Car"))
            {
                Debug.DrawLine(rayOrigin, hit.point, Color.green);
                return true;
            }
        }

        Debug.DrawRay(rayOrigin, directionToPlayer * raycastDistance, Color.red);
        return false;
    }

    void FollowPlayer()
    {
        Vector3 targetPosition = new Vector3(player.position.x, fixedY, player.position.z) + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, currentFollowSpeed * Time.deltaTime);
    }

    void ContinueInLastKnownDirection()
    {
        Vector3 targetPosition = new Vector3(lastKnownPosition.x, fixedY, lastKnownPosition.z) + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, currentFollowSpeed * Time.deltaTime);
    }

    void WanderAround()
    {
        if (Vector3.Distance(transform.position, wanderTarget) < 5f)
        {
            SetRandomWanderTarget();
        }

        Vector3 targetPosition = new Vector3(wanderTarget.x, fixedY, wanderTarget.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, wanderSpeed * Time.deltaTime);
    }

    void SetRandomWanderTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        wanderTarget = new Vector3(randomCircle.x + transform.position.x, fixedY, randomCircle.y + transform.position.z);
    }

    void RotateTowardsMovement()
    {
        Vector3 movementDirection = (transform.position - previousPosition).normalized;

        if (movementDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }

        previousPosition = transform.position;
    }

    void TiltDownSmoothly()
    {
        Quaternion targetTilt = Quaternion.Euler(20f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetTilt, Time.deltaTime * 2f);
    }

    void ResetTiltSmoothly()
    {
        Quaternion resetTilt = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, resetTilt, Time.deltaTime * 2f);
    }
}
