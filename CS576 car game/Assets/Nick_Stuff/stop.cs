using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Following script made by Nick

public class StopZone : MonoBehaviour
{
    private bool hasStopped = false;
    private bool isInZone = false;

    private void OnTriggerEnter(Collider other)
    {
        // Checks if the car entered the zone
        if (other.CompareTag("Player")) 
        {
            Debug.Log("Car entered the stop zone.");
            isInZone = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Continuously checks if the car is in the zone and has stopped
        if (isInZone && other.CompareTag("Player"))
        {
            PrometeoCarController carController = other.GetComponent<PrometeoCarController>();
            //Debug.Log("Car speed: " + carController.carSpeed);
            if (carController.carSpeed < 0.01f && carController.carSpeed > -1f)
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

            else Debug.Log("You didn't come to a complete stop!");
 
            hasStopped = false;
            isInZone = false;
        }
    }
}
