using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _instance;

    public Sprite pipeHeadSprite;
    public Transform pfPipeBody;
    [FormerlySerializedAs("pfPipeHead_1")] public Transform pfPipeHead1;
    [FormerlySerializedAs("pfPipeHead_2")] public Transform pfPipeHead2;
    public Transform pfGround;
    [FormerlySerializedAs("pfCloud_1")] public Transform pfCloud1;
    [FormerlySerializedAs("pfCloud_2")] public Transform pfCloud2;
    [FormerlySerializedAs("pfCloud_3")] public Transform pfCloud3;
    public Transform pfBuildingsFarBackground;
    public Transform pfBuildingsFrontBackground;

    [FormerlySerializedAs("SoundAudioClips")] public SoundAudioClip[] soundAudioClips;

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