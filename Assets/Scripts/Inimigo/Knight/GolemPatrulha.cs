using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDistance), typeof(Damage))]
public class GolemPatrulha_Moviment : MonoBehaviour
{
    [SerializeField]
    private float IdleDuracao;
    [SerializeField]
    private float IdleTimer;

    TouchingDistance touching;

    Rigidbody2D rb;
    Animator animator;
    Damage DamageScript;
    private Vector2 vectorDirecao = Vector2.right;

    public float speed = 4f;
    public float StopRate = 0.2f;
    public DetectionZone attackZona;
    public enum WalkAbleDirecao { Right, Left }

    private WalkAbleDirecao _WalkDirecao;

    public WalkAbleDirecao WalkDirecao
    {
        get { return _WalkDirecao; }
        set
        {
            if (_WalkDirecao != value)
            {
                IdleTimer = 0;
                animator.SetBool(animationstrings.IsIdlePatrulha, false);

                // Definir o Flip
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if (value == WalkAbleDirecao.Right)
                {
                    vectorDirecao = Vector2.right;
                }
                else if (value == WalkAbleDirecao.Left)
                {
                    vectorDirecao = Vector2.left;
                }

            }

            _WalkDirecao = value;
        }
    }


    public bool _Target = false;
    public bool Target
    {
        get { return _Target; }
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

    public float attackCooldown
    {
        get
        {
            return animator.GetFloat(animationstrings.attackCooldown);
        }
        private set
        {
            animator.SetFloat(animationstrings.attackCooldown, Mathf.Max(value, 0));
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touching = GetComponent<TouchingDistance>();
        animator = GetComponent<Animator>();
        DamageScript = GetComponent<Damage>();
    }

    void Update()
    {
        Target = attackZona.detectColliders.Count > 0;
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (touching.IsGrouded && touching.IsOnWall)
        {
            FlipDirecao();
        }

        if (!DamageScript.VelocityLock)
        {
            if (canMove)
            {
                rb.velocity = new Vector2(speed * vectorDirecao.x, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, StopRate), rb.velocity.y);
            }

        }
    }

    private void FlipDirecao()
    {
        IdleTimer += Time.deltaTime;
        animator.SetBool(animationstrings.IsIdlePatrulha, true);

        if (IdleTimer > IdleDuracao)
        {
            if (WalkDirecao == WalkAbleDirecao.Right)
            {
                WalkDirecao = WalkAbleDirecao.Left;
            }
            else if (WalkDirecao == WalkAbleDirecao.Left)
            {
                WalkDirecao = WalkAbleDirecao.Right;
            }
            else
            {
                Debug.LogError("A direcao atual vc vai se fuder");
            }
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}