using UnityEngine;

public class StopLightZone : MonoBehaviour
{
    public StopLight stopLight;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            Debug.Log("Car entered the stoplight zone.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            if (stopLight.GetCurrentLightColor() == StopLight.LightColor.Red)
            {
                Debug.Log("Car left the zone during red light! Failed to stop!");
                OnBreakRules();
            }
            else
            {
                Debug.Log("Car left the zone successfully.");
                OnObeyRules();
            }
        }
    }

    public void OnBreakRules()
    {
        // TODO failure logic
    }

    public void OnObeyRules()
    {
        // TODO success logic
    }
}
