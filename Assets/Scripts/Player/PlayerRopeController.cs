using System.Collections;
using UnityEngine;

public class PlayerRopeController : MonoBehaviour
{
    [SerializeField] private RopeController currentRope;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    public float climbUpSpeed = 3f;
    public float climbDownSpeed = 6f;
    public bool isClimbing = false;

    [SerializeField] private float climbOverDuration = 2.4f;
    [SerializeField] private AnimationCurve climbCurve;
    [SerializeField] private Vector3 topOffset = new Vector3(0, 0.5f, 0);
    [SerializeField] private float topReachDistance = 0.5f;
    [SerializeField] private float topTransitionSpeedMultiplier = 2f;
    [SerializeField] private string climbAnimatorBool = "IsClimbing";
    [SerializeField] private bool useTopTransitionTrigger = false;
    [SerializeField] private string topTransitionTrigger = "ClimbOverTop";

    private bool isTransitioning = false;
    private float defaultGravityScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        defaultGravityScale = rb.gravityScale;
    }

    void Update()
    {
        GameManager.instance.player.HorizontalMovementBlocked = isClimbing;
        GameManager.instance.player.VerticalMovementBlocked = isClimbing;
        if (currentRope == null) return;
        if (isTransitioning) return;

        switch (currentRope.ropeType)
        {
            case RopeType.Normal:
                HandleNormalRope();
                break;

            case RopeType.ClimbInput:
                HandleInputClimbingRope();
                break;
        }

        if (isClimbing)
        {
            HandleClimbing();
        }
    }

    void HandleNormalRope()
    {
        if (currentRope == null) return;
        if (Input.GetKeyDown(KeyCode.W))
        {
            EnterNormalRope();
        }
    }

    void HandleInputClimbingRope()
    {
        if (currentRope == null) return;
        if (GameManager.instance.player.entrar)
        {
            EnterInputRope();
        }
    }

    void EnterNormalRope()
    {
        if (currentRope == null) return;

        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;

        transform.position = currentRope.GetSnapPosition(transform.position);
        isClimbing = true;
        SetClimbAnimation(true);
    }

    void EnterInputRope()
    {
        if (currentRope == null) return;

        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;

        transform.position = currentRope.GetSnapPosition(transform.position);
        isClimbing = true;
        SetClimbAnimation(true);
    }

    void HandleClimbing()
    {
        if (currentRope.topPoint == null)
        {
            ExitRope();
            return;
        }

        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(vertical) < 0.01f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float speed = vertical > 0 ? climbUpSpeed : climbDownSpeed;
        rb.linearVelocity = new Vector2(0, vertical * speed);

        float distanceToTop = currentRope.topPoint.position.y - transform.position.y;

        if (vertical > 0 && distanceToTop <= topReachDistance && !isTransitioning)
        {
            StartCoroutine(ClimbOverTopRoutine());
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExitRope();
        }
    }

    IEnumerator ClimbOverTopRoutine()
    {
        isTransitioning = true;
        SetTopTransitionAnimation();

        rb.gravityScale = 1f; // importante: usar gravidade real aqui

        Vector2 start = rb.position;
        Vector2 end = (Vector2)currentRope.topPoint.position + (Vector2)topOffset;

        float jumpHeight = 1.2f; // controla o arco
        float gravity = Physics2D.gravity.y * rb.gravityScale;

        // cálculo da velocidade vertical
        float velocityY = Mathf.Sqrt(-2f * gravity * jumpHeight);

        // tempo até o topo do arco
        float timeToApex = velocityY / -gravity;

        // tempo total até cair no ponto final
        float totalTime = timeToApex + Mathf.Sqrt(2f * (end.y - start.y + jumpHeight) / -gravity);

        // velocidade horizontal necessária
        float velocityX = (end.x - start.x) / totalTime;

        rb.linearVelocity = new Vector2(velocityX, velocityY);

        yield return new WaitUntil(() =>
            rb.linearVelocity.y <= 0 && transform.position.y <= end.y + 0.1f
        );

        // snap final (evita micro erro)
        rb.position = end;

        FinishClimb();
    }

    void FinishClimb()
    {
        rb.gravityScale = defaultGravityScale;
        isClimbing = false;
        isTransitioning = false;

        SetClimbAnimation(false);

        GameManager.instance.player.HorizontalMovementBlocked = false;
        GameManager.instance.player.VerticalMovementBlocked = false;

        currentRope = null;
    }

    void ExitRope()
    {
        if (currentRope == null) return;

        rb.gravityScale = defaultGravityScale;
        isClimbing = false;
        GameManager.instance.player.VerticalMovementBlocked = false;
        SetClimbAnimation(false);
    }

    // chamado pela corda
    public void SetCurrentRope(RopeController rope)
    {
        currentRope = rope;
    }

    public void ClearCurrentRope(RopeController rope)
    {
        if (isTransitioning) return;

        currentRope = null;
        rb.gravityScale = defaultGravityScale;
        isClimbing = false;
        SetClimbAnimation(false);
        GameManager.instance.player.HorizontalMovementBlocked = false;
        GameManager.instance.player.VerticalMovementBlocked = false;
    }

    private void SetClimbAnimation(bool value)
    {
        if (animator == null || string.IsNullOrEmpty(climbAnimatorBool)) return;
        animator.SetBool(climbAnimatorBool, value);
    }

    private void SetTopTransitionAnimation()
    {
        if (!useTopTransitionTrigger) return;
        if (animator == null || string.IsNullOrEmpty(topTransitionTrigger)) return;
        animator.SetTrigger(topTransitionTrigger);
    }
}
