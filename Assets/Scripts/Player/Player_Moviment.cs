using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Threading.Tasks;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingVariables), typeof(Damage))]
public class PlayerMoviment : MonoBehaviour
{
    public string currentScene;
    public bool AutoMoveAnimations = false;

    [Header("Instances")]
    public float currentZRotation;
    public float maxSpeed = 7f;
    public float speed = 2f;
    public float acelerationSpeed;
    public float airSpeed = 7f;
    public Vector2 moveInput;

    public bool Atacar;
    public bool RecuarAtirar;
    [SerializeField]
    private int numeroDeAttcks;
    public bool Reset = false;
    public float ResetTimer;
    public float ResetTimerLimite;
    private int ataqueCounterAtual;

    //Healing
    [Header("Healing")]
    public potion_script potion_Script;
    public bool healing = false;
    public float healingTimer;

    [HideInInspector]
    public Acorda_Boss acorda_Boss;
    public HealthBar healthBar;

    //Variaveis
    [Header("Variaveis")]
    public Damage DamageScript;
    private int lastHealth;
    private bool isFlashingDamage = false;
    public TouchingVariables touching;
    public bool entrar;
    public bool grabAtivo = false;
    public bool grabAnim = false;

    public Vector2 lastLilithSpawn;
    public GameObject lilithSpawnPoint;
    public RaycastHit2D LastLocationSpawn;
    public float offsetSpawn;

    [HideInInspector]
    public Animator animacao;
    [HideInInspector]
    public Rigidbody2D rb;

    public float RunTiming;
    public float idleTimingRun;
    public float accelerationTimer;

    public SpriteRenderer spriteRenderer;
    public Material originalMaterial;

    //Jump
    [Header("Jump")]
    public bool jumpInput = false;
    public bool IsJumping;
    public float jumpImpulso = 20f;
    public float ContagemJump = 0.05f;

    public float CoyoteTime = 0.2f;
    public float coyoteTimeContador;

    public float jumpBufferTimer = 0.2f;
    public float jumpBufferContador;
    public bool jumpBufferFinal;

    //Sobre o arco
    [Header("Bow")]
    public bool arcoEffect = false;
    public PlayerInput playerInput;
    [HideInInspector] public Bow bow;
    public bool Atirar = false;
    [HideInInspector]
    public bool tempo;
    public float targetTimeScale = 0.3f;
    public float duration = 1f;
    public float elapsedTime = 0f;


    [Header("Wall Slide / Wall Jump")]
    public int facingDirecao = 1;
    public bool wallSlide = false;
    public float WallstateTimer = .4f;
    public bool canJump;
    public bool isWallJumping = false;
    public float wallJumpTimer;

    [Header("Dash States")]
    public bool isDashing;
    public float dashSpeed;
    public float dashDuration;
    public float stateTimerDash;
    public float timerDash;
    public float dashCooldown;
    public ladderScript LadderScript;

    [Header("CameraFollowAnimation")]
    [SerializeField] private GameObject _cameraFollow;
    [SerializeField] public camerafollowObject camerafollowObject;

    [Header("OpenTab")]
    public bool OpenCaderno = false;
    public bool canOpenCaderno = true;

    public float attackCooldown
    {
        get
        {
            return animacao.GetFloat(animationstrings.attackCooldown);
        }
        private set
        {
            animacao.SetFloat(animationstrings.attackCooldown, Mathf.Max(value, 0));
        }
    }

    public float CurrentMoveSpeed
    {
        get
        {
            if (AutoMoveAnimations)
            {
                return maxSpeed;
            }

            if (canMove)
            {
                playerInput.enabled = true;
                if (IsMoving && !touching.IsOnWall)
                {
                    if (touching.IsGrouded && speed <= maxSpeed)
                    {
                        if (arcoEffect)
                        {
                            speed = 4f;
                            airSpeed = 4f;
                        }
                        else if (OpenCaderno)
                        {
                            speed = 0f;
                        }
                        else
                        {
                            speed += Time.deltaTime * acelerationSpeed;
                        }
                        return speed;
                    }
                    else
                    {
                        return airSpeed;
                    }
                }
                else
                {
                    //idle speed é 0;
                    speed = 2f;
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }


    [SerializeField]
    private bool _IsMoving = false;

    public bool IsMoving
    {
        get
        {
            return _IsMoving;
        }
        set
        {
            _IsMoving = value;
            animacao.SetBool(animationstrings.IsMoving, value);
        }
    }

    [SerializeField]
    private bool _IsRunning = false;

    public bool IsRunning
    {
        get
        {
            return _IsRunning;
        }
        set
        {
            _IsRunning = value;
            animacao.SetBool("IsRunning", value);
        }
    }

    [SerializeField]
    private bool _IsAcordada = false;

    public bool IsAcordada
    {
        get
        {
            return _IsAcordada;
        }
        set
        {
            _IsAcordada = value;
            animacao.SetBool(animationstrings.IsAcordada, value);
        }
    }

    public bool _IsRight = true;
    public bool IsRight
    {
        get
        {
            return _IsRight;
        }
        set
        {
            if (_IsRight != value)
            {
                // Flipa para a posicao oposta
                transform.localScale *= new Vector2(-1, 1);
            }
            _IsRight = value;
        }
    }

    public bool canMove
    {
        get
        {
            return animacao.GetBool(animationstrings.canMove);
        }
        set
        {
            animacao.SetBool(animationstrings.canMove, value);
        }
    }

    public bool IsAlive
    {
        get
        {
            return animacao.GetBool(animationstrings.IsAlive);
        }
        set
        {
            animacao.SetBool(animationstrings.IsAlive, value);
        }
    }

    // Chamado antes do start, isso
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animacao = GetComponent<Animator>();
        touching = GetComponent<TouchingVariables>();
        DamageScript = GetComponent<Damage>();
        playerInput = GetComponent<PlayerInput>();
        acorda_Boss = GameObject.FindFirstObjectByType<Acorda_Boss>();
        bow = GameObject.FindFirstObjectByType<Bow>();
        healthBar = GameObject.FindFirstObjectByType<HealthBar>();
        potion_Script = GameObject.FindFirstObjectByType<potion_script>();
        _cameraFollow = GameObject.FindGameObjectWithTag("CameraFollow");
        LadderScript = GameObject.FindFirstObjectByType<ladderScript>();

        camerafollowObject = _cameraFollow.GetComponent<camerafollowObject>();
        stateTimerDash = dashDuration;

        //saber qual cena o jogador esta.
        currentScene = SceneManager.GetActiveScene().name;
        Debug.Log("Nome da cena atual: " + currentScene);
    }

    void Start()
    {
        lastHealth = DamageScript.Health;
        if (GameManager.instance != null)
        {
            GameManager.instance.FullScreenDamageMaterial.SetFloat("_IsPulseActive", 0);   
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    private void Update()
    {
        if (!DamageScript.IsAlive)
        {
            return;
        }

        SaveData.Instance.playTime += Time.deltaTime;
        animacao.SetInteger(animationstrings.counterAtt, ataqueCounterAtual);
        animacao.SetBool("OpenTab", OpenCaderno);

        LastLocationSpawn = Physics2D.Raycast(lilithSpawnPoint.transform.position, Vector2.down, 0.4f, LayerMask.GetMask("Ground"));
        Debug.DrawRay(lilithSpawnPoint.transform.position, Vector2.down * 0.4f, Color.red);

        if (!canMove)
        {
            playerInput.enabled = false;
        }

        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        //Dash cooldown
        timerDash -= Time.deltaTime;
        //Tempo do dash
        if (isDashing)
        {
            stateTimerDash -= Time.deltaTime;
            if (stateTimerDash < 0f)
            {
                animacao.SetBool(animationstrings.isDashing, false);
                isDashing = false;
                stateTimerDash = dashDuration;
                rb.gravityScale = 4.5f;
                playerInput.enabled = true;
            }
            else if (touching.IsOnWall)
            {
                stateTimerDash = 0f;
            }
        }


        if (touching.IsGrouded)
        {
            isWallJumping = false;
        }
        if (LastLocationSpawn.collider != null)
        {
            lastLilithSpawn = transform.position;
            offsetSpawn = transform.localScale.x;
        }

        canJump = (touching.IsOnWall && wallSlide) ? true : false;

        WallSlide();

        currentZRotation = bow.transform.eulerAngles.z;

        if (tempo)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / duration;
            Time.timeScale = Mathf.Lerp(1f, targetTimeScale, t);
            // Debug.Log("Current TimeScale: " + Time.timeScale);
        }

        if (Reset)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ataqueCounterAtual++;
                ResetTimer = 0f;
            }

            if (ataqueCounterAtual > 0)
            {
                ResetTimer += Time.deltaTime;

                if (ResetTimer >= ResetTimerLimite)
                {
                    Reset = false;
                    ResetTimer = 0;
                    ataqueCounterAtual = 0;
                    Debug.Log("Combo Resetado por tempo");
                }
            }
        }

        if (elapsedTime >= 4 || Input.GetMouseButtonDown(1) && RecuarAtirar)
        {
            arcoEffect = false;
            //ARCO arrumar
            bow.bodyCamera = false;
            bow.transposer.m_TrackedObjectOffset = new Vector3(0, 0, bow.transposer.m_TrackedObjectOffset.z);

            animacao.SetBool(animationstrings.Powers, false);
            bow.playerArco.gameObject.SetActive(false);
            bow.gameObject.SetActive(false);

            GetComponent<SpriteRenderer>().enabled = true;
            Time.timeScale = 1f;
            tempo = false;
            elapsedTime = 0f;
            bow.NewArrow = null;
            RecuarAtirar = false;
            foreach (var nuss in bow.points)
            {
                nuss.SetActive(false);
            }
        }
        if (elapsedTime >= 2f)
        {
            RecuarAtirar = true;
        }

        if (healing == true)
        {
            healingTimer -= Time.deltaTime;
            if (healingTimer <= 0)
            {
                healing = false;
                healingTimer = 2;
            }
        }

        if (IsMoving && !isDashing && !wallSlide)
        {
            if (arcoEffect)
            {
                RunTiming = 0f;
            }
            idleTimingRun = 0f;
            RunTiming += Time.deltaTime;
            if (RunTiming >= 2.0f)
            {
                maxSpeed = 10f;
                accelerationTimer += Time.deltaTime;
                float t = Mathf.Clamp01(accelerationTimer / 1f);
                airSpeed = Mathf.Lerp(airSpeed, maxSpeed, t * t);

                speed = Mathf.Lerp(speed, maxSpeed, t * t);
                IsRunning = true;
            }
            else
            {
                speed = 7f;
                airSpeed = 7f;
                maxSpeed = 7f;
                Debug.Log("Está em movimento");
            }
        }
        else
        {
            idleTimingRun += Time.deltaTime;
            if (idleTimingRun > 0.15f)
            {
                RunTiming = 0f;
                accelerationTimer = 0f;
                airSpeed = 7f;
                maxSpeed = 7f;
                IsRunning = false;
            }
        }

        if (!isFlashingDamage && DamageScript.Health != lastHealth)
        {
            if (DamageScript.Health <= 1 && DamageScript.IsAlive)
            {
                GameManager.instance.FullScreenDamageMaterial.SetFloat("_IsPulseActive", 1);
            }
            else
            {
                GameManager.instance.FullScreenDamageMaterial.SetFloat("_IsPulseActive", 0);
            }

            lastHealth = DamageScript.Health;
        }
    }

    private void FixedUpdate()
    {
        if (!DamageScript.VelocityLock)
        {
            if (isWallJumping)
            {
                wallJumpTimer -= Time.deltaTime;
                if (wallJumpTimer < 0f)
                {
                    isWallJumping = false;
                    wallJumpTimer = 0.1f;
                    speed = 2f;
                }
            }

            if (!isWallJumping && !isDashing)
            {
                rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y);
            }

            if (touching.IsGrouded && rb.linearVelocity.y <= 0f)
            {
                IsJumping = false;
                coyoteTimeContador = CoyoteTime;
            }
            else
            {
                coyoteTimeContador -= Time.deltaTime;
            }

            if (jumpBufferFinal)
            {
                jumpBufferContador -= Time.deltaTime;
                if (jumpBufferContador <= 0f)
                {
                    jumpBufferFinal = false;
                }
            }
        }

        if (jumpBufferFinal && touching.IsGrouded && !wallSlide && jumpInput)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulso);
            jumpBufferFinal = false;
        }

        animacao.SetFloat(animationstrings.yVelocity, rb.linearVelocity.y);

        var currentStateInfo = animacao.GetCurrentAnimatorStateInfo(0);
        if (currentStateInfo.IsName("FilhotePegar") && currentStateInfo.normalizedTime >= 0.9f)
        {
            animacao.SetBool("IsFilhote", false);
        }
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive)
        {
            if (!OpenCaderno)
            {
                IsMoving = moveInput != Vector2.zero;

                setDirection(moveInput);
            }
            else
            {
                moveInput = Vector2.zero;
                IsMoving = false;
            }
        }
        else
        {
            IsMoving = false;
        }
    }

    private void setDirection(Vector2 moveInput)
    {
        facingDirecao = transform.localScale.x == 1 ? 1 : -1;

        if (moveInput.x > 0 && !IsRight)
        {
            IsRight = true;
            camerafollowObject.chamarTurn();
        }
        else if (moveInput.x < 0 && IsRight)
        {
            IsRight = false;
            camerafollowObject.chamarTurn();
        }
    }


    public void OnDash(InputAction.CallbackContext context)
    {
        if (SaveData.Instance.DashUnlocked)
        {
            if (context.started && timerDash < 0f)
            {
                if (!arcoEffect && !OpenCaderno)
                {
                    isDashing = true;
                    playerInput.enabled = false;
                    animacao.SetBool(animationstrings.isDashing, true);
                    timerDash = dashCooldown;
                    DamageScript.isInvicible = true;

                    rb.linearVelocity = new Vector2(dashSpeed * facingDirecao, 0);

                    rb.gravityScale = 0f;
                }
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && !OpenCaderno)
        {
            jumpInput = true;

            if (coyoteTimeContador > 0f || jumpBufferFinal)
            {
                Jump();
            }

            if (isDashing)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.7f);
            }
        }

        if (!touching.IsGrouded)
        {
            jumpBufferFinal = true;
            jumpBufferContador = jumpBufferTimer;
        }

        if (context.canceled)
        {
            jumpInput = false;

            if (rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Lerp(rb.linearVelocity.y, 0f, 0.5f));
            }
        }
    }

    private void Jump()
    {
        animacao.SetTrigger(animationstrings.jump);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulso);
        coyoteTimeContador = 0f;
        IsJumping = true;
    }

    private void WallSlide()
    {
        if (SaveData.Instance.WalljumpUnlocked)
        {
            if (!touching.IsGrouded && rb.linearVelocity.y < 0f && touching.IsOnWall)
            {
                if (!arcoEffect && !OpenCaderno)
                {
                    WallstateTimer -= Time.deltaTime;
                    wallSlide = true;
                    animacao.SetBool(animationstrings.IsWallSliding, true);
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.3f);

                    if (WallstateTimer < 0f && Input.GetKeyDown(KeyCode.W))
                    {
                        WallJump();
                    }
                }
            }
            else
            {
                wallSlide = false;
                animacao.SetBool(animationstrings.IsWallSliding, false);
                WallstateTimer = 0.2f;
            }
        }
    }

    private void WallJump()
    {
        if (Input.GetKeyDown(KeyCode.W) && touching.IsOnWall && !touching.IsGrouded)
        {
            float jumpDirection = (facingDirecao == 1) ? -1 : 1;
            rb.linearVelocity = new Vector2(jumpDirection * 7f, jumpImpulso);

            coyoteTimeContador = 0f;
            IsJumping = false;
            isWallJumping = true;
            animacao.SetTrigger(animationstrings.jump);
            Debug.Log("WallJump executado com direção: " + jumpDirection);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (SaveData.Instance.powerUps.Contains(PowerUps.Bastao) && IsAlive)
        {
            if (context.performed && !arcoEffect && !OpenCaderno)
            {
                Atacar = true;
                if (touching.IsGrouded)
                {
                    Reset = true;
                }
                else
                {
                    if (Input.GetKey(KeyCode.S))
                    {
                        animacao.SetTrigger("UpwardTrigger");
                        Debug.LogWarning("ENTRANDO NO ATTACK DE CIMA");
                    }
                    if (Input.GetKey(KeyCode.W) && !touching.IsGrouded)
                    {
                        animacao.SetTrigger("DownwardTrigger");
                        Debug.LogWarning("ENTRANDO NO ATTACK DE BAIXO");
                    }
                }
                animacao.SetTrigger(animationstrings.attack);
            }
        }
    }

    public void OnPowers(InputAction.CallbackContext context)
    {
        if (context.started && !OpenCaderno)
        {
            //Para o Arco.
            if (SaveData.Instance.powerUps.Contains(PowerUps.Arco) && !wallSlide)
            {
                if (bow.NewArrow == null)
                {
                    arcoEffect = true;
                    IsRunning = false;
                    GetComponent<SpriteRenderer>().enabled = false;
                    tempo = true;
                    bow.bodyCamera = true;
                    animacao.SetBool(animationstrings.Powers, true);
                    bow.gameObject.SetActive(true);

                    bow.playerArco.gameObject.SetActive(true);
                    bow.cinemachineVirtualCamera.LookAt = bow.FollowArco;
                }
            }
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        //KNOCKBACK.
        if (DamageScript.Health != 1)
        {
            StartCoroutine(FlashPulseDamage());
        }
        else
        {
            StopCoroutine(FlashPulseDamage());
        }

        GameManager.instance.FecharHUD();
        CancelInvoke(nameof(AbrirHUDelay));
        canOpenCaderno = true;
        OpenCaderno = false;

        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
        GameManager.instance.shakeCamera.ShakeHitDamage();

        if (DamageScript.IsAlive)
        {
            //JUMP
            IsJumping = false;

            //ARCO
            bow.bodyCamera = false;
            bow.newOffset = new Vector3(0, 0, 0);

            animacao.SetBool(animationstrings.Powers, false);
            bow.playerArco.gameObject.SetActive(false);
            bow.gameObject.SetActive(false);
            Time.timeScale = 1f;
            tempo = false;
            elapsedTime = 0f;
            bow.NewArrow = null;
            foreach (var nuss in bow.points)
            {
                nuss.SetActive(false);
            }
        }
        else
        {
            GetComponent<PlayerItemDrop>().GenerateDrop();
        }

        StartCoroutine(OnHitPlayer());
    }

    IEnumerator OnHitPlayer()
    {
        originalMaterial.SetFloat("_HitIntensity", 0.7f);
        yield return new WaitForSeconds(0.1f);
        originalMaterial.SetFloat("_HitIntensity", 0f);
    }

    IEnumerator FlashPulseDamage()
    {
        isFlashingDamage = true;
        GameManager.instance.FullScreenDamageMaterial.SetFloat("_IsPulseActive", 1);
        yield return new WaitForSeconds(0.35f);
        isFlashingDamage = false;

        // Se o player ainda estiver com 1 de vida, mantemos o pulse ativo
        if (DamageScript.Health <= 1 && DamageScript.IsAlive)
        {
            GameManager.instance.FullScreenDamageMaterial.SetFloat("_IsPulseActive", 1);
        }
        else
        {
            GameManager.instance.FullScreenDamageMaterial.SetFloat("_IsPulseActive", 0);
        }
    }


    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.started && !arcoEffect && !OpenCaderno)
        {
            entrar = true;
        }
        else
        {
            entrar = false;
        }
    }

    public void OnHealing(InputAction.CallbackContext context)
    {
        if (context.started && touching.IsGrouded && healingTimer >= 2)
        {
            if (!arcoEffect && !OpenCaderno)
            {
                //Logica para slider
                if (DamageScript.Health == DamageScript.maxHealth || potion_Script.potionInt <= 0)
                {
                    //Logica ainda para 
                    //Ver mais tarde no projeto
                    Debug.Log("Nada curado");
                }
                else
                {
                    healing = true;
                    potion_Script.HealigMetod();
                    animacao.SetBool(animationstrings.IsHealing, true);
                    DamageScript.Health++;
                }
            }
        }
    }

    public void OpenTab(InputAction.CallbackContext context)
    {
        if (!DamageScript.IsAlive)
        {
            Debug.LogWarning("Impossível concluir operação OpenTab");
            return;
        }

        // Quando o botão for pressionado
        if (context.started && touching.IsGrouded && !arcoEffect)
        {
            if (canOpenCaderno)
            {
                OpenCaderno = !OpenCaderno;

                if (OpenCaderno && canOpenCaderno)
                {
                    Debug.Log("Abrindo inventário!!!");
                    Invoke(nameof(AbrirHUDelay), 1.7f);

                    //Opacidade do canvas group em 0.7 segundos, fade out:
                    canOpenCaderno = false;
                    StartCoroutine(GameManager.instance.FadeOutCanvasGroup(GameManager.instance.GUI.GetComponent<CanvasGroup>(), 0.7f));
                }
                else
                {
                    Debug.Log("Fechando inventário!!!");
                    GameManager.instance.FecharHUD();

                    //Opacidade do canvas group em 0.7 segundos, fade in:
                    StartCoroutine(GameManager.instance.FadeInCanvasGroup(GameManager.instance.GUI.GetComponent<CanvasGroup>(), 0.7f));
                }
            }
        }
    }

    public async void AbrirHUDelay()
    {
        GameManager.instance.AbrirHUD();
        await Task.Delay(500);
        canOpenCaderno = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "RicochetePlayer":
                float direction = transform.localScale.x > 0 ? 1 : transform.localScale.x == 0 ? 1 : -1;
                float forceItem = collision.gameObject.GetComponent<RicocheteForcePlayer>().forceItem;

                Vector2 force = new Vector2(maxSpeed * direction, 11f * forceItem);
                rb.AddForce(force, ForceMode2D.Impulse);
                break;
            default:
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Espinhos":
                if (DamageScript.IsAlive)
                {
                    canMove = false;
                    DamageScript.invicibilityTimer = 3f;
                    animacao.SetBool("DanoEspinhosBool", true);
                    animacao.SetTrigger("DanoEspinhos");
                    rb.constraints = RigidbodyConstraints2D.FreezeAll;
                    rb.linearVelocity = Vector2.zero;
                    rb.gravityScale = 0;

                    Invoke(nameof(ResetConstraints), 2f);
                }
                else
                {
                    Debug.Log("Player já está morto, não pode receber dano de espinhos.");
                }
                break;
            default:
                break;
        }
    }

    public void ResetConstraints()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 4.5f;

        DamageScript.invicibilityTimer = 0.25f;
        Vector2 offset = new Vector2(-1.7f * offsetSpawn, 0.5f);
        transform.position = lastLilithSpawn + offset;

        animacao.SetBool("DanoEspinhosBool", true);
    }

    public void PlayerXPRewards(int xp)
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.AddXP(xp);
            Debug.Log("XP Adicionado!");
        }
        else
        {
            Debug.LogWarning("GameManager não encontrado!");
        }
    }
}