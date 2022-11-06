using System;
using System.Runtime.InteropServices;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    private Text _highScoreText;
    private Text _scoreText;
    
    [DllImport("__Internal")]
    private static extern void GameOver (int score);
    
    [DllImport("__Internal")]
    private static extern void PlayAgain ();

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

        InvokePlayAgainWebglEvent();
    }

    public void OnReturnToMainMenu()
    {
        Loader.Load(Loader.Scene.MainMenu);
    }

    private void GameWindow_OnDied(object sender, EventArgs e)
    {
        var score = Level.GetInstance().GetPipesPassedCount();
        
        _scoreText.text = $"SCORE: {score.ToString()}";

        _highScoreText.text = score >= Score.GetHighScore()
            ? "NEW HIGH SCORE!"
            : $"HIGH SCORE: {Score.GetHighScore()}";

        InvokeGameOverWebglEvent(score);

        Show();
    }
    

    private void InvokeGameOverWebglEvent(int score)
    {
        try
        {
            GameOver (score);
        }
        catch (Exception e)
        {
            Console.WriteLine("Probably debugging in Unity Player.");
            Console.WriteLine(e);
        }
    }

    private void InvokePlayAgainWebglEvent()
    {
        try
        {
            GameOverWindow.PlayAgain();
        }
        catch (Exception e)
        {
            Console.WriteLine("Probably debugging in Unity Player.");
            Console.WriteLine(e);
        }
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