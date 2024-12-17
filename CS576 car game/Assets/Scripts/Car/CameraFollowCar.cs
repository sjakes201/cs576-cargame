using UnityEngine;

public class CameraFollowJake : MonoBehaviour
{
    public Transform carTransform; // The car to follow
    public Vector3 offset = new Vector3(0f, 5f, -10f); // Offset from the car
    public float followSpeed = 5f; // The following speed, i.e. how precisely it tracks the car which matters during turning and acceleration
    private Vector3 Velocity = Vector3.zero;

    void FixedUpdate()
    {
        if (carTransform != null)
        {
            Vector3 targetPosition = carTransform.position + carTransform.rotation * offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref Velocity, 0.1f);
            transform.LookAt(carTransform);
        }
    }
}
