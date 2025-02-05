using UnityEngine;

public class vidroScript : MonoBehaviour
{
    public Animator animator;
    public Transform Objeto;
    private SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider2D;
    public bool GeneralActived = false;
    public static bool boxCollider2DActive = false;


    void Awake()
    {
        animator = GetComponent<Animator>();
        Objeto = GameObject.FindFirstObjectByType<PlayerMoviment>().transform;
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

        GameObject Objeto = GameObject.FindFirstObjectByType<GameObject>();
        if (Objeto != null)
        {
            if (Objeto.transform.position.x < transform.position.x)
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
        if (GeneralActived)
        {
            if (colisor.gameObject.CompareTag("Player"))
            {
                Debug.Log("Inimigo SPAWN");
                animator.SetBool("Vidro", true);
                boxCollider2DActive = true;
                boxCollider2D.enabled = false;
            }
        }
    }


}
