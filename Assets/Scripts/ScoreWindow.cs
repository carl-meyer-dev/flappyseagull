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
        Bird.GetInstance().OnDied += ScoreWindow_OnBirdDied;
        Bird.GetInstance().OnStartPlaying += ScoreWindow_OnStartPlaying;
        highScoreText.text = $"HIGH SCORE: {Score.GetHighScore()}";
        Hide();
    }

    

    private void Update()
    {
        scoreText.text = Level.GetInstance().GetPipesPassedCount().ToString();
    }

    private void ScoreWindow_OnBirdDied(object sender, EventArgs e)
    {
        Hide();
    }
    
    private void ScoreWindow_OnStartPlaying(object sender, EventArgs e)
    {
        Show();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    
    private void Show()
    {
        gameObject.SetActive(true);
    }
}