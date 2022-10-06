using System;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _instance;

    public Sprite pipeHeadSprite;
    public Transform pfPipeBody;
    public Transform pfPipeHead;
    public Transform pfGround;
    public Transform pfCloud_1;
    public Transform pfCloud_2;
    public Transform pfCloud_3;

    public SoundAudioClip[] SoundAudioClips;

    private void Awake()
    {
        _instance = this;
    }

    public static GameAssets GetInstance()
    {
        return _instance;
    }

    [Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
}