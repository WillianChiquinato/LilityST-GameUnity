using System.Collections;
using UnityEngine;

public class FadaMoviment : PlayerPoco
{
    Rigidbody2D rb;
    Animator animator;
    Damage DamageScript;
    public enum State { Orbit, Charging, Dash, Returning }
    public State currentState;
    private Item_drop dropInimigo;

    [Header("Orbit")]
    public float orbitRadius = 3f;
    public float orbitSpeed = 2f;
    public float minHeight = 2f;
    public float heightVariation = 0.5f;
    public float orbitSmoothness = 4f;

    [Header("Floating")]
    public float floatAmplitude = 0.3f;
    public float floatFrequency = 2f;
    float floatTime;

    [Header("Dash")]
    public float chargeTime = 0.4f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 2f;
    public float returnSpeed = 5f;
    public bool isDashing = false;

    [Header("Variables")]
    private Vector2 homePosition;
    public float distanciaXWalk;
    public float timerBackToHome;
    public bool returnedToHome = false;

    float angle;
    float dashTimer;
    float cooldownTimer;
    float chargeTimer;
    Vector2 dashDirection;
    Vector2 chargePosition;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        DamageScript = GetComponent<Damage>();
        dropInimigo = GetComponent<Item_drop>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        newMaterial = Resources.Load<Material>("Material/Hit");
    }

    void Start()
    {
        currentState = State.Orbit;
        angle = Random.Range(0f, Mathf.PI);

        homePosition = transform.position;
    }

    void Update()
    {
        if (!DamageScript.IsAlive)
        {
            rb.gravityScale = 2.1f;
            return;
        }

        if (!isDashing)
        {
            FlipDirecao();
        }

        distanciaXWalk = Mathf.Abs(transform.position.x - GameManager.instance.player.transform.position.x);

        if (distanciaXWalk < 10f)
        {
            timerBackToHome = 0f;
            returnedToHome = false;

            switch (currentState)
            {
                case State.Orbit:
                    Orbit();
                    break;

                case State.Charging:
                    Charging();
                    break;

                case State.Dash:
                    Dash();
                    break;

                case State.Returning:
                    Returning();
                    break;
            }
        }
        else
        {
            StartIdle();
        }
    }

    public void StartIdle()
    {
        timerBackToHome += Time.deltaTime;

        if (timerBackToHome >= 2f && !returnedToHome)
        {
            returnedToHome = true;
        }

        if (returnedToHome)
        {
            float distanciaHome = Vector2.Distance(rb.position, homePosition);

            if (distanciaHome > 0.2f)
            {
                Vector2 directionToHome = (homePosition - rb.position).normalized;
                rb.linearVelocity = directionToHome * returnSpeed;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    void Orbit()
    {
        angle += orbitSpeed * Time.deltaTime;

        if (angle > Mathf.PI)
        {
            angle = 0f;
        }

        Vector2 playerPos = GameManager.instance.player.transform.position;
        float horizontalOffset = Mathf.Cos(angle) * orbitRadius;
        floatTime += Time.deltaTime;

        float baseHeight = minHeight;
        float floating = Mathf.Sin(floatTime * floatFrequency) * floatAmplitude;
        float height = baseHeight + floating;

        Vector2 targetPos = new Vector2(
            playerPos.x + horizontalOffset,
            playerPos.y + height
        );

        Vector2 move = (targetPos - rb.position) * orbitSmoothness;
        rb.linearVelocity = move;

        cooldownTimer += Time.deltaTime;

        if (cooldownTimer >= dashCooldown)
        {
            StartCharging();
        }
    }

    void StartCharging()
    {
        currentState = State.Charging;
        chargeTimer = 0f;
        chargePosition = rb.position;
        rb.linearVelocity = Vector2.zero;

        Vector2 playerPos = GameManager.instance.player.transform.position;
        dashDirection = (playerPos - rb.position).normalized;
    }

    void Charging()
    {
        chargeTimer += Time.deltaTime;

        Vector2 pullBackPos =
            chargePosition - dashDirection * 0.5f * (chargeTimer / chargeTime);

        rb.position = Vector2.Lerp(
            chargePosition,
            pullBackPos,
            chargeTimer / chargeTime
        );

        if (chargeTimer >= chargeTime)
        {
            StartDash();
        }
    }

    void StartDash()
    {
        animator.SetTrigger("AttackMode");

        currentState = State.Dash;
        dashTimer = 0f;
        cooldownTimer = 0f;

        Vector2 playerPos = GameManager.instance.player.transform.position;
        dashDirection = (playerPos - rb.position).normalized;
    }

    void Dash()
    {
        isDashing = true;
        rb.linearVelocity = dashDirection * dashSpeed;

        dashTimer += Time.deltaTime;

        if (dashTimer >= dashDuration)
        {
            currentState = State.Returning;
            isDashing = false;
        }
    }

    void Returning()
    {
        animator.ResetTrigger("AttackMode");
        Vector2 playerPos = GameManager.instance.player.transform.position;

        float horizontalOffset = Mathf.Cos(angle) * orbitRadius;
        floatTime += Time.deltaTime;

        float floating = Mathf.Sin(floatTime * floatFrequency) * floatAmplitude;
        float height = minHeight + floating;

        Vector2 orbitPos = new Vector2(
            playerPos.x + horizontalOffset,
            playerPos.y + height
        );

        rb.linearVelocity = (orbitPos - rb.position) * returnSpeed;

        if (Vector2.Distance(rb.position, orbitPos) < 0.5f)
        {
            currentState = State.Orbit;
        }
    }

    private void FlipDirecao()
    {
        if (DamageScript.IsAlive)
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

    public void OnHit(int damage, Vector2 knockback)
    {
        if (!DamageScript.IsAlive)
        {
            dropInimigo.GenerateDrop();
        }
        else
        {
            rb.gravityScale = 0.2f;
            rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
        }
        StartCoroutine(OnHitEnemy());
    }

    IEnumerator OnHitEnemy()
    {
        spriteRenderer.material = newMaterial;

        yield return new WaitForSeconds(0.2f);

        spriteRenderer.material = originalMaterial;
        rb.gravityScale = 0f;
    }
}
