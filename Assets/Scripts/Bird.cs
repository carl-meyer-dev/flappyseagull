using System;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private const float JumpAmount = 90f;

    private static Bird _instance;
    private static readonly int AnimatorState = Animator.StringToHash("State");
    private Animator _animator;
    private Rigidbody2D _birdRigidbody2D;

    private State _state;

    private void Awake()
    {
        _instance = this;
        _birdRigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        _birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        _state = State.WaitingToStart;
        _animator.SetInteger(AnimatorState, _state.GetHashCode());
    }

    private void Update()
    {
        switch (_state)
        {
            case State.WaitingToStart:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    _state = State.Playing;
                    _birdRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                    OnStartPlaying?.Invoke(this, EventArgs.Empty);
                    Jump();
                }

                break;
            case State.Playing:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) Jump();

                transform.eulerAngles = new Vector3(0, 0, _birdRigidbody2D.velocity.y * 0.15f);

                break;
            case State.Dead:
                break;
        }
    }

    private void LateUpdate()
    {
        _animator.SetInteger(AnimatorState, _state.GetHashCode());
    }

    public static Bird GetInstance()
    {
        return _instance;
    }

    public event EventHandler OnDied;
    public event EventHandler OnStartPlaying;

    private enum State
    {
        WaitingToStart = 0,
        Playing = 1,
        Dead = 2
    } // ReSharper disable Unity.PerformanceAnalysis
    private void Jump()
    {
        _birdRigidbody2D.velocity = Vector2.up * JumpAmount;
        SoundManager.PlaySound(SoundManager.Sound.BirdJump);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        _state = State.Dead;
        _birdRigidbody2D.bodyType = RigidbodyType2D.Static; // stop bird from moving
        SoundManager.PlaySound(SoundManager.Sound.Lose);
        OnDied?.Invoke(this, EventArgs.Empty);
    }
}