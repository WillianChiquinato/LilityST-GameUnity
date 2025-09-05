using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GoraflixMoviment : MonoBehaviour
{
    [Header("Instancias")]
    public GameObject Anelgrab;

    TouchingVariables touching;
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
    public bool attackLanca = false;
    public bool grabActived = false;

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
        set
        {
            animator.SetFloat(animationstrings.attackCooldown, Mathf.Max(value, 0));
        }
    }


    void Start()
    {
        Anelgrab.SetActive(false);
        touching = GetComponent<TouchingVariables>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        grabPlayer = GameObject.FindFirstObjectByType<grabPlayer>();
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();

        attackCooldown = 1f;
        playerTransform = playerMoviment.transform;
    }

    void Update()
    {
        shootTempo += Time.deltaTime;
        Target = attackZona.detectColliders.Count > 0;

        if (Target && attackLanca)
        {
            if (attackCooldown <= 0)
            {
                animator.SetBool("Lanca", true);
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