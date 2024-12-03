using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

// Following script made by Nick

public class SpeedMonitor : MonoBehaviour
{
    private Rigidbody carRigidbody; // Reference to the car's Rigidbody
    private bool isSpeeding = false; // Tracks whether the player is currently speeding
    public Text speedText; // Reference to the speed UI text element

    private void Start()
    {
        GameObject car = GameObject.FindGameObjectWithTag("Player");
        carRigidbody = car.GetComponent<Rigidbody>();
            
    }

    private void Update()
    {
        if (carRigidbody != null)
        {
            // Updates speed
            float speed = carRigidbody.velocity.magnitude;

            // Updates the UI speed text
            speedText.text = "Speed: " + speed.ToString("F1") + " m/s";

            // Speeding
            if (speed > 500f && !isSpeeding)
            {
                Debug.Log("Too fast!");
                isSpeeding = true;
            }

            // Stopped speeding
            else if (speed <= 500f && isSpeeding)
            {
                Debug.Log("Returned to appropriate speed.");
                isSpeeding = false;
            }
        }
    }
}
