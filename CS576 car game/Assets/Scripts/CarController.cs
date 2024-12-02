using UnityEngine;

public class CarController : MonoBehaviour
{
    public float accelerationRate = 10f;
    public float decelerationRate = 15f;
    public float naturalDeceleration = 2f; // Decline in speed over time due to friction
    public float maxSpeed = 50f;
    public float maxReverseSpeed = -20f;
    public float maxWheelAngle = 30f;
    public float turnSpeed = 2f;

    public float currentSpeed = 0f;
    private float currentWheelAngle = 0f;

    void Update()
    {
        HandleInput();
        MoveCar();
    }

    private void HandleInput()
    {
        // Accelerate when W is pressed
        if (Input.GetKey(KeyCode.W))
        {
            currentSpeed += accelerationRate * Time.deltaTime;
        }

        // Decelerate (or reverse) when S is pressed
        if (Input.GetKey(KeyCode.S))
        {
            currentSpeed -= decelerationRate * Time.deltaTime;
        }

        // Apply natural deceleration (friction) when no input
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            if (currentSpeed > 0)
                currentSpeed -= naturalDeceleration * Time.deltaTime;
            else if (currentSpeed < 0)
                currentSpeed += naturalDeceleration * Time.deltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, maxReverseSpeed, maxSpeed);

        // Handle wheel (not car) rotation 
        if (Input.GetKey(KeyCode.A))
        {
            currentWheelAngle = Mathf.Clamp(currentWheelAngle - turnSpeed, -maxWheelAngle, maxWheelAngle);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            currentWheelAngle = Mathf.Clamp(currentWheelAngle + turnSpeed, -maxWheelAngle, maxWheelAngle);
        }
        else
        {
            currentWheelAngle = Mathf.Lerp(currentWheelAngle, 0f, Time.deltaTime * 5f);
        }
    }

    private void MoveCar()
    {
        Vector3 forwardMovement = transform.forward * currentSpeed * Time.deltaTime;
        transform.position += new Vector3(forwardMovement.x, 0f, forwardMovement.z);

        // Rotate the car based on wheel angle only when moving
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float turnAmount = (currentWheelAngle / maxWheelAngle) * (currentSpeed / maxSpeed) * turnSpeed;
            transform.Rotate(0f, turnAmount, 0f);
        }
    }
}
