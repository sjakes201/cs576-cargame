using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PoliceCarChase : MonoBehaviour
{
    public Transform player; // Reference to the player's position
    public float maxSpeed = 50f; // Max speed for the police car
    public float detectionRadius = 75f; // Radius within which the police car starts chasing the player
    public float stopChaseDist = 50f; // Cop will stop chasing if the player gets this far away
    private bool isChasing = false; // Flag to track if the cop is currently chasing
    public float minDistFromPlayer = 5f; // Minimum distance to avoid collisions
    private Rigidbody playerCar; // Reference to the player's Rigidbody

    private float chaseTime = 0f;
    public float chaseTimeLimit = 7.5f;
    private bool isFatigued = false;
    private float fatigueTimer = 0f;
    public float fatigueDuration = 3f;

    private NavMeshAgent cop; // Reference to the police car's Nav Mesh Agent
    private AudioSource sirenAudio; // Police siren audio
    private ScoreManager scoreManager;

    private List<HelicopterFollow> helicopters = new List<HelicopterFollow>(); // List of all helicopters
    private float refreshInterval = 10f; // Refresh interval for helicopter list

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Car");
        if (playerObject != null)
        {
            playerCar = playerObject.GetComponent<Rigidbody>();
            player = playerObject.transform;
        }

        cop = GetComponent<NavMeshAgent>();
        cop.speed = 0;

        sirenAudio = GetComponent<AudioSource>();
        scoreManager = FindObjectOfType<ScoreManager>();

        StartCoroutine(RefreshHelicopters()); // Start refreshing the helicopter list
    }

    void Update()
    {
        float playerSpeed = Mathf.Abs(playerCar.velocity.magnitude);

        // Check if any helicopter can see the player
        bool anyHelicopterCanSeePlayer = HelicopterSeesPlayer();

        // Check if the player is within the detection radius or seen by any helicopter
        if (!isChasing && (Vector3.Distance(transform.position, player.position) <= detectionRadius || anyHelicopterCanSeePlayer))
        {
            StartChasing();
        }

        // Stop chase if the player is too far and no helicopter can see the player
        if (isChasing && !anyHelicopterCanSeePlayer && Vector3.Distance(transform.position, player.position) > stopChaseDist)
        {
            StopChasing();
            return;
        }

        // Continue chasing
        if (isChasing)
        {
            ChasePlayer(playerSpeed);
        }
    }

    // Coroutine to refresh the helicopter list every 10 seconds
    IEnumerator RefreshHelicopters()
    {
        while (true)
        {
            helicopters.Clear();
            GameObject[] helicopterObjects = GameObject.FindGameObjectsWithTag("Helicopter");
            foreach (GameObject obj in helicopterObjects)
            {
                HelicopterFollow helicopter = obj.GetComponent<HelicopterFollow>();
                if (helicopter != null)
                {
                    helicopters.Add(helicopter);
                }
            }
            yield return new WaitForSeconds(refreshInterval);
        }
    }

    // Check if any helicopter can see the player
    private bool HelicopterSeesPlayer()
    {
        foreach (HelicopterFollow helicopter in helicopters)
        {
            if (helicopter.CanSeePlayerPublic)
            {
                return true;
            }
        }
        return false;
    }

    private void StartChasing()
    {
        Debug.Log("This police officer sees you");
        isChasing = true;
        Debug.Log("Cop starts chasing the player!");
        if (sirenAudio != null && !sirenAudio.isPlaying)
        {
            sirenAudio.Play();
        }
    }

    private void StopChasing()
    {
        isChasing = false;
        Debug.Log("Cop stopped chasing the player.");
        if (sirenAudio != null && sirenAudio.isPlaying)
        {
            sirenAudio.Stop();
        }
    }

    private void ChasePlayer(float playerSpeed)
    {
        chaseTime += Time.deltaTime;

        if (chaseTime > chaseTimeLimit && !isFatigued) StartFatigue();

        if (isFatigued)
        {
            fatigueTimer += Time.deltaTime;
            if (fatigueTimer >= fatigueDuration) EndFatigue();
        }

        if (Vector3.Distance(transform.position, player.position) < minDistFromPlayer)
        {
            StopChasing();
            Debug.Log("Cop has caught the player.");
            scoreManager.EndGame();
            return;
        }

        float dynamicMinSpeed = Mathf.Max(3f + Mathf.FloorToInt(Vector3.Distance(transform.position, player.position) / 10f), 3f);
        float targetSpeed = Mathf.Min(playerSpeed + 1f, maxSpeed);
        if (isFatigued) targetSpeed *= Random.Range(0.5f, 0.8f);

        cop.speed = Mathf.MoveTowards(cop.speed, Mathf.Max(targetSpeed, dynamicMinSpeed), Time.deltaTime);
        cop.SetDestination(player.position);
    }

    private void StartFatigue()
    {
        isFatigued = true;
        fatigueTimer = 0f;
        Debug.Log("Cop is fatigued!");
    }

    private void EndFatigue()
    {
        isFatigued = false;
        fatigueTimer = 0f;
        chaseTime = 0;
        Debug.Log("Cop has recovered from fatigue");
    }
}
