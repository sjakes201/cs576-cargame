using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // 切換到指定場景
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
    