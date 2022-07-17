using CodeMonkey;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private Rigidbody2D birdRigidbody2D;
  
    private const float JUMP_AMOUNT = 100f;


    private void Awake()
    {
        birdRigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Jump();
        }
    }

    private void Jump()
    {
        birdRigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        CMDebug.TextPopupMouse("Dead!");
    }
}