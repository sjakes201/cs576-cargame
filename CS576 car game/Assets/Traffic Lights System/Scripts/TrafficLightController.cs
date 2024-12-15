using UnityEngine;
using System.Collections;

public class TrafficLightController : MonoBehaviour
{
    public GameObject redLight;      // 红灯球
    public GameObject yellowLight;   // 黄灯球
    public GameObject greenLight;    // 绿灯球

    public float redDuration = 5.0f;
    public float yellowDuration = 2.0f;
    public float greenDuration = 5.0f;

    private string currentLight = "Red"; // 用於存儲當前燈號狀態

    void Start()
    {
        redLight.SetActive(false);
        yellowLight.SetActive(false);
        greenLight.SetActive(false);
        
        StartCoroutine(TrafficLightCycle());
    }

    IEnumerator TrafficLightCycle()
    {
        while (true) 
        {
            // 红灯亮
            Debug.Log("红灯亮");
            SetLightState("Red");
            yield return new WaitForSeconds(redDuration);

            // 绿灯亮
            Debug.Log("绿灯亮");
            SetLightState("Green");
            yield return new WaitForSeconds(greenDuration);

            // 黄灯亮
            Debug.Log("黄灯亮");
            SetLightState("Yellow");
            yield return new WaitForSeconds(yellowDuration);
        }
    }

    private void SetLightState(string lightState)
    {
        currentLight = lightState;

        // 根據燈號啟用或禁用燈球
        redLight.SetActive(lightState == "Red");
        yellowLight.SetActive(lightState == "Yellow");
        greenLight.SetActive(lightState == "Green");
    }

    public string GetCurrentLightState()
    {
        return currentLight;
    }
}
