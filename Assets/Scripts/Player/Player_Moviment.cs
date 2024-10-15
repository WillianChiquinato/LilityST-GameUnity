using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDistance), typeof(Damage))]
public class PlayerMoviment : MonoBehaviour
{
    public static PlayerMoviment Instance { get; private set; }
    public float currentZRotation;
    public bool isFollowing = true;

    //Healing
    public potion_script potion_Script;
    public bool healing = false;
    public float healingTimer;

    [HideInInspector]
    public Acorda_Boss acorda_Boss;
    public SavePoint savePoint;
    public HealthBar healthBar;

    [HideInInspector]
    public PlayerInput playerInput;
    Bow bow;

    public bool entrar;

    //Variaveis
    [HideInInspector]
    public Animator animacao;
    [HideInInspector]
    public Rigidbody2D rb;
    public Damage DamageScript;
    public TouchingDistance touching;

    public float speed = 7f;
    public float airSpeed = 7f;


    //Jump
    public bool IsJumping;
    public float jumpImpulso = 20f;
    public float ContagemJump = 0.05f;

    public float CoyoteTime = 0.2f;
    public float coyoteTimeContador;

    public float jumpBufferTimer = 0.2f;
    public float jumpBufferContador;
    public bool jumpBufferFinal;


    //Sobre o arco
    public bool Atirar = false;
    [HideInInspector]
    public bool tempo;
    public float targetTimeScale = 0.3f;
    public float duration = 1f;
    public float elapsedTime = 0f;
    public float tempoCooldown;


    public bool canWallJump = true;
    public int facingDirecao = 1;
    private bool isWallSliding;
    public float wallSlidingSpeed;
    [SerializeField]
    private Vector2 wallJumpDirecao;

    [HideInInspector]
    public bool DialogosIntro;

    Vector2 moveInput;


    [HideInInspector]
    public bool Atacar;
    public bool RecuarAtirar;
    [SerializeField]
    private int numeroDeAttcks;
    public bool Reset = false;
    public float ResetTimer;
    public float ResetTimerLimite;
    private int ataqueCounterAtual;
    public int AtaqueCounterAtual
    {
        get
        {
            return ataqueCounterAtual;
        }
        private set
        {
            if (value >= numeroDeAttcks)
            {
                ataqueCounterAtual = 0;
            }
            else
            {
                ataqueCounterAtual = value;
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get
        {
            if (canMove)
            {
                playerInput.enabled = true;
                if (IsMoving && !touching.IsOnWall)
                {
                    if (touching.IsGrouded)
                    {
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
        private set
        {
            _IsMoving = value;
            animacao.SetBool(animationstrings.IsMoving, value);
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
        private set
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

    // Chamado antes do start
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animacao = GetComponent<Animator>();
        touching = GetComponent<TouchingDistance>();
        DamageScript = GetComponent<Damage>();
        playerInput = GetComponent<PlayerInput>();
        acorda_Boss = GameObject.FindObjectOfType<Acorda_Boss>();
        savePoint = GameObject.FindObjectOfType<SavePoint>();
        bow = GameObject.FindObjectOfType<Bow>();
        healthBar = GameObject.FindObjectOfType<HealthBar>();
        potion_Script = GameObject.FindObjectOfType<potion_script>();

        transform.position = SavePoint.CheckpointPosition;
    }

    private void Update()
    {
        currentZRotation = bow.transform.eulerAngles.z;

        if (!canMove)
        {
            playerInput.enabled = false;
        }

        if (Input.anyKeyDown)
        {
            animacao.SetBool(animationstrings.IsAcordada, true);
        }
        else if (SavePoint.CheckpointAnim == true && SavePoint.CheckpointAnim2 == true)
        {
            animacao.SetBool(animationstrings.IsAcordada, true);
            animacao.SetBool("Checkpoint", true);
        }

        if (tempo)
        {
            // Atualiza o tempo decorrido
            elapsedTime += Time.unscaledDeltaTime; // Usar Time.unscaledDeltaTime para garantir que a interpolação não seja afetada por timeScale

            // Calcula a fração do tempo decorrido em relação à duração total
            float t = elapsedTime / duration;

            // Interpola suavemente o Time.timeScale do valor inicial para o targetTimeScale
            Time.timeScale = Mathf.Lerp(1f, targetTimeScale, t);

            // Opcional: Debug para verificar o valor atual do Time.timeScale
            Debug.Log("Current TimeScale: " + Time.timeScale);
        }

        if (Reset == true)
        {
            ResetTimer += Time.deltaTime;
            if (ResetTimer >= ResetTimerLimite)
            {
                Reset = false;
                ResetTimer = 0;
                ataqueCounterAtual = 0;
            }
        }

        if (elapsedTime >= 5 || Input.GetMouseButtonDown(1) && RecuarAtirar == true)
        {
            //ARCO
            bow.bodyCamera = false;
            bow.newOffset = new Vector3(0, 0, 0);
            bow.transposer.m_TrackedObjectOffset = bow.newOffset;

            animacao.SetBool(animationstrings.Powers, false);
            bow.gameObject.SetActive(false);
            bow.bowTorax.gameObject.SetActive(false);
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
    }

    private void FixedUpdate()
    {
        if (!DamageScript.VelocityLock)
        {
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);

            if (touching.IsGrouded)
            {
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
                if (jumpBufferFinal && touching.IsGrouded)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpImpulso);
                }
            }
            else
            {
                jumpBufferContador = jumpBufferTimer;
            }

            if (isWallSliding)
            {
                if (rb.velocity.y < -wallSlidingSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -wallSlidingSpeed);
                }
            }
            else if (isWallSliding && canWallJump)
            {
                wallJump();
            }

            if (touching.IsOnWall && !touching.IsGrouded && rb.velocity.y < 0)
            {
                isWallSliding = true;
                animacao.SetBool(animationstrings.IsWallSliding, isWallSliding);
            }
            else
            {
                isWallSliding = false;
                animacao.SetBool(animationstrings.IsWallSliding, isWallSliding);
            }
        }

        animacao.SetFloat(animationstrings.yVelocity, rb.velocity.y);
    }

    public void wallJump()
    {
        Vector2 direcao = new Vector2(wallJumpDirecao.x * -facingDirecao, wallJumpDirecao.y);

        rb.AddForce(direcao, ForceMode2D.Impulse);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;

            setDirection(moveInput);
        }
        else
        {
            IsMoving = false;
        }
    }

    private void setDirection(Vector2 moveInput)
    {
        facingDirecao = facingDirecao * -1;
        if (moveInput.x > 0 && !IsRight)
        {
            // olhar para a direita
            IsRight = true;
        }
        else if (moveInput.x < 0 && IsRight)
        {
            // olhar para a esquerda
            IsRight = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (coyoteTimeContador > 0f || jumpBufferFinal)
            {
                if (touching.IsGrouded || (touching.IsOnWall && isWallSliding))
                {
                    Jump();
                }
            }
        }

        if (!touching.IsGrouded)
        {
            jumpBufferFinal = true;
        }
        else
        {
            jumpBufferFinal = false;
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private void Jump()
    {
        animacao.SetTrigger(animationstrings.jump);
        rb.velocity = new Vector2(rb.velocity.x, jumpImpulso);
        coyoteTimeContador = 0f;
        isWallSliding = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && touching.IsGrouded)
        {
            Atacar = true;
            Reset = true;
            animacao.SetTrigger(animationstrings.attack);
            animacao.SetInteger(animationstrings.counterAtt, AtaqueCounterAtual);

            AtaqueCounterAtual++;
        }
        else if (context.started)
        {
            Atacar = true;
            animacao.SetTrigger(animationstrings.attack);
        }
    }

    public void OnPowers(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (touching.IsGrouded && bow.NewArrow == null)
            {
                tempo = true;
                bow.bodyCamera = true;
                animacao.SetBool(animationstrings.Powers, true);
                bow.gameObject.SetActive(true);
                bow.bowTorax.gameObject.SetActive(true);
                bow.cinemachineVirtualCamera.LookAt = bow.FollowArco;
            }
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        //KNOCKBACK
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
        //JUMP
        IsJumping = false;

        //ARCO
        bow.bodyCamera = false;
        bow.newOffset = new Vector3(0, 0, 0);
        bow.transposer.m_TrackedObjectOffset = bow.newOffset;

        animacao.SetBool(animationstrings.Powers, false);
        bow.gameObject.SetActive(false);
        bow.bowTorax.gameObject.SetActive(false);
        Time.timeScale = 1f;
        tempo = false;
        elapsedTime = 0f;
        bow.NewArrow = null;
        foreach (var nuss in bow.points)
        {
            nuss.SetActive(false);
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.started)
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