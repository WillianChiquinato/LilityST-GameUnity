using TMPro;
using UnityEngine;

public class FilhoteDragão : MonoBehaviour
{
    [Header("Instacias")]
    public GameObject playerMovimentFilhote;
    public Vector3 OffSetFilhote;
    public GameObject boxColliderFuga;
    public Rigidbody2D rb;
    public Animator animator;
    RaycastHit2D groundFront;
    public LayerMask groundCheck;
    public bool Isgrounded;
    public GameObject groundCheckObject;
    public GameObject IsgroundedObject;
    public GameObject targetObjects;


    [Header("Movimentação")]
    public bool FugaLility;
    public bool LilityPegarFilhote;

    public bool targetLility;
    public float speed;
    public float direcao;
    public float direcaoTarget;
    public bool isOnWall;
    public float wallTimerScale;
    public float wallTimerTarget;

    public float ItemTimerScale;
    public float ItemTimerTarget;

    [Header("Pegar Filhote")]
    public float TimerFindObject;
    public bool FimDaLinhaFilhote = false;

    public float velocidade = 2f;
    private Vector3 destino = new Vector3(0f, 0.52f, 0);
    private float progresso = 0f;
    private float progressoDevolver = 0f;
    public Vector3 posicaoInicial;
    public bool filhoteDevolver = false;
    public float jumpForce = 10f;

    [Header("Reset ações")]
    public float distanciaPlayer;
    public float TimerReset;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        boxColliderFuga = transform.GetChild(3).gameObject;
        playerMovimentFilhote = GameManager.instance.player.transform.GetChild(13).gameObject;
        posicaoInicial = playerMovimentFilhote.transform.localPosition;
    }

    void Update()
    {
        Isgrounded = Physics2D.Raycast((Vector2)IsgroundedObject.transform.position, Vector2.down, 0.45f, groundCheck).collider != null;

        if (targetObjects == null)
        {
            ItemTimerScale = 0f;
            targetLility = false;
        }

        if (wallTimerScale > 0f && ItemTimerScale > 0f)
        {
            isOnWall = false;
            wallTimerScale = 0f;
        }

        direcao = Mathf.Sign(transform.localScale.x);
        groundFront = Physics2D.Raycast((Vector2)groundCheckObject.transform.position, Vector2.right * direcao, 0.3f, groundCheck);
        animator.SetBool("IsOnWall", isOnWall);

        //Fugindo do player.
        if (FugaLility)
        {
            targetObjects = null;
            RaycastHit2D groundFrontFuga = Physics2D.Raycast((Vector2)groundCheckObject.transform.position, Vector2.right * direcao, 1f, groundCheck);

            distanciaPlayer = Vector2.Distance(transform.position, GameManager.instance.player.transform.position);
            float direcaoTarget = Mathf.Sign(transform.position.x - GameManager.instance.player.transform.position.x);

            // Ajustar a velocidade: quanto mais próximo, mais rápido
            float velocidade = speed + (8f / (distanciaPlayer + 0.5f));

            if (Isgrounded)
            {
                if (Mathf.Abs(rb.linearVelocity.y) < 0.4f)
                {
                    animator.SetBool("Jumping", false);
                }

                if (distanciaPlayer >= 11f && FimDaLinhaFilhote)
                {
                    TimerReset += Time.deltaTime;
                    if (TimerReset >= 5f)
                    {
                        boxColliderFuga.GetComponent<BoxCollider2D>().enabled = true;
                        FugaLility = false;
                        TimerReset = 0f;
                        FimDaLinhaFilhote = false;
                        animator.SetBool("MedoFilhote", false);
                    }
                }
                else
                {
                    TimerReset = 0f;
                }

                if (groundFrontFuga.collider)
                {
                    if (groundFrontFuga.collider.gameObject.layer == LayerMask.NameToLayer("Ground2"))
                    {
                        FimDaLinha();
                    }
                    else if (Mathf.Abs(rb.linearVelocity.y) < 0.1f)
                    {
                        Vector2 jumpVector = new Vector2(transform.localScale.x * velocidade, jumpForce);
                        animator.SetBool("Jumping", true);

                        rb.linearVelocity = jumpVector;
                    }
                }
                else if (!FimDaLinhaFilhote)
                {
                    // Aplicar movimento ao Rigidbody2D
                    FilhoteVira(direcaoTarget, velocidade);
                }
            }
        }
        else
        {
            distanciaPlayer = 0f;
            if (!targetLility)
            {
                FlipWall();
            }
            else
            {
                direcaoTarget = Mathf.Sign(targetObjects.transform.position.x - transform.position.x);
                float distanceToItem = Mathf.Abs(targetObjects.transform.position.x - transform.position.x);
                ItemTimerScale += Time.deltaTime;

                FilhoteVira(direcaoTarget, speed);

                if (distanceToItem <= 0.9f)
                {
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                    animator.SetBool("IsOnWall", true);

                    TimerFindObject += Time.deltaTime;
                    boxColliderFuga.GetComponent<BoxCollider2D>().enabled = false;
                    if (TimerFindObject >= 4f)
                    {
                        targetObjects = null;
                        targetLility = false;
                        boxColliderFuga.GetComponent<BoxCollider2D>().enabled = true;
                        TimerFindObject = 0f;
                    }
                }
            }
        }

        //Pegando o filhote.
        if (LilityPegarFilhote)
        {
            targetObjects = null;
            animator.SetBool("FilhotePegado", true);

            transform.position = playerMovimentFilhote.transform.position + OffSetFilhote;
            if (progresso < 1.5f)
            {
                progresso += Time.deltaTime * 1.5f;
                playerMovimentFilhote.transform.localPosition = Vector3.Lerp(posicaoInicial, destino, progresso);
            }
            GameManager.instance.player.animacao.SetBool("IsCarryMode", true);
            GameManager.instance.player.isCarrying = true;
            transform.SetParent(GameManager.instance.player.CarryParentPlayer);

            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            TimerFindObject = 0f;
            boxColliderFuga.GetComponent<BoxCollider2D>().enabled = false;

            transform.localScale = new Vector3(GameManager.instance.player.transform.localScale.x, transform.localScale.y, transform.localScale.z);

            // Se o jogador quiser deixar o filhote
            if (GameManager.instance.player.entrar && progresso >= 1f)
            {
                filhoteDevolver = true;
            }
            if (filhoteDevolver)
            {
                if (progressoDevolver < 1.5f)
                {
                    progressoDevolver += Time.deltaTime * 1.5f;
                    playerMovimentFilhote.transform.localPosition = Vector3.Lerp(destino, posicaoInicial, progressoDevolver);

                    if (progressoDevolver >= 1.5f)
                    {
                        animator.SetBool("FilhotePegado", false);
                        GameManager.instance.player.isCarrying = false;
                        targetObjects = null;
                        rb.bodyType = RigidbodyType2D.Dynamic;

                        for (int i = 0; i <= 4; i++)
                        {
                            if (i != 3) // Evita desativar o objeto no índice 3
                            {
                                transform.GetChild(i).gameObject.SetActive(true);
                            }
                        }

                        progresso = 0f;
                        progressoDevolver = 0f;
                        playerMovimentFilhote.transform.localPosition = posicaoInicial;

                        LilityPegarFilhote = false;
                        filhoteDevolver = false;
                    }
                }
            }
        }
    }

    public void FilhoteVira(float direcaoTarget, float speed)
    {
        if (ItemTimerScale >= ItemTimerTarget)
        {
            rb.linearVelocity = new Vector2(direcaoTarget * speed, rb.linearVelocity.y);
            if (direcaoTarget == 1)
            {
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("IsOnWall", true);
        }
    }

    public void FlipWall()
    {
        if (!isOnWall)
        {
            rb.linearVelocity = new Vector2(direcao * speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        if (groundFront.collider && Isgrounded)
        {
            isOnWall = true;
            wallTimerScale += Time.deltaTime;

            if (wallTimerScale >= wallTimerTarget)
            {
                transform.localScale = new Vector3(-direcao, transform.localScale.y, transform.localScale.z);
                wallTimerScale = 0f;
            }
        }
        else
        {
            isOnWall = false;
        }
    }

    public void FimDaLinha()
    {
        FimDaLinhaFilhote = true;
        if (FimDaLinhaFilhote)
        {
            transform.localScale = new Vector3(-direcao, transform.localScale.y, transform.localScale.z);
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("MedoFilhote", true);
        }
    }

    void OnDrawGizmos()
    {
        //Plataformas e colisao a frente.
        Gizmos.color = Color.red;
        Vector2 start = (Vector2)groundCheckObject.transform.position;
        Vector2 direction = Vector2.right * direcao;
        Gizmos.DrawRay(start, direction.normalized * 0.3f);

        Gizmos.color = Color.yellow;
        Vector2 startFuga = (Vector2)groundCheckObject.transform.position;
        Vector2 directionFuga = Vector2.right * direcao;
        Gizmos.DrawRay(startFuga, directionFuga.normalized * 1f);

        Gizmos.color = Color.red;
        Vector3 startGround = (Vector2)IsgroundedObject.transform.position;
        Vector3 end = startGround + Vector3.down * 0.45f;

        Gizmos.DrawLine(startGround, end);
        Gizmos.DrawSphere(end, 0.1f); // Desenha um pequeno ponto no final da linha
    }
}
