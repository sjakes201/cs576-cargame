using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI stopSignText;
    public TextMeshProUGUI stopLightText;
    public TextMeshProUGUI speedLimitText;
    public TextMeshProUGUI totalPrefix;
    public TextMeshProUGUI totalScore;

    private float bigFontSize = 12f;
    private float normalFontSize = 6f;
    private float animationDuration = 0.2f;
    private float animationHoldTime = 0.6f;
    private float delayBetweenScores = 0.8f;

    private Dictionary<string, TextMeshProUGUI> scoreTextElements;
    private Dictionary<string, string> scores = new Dictionary<string, string>();

    private ScoreManager scoreManager;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>(); // Get reference to ScoreManager in the scene
        InitializeTextElements();
        StartCoroutine(DelayedExecution());
    }

    private void InitializeTextElements()
    {
        scoreTextElements = new Dictionary<string, TextMeshProUGUI>
        {
            { "stopsign", stopSignText },
            { "stoplight", stopLightText },
            { "speedlimit", speedLimitText }
        };
        stopSignText.gameObject.SetActive(false);
        stopLightText.gameObject.SetActive(false);
        speedLimitText.gameObject.SetActive(false);
        totalPrefix.gameObject.SetActive(false);
        totalScore.gameObject.SetActive(false);
    }

    private IEnumerator DelayedExecution()
    {
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(ShowScores());
        yield return new WaitForSeconds(15.0f);
        SceneManager.LoadScene("MainMenu");

    }

    private string GetScore(string key)
    {
        if (scoreManager == null) return "N/A";

        switch (key)
        {
            case "stopsign":
                return $"{scoreManager.obeyStopSigns}/{scoreManager.totalStopSigns}";
            case "stoplight":
                return $"{scoreManager.obeyTrafficLights}/{scoreManager.totalTrafficLights}";
            case "speedlimit":
                return "N/A"; // Placeholder for speed limit
            default:
                return "0/0";
        }
    }

    private string GetGrade()
    {
        int score = scoreManager.score;
        // do math for thresholds
        return "B+";
    }

    private IEnumerator ShowScores()
    {
        foreach (var key in scoreTextElements.Keys)
        {
            scores[key] = GetScore(key);
            scoreTextElements[key].text = scores[key];

            yield return StartCoroutine(AnimateFontSize(scoreTextElements[key]));
            yield return new WaitForSeconds(delayBetweenScores);
        }

        yield return StartCoroutine(ShowTotalPrefix("FINAL GRADE: "));
        totalScore.text = GetGrade();
        if (totalScore.text == "A+")
        {
            totalScore.fontStyle = FontStyles.Bold;
            totalScore.color = new Color(1f, 0.84f, 0f, 1f);
        }
        else if (totalScore.text == "A" || totalScore.text == "A-" || totalScore.text == "B+")
        {
            totalScore.color = new Color(0.1f, 0.5f, 0.1f, 1f);
        }
        else if (totalScore.text == "D" || totalScore.text == "F")
        {
            totalScore.color = Color.red;
        }

        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(AnimateFontSize(totalScore, 2.0f, 1.5f));
    }

    private IEnumerator ShowTotalPrefix(string prefix)
    {
        totalPrefix.gameObject.SetActive(true);
        totalPrefix.text = "";

        for (int i = 0; i < prefix.Length; i++)
        {
            totalPrefix.text += prefix[i];
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator AnimateFontSize(TextMeshProUGUI textElement, float animationTimeMultiple = 1.0f, float fontSizeMultiple = 1.0f)
    {
        textElement.gameObject.SetActive(true);
        float elapsedTime = 0f;

        while (elapsedTime < (animationDuration * animationTimeMultiple) / 2)
        {
            elapsedTime += Time.deltaTime;
            textElement.fontSize = Mathf.Lerp(normalFontSize * fontSizeMultiple, bigFontSize * fontSizeMultiple, elapsedTime / (animationDuration / 2));
            yield return null;
        }

        yield return new WaitForSeconds(animationHoldTime * animationTimeMultiple);

        elapsedTime = 0f;
        while (elapsedTime < (animationDuration * animationTimeMultiple) / 2)
        {
            elapsedTime += Time.deltaTime;
            textElement.fontSize = Mathf.Lerp(bigFontSize * fontSizeMultiple, normalFontSize * fontSizeMultiple, elapsedTime / (animationDuration / 2));
            yield return null;
        }

        textElement.fontSize = normalFontSize * fontSizeMultiple;
    }
}
