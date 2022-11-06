using UnityEngine;

public class SeagullMenuMovement : MonoBehaviour
{
    private const float WalkSpeed = 10f;
    private static readonly int AnimatorState = Animator.StringToHash("State");
    private Animator _animator;
    private Vector3 _direction = Vector3.right;
    private bool _hasPecked;
    private float _idleAnimationTimer;
    private float _idleAnimationTimerMax;
    private float _peckingAnimationTimer;
    private float _peckingAnimationTimerMax;
    private SpriteRenderer _sprite;
    private State _state;
    private float _walkingAnimationTimer;
    private float _walkingAnimationTimerMax;

    private void Awake()
    {
        _walkingAnimationTimerMax = 5f;
        _idleAnimationTimerMax = 1f;
        _peckingAnimationTimerMax = 0.45f;

        _animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _state = State.Walking;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_state == State.Walking) HandleMovement();

        if (_state == State.Idle) HandleIdle();

        if (_state == State.Pecking) HandlePecking();

        UpdateAnimationState();
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("On Trigger Enter!");
        _direction = _direction == Vector3.right ? Vector3.left : Vector3.right;
        _sprite.flipX = !_sprite.flipX;
    }

    private void HandlePecking()
    {
        _peckingAnimationTimer += Time.deltaTime;
        if (_peckingAnimationTimer > _peckingAnimationTimerMax)
        {
            _state = State.Idle;
            _peckingAnimationTimer = 0f;
            _hasPecked = true;
            Debug.Log("Done Pecking, Change State to Idle");
        }
    }

    private void HandleIdle()
    {
        _idleAnimationTimer += Time.deltaTime;
        if (_idleAnimationTimer > _idleAnimationTimerMax)
        {
            if (_hasPecked)
            {
                Debug.Log("Change State to Walking");
                _state = State.Walking;
                _idleAnimationTimer = 0f;
                _hasPecked = false;
            }
            else
            {
                Debug.Log("Change State to Pecking");
                _state = State.Pecking;
                _idleAnimationTimer = 0f;
            }
        }
    }

    private void UpdateAnimationState()
    {
        _animator.SetInteger(AnimatorState, _state.GetHashCode());
    }

    private void HandleMovement()
    {
        _walkingAnimationTimer += Time.deltaTime;

        if (_walkingAnimationTimer > _walkingAnimationTimerMax)
        {
            Debug.Log("Change State to Idle");
            _state = State.Idle;
            _walkingAnimationTimer = 0f;
        }
        else
        {
            transform.position += _direction * (WalkSpeed * Time.deltaTime);
        }
    }

    private enum State
    {
        Walking = 0,
        Idle = 1,
        Pecking = 2
    }
}