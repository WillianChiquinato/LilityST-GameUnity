using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damage : MonoBehaviour
{
    public UnityEvent<int, Vector2> DamageHit;
    Animator animator;
    PlayerMoviment playerMoviment;
    Damage playerMovimento;


    [SerializeField]
    private int _maxHealth = 100;

    public int maxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }

    [SerializeField]
    private int _health = 100;
    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;

            //if for 0, nao esta mais vivo
            if (_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    [SerializeField]
    public bool isInvicible = false;
    private float timeSincehit;
    [SerializeField]
    private float invicibilityTimer = 0.25f;



    [SerializeField]
    private bool _IsAlive = true;
    public bool IsAlive
    {
        get
        {
            return _IsAlive;
        }
        set
        {
            _IsAlive = value;
            animator.SetBool(animationstrings.IsAlive, value);
        }
    }

    public bool VelocityLock
    {
        get
        {
            return animator.GetBool(animationstrings.VelocityLock);
        }
        set
        {
            animator.SetBool(animationstrings.VelocityLock, value);
        }
    }


    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isInvicible)
        {
            if (timeSincehit > invicibilityTimer)
            {
                // Remove a invencibilidade
                isInvicible = false;
                timeSincehit = 0;
            }

            timeSincehit += Time.deltaTime;
        }
    }

    public bool hit(int damage, Vector2 knockback)
    {
        //Se tomar dano hit é true, senao é false
        if (IsAlive && !isInvicible)
        {
            Health -= damage;
            isInvicible = true;

            // Criando um evento, para quando for chamado esses 2 parametros, o dano e o knockback
            animator.SetTrigger(animationstrings.hit);
            VelocityLock = true;
            DamageHit?.Invoke(damage, knockback);

            return true;
        }
        return false;
    }

}
