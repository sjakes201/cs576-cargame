using UnityEngine;
using UnityEngine.SceneManagement; // 用於重新加載場景
using System.Collections.Generic;

public class StopDetection : MonoBehaviour
{
    private Dictionary<GameObject, bool> carStopStatus = new Dictionary<GameObject, bool>();
    public GameObject warningCubePrefab;
    public Vector3 cubeOffset = new Vector3(0, 1.5f, 20);
    public float displayDuration = 3.0f;

    private GameObject activeWarningCube;
    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        if (cameraTransform == null)
        {
            Debug.LogError("Main Camera not found! Make sure your Main Camera is tagged as 'MainCamera'.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PrometeoCarController carController = other.GetComponentInParent<PrometeoCarController>();
        if (carController != null)
        {
            if (!carStopStatus.ContainsKey(other.gameObject))
            {
                carStopStatus[other.gameObject] = false;
            }

            Debug.Log("Car entered trigger zone with speed: " + carController.carSpeed + " km/h");
        }
        else
        {
            Debug.Log("Non-car object entered trigger zone: " + other.name);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        PrometeoCarController carController = other.GetComponentInParent<PrometeoCarController>();
        if (carController != null)
        {
            float carSpeed = carController.carSpeed;

            if (carSpeed <= 0 && carStopStatus.ContainsKey(other.gameObject))
            {
                carStopStatus[other.gameObject] = true;
                Debug.Log("Car has stopped.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PrometeoCarController carController = other.GetComponentInParent<PrometeoCarController>();
        if (carController != null)
        {
            if (carStopStatus.ContainsKey(other.gameObject) && !carStopStatus[other.gameObject])
            {
                Debug.Log("Stop Sign Not Stopped!!");
                ShowWarningCube(cameraTransform);
            }
            else
            {
                Debug.Log("Car exited trigger zone after stopping.");
            }

            carStopStatus.Remove(other.gameObject);
        }
    }

    private void ShowWarningCube(Transform referenceTransform)
    {
        if (activeWarningCube == null)
        {
            Vector3 cubePosition = referenceTransform.position + referenceTransform.rotation * cubeOffset;
            activeWarningCube = Instantiate(warningCubePrefab, cubePosition, Quaternion.identity);

            StartCoroutine(HideWarningCubeAfterDelay());
        }
    }

    private void Update()
    {
        if (activeWarningCube != null && cameraTransform != null)
        {
            activeWarningCube.transform.position = cameraTransform.position + cameraTransform.rotation * cubeOffset;
            activeWarningCube.transform.rotation = cameraTransform.rotation;
        }
    }

    private System.Collections.IEnumerator HideWarningCubeAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        if (activeWarningCube != null)
        {
            Destroy(activeWarningCube);
            activeWarningCube = null;
        }

        // 在警告訊息消失後重置遊戲
        RestartGame();
    }

    private void RestartGame()
    {
        Debug.Log("Restarting Game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
