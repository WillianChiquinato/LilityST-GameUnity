using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDistance), typeof(Damage))]
public class SlimeMoviment : MonoBehaviour
{
    [Header("Instancias")]
    TouchingDistance touching;
    private Item_drop dropInimigo;
    Rigidbody2D rb;
    Animator animator;
    Damage DamageScript;
    public DetectionZoneSlime attackZona;
    public PlayerMoviment playerMoviment;
    public GameObject targetLilityObject;
    public BoxCollider2D boxCollider;


    [Header("Variaveis")]
    public bool targetLility;
    public float speed = 4f;
    public float direcao;
    public float direcaoTarget;

    public float ItemTimerScale;
    public float ItemTimerTarget;

    public int attackDamage = 0;
    public Vector2 knockbackAttack = Vector2.zero;


    public bool _Target = false;
    public bool Target
    {
        get
        {
            return _Target;
        }
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

    private void Awake()
    {
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        rb = GetComponent<Rigidbody2D>();
        touching = GetComponent<TouchingDistance>();
        animator = GetComponent<Animator>();
        DamageScript = GetComponent<Damage>();
        dropInimigo = GetComponent<Item_drop>();
    }

    void Update()
    {
        Target = attackZona.detectColliders.Count > 0;
    }

    private void FixedUpdate()
    {
        direcao = Mathf.Sign(transform.localScale.x);

        if (targetLilityObject == null)
        {
            ItemTimerScale = 0f;
            targetLility = false;
        }
        else
        {
            direcaoTarget = Mathf.Sign(targetLilityObject.transform.position.x - transform.position.x);
            float distanceToItem = Mathf.Abs(targetLilityObject.transform.position.x - transform.position.x);

            if (targetLility)
            {
                rb.linearVelocity = new Vector2(direcaoTarget * speed, rb.linearVelocity.y);
            }

            if (distanceToItem <= 0.6f)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }

            if (direcaoTarget == -1)
            {
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
        }

        // FlipDirecao();
    }

    private void FlipDirecao()
    {
        if (touching.IsGrouded)
        {
            if (transform.position.x > playerMoviment.transform.position.x)
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
            float scaleFactor = boxCollider != null ? boxCollider.bounds.size.x : 1f;
            
            float direction = (playerMoviment.transform.position.x > transform.position.x) ? 1 : -1;
            rb.linearVelocity = new Vector2(knockback.x / 6, rb.linearVelocity.y + knockback.y);

            Damage DamageScript = playerMoviment.GetComponent<Damage>();

            if (DamageScript != null)
            {
                Vector2 flipknockback = new Vector2(direction * knockbackAttack.x * scaleFactor, knockbackAttack.y);

                // ataque ao alvo
                bool goHit = DamageScript.hit(attackDamage, flipknockback);
                if (goHit)
                {
                    Debug.Log("AtaqueInimigo");
                }
            }   
        }
    }
}