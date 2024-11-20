using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maga_Movement : MonoBehaviour
{
    private Item_drop dropInimigo;
    public Maga_RangedAttack maga_RangedAttack;
    Animator animator;
    Rigidbody2D rb;
    public Damage damageScript;
    Trigger_Rolar trigger_Rolar;


    public GameObject objetoInvocacao;
    public Transform Player;
    public bool podeInvocar = false;
    private float distanciaAbaixo = 1.0f;
    public float speedRolar;

    public float timingRolar;
    public float timingRolarCounter;
    public float timingAttack;
    public float timingAttackCount;

    public LayerMask groundLayer;

    public bool canMove
    {
        get
        {
            return animator.GetBool(animationstrings.canMove);
        }
    }

    void Start()
    {
        Player = GameObject.FindObjectOfType<PlayerMoviment>().GetComponentInChildren<Transform>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        maga_RangedAttack = GameObject.FindObjectOfType<Maga_RangedAttack>();
        trigger_Rolar = GameObject.FindObjectOfType<Trigger_Rolar>();
        damageScript = GetComponent<Damage>();
        dropInimigo = GetComponent<Item_drop>();

        timingAttack = timingAttackCount;
        timingRolar = timingRolarCounter;
    }

    void Update()
    {
        if (damageScript.IsAlive)
        {
            if (trigger_Rolar.distanciaRolar)
            {
                timingRolar -= Time.deltaTime;
                //Metodo para fazer o inimigo rolar pra tras.
                StartCoroutine(rolar());
            }
            else
            {
                //Testes
                timingRolar = timingRolarCounter;
            }
            if (maga_RangedAttack.rangedAttack)
            {
                timingAttack -= Time.deltaTime;
                if (timingAttack < 0)
                {
                    animator.SetBool(animationstrings.IsAttackMaga, true);
                    timingAttack = timingAttackCount;
                    podeInvocar = true;
                }
                else
                {
                    animator.SetBool(animationstrings.IsAttackMaga, false);
                }
            }
            else
            {
                animator.SetBool(animationstrings.IsAttackMaga, false);
                timingAttack = timingAttackCount;
            }


            //invocação do objeto
            if (podeInvocar)
            {
                RaycastHit2D hit = Physics2D.Raycast(Player.position, Vector2.down, Mathf.Infinity, groundLayer);

                if (hit.collider != null)
                {
                    Vector3 posicaoChao = hit.point;
                    posicaoChao.y += distanciaAbaixo;
                    Instantiate(objetoInvocacao, posicaoChao, Quaternion.identity);
                }

                podeInvocar = false;
            }
            Flip();
        }
    }

    public void Flip()
    {
        if (transform.position.x > Player.transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    IEnumerator rolar()
    {
        if (timingRolar < 0)
        {
            Vector2 moveDirection;

            if (transform.position.x > Player.transform.position.x)
            {
                moveDirection = Vector2.right;
            }
            else
            {
                moveDirection = Vector2.left;
            }

            rb.MovePosition(rb.position + moveDirection * speedRolar * Time.deltaTime);
            animator.SetBool("MakeDash", true);

            yield return new WaitForSeconds(0.4f);

            animator.SetBool("MakeDash", false);
            timingRolar = timingRolarCounter;
            trigger_Rolar.distanciaRolar = false;
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        if (!damageScript.IsAlive)
        {
            dropInimigo.GenerateDrop();
        }
        else
        {
            rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
            timingAttack = timingAttackCount;
        }
    }
}
