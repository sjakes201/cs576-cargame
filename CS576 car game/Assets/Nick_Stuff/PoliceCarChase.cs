using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Script written by Nick

public class PoliceCarChase : MonoBehaviour
{
    public Transform player; // Reference to the player's position
    public float maxSpeed = 50f; // Max speed for the police car
    public float detectionRadius = 50f; // Radius within which the police car starts chasing the player
    public float stopChaseDist = 50f; // Cop will stop chasing if the player gets this far away
    private bool isChasing = false; // Flag to track if the cop is currently chasing
    public float minDistFromPlayer = 5f; // The minimum distance the cop can get to the player, to avoid weird collisions
    private Rigidbody playerCar; // Reference to the player's Rigidbody
    private float chaseTime = 0f; // Time the police car has been chasing
    public float chaseTimeLimit = 7.5f; // Police car gets fatigued after this time limit

    private bool isFatigued = false; // Flag to track if the cop is currently fatigued
    private float fatigueTimer = 0f; // Timer for fatigue slowdown duration
    public float fatigueDuration = 3f; // Determines how long the cop is fatigued

    public HelicopterFollow helicopter; // Reference to the helicopter

    private NavMeshAgent cop; // Reference to the police car's Nav Mesh Agent
    private AudioSource sirenAudio; // Reference to the police siren audio

    private ScoreManager scoreManager;
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Car");
        if (playerObject != null)
        {
            playerCar = playerObject.GetComponent<Rigidbody>();
            player = playerObject.transform;
        }

        GameObject helicopterObject = GameObject.FindGameObjectWithTag("Helicopter");
        if (helicopterObject != null) helicopter = helicopterObject.GetComponent<HelicopterFollow>();

        cop = GetComponent<NavMeshAgent>();
        cop.speed = 0;

        sirenAudio = GetComponent<AudioSource>();

        scoreManager = FindObjectOfType<ScoreManager>();
    }

    void Update()
    {
        float playerSpeed = Mathf.Abs(playerCar.velocity.magnitude);
        bool helicopterCanSeePlayer = helicopter != null && helicopter.CanSeePlayerPublic;

        // Check if the player is within the detection radius
        if (!isChasing && Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            StartChasing();
        }

        if(!isChasing && helicopterCanSeePlayer) StartChasing();

        // If chasing cop will move towards the player
        if (isChasing)
        {
            // If the player is too far, then stop chasing
            if (!helicopterCanSeePlayer && Vector3.Distance(transform.position, player.position) > stopChaseDist)
            {
                StopChasing();
                Debug.Log("Cop stopped chasing: Player is too far away.");
                return;
            }

            // Otherwise, continue chasing
            ChasePlayer(playerSpeed);
        }

        if (!isChasing && helicopterCanSeePlayer) StartChasing();

        // If chasing cop will move towards the player
        if (isChasing)
        {
            // If the player is too far, then stop chasing
            if (!helicopterCanSeePlayer && Vector3.Distance(transform.position, player.position) > stopChaseDist)
            {
                StopChasing();
                Debug.Log("Cop stopped chasing: Player is too far away.");
                return;
            }

            // Otherwise, continue chasing
            ChasePlayer(playerSpeed);
        }
    }

    // Start chasing the player
    private void StartChasing()
    {
        isChasing = true;
        Debug.Log("Cop starts chasing the player!");

        // Start playing the siren audio if it's not already playing
        if (sirenAudio != null && !sirenAudio.isPlaying)
        {
            sirenAudio.Play();
        }
    }

    // Stop chasing the player
    private void StopChasing()
    {
        isChasing = false;
        Debug.Log("Cop stopped chasing the player.");

        // Stop the siren audio
        if (sirenAudio != null && sirenAudio.isPlaying)
        {
            sirenAudio.Stop();
        }
    }

    // Move the police car towards the player
    private void ChasePlayer(float playerSpeed)
    {
        // Tracks chase time for fatigue
        chaseTime += Time.deltaTime;

        // Cop becomes fatigued after the time limit is reached (maybe make time limit random?)
        if (chaseTime > chaseTimeLimit && !isFatigued) StartFatigue();

        // Handle fatigue slowdown logic
        // Slow down player for the fatigueDuration (maybe make fatigueDuration random?)
        if (isFatigued)
        {
            fatigueTimer += Time.deltaTime;
            if (fatigueTimer >= fatigueDuration)
            {
                EndFatigue();
            }
        }

        // Stops the police car model from getting too close to the player's car model
        // Also, stops the chase
        if (Vector3.Distance(transform.position, player.position) < minDistFromPlayer)
        {
            StopChasing();          
            Debug.Log("Cop has caught the player.");
            scoreManager.EndGame();
            return; // Stop further movement and chase
        }

        // Calculates the minimum speed based on distance from player, but also ensures min speed is always at least 3f
        float dynamicMinSpeed = Mathf.Max(3f + Mathf.FloorToInt(Vector3.Distance(transform.position, player.position) / 10f), 3f);

        // Determines target position and speed
        float targetSpeed = Mathf.Min(playerSpeed + 1f, maxSpeed);

        // Apply random fatigue factor
        if (isFatigued) targetSpeed *= Random.Range(0.5f, 0.8f);

        cop.speed = Mathf.MoveTowards(cop.speed, Mathf.Max(targetSpeed, dynamicMinSpeed), Time.deltaTime);
        //Debug.Log("Cop speed: " + cop.speed * 3);

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

