using System;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    private Text scoreText;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();

        transform.Find("RetryButton").GetComponent<Button_UI>().ClickFunc = () =>
        {
            Loader.Load(Loader.Scene.GameScene);
        };
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;

        Hide();
    }

    private void Bird_OnDied(object sender, EventArgs e)
    {
        Debug.Log("Game Over");

        scoreText.text = Level.GetInstance().GetPipesPassedCount().ToString();

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