using UnityEngine;
using UnityEngine.SceneManagement; // 用於場景切換

public class FinishDetectionZone : MonoBehaviour
{
    public string targetSceneName = "MainMenu"; // 目標場景名稱，默認為 MainMenu
    public GameObject player; // 玩家物件，用於檢查是否進入區域

    private void OnTriggerEnter(Collider other)
    {
        // 確保是 Player 進入 FinishDetectionZone
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            Debug.Log("Player entered FinishDetectionZone. Switching to MainMenu...");

            // 切換到目標場景
            LoadTargetScene();
        }
    }

    private void LoadTargetScene()
    {
        // 檢查目標場景名稱是否有效
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName); // 加載目標場景
        }
        else
        {
            Debug.LogError("Target scene name is not set!");
        }
    }
}

