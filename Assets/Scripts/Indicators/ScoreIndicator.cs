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
    }

    public static void AddToScore(int scoreToadd)
    {
        ScoreIndicatorInstance.score += scoreToadd;
        ScoreIndicatorInstance.scoreText.text = $"{ScoreIndicatorInstance.score}";
    }
}
