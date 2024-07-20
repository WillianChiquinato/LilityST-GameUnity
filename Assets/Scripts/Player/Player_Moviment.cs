using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDistance), typeof(Damage))]
public class PlayerMoviment : MonoBehaviour
{
    [HideInInspector]
    public Acorda_Boss acorda_Boss;
    public PlayerInput playerInput;
    public SavePoint savePoint;
    public Bow bow;

    public bool entrar;
    public bool ArcoTime;

    //Variaveis
    public Rigidbody2D rb;
    public Animator animacao;
    TouchingDistance touching;
    Damage DamageScript;
    Dialogos dialogosIntro;

    public float speed = 7f;
    public float airSpeed = 7f;

    public bool IsJumping;
    public float jumpImpulso = 10f;
    public float ContagemJump = 0.5f;

    //Sobre o arco
    public bool Atirar = false;
    public bool SlowArco = false;
    public float zOffset = -10f;
    public float followSpeed = 10f;
    public bool tempo;
    public float targetTimeScale = 0.3f;
    public float duration = 1f;
    public float elapsedTime = 0f;


    private bool isWallSliding;
    public float wallSlidingSpeed;

    public bool DialogosIntro;

    Vector2 moveInput;

    //Ataque combo da lility
    public bool Atacar;

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
            dialogosIntro.animator.SetBool("Sumir", true);
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
    }

    private void FixedUpdate()
    {
        if (!DamageScript.VelocityLock)
        {
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);

            if (IsJumping)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    ContagemJump -= Time.deltaTime;
                }

                if (ContagemJump > 0)
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
        if (context.started && touching.IsGrouded && canMove || !isWallSliding && touching.IsOnWall)
        {
            animacao.SetTrigger(animationstrings.jump);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulso);
            IsJumping = true;
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            IsJumping = false;
            ContagemJump = 0.4f;
        }
        else if (DamageScript.isInvicible == false)
        {
            ContagemJump = 0.4f;
        }
        else if (isWallSliding && touching.IsOnWall)
        {
            isWallSliding = false;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animacao.SetTrigger(animationstrings.attack1);
            Atacar = true;
        }
    }

    public void OnPowers(InputAction.CallbackContext context)
    {
        if (context.started && touching.IsGrouded && bow.NewArrow == null)
        {
            tempo = true;
            bow.bodyCamera = true;
            animacao.SetBool(animationstrings.Powers, true);
            bow.gameObject.SetActive(true);
            bow.cinemachineVirtualCamera.LookAt = bow.FollowArco;
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
        ContagemJump = 0;

        bow.bodyCamera = false;
        bow.newOffset = new Vector3(0, 0, 0);
        bow.transposer.m_TrackedObjectOffset = bow.newOffset;
        
        animacao.SetBool(animationstrings.Powers, false);
        bow.gameObject.SetActive(false);
        Time.timeScale = 1f;
        tempo = false;
        elapsedTime = 0f;
        bow.NewArrow = null;
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