using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // References the Score text element
    public int score = 0; // initial score
    public int obeyStopSigns = 0;
    public int totalStopSigns = 0;
    public int obeyTrafficLights = 0;
    public int totalTrafficLights = 0;

    // Timer related variables
    public TextMeshProUGUI timerText; // Reference to the Timer text element
    public int timerSecDuration = 480; // Total timer duration in seconds
    public Canvas gameCanvas; // Canvas to disable when the timer runs out
    public Canvas endGameCanvas; // Canvas to enable when the timer runs out

    private void Start()
    {
        UpdateScoreText();
        StartCoroutine(StartTimer());
    }

    // Deducts any number of points depending on the violation
    public void DeductPoints(int points)
    {
        if (score > 0)
        {
            score -= Mathf.Min(points, score); // Ensure score does not go below 0
       
            UpdateScoreText();
        }
    }

    public void failToStop(int type)
    {
        // type 0 is stop signs
        if (type == 0)
        {
            totalStopSigns++;
            Debug.Log("obeyStopSigns: " + obeyStopSigns);
            Debug.Log("totalStopSigns: " + totalStopSigns);
        }

        // type 1 is traffic lights
        if (type == 1)
        {
            totalTrafficLights++;
            Debug.Log("obeyTrafficLights: " + obeyTrafficLights);
            Debug.Log("totalTrafficLights: " + totalTrafficLights);
        }
    }

    public void AddPoints(int points, int type)
    {

        score += points;
        UpdateScoreText();

        // type 0 is stop signs
        if (type == 0)
        {
            obeyStopSigns++;
            totalStopSigns++;
            Debug.Log("obeyStopSigns: " + obeyStopSigns);
            Debug.Log("totalStopSigns: " + totalStopSigns);
        }

        // type 1 is traffic lights
        if (type == 1)
        {
            obeyTrafficLights++;
            totalTrafficLights++;
            Debug.Log("obeyTrafficLights: " + obeyTrafficLights);
            Debug.Log("totalTrafficLights: " + totalTrafficLights);
        }
    }

    // Updates score text element
    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    private IEnumerator StartTimer()
    {
        int totalSeconds = timerSecDuration; 

        // Updates timer every second
        while (totalSeconds > 0)
        {
            timerText.text = $"Time left: {totalSeconds / 60:00}:{totalSeconds % 60:00}";

            yield return new WaitForSeconds(1f);
            totalSeconds--;
        }

        timerText.text = "Time left: 00:00";
        EndGame();
    }

    public void EndGame()
    {
        gameCanvas.gameObject.SetActive(false);
        endGameCanvas.gameObject.SetActive(true);
        StopCoroutine(StartTimer());

        Debug.Log("Game over");
    }
}
