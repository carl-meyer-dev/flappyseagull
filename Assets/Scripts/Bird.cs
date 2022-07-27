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
    public event EventHandler OnStartPlaying;

    private State state;

    private enum State
    {
        WaitingToStart,
        Playing,
        Dead
    }

    private void Awake()
    {
        _instance = this;
        birdRigidbody2D = GetComponent<Rigidbody2D>();
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;
    }

    void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    state = State.Playing;
                    birdRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                    OnStartPlaying?.Invoke(this, EventArgs.Empty);
                    Jump();
                }

                break;
            case State.Playing:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    Jump();
                }

                break;
            case State.Dead:
                break;
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