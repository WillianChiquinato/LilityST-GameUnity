using UnityEngine;

public class RicocheteItens : MonoBehaviour
{
    private Vector2 lastVelocity;
    private Rigidbody2D rb;

    public int countBatidas = 0;

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
            if (countBatidas < 3)
            {
                Vector2 normal = collision.contacts[0].normal;
                Vector2 reflectDirection = Vector2.Reflect(lastVelocity, normal);
                reflectDirection.y *= 3.2f;
                reflectDirection.x *= 1.3f;
                rb.linearVelocity = reflectDirection;
                countBatidas++;
            }
        }
    }
}
