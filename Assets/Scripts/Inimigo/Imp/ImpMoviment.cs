using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Damage))]
public class ImpMoviment : PlayerPoco
{
    private enum ImpState
    {
        Idle,
        Attack,
        Flee,
        Scream,
        Engage,
        RunningAttack
    }

    [Header("Instances")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TouchingVariables touching;
    [SerializeField] private Damage damageScript;
    [SerializeField] private Item_drop dropInimigo;

    private Vector2 homePosition;

    [Header("Perception")]
    [SerializeField] private float distanceX;

    [SerializeField] private float detectRange = 8f;
    [SerializeField] private float loseSightRange = 12f;
    [SerializeField] private float maxVerticalDistance = 3f;
    [SerializeField] private float lostSightDelay = 1.5f;
    [SerializeField] private float fleeThrowDelay = 2.2f;

    [Header("Distances")]
    [SerializeField] private float closeRange = 1.3f;
    [SerializeField] private float midRange = 4f;
    [SerializeField] private float safeDistance = 6f;
    private bool isApproaching;
    [HideInInspector] public bool isThrowing = false;

    private bool isAttacking;
    private bool isAttackStepping;

    [Header("Movement")]
    [SerializeField] private float chaseSpeed = 2.6f;
    [SerializeField] private float fleeSpeed = 4.2f;
    [SerializeField] private float runAttackSpeed = 6.5f;
    [SerializeField] private float runAttackDuration = 0.35f;
    [SerializeField] private float runAttackDistanceStart = 3.5f;

    [Header("Attacks")]
    [SerializeField] private float lightAttackCooldown = 1.1f;
    [SerializeField] private float throwAttackCooldown = 1.6f;
    [SerializeField] private float runAttackCooldown = 2.4f;
    [SerializeField] private float lightAttackLock = 0.2f;
    [SerializeField] private float throwAttackLock = 0.25f;
    [SerializeField] private float attackDecisionDelay = 0.35f;
    [SerializeField, Range(0f, 1f)] private float runAttackChance = 0.45f;
    [SerializeField] private float screamDuration = 0.8f;

    [Header("Stone Projectile")]
    [SerializeField] private GameObject stoneProjectilePrefab;
    [SerializeField] private Transform stoneSpawnPoint;
    [SerializeField] private float stoneSpeed = 7f;

    [Header("Animator Params")]
    private string moveBool = "IsMoving";
    private string searchBool = "IsSearching";
    private string runBool = "IsRunningAttack";
    private string lightAttackTrigger = "LightAttack";
    private string throwAttackTrigger = "ThrowAttack";
    private string runAttackTrigger = "RunAttack";
    private string screamTrigger = "Scream";

    [SerializeField] private ImpState currentState = ImpState.Idle;
    private float lostSightTimer;
    private float fleeThrowTimer;
    private float lightCooldownTimer;
    private float throwCooldownTimer;
    private float runCooldownTimer;
    private float attackLockTimer;
    private float attackDecisionTimer;
    [SerializeField] private float screamTimer;
    private bool pendingScream;
    private bool pendingPostScreamThrow;
    private bool forceFleeAfterHit;
    private bool hasTakenHitAggro;
    private bool hasScreamedAfterHit;
    private Coroutine runAttackCoroutine;

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

    private void Awake()
    {
        touching = GetComponent<TouchingVariables>();
        damageScript = GetComponent<Damage>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        dropInimigo = GetComponent<Item_drop>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        newMaterial = Resources.Load<Material>("Material/Hit");
    }

    void Start()
    {
        animator.SetBool(searchBool, true);

        homePosition = transform.position;
    }

    private void Update()
    {
        if (!damageScript.IsAlive)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (currentState == ImpState.RunningAttack)
        {
            UpdateAnimatorMovement();
        }

        distanceX = Mathf.Abs(GameManager.instance.player.transform.position.x - transform.position.x);

        if (canMove)
        {
            if (currentState == ImpState.Scream)
            {
                FlipTowards(Mathf.Sign(GameManager.instance.player.transform.position.x - transform.position.x));
            }

            if (currentState == ImpState.Flee)
            {
                FlipTowards(Mathf.Sign(transform.position.x - GameManager.instance.player.transform.position.x));
            }
            else
            {
                if (currentState == ImpState.RunningAttack)
                {
                    return;
                }
                FlipTowards(Mathf.Sign(GameManager.instance.player.transform.position.x - transform.position.x));
            }

            UpdateStateLogic();
            UpdateTimers();
            UpdatePerception();
            UpdateAnimatorMovement();
            if (attackLockTimer <= 0f && currentState != ImpState.RunningAttack)
            {
                isAttacking = false;
            }

            if (isAttacking && !isAttackStepping)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
        else
        {
            if (isThrowing)
            {
                FlipTowards(Mathf.Sign(GameManager.instance.player.transform.position.x - transform.position.x));
            }
        }
    }

    private void FixedUpdate()
    {
        if (!damageScript.IsAlive) return;
        if (!canMove) return;

        if (currentState == ImpState.Flee)
        {
            RunAwayFromPlayer();
            return;
        }

        if (currentState == ImpState.Attack)
        {
            HandleApproach(safeDistance);
            return;
        }

        if (currentState == ImpState.Engage)
        {
            HandleApproach(midRange);
        }

        if (currentState == ImpState.RunningAttack)
        {
            return;
        }
    }

    private void UpdateStateLogic()
    {
        if (currentState == ImpState.RunningAttack)
            return;

        if (forceFleeAfterHit && currentState != ImpState.Flee && currentState != ImpState.Scream)
        {
            currentState = ImpState.Flee;
        }

        if (!pendingScream && !hasTakenHitAggro)
        {
            if (distanceX > 30f)
            {
                ReturnHomePosition();
                return;
            }
        }

        if (currentState == ImpState.Flee)
        {
            if (distanceX >= safeDistance)
            {
                StartScream();
            }
            return;
        }

        if (currentState == ImpState.Scream)
        {
            if (screamTimer <= 0f && pendingScream)
            {
                pendingScream = false;
                if (pendingPostScreamThrow)
                {
                    TriggerThrowAttack();
                    pendingPostScreamThrow = false;
                }
                attackDecisionTimer = 0.4f;
                currentState = ImpState.Engage;
            }
            return;
        }

        if (currentState == ImpState.Attack)
        {
            UpdateFleeThrowTimer();
            TryAttack(distanceX, allowRunAttack: false);
            return;
        }

        if (currentState == ImpState.Engage)
        {
            ResetFleeThrowTimer();
            TryAttack(distanceX, allowRunAttack: true);
        }
    }

    private void UpdateFleeThrowTimer()
    {
        if (distanceX > safeDistance && distanceX <= loseSightRange)
        {
            fleeThrowTimer += Time.deltaTime;
            if (fleeThrowTimer >= fleeThrowDelay)
            {
                TriggerThrowAttack();
                fleeThrowTimer = 0f;
            }
        }
        else
        {
            ResetFleeThrowTimer();
        }
    }

    private void ResetFleeThrowTimer()
    {
        fleeThrowTimer = 0f;
    }

    public void ReturnHomePosition()
    {
        float distanceX = Mathf.Abs(homePosition.x - transform.position.x);

        if (distanceX > 0.5f)
        {
            float direction = Mathf.Sign(homePosition.x - transform.position.x);
            MoveHorizontal(direction, chaseSpeed);
            FlipTowards(direction);
            animator.SetBool(moveBool, true);
        }
        else
        {
            MoveHorizontal(0f, 0f);
            animator.SetBool(moveBool, false);
            animator.SetBool(searchBool, true);
        }
    }

    private void UpdateTimers()
    {
        if (lightCooldownTimer > 0f)
        {
            lightCooldownTimer -= Time.deltaTime;
        }

        if (throwCooldownTimer > 0f)
        {
            throwCooldownTimer -= Time.deltaTime;
        }

        if (runCooldownTimer > 0f)
        {
            runCooldownTimer -= Time.deltaTime;
        }

        if (attackLockTimer > 0f)
        {
            attackLockTimer -= Time.deltaTime;
        }

        if (attackDecisionTimer > 0f)
        {
            attackDecisionTimer -= Time.deltaTime;
        }

        if (screamTimer > 0f)
        {
            screamTimer -= Time.deltaTime;
        }
    }

    private void UpdatePerception()
    {
        float distanceY = Mathf.Abs(GameManager.instance.player.transform.position.y - transform.position.y);
        bool canSeePlayer = distanceX <= detectRange && distanceY <= maxVerticalDistance;

        if (currentState == ImpState.RunningAttack || currentState == ImpState.Scream)
        {
            return;
        }

        if (currentState == ImpState.Idle && canSeePlayer)
        {
            currentState = ImpState.Attack;
            return;
        }

        bool lostSight = distanceX > loseSightRange || distanceY > maxVerticalDistance;

        if (!hasTakenHitAggro)
        {
            if (currentState == ImpState.Attack || currentState == ImpState.Engage)
            {
                if (lostSight)
                {
                    lostSightTimer += Time.deltaTime;

                    if (lostSightTimer >= lostSightDelay)
                    {
                        currentState = ImpState.Idle;
                        lostSightTimer = 0f;
                    }
                }
                else
                {
                    lostSightTimer = 0f;
                }
            }
        }
    }

    private void TryAttack(float distanceX, bool allowRunAttack)
    {
        if (isAttacking)
            return;

        if (attackLockTimer > 0f || damageScript.VelocityLock || attackDecisionTimer > 0f)
            return;

        if (currentState == ImpState.RunningAttack)
            return;

        // CLOSE
        if (distanceX <= closeRange)
        {
            TriggerLightAttack();
            return;
        }

        // MID
        if (distanceX <= midRange)
        {
            if (allowRunAttack && runCooldownTimer <= 0f && distanceX > runAttackDistanceStart && Random.value <= runAttackChance)
            {
                runAttackCoroutine = StartCoroutine(RunAttackRoutine());
                return;
            }

            TriggerThrowAttack();
            return;
        }

        // FAR
        if (allowRunAttack)
        {
            if (runCooldownTimer <= 0f && distanceX > runAttackDistanceStart && Random.value <= runAttackChance)
            {
                runAttackCoroutine = StartCoroutine(RunAttackRoutine());
                return;
            }

            TriggerThrowAttack();
        }
    }

    public void AttackStep(float force = 2f)
    {
        if (!touching.IsGrouded) return;

        isAttackStepping = true;
        damageScript.VelocityLock = true;

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
        damageScript.VelocityLock = false;
        isAttackStepping = false;
    }

    private void TriggerLightAttack()
    {
        if (lightCooldownTimer > 0f)
        {
            return;
        }

        isAttacking = true;
        animator.SetTrigger(lightAttackTrigger);
        lightCooldownTimer = lightAttackCooldown;
        attackLockTimer = lightAttackLock;
        attackDecisionTimer = attackDecisionDelay;
    }

    private void TriggerThrowAttack()
    {
        if (throwCooldownTimer > 0f)
        {
            return;
        }
        // isThrowing = true;

        isAttacking = true;
        animator.SetTrigger(throwAttackTrigger);
        throwCooldownTimer = throwAttackCooldown;
        attackLockTimer = throwAttackLock;
        attackDecisionTimer = attackDecisionDelay;
    }

    private IEnumerator RunAttackRoutine()
    {
        if (currentState == ImpState.RunningAttack)
            yield break;

        currentState = ImpState.RunningAttack;
        isAttacking = true;

        animator.SetBool(runBool, true);

        // 🔴 CAPTURA DIREÇÃO UMA ÚNICA VEZ
        float lockedDirection = Mathf.Sign(
            GameManager.instance.player.transform.position.x - transform.position.x
        );

        // Garante que não seja 0
        if (Mathf.Abs(lockedDirection) < 0.01f)
            lockedDirection = transform.localScale.x > 0 ? -1 : 1;

        float chaseElapsed = 0f;
        float maxChaseDuration = 1.5f;

        while (
            Mathf.Abs(GameManager.instance.player.transform.position.x - transform.position.x)
            > runAttackDistanceStart
            && chaseElapsed < maxChaseDuration)
        {
            rb.linearVelocity = new Vector2(
                lockedDirection * runAttackSpeed,
                rb.linearVelocity.y
            );

            chaseElapsed += Time.deltaTime;
            yield return null;
        }

        animator.SetTrigger(runAttackTrigger);

        float elapsed = 0f;
        while (elapsed < runAttackDuration)
        {
            rb.linearVelocity = new Vector2(
                lockedDirection * runAttackSpeed,
                rb.linearVelocity.y
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        animator.SetBool(runBool, false);

        isAttacking = false;
        runCooldownTimer = runAttackCooldown;
        attackDecisionTimer = attackDecisionDelay;

        isApproaching = true;
        currentState = ImpState.Engage;
        runAttackCoroutine = null;
    }


    private void HandleApproach(float desiredRange)
    {
        float enterRange = desiredRange;
        float exitRange = desiredRange + 6f;

        if (!isApproaching && distanceX > 1.5f && distanceX < enterRange)
        {
            isApproaching = true;
        }
        else if (isApproaching && distanceX > exitRange)
        {
            isApproaching = false;
        }

        if (isApproaching)
        {
            float direction = Mathf.Sign(
                GameManager.instance.player.transform.position.x - transform.position.x
            );

            MoveHorizontal(direction, chaseSpeed);
            animator.SetBool(moveBool, true);
            animator.SetBool(searchBool, false);
        }
        else
        {
            MoveHorizontal(0f, 0f);
            animator.SetBool(moveBool, false);
        }
    }

    private void RunAwayFromPlayer()
    {
        float direction = Mathf.Sign(transform.position.x - GameManager.instance.player.transform.position.x);
        MoveHorizontal(direction, fleeSpeed);
        animator.SetBool(moveBool, true);
    }

    private void StartScream()
    {
        ResetAttackState();
        animator.SetTrigger(screamTrigger);
        animator.SetBool(moveBool, false);
        screamTimer = screamDuration;
        currentState = ImpState.Scream;
        forceFleeAfterHit = false;
        attackLockTimer = screamDuration;
        attackDecisionTimer = screamDuration;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private void ResetAttackState()
    {
        lostSightTimer = 0f;

        isAttacking = false;
        isAttackStepping = false;
        isThrowing = false;

        if (runAttackCoroutine != null)
        {
            StopCoroutine(runAttackCoroutine);
            runAttackCoroutine = null;
        }

        animator.ResetTrigger(lightAttackTrigger);
        animator.ResetTrigger(throwAttackTrigger);
        animator.ResetTrigger(runAttackTrigger);

        animator.SetBool(runBool, false);
    }

    public void SpawnStone()
    {
        if (stoneProjectilePrefab == null)
        {
            return;
        }

        Transform spawnPoint = stoneSpawnPoint != null ? stoneSpawnPoint : transform;
        GameObject stoneInstance = Instantiate(stoneProjectilePrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody2D stoneRb = stoneInstance.GetComponent<Rigidbody2D>();

        if (stoneRb != null)
        {
            float direction = Mathf.Sign(GameManager.instance.player.transform.position.x - spawnPoint.position.x);
            stoneRb.linearVelocity = new Vector2(direction * stoneSpeed, stoneRb.linearVelocity.y);
        }
    }

    private void UpdateAnimatorMovement()
    {
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.05f;
        animator.SetBool(moveBool, isMoving);
    }

    private void MoveHorizontal(float direction, float speed)
    {
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    private void FlipTowards(float direction)
    {
        if (touching != null && !touching.IsGrouded)
            return;

        if (Mathf.Abs(direction) < 0.01f)
            return;

        if (direction < 0f)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        if (!damageScript.IsAlive)
        {
            if (dropInimigo != null)
            {
                dropInimigo.GenerateDrop();
            }
            return;
        }

        if (runAttackCoroutine != null)
        {
            StopCoroutine(runAttackCoroutine);
            currentState = ImpState.Flee;
            animator.SetBool(runBool, false);
            runAttackCoroutine = null;
        }

        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
        if (!hasScreamedAfterHit)
        {
            pendingScream = true;
            pendingPostScreamThrow = true;
            currentState = ImpState.Flee;
            forceFleeAfterHit = true;
            hasTakenHitAggro = true;
            hasScreamedAfterHit = true;
        }
        animator.SetBool(moveBool, false);
        ResetAttackState();
        ResetFleeThrowTimer();

        lightCooldownTimer = 0f;
        throwCooldownTimer = 0f;
        runCooldownTimer = 0f;
        attackLockTimer = 0f;
        attackDecisionTimer = 0f;

        StartCoroutine(OnHitEnemy());
    }

    IEnumerator OnHitEnemy()
    {
        spriteRenderer.material = newMaterial;

        yield return new WaitForSeconds(0.2f);

        spriteRenderer.material = originalMaterial;
    }
}