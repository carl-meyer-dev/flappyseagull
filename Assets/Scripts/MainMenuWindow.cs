using System;
using System.Runtime.InteropServices;
using CodeMonkey.Utils;
using UnityEngine;

public class MainMenuWindow : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Quit ();
    private void Awake()
    {
        transform.Find("PlayButton").GetComponent<Button_UI>().AddButtonSounds();
        transform.Find("QuitButton").GetComponent<Button_UI>().AddButtonSounds();
    }

    public void OnPlay()
    {
        Loader.Load(Loader.Scene.Game);
    }

    public void OnQuit()
    {
        InvokeQuiteWebglEvent();
        Application.Quit();
    }

    private void InvokeQuiteWebglEvent()
    {
        try
        {
            Quit();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}