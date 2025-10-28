using UnityEngine;

public class RobertMoviment : MonoBehaviour
{
    public Animator animacao;
    public Rigidbody2D rb;
    public PlayerMoviment playerMovement;
    [HideInInspector] public bool lilithChecked = false;
    [HideInInspector] public bool isMoving = false;
    public float distanceToLilith;
    public float distanceToFinalPoint;
    public float speed = 2f;

    public GameObject pointToRun;
    public bool pointToRendalla;

    public bool CanMove
    {
        get => animacao.GetBool(animationstrings.canMove);
        set => animacao.SetBool(animationstrings.canMove, value);
    }

    void Start()
    {
        animacao = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GameObject.FindFirstObjectByType<PlayerMoviment>();
    }

    void Update()
    {
        if (!CanMove)
        {
            FacePlayer();
            return;
        }

        distanceToLilith = Vector2.Distance(transform.position, playerMovement.transform.position);
        distanceToFinalPoint = Mathf.Abs(transform.position.x - pointToRun.transform.position.x);

        if (distanceToLilith < 7f)
        {
            lilithChecked = true;
        }

        if (lilithChecked)
        {
            MoveTowardsPoint();
        }
        else
        {
            MoveTowardsPlayer();
        }

        if (distanceToFinalPoint < 1f)
        {
            CanMove = false;
            pointToRendalla = true;
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (playerMovement.transform.position - transform.position).normalized;
        Vector2 newPos = rb.position + direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    void MoveTowardsPoint()
    {
        Vector2 direction = (pointToRun.transform.position - transform.position).normalized;
        Vector2 newPos = rb.position + direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
        transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    public void FacePlayer()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
