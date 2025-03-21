using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lagartin_Moviment : PlayerPoco
{
    [Header("Instancias")]
    private Item_drop dropInimigo;
    public PlayerMoviment playerMoviment;
    TouchingDistance touching;

    public DetectionLagartin attackZona1;
    public GameObject attackDetector;
    public GameObject attackLagartin;
    Damage DamageScript;
    Rigidbody2D rb;
    public Animator animator;


    [Header("Variaveis")]
    public float direcao;
    public float speed;
    public float distanciaAttack;
    public bool perseguir;


    public bool _Target = false;

    public bool Target
    {
        get
        {
            return _Target;
        }
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

    void Start()
    {
        playerMoviment = GameObject.FindAnyObjectByType<PlayerMoviment>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        DamageScript = GetComponent<Damage>();
        touching = GetComponent<TouchingDistance>();
        dropInimigo = GetComponent<Item_drop>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        newMaterial = Resources.Load<Material>("Material/Hit");
    }

    void Update()
    {
        distanciaAttack = Mathf.Abs(transform.position.x - playerMoviment.transform.position.x);
        direcao = Mathf.Sign(playerMoviment.transform.position.x - transform.position.x);

        Target = attackZona1.detectColliders.Count > 0;

        if (DamageScript.IsAlive)
        {
            if (attackZona1.perseguir == true)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                animator.SetBool(animationstrings.Comeco, true);
            }
            if (touching.IsGrouded && canMove)
            {
                if (!DamageScript.VelocityLock)
                {
                    attackDetector.SetActive(false);
                    if (distanciaAttack < 20)
                    {
                        rb.linearVelocity = new Vector2(direcao * speed, rb.linearVelocity.y);
                    }
                }
            }
            FlipDirecao();
        }
        else
        {
            attackLagartin.SetActive(false);
            rb.gravityScale = 2f;
        }
    }

    private void FlipDirecao()
    {
        if (touching.IsGrouded)
        {
            if (transform.position.x > playerMoviment.transform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
                animator.SetTrigger(animationstrings.DireitaLagartin);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
                animator.SetTrigger(animationstrings.DireitaLagartin);
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
        }
        StartCoroutine(OnHitEnemy());
    }

    IEnumerator OnHitEnemy()
    {
        spriteRenderer.material = newMaterial;

        yield return new WaitForSeconds(0.2f);

        spriteRenderer.material = originalMaterial;
    }
}
