using UnityEngine;

public class vidroScript : MonoBehaviour
{
    //Vai sumir logo logo, apenas o general.
    public GameObject prefabEnemy;
    public GameObject prefabGeneral;
    public GameObject spawnSoldado;
    public GameObject spawnGeneral;

    public Animator animator;
    public Transform player;
    private SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider2D;
    public static bool boxCollider2DActive = false;


    void Awake()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindFirstObjectByType<PlayerMoviment>().transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        if (boxCollider2DActive)
        {
            boxCollider2D.enabled = false;
        }

    }

    void Update()
    {
        int order = 1;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            if (player.transform.position.x < transform.position.x)
            {
                order = Mathf.Max(order, 20);
            }
            else
            {
                order = Mathf.Min(order, -1);
            }
        }

        spriteRenderer.sortingOrder = order;
    }

    private void OnTriggerEnter2D(Collider2D colisor)
    {
        if (colisor.gameObject.CompareTag("Player"))
        {
            Debug.Log("Inimigo SPAWN");
            animator.SetBool("Vidro", true);
            Instantiate(prefabEnemy, spawnSoldado.transform.position, Quaternion.identity);
            Instantiate(prefabGeneral, spawnGeneral.transform.position, Quaternion.identity);
            boxCollider2DActive = true;
            boxCollider2D.enabled = false;
        }
    }


}
