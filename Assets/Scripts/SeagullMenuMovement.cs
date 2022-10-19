using UnityEngine;

public class SeagullMenuMovement : MonoBehaviour
{
    private const float WalkSpeed = 10f;
    private static readonly int AnimatorState = Animator.StringToHash("State");
    private Animator animator;
    private Vector3 direction = Vector3.right;
    private bool hasPecked;
    private float idleAnimationTimer;
    private float idleAnimationTimerMax;
    private float peckingAnimationTimer;
    private float peckingAnimationTimerMax;
    private SpriteRenderer sprite;
    private State state;
    private float walkingAnimationTimer;
    private float walkingAnimationTimerMax;

    private void Awake()
    {
        walkingAnimationTimerMax = 5f;
        idleAnimationTimerMax = 1f;
        peckingAnimationTimerMax = 0.45f;

        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        state = State.Walking;
    }

    // Update is called once per frame
    private void Update()
    {
        if (state == State.Walking) HandleMovement();

        if (state == State.Idle) HandleIdle();

        if (state == State.Pecking) HandlePecking();

        UpdateAnimationState();
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("On Trigger Enter!");
        direction = direction == Vector3.right ? Vector3.left : Vector3.right;
        sprite.flipX = !sprite.flipX;
    }

    private void HandlePecking()
    {
        peckingAnimationTimer += Time.deltaTime;
        if (peckingAnimationTimer > peckingAnimationTimerMax)
        {
            state = State.Idle;
            peckingAnimationTimer = 0f;
            hasPecked = true;
            Debug.Log("Done Pecking, Change State to Idle");
        }
    }

    private void HandleIdle()
    {
        idleAnimationTimer += Time.deltaTime;
        if (idleAnimationTimer > idleAnimationTimerMax)
        {
            if (hasPecked)
            {
                Debug.Log("Change State to Walking");
                state = State.Walking;
                idleAnimationTimer = 0f;
                hasPecked = false;
            }
            else
            {
                Debug.Log("Change State to Pecking");
                state = State.Pecking;
                idleAnimationTimer = 0f;
            }
        }
    }

    private void UpdateAnimationState()
    {
        animator.SetInteger(AnimatorState, state.GetHashCode());
    }

    private void HandleMovement()
    {
        walkingAnimationTimer += Time.deltaTime;

        if (walkingAnimationTimer > walkingAnimationTimerMax)
        {
            Debug.Log("Change State to Idle");
            state = State.Idle;
            walkingAnimationTimer = 0f;
        }
        else
        {
            transform.position += direction * (WalkSpeed * Time.deltaTime);
        }
    }

    private enum State
    {
        Walking = 0,
        Idle = 1,
        Pecking = 2
    }
}