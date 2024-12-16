using UnityEngine;
using TMPro;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused = false;
    private Canvas pauseMenuCanvas;
    public TextMeshProUGUI pauseText; // Reference to the TMP Text element
    private Coroutine flashCoroutine;

    void Start()
    {
        // Get the Canvas component
        pauseMenuCanvas = GetComponent<Canvas>();

        // Ensure the Pause Menu is hidden initially
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.enabled = false;

        if (pauseText != null)
            pauseText.text = "Paused";
    }

    void Update()
    {
        // Toggle Pause when Escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Pause the game, show the Pause Menu, and start flashing text
            Time.timeScale = 0;
            pauseMenuCanvas.enabled = true;

            if (pauseText != null)
                flashCoroutine = StartCoroutine(FlashPauseText());
        }
        else
        {
            // Unpause the game, hide the Pause Menu, and stop flashing text
            Time.timeScale = 1;
            pauseMenuCanvas.enabled = false;

            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
                flashCoroutine = null;
            }

            if (pauseText != null)
                pauseText.text = "Paused"; // Reset text to default
        }
    }

    private IEnumerator FlashPauseText()
    {
        while (true)
        {
            pauseText.text = "Paused ▐▐";
            yield return new WaitForSecondsRealtime(0.55f); // Wait for half a second in real time

            pauseText.text = "Paused";
            yield return new WaitForSecondsRealtime(0.55f); // Wait for half a second in real time
        }
    }
}
