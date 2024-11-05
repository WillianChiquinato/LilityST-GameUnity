using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GoraflixMoviment : MonoBehaviour
{
    TouchingDistance touching;
    public PlayerMoviment playerMoviment;
    private float speed = 5.5f;
    public Animator animator;
    public bool atacar = false;
    public float timingAttack;

    [Header("Targets")]
    public GameObject paredes_pretas;
    public GameObject nomeBoss;
    public Transform firstTarget;
    public Transform secondTarget;
    public CinemachineFramingTransposer transposer;
    public bool offsetAjustado = false;

    //
    public CinemachineVirtualCamera cinemachineVirtualCamera;


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
        touching = GetComponent<TouchingDistance>();
        animator = GetComponent<Animator>();
        playerMoviment = GameObject.FindObjectOfType<PlayerMoviment>();
        cinemachineVirtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        transposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        nomeBoss.SetActive(false);
    }

    void Update()
    {
        FlipDirecao();

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
                animator.SetBool("SeguirPlayer", true);
            }
        }
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
        paredes_pretas.SetActive(false);
        nomeBoss.SetActive(false);
        Vector2 targetPosition = new Vector2(playerMoviment.transform.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        StartCoroutine(TransicaoCamera(firstTarget));

        playerMoviment.canMove = true;
    }

    private void FlipDirecao()
    {
        if (touching.IsGrouded)
        {
            if (transform.position.x > playerMoviment.transform.position.x)
            {
                transform.localScale = new Vector3(-8, 8, 8);
            }
            else
            {
                transform.localScale = new Vector3(8, 8, 8);
            }
        }
    }
}