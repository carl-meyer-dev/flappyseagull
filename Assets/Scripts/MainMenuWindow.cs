using CodeMonkey.Utils;
using UnityEngine;

public class MainMenuWindow : MonoBehaviour
{
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
        Application.Quit();
    }
}