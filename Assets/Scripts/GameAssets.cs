using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _instance;

    public static GameAssets GetInstance()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    public Sprite pipeHeadSprite;
    public Transform pfPipeBody;
    public Transform pfPipeHead;
}
