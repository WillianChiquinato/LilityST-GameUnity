using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voador_Moviment : MonoBehaviour
{
    public float speed;
    public float distanciaAttack;
    public bool Shooting;
    public float shootTempo;
    public float shootTimerTarget;

    public PlayerMoviment playerMoviment;
    public GameObject projetil;
    public Transform projetilPos;
    public Vector3 direction;


    [Header("Field of View")]
    public Transform playerTransform;
    public float detectionRange = 10f;
    public LayerMask obstacleLayer;
    [SerializeField]
    private bool isPlayerInSight;
    public RaycastHit2D hit;


    Damage DamageScript;
    Rigidbody2D rb;
    Animator animator;

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

    void Start()
    {
        playerMoviment = GameObject.FindAnyObjectByType<PlayerMoviment>();
        playerTransform = playerMoviment.GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        DamageScript = GetComponent<Damage>();

        direction = (playerMoviment.transform.position - transform.position).normalized;
    }


    void Update()
    {
        shootTempo += Time.deltaTime;
        distanciaAttack = Mathf.Abs(transform.position.x - playerMoviment.transform.position.x);

        CheckPlayerInSight();

        if (isPlayerInSight)
        {
            if (DamageScript.IsAlive)
            {
                if (canMove)
                {
                    Perseguir();
                    Flip();
                }
            }
            else
            {
                rb.gravityScale = 2f;
            }

            if (distanciaAttack < 10)
            {
                Target = true;
                if (Shooting == true && shootTempo >= shootTimerTarget)
                {
                    Projetil();

                    shootTempo = 0;
                }
            }
            else
            {
                Target = false;
            }
        }

        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    public void Perseguir()
    {
        if (distanciaAttack < 7)
        {
            // Fica parado na posicao Y
            transform.position += new Vector3(direction.x, 0.2f, direction.z) * speed * Time.deltaTime;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, playerMoviment.transform.position, speed * Time.deltaTime);
        }

        rb.MovePosition(transform.position);
    }

    public void Flip()
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

    public void Projetil()
    {
        Instantiate(projetil, projetilPos.position, Quaternion.identity);
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        if (isPlayerInSight)
        {
            rb.velocity = new Vector2(knockback.x, knockback.y);
        }
        else
        {
            Debug.Log("Atacou");
        }
    }

    void CheckPlayerInSight()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

            hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

            if (hit.collider == null)
            {
                if (distanciaAttack < 10)
                {
                    Target = true;
                    if (Shooting == true && shootTempo >= shootTimerTarget)
                    {
                        Projetil();

                        shootTempo = 0;
                    }
                }
                isPlayerInSight = true;
                return;
            }
        }

        Target = false;
        isPlayerInSight = false;
    }


    //Para depuração.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        if (playerTransform != null)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

            float distanceToDraw = hit.collider == null
                ? Vector2.Distance(transform.position, playerTransform.position)
                : Vector2.Distance(transform.position, hit.point);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)directionToPlayer * distanceToDraw);

            if (hit.collider != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(hit.point, 0.2f);
            }
        }
    }
}
