using UnityEngine;
using UnityEngine.SceneManagement; // 用於重新加載場景
using System.Collections;

public class TrafficLightDetectionZone : MonoBehaviour
{
    public TrafficLightController trafficLightController; // 引用 TrafficLightController
    public GameObject warningCubePrefab; // 預製件，用於生成警告訊息的立方體
    public Vector3 cubeOffset = new Vector3(0, 1.5f, 20); // 相對於 Main Camera 的偏移位置
    public float displayDuration = 3.0f; // 警告訊息的持續時間

    private GameObject activeWarningCube; // 當前的警告立方體
    private Transform cameraTransform; // 用於記錄 Main Camera 的 Transform

    private void Start()
    {
        // 在場景中找到 Main Camera 並記錄其 Transform
        cameraTransform = Camera.main.transform;
        if (cameraTransform == null)
        {
            Debug.LogError("Main Camera not found! Make sure your Main Camera is tagged as 'MainCamera'.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Detected: {other.name} in {other.transform.root.name}");
        
        // 確保是 Player 或其根物件進入區域
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            Debug.Log("Entering the intersection");
            
            // 檢查燈號狀態
            string lightState = trafficLightController.GetCurrentLightState();

            if (lightState == "Red")
            {
                // 顯示警告立方體
                ShowWarningCube(cameraTransform);
                Debug.Log("Do not go on red !!!");
            }
        }
        else
        {
            Debug.Log($"Ignored object: {other.name} (Tag: {other.tag})");
        }
    }

    private void ShowWarningCube(Transform referenceTransform)
    {
        if (activeWarningCube == null) // 確保同一時間只有一個立方體
        {
            // 在 Main Camera 前方生成警告立方體
            Vector3 cubePosition = referenceTransform.position + referenceTransform.rotation * cubeOffset;
            activeWarningCube = Instantiate(warningCubePrefab, cubePosition, Quaternion.identity);

            // 啟動協程，延遲刪除警告立方體並重置遊戲
            StartCoroutine(HideWarningCubeAndRestartAfterDelay());
        }
    }

    private void Update()
    {
        // 如果警告立方體存在，讓它跟隨 Main Camera 的位置和旋轉
        if (activeWarningCube != null && cameraTransform != null)
        {
            activeWarningCube.transform.position = cameraTransform.position + cameraTransform.rotation * cubeOffset;
            activeWarningCube.transform.rotation = cameraTransform.rotation; // 跟隨旋轉
        }
    }

    private IEnumerator HideWarningCubeAndRestartAfterDelay()
    {
        // 等待警告訊息的顯示時間
        yield return new WaitForSeconds(displayDuration);

        // 刪除警告立方體
        if (activeWarningCube != null)
        {
            Destroy(activeWarningCube);
            activeWarningCube = null;
        }

        // 重置遊戲
        RestartGame();
    }

    private void RestartGame()
    {
        Debug.Log("Restarting Game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
