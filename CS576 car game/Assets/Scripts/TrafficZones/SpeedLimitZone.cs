using UnityEngine;
using UnityEngine.SceneManagement;

public class SpeedLimitZone : MonoBehaviour
{
    public float speedLimit = 50f; 
    private bool isCooldown = false;
    private ScoreManager scoreManager;

    private Rigidbody carRigidbody;

    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        GameObject car = GameObject.FindGameObjectWithTag("Car");
        carRigidbody = car.GetComponent<Rigidbody>();        
    }

    void OnTriggerStay(Collider other)
    {
        if (isCooldown || !other.CompareTag("Car")) return;

        PrometeoCarController carController = other.GetComponent<PrometeoCarController>();
        if (carController != null)
        {
            //float carSpeed = Mathf.Abs(carController.carSpeed);
            float carSpeed = carRigidbody.velocity.magnitude * 3;
            if (carSpeed > speedLimit)
            {
                float surplus = carSpeed - speedLimit;
                onSpeed(surplus);
                Debug.Log("Speed: " + carSpeed);
                StartCoroutine(Cooldown());
            }
        }
    }

    private System.Collections.IEnumerator Cooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(3f);
        isCooldown = false;
    }

    public void onSpeed(float surplus)
    {
        Debug.Log($"Speeding! Surplus: {surplus}");
        // TODO failure logic with different levels of speeding
        if (surplus > 25)
        {
            scoreManager.DeductPoints(30);
            Debug.Log("Reckless driving violation. -30 points.");
            // reckless driving
        } else if (surplus > 15)
        {
            scoreManager.DeductPoints(20);
            Debug.Log("Serious speeding violation. -20 points.");
            // serious violation
        } else if (surplus > 10)
        {
            scoreManager.DeductPoints(10);
            Debug.Log("Moderate speeding violation. -10 points.");
            // moderate violation
        } else
        {
            scoreManager.DeductPoints(5);
            Debug.Log("Minor speeding violation. -5 points.");
            // minor violation
        }
    }
}
