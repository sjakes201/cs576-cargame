using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script written by Nick

public class PoliceCarChase : MonoBehaviour
{
    public Transform player; // Reference to the player's position
    public float maxSpeed = 50f; // Max speed for the police car
    public float speeding = 20f; // Cop will begin chasing once the player passes this speed
    public float stopChaseDist = 50f; // Cop will stop chasing if the player gets this far away
    private bool isChasing = false; // Flag to track if the cop is currently chasing
    public float minDistFromPlayer = 5f; // The minimum distance the cop can get to the player, to avoid weird collisions
    private Rigidbody playerCar; // Reference to the player's Rigidbody
    private float currSpeed = 0f; // Current speed of the police car
    private float chaseTime = 0f; // Time the police car has been chasing
    public float chaseTimeLimit = 7.5f; // Police car gets fatigued after this time limit

    private bool isFatigued = false; // Flag to track if the cop is currently fatigued
    private float fatigueTimer = 0f; // Timer for fatigue slowdown duration
    public float fatigueDuration = 3f; // Determines how long the cop is fatigued

    void Start()
    {
        playerCar = GameObject.FindGameObjectWithTag("Car").GetComponent<Rigidbody>();
    }

    void Update()
    {
        float playerSpeed = Mathf.Abs(playerCar.velocity.magnitude);

        // If the player is speeding, then start chasing
        if (playerSpeed * 3 > speeding && !isChasing) StartChasing();

        // If chasing cop will move towards the player
        if (isChasing)
        {
            // If the player is too far, then stop chasing
            if (Vector3.Distance(transform.position, player.position) > stopChaseDist)
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
    }

    // Stop chasing the player
    private void StopChasing()
    {
        isChasing = false;
        Debug.Log("Cop stopped chasing the player.");
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
            return; // Stop further movement and chase
        }

        // Target speed is calculated to be slightly faster than player, but no faster than maxSpeed
        float targetSpeed = Mathf.Min(playerSpeed + 2f, maxSpeed);

        // Randomly slows the cop down if fatigued
        if (isFatigued)
        {
            targetSpeed *= Random.Range(0.5f, 0.8f); // Apply random fatigue factor
        }

        // Gradually adjust current speed towards the target speed
        currSpeed = Mathf.MoveTowards(currSpeed, targetSpeed, Time.deltaTime);
        //Debug.Log("Target Speed: " + targetSpeed);
        //Debug.Log("Cop Current Speed: " + currSpeed * 3);

        // Simple player's next position prediction
        Vector3 posPrediction = player.position + playerCar.velocity.normalized * 3f;

        // Moves cop towards the predicted position
        Vector3 playerDirection = (posPrediction - transform.position).normalized;
        transform.position += playerDirection * currSpeed * Time.deltaTime;

        // Rotates police car model toward the prediction position
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerDirection), Time.deltaTime * 2f);
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

