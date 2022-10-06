using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour
{
    private Text highScoreText;
    private Text scoreText;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        highScoreText = transform.Find("highScoreText").GetComponent<Text>();
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;
        highScoreText.text = $"HIGH SCORE: {Score.GetHighScore()}";
    }

    private void Update()
    {
        scoreText.text = Level.GetInstance().GetPipesPassedCount().ToString();
    }

    private void Bird_OnDied(object sender, EventArgs e)
    {
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}