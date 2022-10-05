using CodeMonkey.Utils;
using UnityEngine;

public static class SoundManager
{
    public enum Sound
    {
        BirdJump,
        Score,
        Lose,
        ButtonOver,
        ButtonClick
    }

    public static void PlaySound(Sound sound)
    {
        var gameObject = new GameObject("Sound", typeof(AudioSource));
        var audioSource = gameObject.GetComponent<AudioSource>();

        audioSource.PlayOneShot(GetAudioClip(sound));
    }

    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.GetInstance().SoundAudioClips)
            if (soundAudioClip.sound == sound)
                return soundAudioClip.audioClip;

        Debug.LogError($"Sound {sound} not found!");
        return null;
    }

    public static void AddButtonSounds(this Button_UI buttonUI)
    {
        buttonUI.MouseOverOnceFunc += () => PlaySound(Sound.ButtonOver);
        buttonUI.ClickFunc += () => PlaySound(Sound.ButtonClick);
    }
}