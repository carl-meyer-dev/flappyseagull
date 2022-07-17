using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GameManager.Start");

        GameObject gameObject = new GameObject("Pipe", typeof(SpriteRenderer));
        gameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.GetInstance().pipeHeadSprite;
    }
}