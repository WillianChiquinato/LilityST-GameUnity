using UnityEngine;

public class lanca_General : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    CapsuleCollider2D ColliderLanca;
    SpriteRenderer spriteRenderer;

    [Header("Instancias")]
    public Transform player;
    public GoraflixMoviment objetoSaidaProjetil;
    public Vector3 pontoInicial;
    Vector3 posicaoAnterior;
    public float duracaoTrajetoria;
    public float tempo;

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
        if (objetoSaidaProjetil == null)
        {
            objetoSaidaProjetil = GameObject.FindFirstObjectByType<GoraflixMoviment>();
            pontoInicial = objetoSaidaProjetil.transform.localPosition;
        }
        if (player == null)
        {
            player = GameObject.FindFirstObjectByType<PlayerMoviment>().transform;
        }

        if (tempo < duracaoTrajetoria)
        {
            tempo += Time.deltaTime;
            float t = tempo / duracaoTrajetoria;

            Vector3 posicao = Vector3.Lerp(pontoInicial, player.position, t);
            float altura = Mathf.Sin(t * Mathf.PI) * 30f;
            posicao.y = Mathf.Lerp(pontoInicial.y, player.position.y, t) + altura;

            Vector3 direcao = posicao - posicaoAnterior;
            if (direcao.magnitude > 0.01f)
            {
                float angle = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            transform.position = posicao;
            posicaoAnterior = posicao;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetTrigger(animationstrings.Impacto);
            rb.bodyType = RigidbodyType2D.Static;
            ColliderLanca.enabled = false;
            Destroy(this.gameObject, 1f);
        }
    }
}
