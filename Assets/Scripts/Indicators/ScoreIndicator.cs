using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreIndicator : MonoBehaviour
{
    private static ScoreIndicator ScoreIndicatorInstance;
    private Text scoreText;
    private int score = 0;
    private void Awake()
    {
        ScoreIndicatorInstance = this;
        scoreText = GetComponentInChildren<Text>();
        var persistentScore = PersistentScript.TotalScore;
        if (persistentScore > 0) { AddToScore(persistentScore); }
    }

    public static void AddToScore(int scoreToadd)
    {
        ScoreIndicatorInstance.score += scoreToadd;
        ScoreIndicatorInstance.scoreText.text = $"{ScoreIndicatorInstance.score}";
    }
    public static void ResetScore()
    {
        ScoreIndicatorInstance.score = 0;
    }

    private void OnDestroy()
    {
        PersistentScript.TotalScore = score;
    }

    private void OnDisable()
    {
        PersistentScript.TotalScore = score;
    }
}
