using UnityEngine;

public class LagartoPatrulha : PlayerPoco
{
    [Header("Instacias")]
    public Rigidbody2D rb;
    public Animator animator;
    RaycastHit2D groundFront;
    public LayerMask groundCheck;
    public GameObject groundCheckObject;
    public GameObject targetLilityObject;


    [Header("Movimentação")]
    public bool targetLility;
    public float speed;
    private float originalSpeed;
    public float direcao;
    public float direcaoTarget;
    public bool isOnWall;
    public float wallTimerScale;
    public float wallTimerTarget;

    public float ItemTimerScale;
    public float ItemTimerTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetLilityObject == null)
        {
            ItemTimerScale = 0f;
            targetLility = false;
        }

        if (wallTimerScale > 0f && ItemTimerScale > 0f)
        {
            isOnWall = false;
            wallTimerScale = 0f;
        }

        direcao = Mathf.Sign(transform.localScale.x);
        groundFront = Physics2D.Raycast((Vector2)groundCheckObject.transform.position + new Vector2(0, -0.8f), Vector2.right * -direcao, 0.3f, groundCheck);
        animator.SetBool("IsOnWall", isOnWall);

        if (!targetLility)
        {
            speed = originalSpeed;
            if (!isOnWall)
            {
                rb.linearVelocity = new Vector2(-direcao * speed, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }

            if (groundFront.collider)
            {
                isOnWall = true;
                wallTimerScale += Time.deltaTime;

                if (wallTimerScale >= wallTimerTarget)
                {
                    transform.localScale = new Vector3(-direcao, transform.localScale.y, transform.localScale.z);
                    wallTimerScale = 0f;
                }
            }
            else
            {
                isOnWall = false;
            }
        }
        else
        {
            speed = originalSpeed + 2f;
            direcaoTarget = Mathf.Sign(targetLilityObject.transform.position.x - transform.position.x);
            float distanceToItem = Mathf.Abs(targetLilityObject.transform.position.x - transform.position.x);
            ItemTimerScale += Time.deltaTime;

            if (ItemTimerScale >= ItemTimerTarget)
            {
                rb.linearVelocity = new Vector2(direcaoTarget * speed, rb.linearVelocity.y);
                if (direcaoTarget == -1)
                {
                    transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                }
            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                animator.SetBool("IsOnWall", true);
            }

            if (distanceToItem <= 0.4f)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                animator.SetBool("IsOnWall", true);
            }
        }
    }

    void OnDrawGizmos()
    {
        //Plataformas e colisao a frente.
        Gizmos.color = Color.red;
        Vector2 start = (Vector2)groundCheckObject.transform.position + new Vector2(0, -0.8f);
        Vector2 direction = Vector2.right * -direcao;
        Gizmos.DrawRay(start, direction * 0.3f);
    }
}
