using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Following script made by Nick

public class StopZone : MonoBehaviour
{
    private bool hasStopped = false; // Tracks if the player has stopped
    private bool isInZone = false; // Tracks if the player is in the stop zone
    private float timeInZone = 0f; // Tracks how long the car has been in the zone
    public float stopTimeThreshold = 2f; // Time required to count as a valid stop

    private ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    private void Update()
    {
        if (isInZone && !hasStopped)
        {
            timeInZone += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checks if the car entered the zone
        if (other.CompareTag("Player")) 
        {
            Debug.Log("Car entered the stop zone.");
            isInZone = true;
            hasStopped = false;
            timeInZone = 0f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Continuously checks if the car is in the zone and has stopped
        if (isInZone && other.CompareTag("Player"))
        {
            PrometeoCarController carController = other.GetComponent<PrometeoCarController>();
            if (carController.carSpeed < 0.01f && carController.carSpeed > -1f && timeInZone >= stopTimeThreshold)
            {
                hasStopped = true; 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the car leaves the zone
        if (other.CompareTag("Player"))
        {
            if (hasStopped) Debug.Log("Stopped correctly.");

            else 
            {
                Debug.Log("You didn't come to a complete stop!");
                scoreManager?.failedToStop();
            }
 
            hasStopped = false;
            isInZone = false;
        }
    }
}
