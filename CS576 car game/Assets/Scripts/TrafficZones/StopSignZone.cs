using UnityEngine;
using UnityEngine.SceneManagement;

public class StopSignZone : MonoBehaviour
{
    private bool hasStopped = false;
    private float carSpeed = 0f;
    private ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car")) 
        {
            hasStopped = false;
            Debug.Log("Entered Stop Zone. You must stop!");
        }
        else
        {
            Debug.Log("Not a car");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Car")) 
        {
            PrometeoCarController carController = other.GetComponent<PrometeoCarController>();
            if (carController != null)
            {
                carSpeed = Mathf.Abs(carController.carSpeed);
                if (carSpeed <= 1f)
                {
                    hasStopped = true;
                    Debug.Log("Car has stopped in the zone.");
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car")) 
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
        // TODO record failure     
        //scoreManager.DeductPoints(10);
        scoreManager.failToStop(0);
    }

    public void onSuccessfulStop()
    {
        // TODO record success
        scoreManager.AddPoints(5, 0);
    }
}
