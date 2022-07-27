using System;
using CodeMonkey;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    private Text scoreText;

    private void Awake()
    {
        scoreText = transform.Find("scoreTex").GetComponent<Text>();

        transform.Find("RetryButton").GetComponent<Button_UI>().ClickFunc = () =>
        {
            FunctionTimer.Create(() => { SceneManager.LoadScene("GameScene"); }, 1f);
        };

        Hide();
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += Bird_Died;
    }

    private void Bird_Died(object sender, EventArgs e)
    {
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