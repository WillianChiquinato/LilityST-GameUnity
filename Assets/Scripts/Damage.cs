using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damage : MonoBehaviour
{
    public UnityEvent<int, Vector2> DamageHit;
    public UnityEvent<int, int> healthChange;
    Animator animator;

    [SerializeField]
    private int _maxHealth = 4;

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
    private int _health = 4;
    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            healthChange?.Invoke(_health, maxHealth);

            //if for 0, nao esta mais vivo
            if (_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    public bool isInvicible = false;
    private float timeSincehit;
    public float invicibilityTimer = 0.25f;

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

    public bool Hit(int damage, Vector2 knockback, Transform attacker)
    {
        if (!IsAlive || isInvicible)
            return false;

        // Verifica se esse inimigo pode bloquear
        IBlockDamage block = GetComponent<IBlockDamage>();

        if (block != null && block.CanBlock(attacker))
        {
            block.OnBlock();
            return false;
        }

        Health -= damage;
        isInvicible = true;

        animator.SetTrigger(animationstrings.hit);
        VelocityLock = true;
        DamageHit?.Invoke(damage, knockback);

        return true;
    }

    public void Reset()
    {
        Health = maxHealth;
        IsAlive = true;
    }
}
