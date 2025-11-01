using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingVariables), typeof(Damage))]
public class GolemPatrulha_Moviment : PlayerPoco
{
    [Header("Instances")]
    [SerializeField] private Item_drop dropInimigo;
    TouchingVariables touching;
    Rigidbody2D rb;
    Animator animator;
    Damage DamageScript;
    public DetectionZone attackZona;
    public LayerMask groundCheck;

    [Header("Sobre o Hit")]
    [SerializeField] private float IdleDuracao;
    [SerializeField] private float IdleTimer;
    private Vector2 vectorDirecao = Vector2.right;
    public int contagemHit = 0;
    public float contagemStagger;
    public bool contagemStaggerBool = false;
    public float blinkDuration = 0.1f;
    public int blinkCount = 1;

    public float speed = 4f;
    public float StopRate = 0.2f;
    public enum WalkAbleDirecao { Right, Left }

    private WalkAbleDirecao _WalkDirecao;

    public WalkAbleDirecao WalkDirecao
    {
        get { return _WalkDirecao; }
        set
        {
            if (_WalkDirecao != value)
            {
                IdleTimer = 0;
                animator.SetBool(animationstrings.IsIdlePatrulha, false);

                // Definir o Flip
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if (value == WalkAbleDirecao.Right)
                {
                    vectorDirecao = Vector2.right;
                }
                else if (value == WalkAbleDirecao.Left)
                {
                    vectorDirecao = Vector2.left;
                }

            }

            _WalkDirecao = value;
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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touching = GetComponent<TouchingVariables>();
        animator = GetComponent<Animator>();
        DamageScript = GetComponent<Damage>();
        dropInimigo = GetComponent<Item_drop>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        newMaterial = Resources.Load<Material>("Material/Hit");
    }

    void Update()
    {
        if (DamageScript.IsAlive)
        {
            Target = attackZona.detectColliders.Count > 0;
            if (attackCooldown > 0)
            {
                attackCooldown -= Time.deltaTime;
            }

            if (contagemHit == 5)
            {
                StartCoroutine(ContagemHitAnim());
            }

            if (contagemStaggerBool)
            {
                contagemStagger += Time.deltaTime;
                if (contagemStagger >= 2)
                {
                    contagemHit = 0;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(vectorDirecao.x * 1.1f, -0.5f, 0), Vector2.down, 2f, groundCheck);
        RaycastHit2D wallCheck = Physics2D.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector2.right * vectorDirecao.x, 2f, groundCheck);

        if (touching.IsGrouded)
        {
            if (wallCheck.collider != null || gapAhead.collider == null)
            {
                FlipDirecao();
            }
        }

        if (!DamageScript.VelocityLock)
        {
            if (canMove)
            {
                rb.linearVelocity = new Vector2(speed * vectorDirecao.x, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, StopRate), rb.linearVelocity.y);
            }

        }
    }

    private void FlipDirecao()
    {
        IdleTimer += Time.deltaTime;
        animator.SetBool(animationstrings.IsIdlePatrulha, true);

        if (IdleTimer > IdleDuracao)
        {
            animator.SetBool(animationstrings.IsIdlePatrulha, false);
            if (WalkDirecao == WalkAbleDirecao.Right)
            {
                WalkDirecao = WalkAbleDirecao.Left;
            }
            else if (WalkDirecao == WalkAbleDirecao.Left)
            {
                WalkDirecao = WalkAbleDirecao.Right;
            }
            else
            {
                Debug.LogError("A direcao definida nao e valida");
            }
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        if (!DamageScript.IsAlive)
        {
            dropInimigo.GenerateDrop();
        }
        else
        {
            rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
            animator.SetBool(animationstrings.IsIdlePatrulha, false);
            contagemStaggerBool = true;
            contagemStagger = 0f;
            StartCoroutine(OnHitPatrulha());
        }
    }

    //Piscando e tomando o HIT
    IEnumerator OnHitPatrulha()
    {
        contagemHit++;
        spriteRenderer.material = newMaterial;
        yield return new WaitForSeconds(0.2f);

        spriteRenderer.material = originalMaterial;
        yield return new WaitForSeconds(0.1f);

        DamageScript.VelocityLock = false;
    }

    IEnumerator ContagemHitAnim()
    {
        animator.SetBool(animationstrings.ContagemHit, true);
        rb.gravityScale = 10;

        yield return new WaitForSeconds(1.5f);

        rb.gravityScale = 1;
        contagemHit = 0;
        animator.SetBool(animationstrings.ContagemHit, false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 origemParede = transform.position + new Vector3(0, 0.5f, 0);
        Vector3 direcaoParede = Vector2.right * vectorDirecao.x;
        Gizmos.DrawRay(origemParede, direcaoParede * 2f);

        Gizmos.color = Color.blue;
        Vector3 origemLacuna = transform.position + new Vector3(vectorDirecao.x * 1.1f, -0.5f, 0);
        Vector3 direcaoLacuna = Vector2.down;
        Gizmos.DrawRay(origemLacuna, direcaoLacuna * 2f);
    }
}
