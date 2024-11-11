using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GoraflixMoviment : MonoBehaviour
{
    TouchingDistance touching;
    public PlayerMoviment playerMoviment;
    public Transform playerTransform;
    private float speed = 6.5f;
    public Animator animator;
    public Rigidbody2D rb;
    public bool atacar = false;
    public float timingAttack;
    public bool flipDelayed = false;
    public bool SpeedDelayed = false;

    //Idle
    public float obstacleCheckDistance = 2f;
    private float stopDistance = 2f;
    public float distanceToPlayer;
    public float distanceToPlayerPlayerY;
    public float timerTP = 2f;


    [Header("Targets")]
    public GameObject paredes_pretas;
    public GameObject nomeBoss;
    public Transform firstTarget;
    public Transform secondTarget;
    public CinemachineFramingTransposer transposer;
    public DetectionZone attackZona;
    public Vector3 originalPosition;


    public bool playerSeguir = false;

    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public float targetOrthographicSize = 6f;

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
        playerMoviment = GameObject.FindObjectOfType<PlayerMoviment>();
        cinemachineVirtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        transposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        nomeBoss.SetActive(false);
    }

    void Update()
    {
        Target = attackZona.detectColliders.Count > 0;

        if (Target && attackCooldown == 0)
        {
            Time.timeScale = 0.35f;
            StartCoroutine(FlipDelayed());
        }
        else if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
            Time.timeScale = 1f;
        }

        FlipDirecao();
        //Sobre o idle.
        distanceToPlayer = Mathf.Abs(playerTransform.position.x - transform.position.x);
        distanceToPlayerPlayerY = Mathf.Abs(playerMoviment.transform.position.y - transform.position.y);

        if (atacar)
        {
            timingAttack -= Time.deltaTime;
            paredes_pretas.SetActive(true);
            nomeBoss.SetActive(true);
            playerMoviment.canMove = false;
            StartCoroutine(TransicaoCamera(secondTarget));

            if (timingAttack < 0f)
            {
                ataqueGeneral();
            }
        }

        if (speed == 0f)
        {
            if (playerSeguir && !playerMoviment.IsMoving)
            {
                timerTP -= Time.deltaTime;
                if (timerTP < 0.6f && distanceToPlayerPlayerY > 5f)
                {
                    animator.SetBool(animationstrings.Teleporte, true);
                    if (timerTP < 0f)
                    {
                        StartCoroutine(TpOnPlayer());
                        timerTP = 2f;
                    }
                }

            }
        }
    }

    IEnumerator TpOnPlayer()
    {
        originalPosition = transform.position;
        yield return new WaitForSeconds(0.1f);

        animator.SetBool(animationstrings.Teleporte, false);
        transform.position = playerTransform.position;

        yield return new WaitForSeconds(1.7f);
        transform.position = originalPosition;
    }

    IEnumerator TransicaoCamera(Transform target)
    {
        cinemachineVirtualCamera.Follow = target;

        while (Vector3.Distance(cinemachineVirtualCamera.Follow.position, target.position) > 0.1f)
        {
            transposer.m_XDamping = 2;
            transposer.m_YDamping = 2;
            transposer.m_ZDamping = 2;
            cinemachineVirtualCamera.Follow = target;
            yield return null;
        }
    }

    private void ataqueGeneral()
    {
        playerSeguir = true;
        paredes_pretas.SetActive(false);
        nomeBoss.SetActive(false);
        if (distanceToPlayer > stopDistance && !touching.IsOnWall && !SpeedDelayed)
        {
            speed = 6.5f;
            Vector2 targetPosition = new Vector2(playerTransform.position.x, rb.position.y);
            transform.position = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.deltaTime);
            animator.SetBool(animationstrings.SeguirPlayer, true);
            animator.SetBool(animationstrings.Teleporte, false);
        }
        else
        {
            speed = 0f;
            animator.SetBool(animationstrings.SeguirPlayer, false);
        }

        StartCoroutine(TransicaoCamera(firstTarget));
        playerMoviment.canMove = true;
    }

    private void FlipDirecao()
    {
        if (touching.IsGrouded && !flipDelayed)
        {
            if (transform.position.x > playerMoviment.transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    IEnumerator FlipDelayed()
    {
        flipDelayed = true;
        SpeedDelayed = true;

        yield return new WaitForSeconds(2f);

        flipDelayed = false;
        SpeedDelayed = false;
    }
}