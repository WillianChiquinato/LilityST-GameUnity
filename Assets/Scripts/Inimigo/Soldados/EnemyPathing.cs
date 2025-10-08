using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    [Header("Instancias")]
    private Rigidbody2D rb;
    public Animator animator;
    public DetectionZone attackZona;
    public Transform player;
    RaycastHit2D groundFront;
    public LayerMask groundCheck;

    [Header("Variaveis")]
    public float direcao;
    public float distanceX;
    public float distancePlayer;
    public bool distancePlayerYBool = false;
    public float speed;
    public float jumpForce;
    public bool shouldJump;
    public bool Isgrounded;
    public float minSpeed;
    public float maxSpeed;
    public float acceleration = 0.8f;

    public float attackCooldown
    {
        get
        {
            return animator.GetFloat(animationstrings.attackCooldown);
        }
        private set
        {
            animator.SetFloat(animationstrings.attackCooldown, Mathf.Max(value, 0));
        }
    }

    public bool _Target = false;
    public bool Target
    {
        get { return _Target; }
        private set
        {
            _Target = value;
            animator.SetBool(animationstrings.Target, value);
        }
    }

    public bool canMove
    {
        get
        {
            return animator.GetBool(animationstrings.canMove);
        }
        set
        {
            animator.SetBool(animationstrings.canMove, value);
        }
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindFirstObjectByType<PlayerMoviment>().transform;
    }

    void Update()
    {
        if (!distancePlayerYBool)
        {
            distancePlayer = Mathf.Abs(transform.position.y - player.transform.position.y);
        }
        distanceX = Mathf.Abs(transform.position.x - player.position.x);
        float velocityY = rb.linearVelocity.y;

        animator.SetFloat(animationstrings.yVelocity, velocityY);

        if (canMove)
        {
            // Verifica se está no chão
            Isgrounded = Physics2D.Raycast(transform.position + new Vector3(0.5f, 0f, 0f), Vector2.down, 2.1f, groundCheck);
            animator.SetBool("Jumping", shouldJump);

            if (Isgrounded)
            {
                animator.SetBool("IsGround", true);
                animator.SetBool("Running", true);
                animator.SetBool("Jumping", false);
                rb.linearVelocity = new Vector2(direcao * speed, rb.linearVelocity.y);

                // Verificações para pathing inteligente
                groundFront = Physics2D.Raycast((Vector2)transform.position + new Vector2(0, -0.3f), Vector2.right * direcao, 2f, groundCheck);
                RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direcao * 0.7f, -0.5f, 0), Vector2.down, 2f, groundCheck);
                RaycastHit2D platformAbove = Physics2D.Raycast(transform.position + new Vector3(0, -0.3f, 0), Vector2.right * direcao, 2f, groundCheck);
                RaycastHit2D wallCheck = Physics2D.Raycast(transform.position + new Vector3(0, 1f, 0), Vector2.right * direcao, 2f, groundCheck);

                //Logica para o pulo, se estiver em baixo do inimigo, nao se aplica o pulo.
                if (player.transform.position.y < transform.position.y - 2f)
                {
                    shouldJump = false;
                }
                else
                {
                    if (!gapAhead.collider && !groundFront.collider)
                    {
                        shouldJump = true;
                        Debug.Log("furo no chao");
                    }
                    else if (groundFront.collider || platformAbove.collider && !wallCheck.collider)
                    {
                        if (wallCheck.collider)
                        {
                            Debug.Log("parede detectada");
                            direcao *= -1;
                            FlipDirecao();
                        }
                        else
                        {
                            shouldJump = true;
                            Debug.Log("Apenas plaatforma");
                        }
                    }
                }

                //Distancia e direção do player.
                if (distancePlayer < 2f)
                {
                    if (distanceX > 0.5f)
                    {
                        direcao = Mathf.Sign(player.position.x - transform.position.x);
                        FlipDirecao();
                        Target = attackZona.detectColliders.Count > 0;

                        if (Target)
                        {
                            speed = minSpeed;
                        }

                        if (attackCooldown > 0)
                        {
                            attackCooldown -= Time.deltaTime;
                        }

                        if (!shouldJump)
                        {
                            speed = Mathf.MoveTowards(speed, maxSpeed, acceleration * Time.deltaTime);
                        }
                        else
                        {
                            speed = minSpeed;
                        }
                    }
                }
            }
            else
            {
                animator.SetBool("Jumping", true);
                animator.SetBool("IsGround", false);
            }

            if (rb.linearVelocity.x <= 0)
            {
                rb.linearVelocity = new Vector2(transform.localScale.x * speed, rb.linearVelocity.y);
            }

        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

    }

    void FixedUpdate()
    {
        if (Isgrounded && shouldJump)
        {
            shouldJump = false;

            if (Mathf.Abs(rb.linearVelocity.y) < 0.05f)
            {
                //Desacelera e pula.
                speed = minSpeed;
                Vector2 jumpVector = new Vector2(transform.localScale.x * minSpeed, jumpForce);

                rb.linearVelocity = jumpVector;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (player == null) return;

        //IsGround.
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position + new Vector3(0.5f, 0f, 0f), Vector2.down * 2.1f);

        //Direção do movimento.
        float direcao = Mathf.Sign(player.position.x - transform.position.x);

        //Detect Player.
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + new Vector3(0, -0.3f, 0), new Vector2(direcao, 0) * 2f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + new Vector3(0, 1f, 0), new Vector2(direcao, 0) * 2f);

        if (Isgrounded)
        {
            //Plataformas e colisao a frente.
            Gizmos.color = Color.red;
            Gizmos.DrawRay((Vector2)transform.position + new Vector2(0, -0.5f), new Vector2(direcao, 0) * 2f);

            //Lacuna do ground.
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position + new Vector3(direcao * 0.7f, -0.3f, 0), Vector2.down * 2f);
        }
    }

    private void FlipDirecao()
    {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direcao, transform.localScale.y, transform.localScale.z);
    }
}
