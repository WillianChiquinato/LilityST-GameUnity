using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lagartin_Moviment : MonoBehaviour
{
    private Item_drop dropInimigo;
    public float speed;
    public float distanciaAttack;
    public bool perseguir;

    public PlayerMoviment playerMoviment;
    TouchingDistance touching;

    public DetectionLagartin attackZona1;
    public GameObject attackDetector;
    public GameObject attackLagartin;
    Damage DamageScript;
    Rigidbody2D rb;
    public Animator animator;

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

    void Start()
    {
        playerMoviment = GameObject.FindAnyObjectByType<PlayerMoviment>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        DamageScript = GetComponent<Damage>();
        touching = GetComponent<TouchingDistance>();
        dropInimigo = GetComponent<Item_drop>();
    }


    void Update()
    {
        distanciaAttack = Mathf.Abs(transform.position.x - playerMoviment.transform.position.x);

        Target = attackZona1.detectColliders.Count > 0;

        if (DamageScript.IsAlive)
        {
            if (attackZona1.perseguir == true)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                animator.SetBool(animationstrings.Comeco, true);
                if (touching.IsGrouded && canMove)
                {
                    attackDetector.SetActive(false);
                    if (distanciaAttack < 20)
                    {
                        transform.position = Vector2.MoveTowards(transform.position, playerMoviment.transform.position, speed * Time.deltaTime);
                    }
                }
            }
            FlipDirecao();
        }
        else
        {
            attackLagartin.SetActive(false);
            rb.gravityScale = 2f;
        }
    }

    private void FlipDirecao()
    {
        if (touching.IsGrouded)
        {
            if (transform.position.x > playerMoviment.transform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
                animator.SetTrigger(animationstrings.DireitaLagartin);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
                animator.SetTrigger(animationstrings.DireitaLagartin);
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
            rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
        }
    }
}
