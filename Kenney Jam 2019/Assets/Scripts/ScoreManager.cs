using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int Score { get; private set; }
    public static int HighScore { get; private set; }
    public static int ScoreMultiplier { get; private set; }

    public static event Action ScoreChanged;

    private void Awake()
    {
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
    
        //HACK ScoreReset
        //Score = 0;
        ScoreMultiplier = 1;
    }

    public static int AddPoints(int points)
    {
        int pts = points * ScoreMultiplier;

        Score += pts;

        ScoreChanged?.Invoke();

        return pts;
    }

    public static void SetHighScore()
    {
        if (Score > HighScore)
        {
            HighScore = Score;

            PlayerPrefs.SetInt("HighScore", HighScore);
        }

        ScoreChanged?.Invoke();
    }

    public static void ResetScore()
    {
        Score = 0;
        ScoreMultiplier = 1;

        ScoreChanged?.Invoke();
    }

    public static void ResetScoreMultiplier()
    {
        ScoreMultiplier = 1;

        ScoreChanged?.Invoke();
    }

    public static void IncrementMultiplier()
    {
        ScoreMultiplier++;

        ScoreChanged?.Invoke();
    }
}
