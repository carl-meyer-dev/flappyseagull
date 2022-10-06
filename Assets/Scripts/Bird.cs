using System;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private const float JumpAmount = 90f;

    private static Bird _instance;
    private Rigidbody2D birdRigidbody2D;

    private State state;

    private void Awake()
    {
        _instance = this;
        birdRigidbody2D = GetComponent<Rigidbody2D>();
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;
    }

    private void Update()
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
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) Jump();

                break;
            case State.Dead:
                break;
        }
    }

    public static Bird GetInstance()
    {
        return _instance;
    }

    public event EventHandler OnDied;
    public event EventHandler OnStartPlaying;

    private enum State
    {
        WaitingToStart,
        Playing,
        Dead
    } // ReSharper disable Unity.PerformanceAnalysis
    private void Jump()
    {
        birdRigidbody2D.velocity = Vector2.up * JumpAmount;
        SoundManager.PlaySound(SoundManager.Sound.BirdJump);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        birdRigidbody2D.bodyType = RigidbodyType2D.Static; // stop bird from moving
        SoundManager.PlaySound(SoundManager.Sound.Lose);
        OnDied?.Invoke(this, EventArgs.Empty);
    }
}