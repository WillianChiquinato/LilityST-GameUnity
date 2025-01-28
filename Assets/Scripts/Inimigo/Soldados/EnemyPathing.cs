using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    public DetectionZone attackZona;
    RaycastHit2D groundFront;
    public float direcao;
    public float distancePlayer;

    public Transform player;
    public float speed;
    public float jumpForce;
    public bool shouldJump;
    public bool Isgrounded;
    public LayerMask groundCheck;

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
            //Tocando no chao.
            Isgrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.2f, groundCheck);

            //Detectando o player (Talvez nem precise).
            bool isParede = Physics2D.Raycast(transform.position, Vector2.up, 3f, 1 << player.gameObject.layer);

            if (Isgrounded)
            {
                animator.SetBool("IsGround", true);
                animator.SetBool("Running", true);
                animator.SetBool("Jumping", false);
                rb.linearVelocity = new Vector2(direcao * speed, rb.linearVelocity.y);

                groundFront = Physics2D.Raycast((Vector2)transform.position + new Vector2(0, 0.6f), new Vector2(direcao, 0), 2f, groundCheck);
                RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direcao * 0.7f, 0, 0), Vector2.down, 2f, groundCheck);
                RaycastHit2D platFormAbove = Physics2D.Raycast(transform.position, new Vector2(direcao, 0), 3f, groundCheck);

                if (!groundFront.collider && !gapAhead.collider)
                {
                    shouldJump = true;
                }
                else if (groundFront.collider)
                {
                    shouldJump = true;
                }
            }
            else
            {
                animator.SetBool("Jumping", true);
                animator.SetBool("IsGround", false);
            }


            if (distancePlayer < 3f)
            {
                //Direcao
                direcao = Mathf.Sign(player.position.x - transform.position.x);
                FlipDirecao();
                Target = attackZona.detectColliders.Count > 0;

                if (attackCooldown > 0)
                {
                    attackCooldown -= Time.deltaTime;
                }
            }
            else
            {
                if (groundFront.collider)
                {
                    // transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
                }
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
                Vector2 JumpDirecao = direcaoAtePlayer * 1.3f;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (player == null) return;

        //IsGround.
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector2.down * 1.2f);

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
        if (Isgrounded)
        {
            if (transform.position.x > player.transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
