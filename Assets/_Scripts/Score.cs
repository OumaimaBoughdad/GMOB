using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class Score : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Change type to TextMeshProUGUI
    private int scoreCount;

    void Start()
    {
        scoreCount = 0;
        UpdateScoreText();
        Debug.Log(scoreCount);
    }

    public void AddScore(int value)
    {
        scoreCount += value;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + scoreCount;
        }
        else
        {
            Debug.LogError("Score Text is not assigned.");
        }
    }
}
