using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLogicScript : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void Quit()
    {
        Debug.Log("Game is closing..."); // Log message for testing in the editor
        Application.Quit();
    }
}
