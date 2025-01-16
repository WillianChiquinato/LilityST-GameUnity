using UnityEngine;

public class ladderScript : MonoBehaviour
{
    private float vertical;
    private float speed = 10f;
    public bool isLadder;
    public bool isClimbing = false;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerMoviment playerMoviment;
    [SerializeField] private PlayerBebe_Moviment playerBebe_Moviment;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (playerMoviment == null)
        {
            playerBebe_Moviment = GameObject.FindFirstObjectByType<PlayerBebe_Moviment>();
        }
        else if (playerBebe_Moviment == null)
        {
            playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        }
    }

    void Update()
    {
        vertical = Input.GetAxis("Vertical");

        if (playerMoviment == null)
        {
            if (isLadder && Mathf.Abs(vertical) > 0f)
            {
                isClimbing = true;
                playerBebe_Moviment.animacao.SetBool("IsClimbing", true);
            }
        }
        else if (playerBebe_Moviment == null)
        {
            if (isLadder && Mathf.Abs(vertical) > 0f)
            {
                isClimbing = true;
                playerMoviment.animacao.SetBool("IsClimbing", true);
            }
        }
    }

    private void FixedUpdate()
    {
        if (playerMoviment != null && playerMoviment.isDashing) return;

        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, vertical * speed);
        }
        else
        {
            rb.gravityScale = 4.5f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;
            if (playerMoviment == null)
            {
                playerBebe_Moviment.touching.groundDistancia = 0.02f;
            }
            else if (playerBebe_Moviment == null)
            {
                playerMoviment.touching.groundDistancia = 0.02f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
            if (playerMoviment == null)
            {
                playerBebe_Moviment.touching.groundDistancia = 0.36f;
                playerBebe_Moviment.animacao.SetBool("IsClimbing", false);
            }
            else if (playerBebe_Moviment == null)
            {
                playerMoviment.touching.groundDistancia = 0.36f;
                playerMoviment.animacao.SetBool("IsClimbing", false);
            }
        }
    }
}
