using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Following script made by Nick

public class speedZone : MonoBehaviour
{
    private Rigidbody carRigidbody; // Reference to the car's Rigidbody
    private bool isSpeeding = false; // Tracks whether the player is currently speeding
    private bool isInZone = false; // Tracks if the car is in the speed zone
    public float maxSpeed = 30f; // Maximum speed allowed in this zone (configurable per zone)

    private ScoreManager scoreManager;

    private void Start()
    {
        GameObject car = GameObject.FindGameObjectWithTag("Player");
        carRigidbody = car.GetComponent<Rigidbody>();

    }

    private void Update()
    {
        if (isInZone && carRigidbody != null)
        {
            // Updates speed
            float speed = carRigidbody.velocity.magnitude;

            // Speeding
            if (speed > maxSpeed && !isSpeeding)
            {
                Debug.Log("Too fast in the speed zone! Max allowed speed: " + maxSpeed + " m/s");
                isSpeeding = true;
                scoreManager.StartSpeeding();
            }

            // Stopped speeding
            else if (speed <= maxSpeed && isSpeeding)
            {
                Debug.Log("Returned to appropriate speed within the speed zone.");
                isSpeeding = false;
                scoreManager.StopSpeeding();
            }
        }

        scoreManager = FindObjectOfType<ScoreManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checks if the car entered the speed zone
        if (other.CompareTag("Player"))
        {
            Debug.Log("Car entered the speed zone.");
            isInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Checks if the car has exited the speed zone
        if (other.CompareTag("Player"))
        {
            Debug.Log("Car exited the speed zone.");
            isInZone = false;
            isSpeeding = false;
            scoreManager.StopSpeeding();
        }
    }
}
