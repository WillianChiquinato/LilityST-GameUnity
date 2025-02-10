using UnityEngine;

public class LagartoPatrulha : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    RaycastHit2D groundFront;
    public LayerMask groundCheck;
    public GameObject groundCheckObject;

    public float speed;
    public float direcao;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
        void Update()
        {
            direcao = Mathf.Sign(transform.localScale.x);
            groundFront = Physics2D.Raycast((Vector2)groundCheckObject.transform.position + new Vector2(0, -0.8f), Vector2.right * direcao, 0.3f, groundCheck);
            rb.linearVelocity = new Vector2(direcao * speed, rb.linearVelocity.y);

            if (groundFront.collider)
            {
                transform.localScale = new Vector3(-direcao, transform.localScale.y, transform.localScale.z);
            }
        }

    void OnDrawGizmos()
    {
        //Plataformas e colisao a frente.
        Gizmos.color = Color.red;
        Gizmos.DrawRay((Vector2)groundCheckObject.transform.position + new Vector2(0, -0.8f), new Vector2(direcao, 0) * 0.3f);

    }
}
