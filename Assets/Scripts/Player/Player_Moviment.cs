using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDistance), typeof(Damage))]
public class PlayerMoviment : MonoBehaviour
{
    [HideInInspector]
    public Acorda_Boss acorda_Boss;
    public SavePoint savePoint;
    [HideInInspector]
    public PlayerInput playerInput;
    Bow bow;

    public bool entrar;

    //Variaveis
    [HideInInspector]
    public Animator animacao;
    [HideInInspector]
    public Rigidbody2D rb;
    TouchingDistance touching;
    Damage DamageScript;
    Dialogos dialogosIntro;

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


    private bool isWallSliding;
    public float wallSlidingSpeed;

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
        dialogosIntro = GameObject.FindAnyObjectByType<Dialogos>();
        savePoint = GameObject.FindObjectOfType<SavePoint>();
        bow = GameObject.FindObjectOfType<Bow>();

        transform.position = SavePoint.CheckpointPosition;
    }

    private void Update()
    {
        Debug.Log("Current TimeScale: " + Time.timeScale);
        if (!canMove)
        {
            playerInput.enabled = false;
        }

        if (Input.anyKeyDown)
        {
            DialogosIntro = true;

            animacao.SetBool(animationstrings.IsAcordada, true);
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

            if (IsJumping)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    ContagemJump -= Time.deltaTime;
                }

                if (ContagemJump >= 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpImpulso);
                }
                else
                {
                    IsJumping = false;
                }
            }

            if (isWallSliding)
            {
                if (rb.velocity.y < -wallSlidingSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -wallSlidingSpeed);
                }
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
        //Checar se esta no chao e tbm vivo
        if (context.started)
        {
            if (coyoteTimeContador > 0f)
            {
                IsJumping = true;
            }
            if (touching.IsGrouded && canMove || !isWallSliding && touching.IsOnWall)
            {
                IsJumping = true;
                animacao.SetTrigger(animationstrings.jump);
                rb.velocity = new Vector2(rb.velocity.x, jumpImpulso);
            }
        }

        if (context.started && !touching.IsGrouded)
        {
            jumpBufferFinal = true;
        }
        else if (touching.IsGrouded)
        {
            jumpBufferFinal = false;
        }

        if (Input.GetKeyUp(KeyCode.W) && rb.velocity.y > 0f)
        {
            IsJumping = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulso * 0.3f);
            ContagemJump = 0.05f;
            coyoteTimeContador = 0f;
        }
        else if (DamageScript.isInvicible == false)
        {
            ContagemJump = 0.05f;
        }
        else if (isWallSliding && touching.IsOnWall)
        {
            isWallSliding = false;
        }
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
                bow.cinemachineVirtualCamera.LookAt = bow.FollowArco;
            }
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        //KNOCKBACK
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
        //JUMP
        ContagemJump = 0;

        //ARCO
        bow.bodyCamera = false;
        bow.newOffset = new Vector3(0, 0, 0);
        bow.transposer.m_TrackedObjectOffset = bow.newOffset;

        animacao.SetBool(animationstrings.Powers, false);
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
}