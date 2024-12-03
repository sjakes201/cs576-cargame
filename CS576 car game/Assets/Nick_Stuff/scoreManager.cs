using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// Following script made by Nick

public class ScoreManager : MonoBehaviour
{
    public Text scoreText; // Reference to the score text element
    public int score = 100; // Initial score
    private bool isSpeeding = false; // Tracks if the player is currently speeding
    private Coroutine speedingCoroutine; // Handles deducting points repeatedly

    private void Start()
    {
        UpdateScoreText();
    }

    // Subtracts 10 points for failing to stop
    public void failedToStop()
    {
        if (score > 0)
        {
            score -= Mathf.Min(10, score); // Deduct 10 points but ensure score doesn't go below 0
            UpdateScoreText();
        }
    }

    // Call this method when the player starts speeding in a SpeedZone
    public void StartSpeeding()
    {
        
        isSpeeding = true;
        speedingCoroutine = StartCoroutine(DeductPointsForSpeeding());

    }

    // Call this method when the player stops speeding in a SpeedZone
    public void StopSpeeding()
    {
       
        isSpeeding = false;         
        StopCoroutine(speedingCoroutine);
        speedingCoroutine = null;
            
    }

    // Subtracts one point for every two second the player is speeding
    private IEnumerator DeductPointsForSpeeding()
    {
        while (isSpeeding)
        {
            yield return new WaitForSeconds(2f);
            if (score > 0)
            {
                score -= 1;
                UpdateScoreText();
            }
        }
    }

    // Updates the score text to display the current score
    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }
}
