using UnityEngine;

public class ladderScript : MonoBehaviour
{
    private float vertical;
    public float speed = 8f;
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
            if (isLadder)
            {
                if (Mathf.Abs(vertical) > 0f)
                {
                    isClimbing = true;
                }
            }
        }
        else if (playerBebe_Moviment == null)
        {
            if (isLadder)
            {
                if (Mathf.Abs(vertical) > 0f)
                {
                    isClimbing = true;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (playerMoviment != null && playerMoviment.isDashing) return;

        if (playerMoviment == null)
        {
            if (isClimbing)
            {
                playerBebe_Moviment.animacao.SetBool("IsClimbing", true);
                rb.gravityScale = 0f;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, vertical * speed);
                if (Mathf.Abs(vertical) > 0f)
                {
                    playerBebe_Moviment.animacao.speed = 1f;
                }
                else if (!playerBebe_Moviment.touching.IsGrouded)
                {
                    playerBebe_Moviment.animacao.speed = 0f;
                }
                else
                {
                    playerBebe_Moviment.animacao.speed = 1f;
                }
            }
            else
            {
                playerBebe_Moviment.animacao.SetBool("IsClimbing", false);
                playerBebe_Moviment.animacao.speed = 1f;
                rb.gravityScale = 2f;
            }
        }
        else if (playerBebe_Moviment == null)
        {
            if (isClimbing)
            {
                playerMoviment.animacao.SetBool("IsClimbing", true);
                rb.gravityScale = 0f;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, vertical * speed);
                if (Mathf.Abs(vertical) > 0f)
                {
                    playerMoviment.animacao.speed = 1f;
                }
                else if (!playerMoviment.touching.IsGrouded)
                {
                    playerMoviment.animacao.speed = 0f;
                }
                else
                {
                    playerMoviment.animacao.speed = 1f;
                }
            }
            else
            {
                playerMoviment.animacao.SetBool("IsClimbing", false);
                playerMoviment.animacao.speed = 1f;
                rb.gravityScale = 4.5f;
            }
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
