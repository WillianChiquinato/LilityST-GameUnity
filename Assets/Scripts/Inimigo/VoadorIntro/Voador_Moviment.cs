using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voador_Moviment : MonoBehaviour
{
    public float speed;
    public float distanciaAttack;
    public float distanciaAttack2;
    public bool Shooting;
    private float shootTimerTarget = 1f;
    public float shootTempo;

    public PlayerMoviment playerMoviment;
    public DetectionVoador attackZona;
    public GameObject projetil;
    public Transform projetilPos;
    public Vector3 direction;


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
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        DamageScript = GetComponent<Damage>();

        direction = (playerMoviment.transform.position - transform.position).normalized;
    }


    void Update()
    {
        shootTempo += Time.deltaTime;
        distanciaAttack = Mathf.Abs(transform.position.x - playerMoviment.transform.position.x);
        distanciaAttack2 = Mathf.Abs(transform.position.y - playerMoviment.transform.position.y);

        if (distanciaAttack < 8 && distanciaAttack2 < 5)
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

        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        if (DamageScript.IsAlive)
        {
            if (attackZona.perseguindo == true && canMove == true)
            {
                Perseguir();
            }
        }
        else
        {
            rb.gravityScale = 2f;
        }
        Flip();
    }

    public void Perseguir()
    {
        if (distanciaAttack2 < 4 && distanciaAttack < 7)
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
        rb.velocity = new Vector2(knockback.x, knockback.y);
    }
}
