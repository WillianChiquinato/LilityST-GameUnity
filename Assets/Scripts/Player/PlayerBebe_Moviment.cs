using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDistance))]
public class PlayerBebe_Moviment : MonoBehaviour
{
    public static PlayerBebe_Moviment Instance { get; private set; }


    [Header("Instances")]
    public PlayerInput playerInput;
    public float speed;
    public float airSpeed;
    public Vector2 moveInput;
    public int facingDirecao = 1;


    [Header("Variaveis")]
    [HideInInspector]
    public Animator animacao;
    [HideInInspector]
    public Rigidbody2D rb;
    public TouchingDistance touching;
    public bool entrar;

    [Header("CameraFollowAnimation")]
    [SerializeField] private GameObject _cameraFollow;
    public cameraFollowBaby camerafollowObject;

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
        touching = GetComponent<TouchingDistance>();
        _cameraFollow = GameObject.FindGameObjectWithTag("CameraFollow");
        playerInput = GetComponent<PlayerInput>();

        transform.position = SavePoint.CheckpointPosition;
        camerafollowObject = _cameraFollow.GetComponent<cameraFollowBaby>();
    }

    private void Update()
    {
        rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
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
