using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    [Header("Instancias")]
    private Rigidbody2D rb;
    private Animator animator;
    public DetectionZone attackZona;
    public Transform player;
    RaycastHit2D groundFront;
    public LayerMask groundCheck;

    [Header("Variaveis")]
    public float direcao;
    public float distancePlayer;
    public float speed;
    public float jumpForce;
    public bool shouldJump;
    public bool Isgrounded;
    private float minSpeed = 4.7f;
    private float maxSpeed = 8f;
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
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindFirstObjectByType<PlayerMoviment>().transform;
    }

    void Update()
    {
        distancePlayer = Mathf.Abs(transform.position.y - player.transform.position.y);

        if (canMove)
        {
            // Verifica se está no chão
            Isgrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundCheck);

            if (Isgrounded)
            {
                animator.SetBool("IsGround", true);
                animator.SetBool("Running", true);
                animator.SetBool("Jumping", false);
                rb.linearVelocity = new Vector2(direcao * speed, rb.linearVelocity.y);

                // Verificações para pathing inteligente
                groundFront = Physics2D.Raycast((Vector2)transform.position + new Vector2(0, 0.6f), Vector2.right * direcao, 2f, groundCheck);
                RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direcao * 0.7f, 0, 0), Vector2.down, 2f, groundCheck);
                RaycastHit2D platformAbove = Physics2D.Raycast(transform.position + new Vector3(0, 0.8f, 0), Vector2.right * direcao, 2f, groundCheck);

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
                    }
                    else if (groundFront.collider && platformAbove.collider)
                    {
                        shouldJump = true;
                    }
                }
            }
            else
            {
                animator.SetBool("Jumping", true);
                animator.SetBool("IsGround", false);
            }

            if (distancePlayer < 3.5f)
            {
                direcao = Mathf.Sign(player.position.x - transform.position.x);
                FlipDirecao();
                Target = attackZona.detectColliders.Count > 0;

                speed = Mathf.MoveTowards(speed, maxSpeed, acceleration * Time.deltaTime);

                if (attackCooldown > 0)
                {
                    attackCooldown -= Time.deltaTime;
                }
            }
            else
            {
                speed = minSpeed;
            }
        }
    }

    void FixedUpdate()
    {
        if (Isgrounded && shouldJump)
        {
            shouldJump = false;

            if (Mathf.Abs(rb.linearVelocity.y) < 0.1f)
            {
                Vector2 direcaoAtePlayer = (player.transform.position - transform.position).normalized;
                Vector2 jumpVector = new Vector2(direcaoAtePlayer.x * minSpeed, jumpForce);

                rb.linearVelocity = jumpVector;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (player == null) return;

        //IsGround.
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector2.down * 1.1f);

        //Direção do movimento.
        float direcao = Mathf.Sign(player.position.x - transform.position.x);

        //Detect Player.
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector2.up * 4f);

        if (Isgrounded)
        {
            //Plataformas e colisao a frente.
            Gizmos.color = Color.red;
            Gizmos.DrawRay((Vector2)transform.position + new Vector2(0, 0.6f), new Vector2(direcao, 0) * 2f);

            //Lacuna do ground.
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position + new Vector3(direcao * 0.7f, 0, 0), Vector2.down * 2f);
        }
    }

    private void FlipDirecao()
    {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direcao, transform.localScale.y, transform.localScale.z);
    }
}
