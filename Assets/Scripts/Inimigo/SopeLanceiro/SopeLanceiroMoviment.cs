using System;
using System.Collections;
using UnityEngine;

public class SopeLanceiroMoviment : PlayerPoco, IBlockDamage
{
    [Header("Variaveis de Ambiente")]
    TouchingVariables touching;
    Damage DamageScript;
    Rigidbody2D rb;
    public Animator animator;
    private Item_drop dropInimigo;
    public bool isWalking;
    public GameObject EscudoGameObject;

    [Header("Variaveis de Movimento")]
    private Vector2 homePosition;
    public float distanciaXWalk;
    private float direcao;
    public float speed;

    public float timerBackToHome;
    public bool returnedToHome = false;

    [Header("Ataque")]
    private bool counterAttackPending;

    private float timeToAttack = 0.5f;
    private float attackTimer;
    private float attackRange = 2.85f;
    private bool isAttacking;
    private bool isAttackStepping;
    private float distanciaEncarar = 2.7f;

    public enum AtaqueEscolhido
    {
        ShieldAttack,
        LancaAttack
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
        DamageScript = GetComponent<Damage>();
        touching = GetComponent<TouchingVariables>();
        dropInimigo = GetComponent<Item_drop>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        newMaterial = Resources.Load<Material>("Material/Hit");

        EscudoGameObject.SetActive(false);
        homePosition = transform.position;
    }

    void Update()
    {
        animator.SetBool("IsGrouded", touching.IsGrouded);
        animator.SetBool("isWalking", isWalking);

        if (!DamageScript.IsAlive)
        {
            EscudoGameObject.SetActive(false);
            return;
        }

        if (isAttacking && !isAttackStepping)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        distanciaXWalk = Mathf.Abs(transform.position.x - GameManager.instance.player.transform.position.x);
        direcao = Mathf.Sign(GameManager.instance.player.transform.position.x - transform.position.x);

        if (canMove)
        {
            if (!DamageScript.VelocityLock)
            {
                //Moviment Área.
                if (distanciaXWalk > 1.0f && distanciaXWalk < 10f)
                {
                    timerBackToHome = 0f;
                    returnedToHome = false;
                    if (distanciaXWalk < distanciaEncarar)
                    {
                        isWalking = false;
                        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

                        TravarCorpo();
                        FlipDirecao();
                    }
                    else
                    {
                        PerseguirPlayer();
                        LiberarCorpo();
                    }
                }
                else
                {
                    StateIdle();
                }

                //Ataque Área.
                if (PlayerNaFrente() && !isAttacking)
                {
                    EscudoGameObject.SetActive(true);
                    if (counterAttackPending)
                    {
                        counterAttackPending = false;
                        animator.SetTrigger("ShieldAttack");
                        return;
                    }

                    attackTimer += Time.deltaTime;
                    if (attackTimer >= timeToAttack)
                    {
                        EscolherAtaque();
                        attackTimer = 0f;
                        EscudoGameObject.SetActive(false);
                    }

                    return;
                }
                else
                {
                    attackTimer = 0f;
                }

            }
        }
    }

    void PerseguirPlayer()
    {
        isWalking = true;
        rb.linearVelocity = new Vector2(direcao * speed, rb.linearVelocity.y);
        FlipDirecao();
    }

    public void StateIdle()
    {
        EscudoGameObject.SetActive(true);
        timerBackToHome += Time.deltaTime;

        if (timerBackToHome < 2f)
        {
            isWalking = false;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float distanciaHome = Mathf.Abs(transform.position.x - homePosition.x);

        if (distanciaHome > 0.2f)
        {
            isWalking = true;
            float direcaoHome = Mathf.Sign(homePosition.x - transform.position.x);
            rb.linearVelocity = new Vector2(direcaoHome * speed, rb.linearVelocity.y);

            // Flip olhando pro home
            transform.localScale = new Vector3(direcaoHome > 0 ? -1 : 1, 1, 1);
        }
        else
        {
            // Chegou em casa
            isWalking = false;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            returnedToHome = true;
            timerBackToHome = 0f;
            FlipDirecao();
        }
    }

    private void FlipDirecao()
    {
        if (touching.IsGrouded)
        {
            if (transform.position.x > GameManager.instance.player.transform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    bool PlayerNaFrente()
    {
        float direcaoPlayer = Mathf.Sign(GameManager.instance.player.transform.position.x - transform.position.x);
        float direcaoInimigo = transform.localScale.x > 0 ? -1 : 1;

        float distancia = Mathf.Abs(GameManager.instance.player.transform.position.x - transform.position.x);

        return direcaoPlayer == direcaoInimigo && distancia <= attackRange;
    }

    void EscolherAtaque()
    {
        isAttacking = true;

        int ataqueEscolhido = UnityEngine.Random.Range(0, 2);

        switch (ataqueEscolhido)
        {
            case 0:
                animator.SetTrigger("ShieldAttack");
                break;
            case 1:
                animator.SetTrigger("LancaAttack");
                break;
        }
    }

    public void FinalizarAtaque()
    {
        isAttacking = false;
        attackTimer = 0f;
        TravarCorpo();
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

            if (PlayerNaFrente() && distanciaXWalk <= attackRange + 0.5f)
            {
                counterAttackPending = true;
            }
            FinalizarAtaque();
        }
        StartCoroutine(OnHitEnemy());
    }

    IEnumerator OnHitEnemy()
    {
        spriteRenderer.material = newMaterial;

        yield return new WaitForSeconds(0.2f);

        spriteRenderer.material = originalMaterial;
    }

    public void AttackStep(float force = 2f)
    {
        LiberarCorpo();
        if (!touching.IsGrouded) return;


        isAttackStepping = true;
        DamageScript.VelocityLock = true;

        float direction = transform.localScale.x > 0 ? -1 : 1;
        Vector2 attackDir = new Vector2(direction, 0f);

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(attackDir * force, ForceMode2D.Impulse);

        StartCoroutine(StopAttackStep(0.12f));
    }


    private IEnumerator StopAttackStep(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        DamageScript.VelocityLock = false;
    }

    public bool IsBlockingAttack(Transform attacker)
    {
        float enemyDir = transform.localScale.x > 0 ? -1 : 1;

        float attackerDir = Mathf.Sign(attacker.position.x - transform.position.x);

        bool frontAttack = attackerDir == enemyDir;

        bool attackFromAbove = attacker.position.y > transform.position.y + 0.5f;
        return frontAttack && !attackFromAbove;
    }

    public bool CanBlock(Transform attacker)
    {
        // Direção que o inimigo está olhando
        float enemyDir = transform.localScale.x > 0 ? -1 : 1;

        // Direção do atacante
        float attackerDir = Mathf.Sign(attacker.position.x - transform.position.x);

        bool frontAttack = attackerDir == enemyDir;
        bool attackFromAbove = attacker.position.y > transform.position.y + 0.5f;
        return frontAttack && !attackFromAbove;
    }

    public void OnBlock()
    {
        EscudoGameObject.SetActive(true);

        animator.SetTrigger("ShieldBlock");
        // - tocar som
        // - gerar partículas
    }

    void TravarCorpo()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionX
                       | RigidbodyConstraints2D.FreezeRotation;
    }

    void LiberarCorpo()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}