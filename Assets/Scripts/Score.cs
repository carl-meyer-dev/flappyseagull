using System;
using UnityEngine;

public static class Score
{
    public static void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;
    }

    private static void Bird_OnDied(object sender, EventArgs e)
    {
        TrySetNewHighScore(Level.GetInstance().GetPipesPassedCount());
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt("highscore");
    }

    public static bool TrySetNewHighScore(int score)
    {
        var currentHighScore = GetHighScore();

        if (score <= currentHighScore) return false;
        PlayerPrefs.SetInt("highscore", score);
        PlayerPrefs.Save();
        return true;
    }

    public static void ResetHighScore()
    {
        PlayerPrefs.SetInt("highscore", 0);
        PlayerPrefs.Save();
    }
}