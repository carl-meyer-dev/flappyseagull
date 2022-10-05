using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;

public class MainMenuWindow : MonoBehaviour
{
    private void Awake()
    {
        transform.Find("PlayButton").GetComponent<Button_UI>().ClickFunc = () =>
        {
            Loader.Load(Loader.Scene.Game);
        };
        
        transform.Find("QuitButton").GetComponent<Button_UI>().ClickFunc = Application.Quit;
    }
}
