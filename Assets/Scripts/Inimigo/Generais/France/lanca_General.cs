using UnityEngine;

public class lanca_General : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    CapsuleCollider2D ColliderLanca;
    SpriteRenderer spriteRenderer;

    [Header("Instancias")]
    public Transform player;
    public FranceMoviment objetoSaidaProjetil;
    private Vector3 pontoInicial;
    private Vector3 posicaoAnterior;

    private float tempo = 0f;
    private bool seguindoPlayer = true;

    public float duracaoTrajetoria;
    private float distanciaPararDeSeguir = 3f;
    public Vector3 ultimaDirecaoValida;
    public bool Colidiu = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ColliderLanca = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        posicaoAnterior = transform.position;
        tempo = 0f;
    }


    void Update()
    {
        if (Colidiu) return;

        if (objetoSaidaProjetil == null)
        {
            objetoSaidaProjetil = GameObject.FindFirstObjectByType<FranceMoviment>();
            pontoInicial = objetoSaidaProjetil.transform.position;
        }
        if (player == null)
        {
            player = GameObject.FindFirstObjectByType<PlayerMoviment>().transform;
        }

        if (seguindoPlayer)
        {
            tempo += Time.deltaTime;
            float t = tempo / duracaoTrajetoria;

            Vector3 posicao = Vector3.Lerp(pontoInicial, player.position, t);
            float altura = Mathf.Sin(t * Mathf.PI) * 30f;
            posicao.y = Mathf.Lerp(pontoInicial.y, player.position.y, t) + altura;

            Vector3 direcao = posicao - posicaoAnterior;
            if (direcao.magnitude > 0.1f)
            {
                ultimaDirecaoValida = direcao.normalized;

                float angle = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            transform.position = posicao;
            posicaoAnterior = posicao;

            float distancia = Vector3.Distance(transform.position, player.position);
            if (distancia < distanciaPararDeSeguir)
            {
                seguindoPlayer = false;
            }
        }
        else
        {
            transform.position += ultimaDirecaoValida * Time.deltaTime * 40f;
        }

        if (transform.localScale.x == -1)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground"))
        {
            animator.SetTrigger(animationstrings.Impacto);
            rb.bodyType = RigidbodyType2D.Static;
            ColliderLanca.enabled = false;
            Colidiu = true;
            Destroy(this.gameObject, 1f);
        }
    }
}
