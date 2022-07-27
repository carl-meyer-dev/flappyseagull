using System;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private Rigidbody2D birdRigidbody2D;

    private static Bird _instance;

    public static Bird GetInstance()
    {
        return _instance;
    }

    private const float JumpAmount = 100f;

    public event EventHandler OnDied;


    private void Awake()
    {
        _instance = this;
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
        birdRigidbody2D.velocity = Vector2.up * JumpAmount;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        birdRigidbody2D.bodyType = RigidbodyType2D.Static; // stop bird from moving
        OnDied?.Invoke(this, EventArgs.Empty);
    }
}