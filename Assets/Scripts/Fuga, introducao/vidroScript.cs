using UnityEngine;

public class vidroScript : MonoBehaviour
{
    public GameObject prefabEnemy;
    public GameObject spawnSoldado;
    public Animator animator;
    public Transform player;
    private SpriteRenderer spriteRenderer;


    void Awake()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindFirstObjectByType<PlayerMoviment>().transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        }
    }


}
