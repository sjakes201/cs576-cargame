using UnityEngine;

public class StopSignZone : MonoBehaviour
{
    public GameObject car; 
    private bool hasStopped = false; 
    private float carSpeed = 0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == car)
        {
            hasStopped = false; 
            Debug.Log("Entered Stop Zone. You must stop!");
        }
        else
        {
            Debug.Log("Not car");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == car)
        {
            CarController carController = car.GetComponent<CarController>();
            if (carController != null)
            {
                carSpeed = Mathf.Abs(carController.currentSpeed);
                if (carSpeed <= 0.1f) 
                {
                    hasStopped = true;
                    Debug.Log("Car has stopped in the zone.");
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == car)
        {
            if (!hasStopped) 
            {
                Debug.Log("Failed to stop at the stop sign!");
                onFailToStop(); 
            }
            else
            {
                Debug.Log("Successfully stopped at the stop sign!");
                onSuccessfulStop();
            }
        }
    }

    public void onFailToStop()
    {
        // TODO
    }

    public void onSuccessfulStop()
    {

    }
}
