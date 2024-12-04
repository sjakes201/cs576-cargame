using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class speedTracker : MonoBehaviour
{
    public TextMeshProUGUI speedText;
    private Rigidbody carRigidbody;

    void Start()
    {
        GameObject car = GameObject.FindGameObjectWithTag("Car");
        carRigidbody = car.GetComponent<Rigidbody>();
    }

    void Update()
    {
        float speed = carRigidbody.velocity.magnitude * 3;
        speedText.text = "Speed: " + speed.ToString("F1") + " mph";
    }
}
