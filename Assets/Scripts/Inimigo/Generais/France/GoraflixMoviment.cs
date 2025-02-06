using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GoraflixMoviment : MonoBehaviour
{
    [Header("Instancias")]
    TouchingDistance touching;
    public PlayerMoviment playerMoviment;
    public grabPlayer grabPlayer;
    public Transform playerTransform;
    public Animator animator;
    public Rigidbody2D rb;
    public DetectionZone attackZona;

    public GameObject projetilLanca;
    public Transform projetilInstance;



    [Header("Variaveis")]
    public bool SpawnCheck = false;
    public float distanceToPlayer;
    public bool LancaTrigger = false;
    public float shootTempo;
    public float shootTimerTarget;

    [Header("Grab Player")]
    public bool grab = true;


    public bool canMove
    {
        get
        {
            return animator.GetBool(animationstrings.canMove);
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
        touching = GetComponent<TouchingDistance>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        grabPlayer = GameObject.FindFirstObjectByType<grabPlayer>();
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        playerTransform = playerMoviment.transform;
    }

    void Update()
    {
        shootTempo += Time.deltaTime;
        Target = attackZona.detectColliders.Count > 0;

        if (Target)
        {
            if (attackCooldown <= 0)
            {
                if (LancaTrigger && shootTempo >= shootTimerTarget)
                {
                    Instantiate(projetilLanca, projetilInstance.position, Quaternion.identity);
                    shootTempo = 0f;
                    LancaTrigger = false;
                    animator.SetBool("Lanca", false);
                }
            }
        }

        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        FlipDirecao();
        //TODO: Caso querer.
        distanceToPlayer = Mathf.Abs(playerTransform.position.x - transform.position.x);
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
}