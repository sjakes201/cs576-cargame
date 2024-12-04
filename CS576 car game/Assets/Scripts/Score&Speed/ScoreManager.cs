using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // References the Score text element
    private int score = 100; // initial score

    private void Start()
    {
        UpdateScoreText();
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

    // Updates score text element
    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }
}
