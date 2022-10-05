using CodeMonkey.Utils;
using UnityEngine;

public class MainMenuWindow : MonoBehaviour
{
    private void Awake()
    {
        transform.Find("PlayButton").GetComponent<Button_UI>().ClickFunc = () =>
        {
            Debug.Log("Play Clicked!");
            Loader.Load(Loader.Scene.Game);
        };
        transform.Find("PlayButton").GetComponent<Button_UI>().AddButtonSounds();
        transform.Find("QuitButton").GetComponent<Button_UI>().ClickFunc = Application.Quit;
        transform.Find("QuitButton").GetComponent<Button_UI>().AddButtonSounds();
    }
}