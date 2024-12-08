using UnityEngine;

public class SpeedLimitZone : MonoBehaviour
{
    public float speedLimit = 50f; 
    private bool isCooldown = false;

    void OnTriggerStay(Collider other)
    {
        if (isCooldown || !other.CompareTag("Car")) return;

        CarController carController = other.GetComponent<CarController>();
        if (carController != null)
        {
            float carSpeed = Mathf.Abs(carController.currentSpeed);
            if (carSpeed > speedLimit)
            {
                float surplus = carSpeed - speedLimit;
                onSpeed(surplus);
                StartCoroutine(Cooldown());
            }
        }
    }

    private System.Collections.IEnumerator Cooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(5f);
        isCooldown = false;
    }

    public void onSpeed(float surplus)
    {
        Debug.Log($"Speeding! Surplus: {surplus}");
        // TODO failure logic with different levels of speeding
        if (surplus > 25)
        {
            // reckless driving
        } else if (surplus > 15)
        {
            // serious violation
        } else if (surplus > 10)
        {
            // moderate violation
        } else
        {
            // minor violation
        }
    }
}
