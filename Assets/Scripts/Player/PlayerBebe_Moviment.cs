using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingVariables))]
public class PlayerBebe_Moviment : MonoBehaviour
{
    public static PlayerBebe_Moviment Instance { get; private set; }

    [Header("Instances")]
    public PlayerInput playerInput;
    public float speed;
    public float airSpeed;
    public Vector2 moveInput;
    public int facingDirecao = 1;
    public ladderScript ladderScript;

    [Header("Variaveis")]
    [HideInInspector]
    public Animator animacao;
    [HideInInspector]
    public Rigidbody2D rb;
    public TouchingVariables touching;
    public bool entrar;
    public float jumpImpulso;
    public bool IsJumping;


    [Header("CameraFollowAnimation")]
    [SerializeField] private GameObject _cameraFollow;
    public cameraFollowBaby camerafollowObject;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public GameObject cadeira;
    public GameObject roupaCadeira;

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

    [SerializeField]
    public bool _IsMoving = false;

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

    public float CurrentMoveSpeed
    {
        get
        {
            if (canMove)
            {
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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animacao = GetComponent<Animator>();
        touching = GetComponent<TouchingVariables>();
        _cameraFollow = GameObject.FindGameObjectWithTag("CameraFollow");
        cinemachineVirtualCamera = FindFirstObjectByType<CinemachineVirtualCamera>();
        playerInput = GetComponent<PlayerInput>();
        ladderScript = GetComponent<ladderScript>();

        transform.position = new Vector2(-97.18f, 24.04f);
        camerafollowObject = _cameraFollow.GetComponent<cameraFollowBaby>();

        StartCoroutine(StartMachine());
        cinemachineVirtualCamera.m_Lens.OrthographicSize = 4f;
    }

    private void Update()
    {
        SaveData.Instance.playTime += Time.deltaTime;
        if (!canMove)
        {
            playerInput.enabled = false;
        }
        else
        {
            playerInput.enabled = true;
        }

        if (touching.IsGrouded && rb.linearVelocity.y <= 0f)
        {
            IsJumping = false;
        }

        rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive && canMove)
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
            facingDirecao = 1;
            IsRight = true;
            camerafollowObject.chamarTurn();
        }
        else if (moveInput.x < 0 && IsRight)
        {
            facingDirecao = -1;
            IsRight = false;
            camerafollowObject.chamarTurn();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touching.IsGrouded && canMove)
        {
            Jump();
        }
    }

    public void Jump()
    {
        animacao.SetTrigger(animationstrings.jump);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulso);
        IsJumping = true;
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

    IEnumerator StartMachine()
    {
        yield return new WaitForSeconds(0.2f);
        cinemachineVirtualCamera.GetComponent<CinemachineConfiner2D>().InvalidateCache();

        yield return new WaitForSeconds(7.3f);

        cadeira.SetActive(false);
        roupaCadeira.SetActive(false);
        cinemachineVirtualCamera.GetComponent<Animator>().SetBool("CameraStart", true);

        yield return new WaitForSeconds(1.5f);
        cinemachineVirtualCamera.GetComponent<CinemachineConfiner2D>().InvalidateCache();
        Debug.Log($"Camera Invalidate Cache called {cinemachineVirtualCamera.GetComponent<CinemachineConfiner2D>()}");
    }
}
