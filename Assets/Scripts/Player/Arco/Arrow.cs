using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    CapsuleCollider2D ColliderArrow;
    SpriteRenderer spriteRenderer;
    Tiles_Entrelacados tiles_Entrelacados;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ColliderArrow = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        tiles_Entrelacados = GameObject.FindFirstObjectByType<Tiles_Entrelacados>();

        Destroy(this.gameObject, 2f);
    }


    void Update()
    {
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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
        if (collision.CompareTag("Inimigos") || collision.CompareTag("Ground"))
        {
            animator.SetTrigger(animationstrings.Impacto);
            rb.bodyType = RigidbodyType2D.Static;
            ColliderArrow.enabled = false;
            Destroy(this.gameObject, 0.4f);
        }
    }
}
