using System;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    private Text highScoreText;
    private Text scoreText;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        highScoreText = transform.Find("highScoreText").GetComponent<Text>();

        transform.Find("RetryButton").GetComponent<Button_UI>().ClickFunc = () => { Loader.Load(Loader.Scene.Game); };

        transform.Find("RetryButton").GetComponent<Button_UI>().AddButtonSounds();

        transform.Find("MainMenuButton").GetComponent<Button_UI>().ClickFunc = () =>
        {
            Loader.Load(Loader.Scene.MainMenu);
        };
        transform.Find("MainMenuButton").GetComponent<Button_UI>().AddButtonSounds();
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;

        Hide();
    }

    private void Bird_OnDied(object sender, EventArgs e)
    {
        Debug.Log("Game Over");

        scoreText.text = $"SCORE: {Level.GetInstance().GetPipesPassedCount().ToString()}";

        highScoreText.text = Level.GetInstance().GetPipesPassedCount() >= Score.GetHighScore()
            ? "NEW HIGH SCORE!"
            : $"HIGH SCORE: {Score.GetHighScore()}";

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