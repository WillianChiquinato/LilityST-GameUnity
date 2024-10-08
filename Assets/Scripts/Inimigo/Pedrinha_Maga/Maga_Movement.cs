using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Maga_Movement : MonoBehaviour
{
    public Maga_RangedAttack maga_RangedAttack;
    Animator animator;
    Rigidbody2D rb;
    Damage damageScript;
    Trigger_Rolar trigger_Rolar;


    public GameObject objetoInvocacao;
    public Transform Player;
    public bool podeInvocar = false;
    private float distanciaAbaixo = 3.0f;

    public float timingRolar;
    public float timingRolarCounter;
    public float timingAttack;
    public float timingAttackCount;

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

        timingAttack = timingAttackCount;
        timingRolar = timingRolarCounter;
    }

    void Update()
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

        if (damageScript.IsAlive)
        {
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
                Vector3 posicaoAbaixo = Player.transform.position - new Vector3(0, distanciaAbaixo, 0);

                Instantiate(objetoInvocacao, posicaoAbaixo, Quaternion.identity);

                podeInvocar = false;
            }
        }
        Flip();
    }

    public void Flip()
    {
        if (transform.position.x > Player.transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    IEnumerator rolar()
    {
        if (timingRolar < 0)
        {
            if (transform.position.x > Player.transform.position.x)
            {
                transform.position += Vector3.right * 10 * Time.deltaTime;

                yield return new WaitForSeconds(0.5f);

                //Trocar essa linha dps
                transform.position += Vector3.right * 0 * Time.deltaTime;
                timingRolar = timingRolarCounter;
                trigger_Rolar.distanciaRolar = false;
            }
            else
            {
                transform.position += Vector3.left * 10 * Time.deltaTime;

                yield return new WaitForSeconds(0.5f);

                //Trocar essa linha dps
                transform.position += Vector3.left * 0 * Time.deltaTime;
                timingRolar = timingRolarCounter;
                trigger_Rolar.distanciaRolar = false;
            }
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
        timingAttack = timingAttackCount;
    }
}
