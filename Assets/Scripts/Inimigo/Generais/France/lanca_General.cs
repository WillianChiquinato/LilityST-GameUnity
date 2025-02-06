using UnityEngine;

public class lanca_General : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    CapsuleCollider2D ColliderLanca;
    SpriteRenderer spriteRenderer;

    [Header("Instancias")]
    public Transform player;
    public GameObject objetoSaidaProjetil;
    public Vector3 pontoInicial;
    public float duracaoTrajetoria;
    public float tempo;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ColliderLanca = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        pontoInicial = objetoSaidaProjetil.transform.position;
        tempo = 0f;
    }


    void Update()
    {
        float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (tempo < duracaoTrajetoria)
        {
            tempo += Time.deltaTime;
            float t = tempo / duracaoTrajetoria;

            float altura = Mathf.Sin(t * Mathf.PI) * 2f;
            Vector3 posicao = Vector3.Lerp(pontoInicial, player.position, t);
            transform.position = posicao;
            //
        }

        if (transform.localScale.x == 1)
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
            Destroy(this.gameObject, 0.4f);
        }
    }
}
