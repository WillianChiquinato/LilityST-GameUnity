using UnityEngine;

public class RicocheteItens : MonoBehaviour
{
    private Vector2 lastVelocity;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        lastVelocity = rb.linearVelocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (lastVelocity.magnitude <= 0.01f) return;

        if (collision.gameObject.CompareTag("Richochete"))
        {
            Vector2 normal = collision.contacts[0].normal;
            Vector2 reflectDirection = Vector2.Reflect(lastVelocity, normal);
            reflectDirection.y *= 1.8f;
            reflectDirection.x += 6f;
            rb.linearVelocity = reflectDirection;

            Debug.Log($"Ricochete! Velocidade antiga: {lastVelocity}, nova: {rb.linearVelocity}");
        }
    }
}
