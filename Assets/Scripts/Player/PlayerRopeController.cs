using UnityEngine;

public class PlayerRopeController : MonoBehaviour
{
    [SerializeField] private RopeController currentRope;
    [SerializeField] private Rigidbody2D rb;

    public float climbUpSpeed = 3f;
    public float climbDownSpeed = 6f;
    public bool isClimbing = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GameManager.instance.player.HorizontalMovementBlocked = isClimbing;
        if (currentRope == null) return;

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
    }

    void EnterInputRope()
    {
        if (currentRope == null) return;

        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;

        transform.position = currentRope.GetSnapPosition(transform.position);
        isClimbing = true;
    }

    void HandleClimbing()
    {
        if (GameManager.instance.player.isDashing)
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

        if (GameManager.instance.player.touching.IsGrouded)
        {
            ExitRope();
        }
    }

    void ExitRope()
    {
        if (currentRope == null) return;

        rb.gravityScale = 1;
        isClimbing = false;
        GameManager.instance.player.VerticalMovementBlocked = false;
    }

    // chamado pela corda
    public void SetCurrentRope(RopeController rope)
    {
        currentRope = rope;
    }

    public void ClearCurrentRope(RopeController rope)
    {
        currentRope = null;
        rb.gravityScale = 1;
        isClimbing = false;
    }
}
