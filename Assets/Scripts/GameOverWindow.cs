using System;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    private Text _highScoreText;
    private Text _scoreText;

    private void Awake()
    {
        _scoreText = transform.Find("scoreText").GetComponent<Text>();
        _highScoreText = transform.Find("highScoreText").GetComponent<Text>();

        SetupUI();
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += GameWindow_OnDied;

        Hide();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            // Retry
            Loader.Load(Loader.Scene.Game);
    }

    private void SetupUI()
    {
        transform.Find("RetryButton").GetComponent<Button_UI>().AddButtonSounds();


        transform.Find("MainMenuButton").GetComponent<Button_UI>().AddButtonSounds();

        transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    public void OnPlayAgain()
    {
        Loader.Load(Loader.Scene.Game);
    }

    public void OnReturnToMainMenu()
    {
        Loader.Load(Loader.Scene.MainMenu);
    }

    private void GameWindow_OnDied(object sender, EventArgs e)
    {
        _scoreText.text = $"SCORE: {Level.GetInstance().GetPipesPassedCount().ToString()}";

        _highScoreText.text = Level.GetInstance().GetPipesPassedCount() >= Score.GetHighScore()
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