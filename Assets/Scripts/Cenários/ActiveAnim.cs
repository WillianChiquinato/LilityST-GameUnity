using UnityEngine;

public class ActiveAnim : MonoBehaviour
{
    [Header("Configurações do Raycast")]
    public Vector3 offset;
    public Vector3 offset2;
    public Vector3 offset3;
    public float rayDistance = 3f;
    public Vector2 direcaoRaycast = Vector2.left;


    [Header("Configuracoes normais")]
    public Animator animator;
    private PlayerMoviment player;
    public string[] tagToCheck = new string[0];
    public string[] layersToCheck = new string[0];
    public string animationTrigger = "";
    public bool actionActive = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = FindFirstObjectByType<PlayerMoviment>();
    }

    void Update()
    {
        if (CompareTag("Vidro"))
        {
            // Cria um array com os três offsets.
            Vector3[] offsets = new Vector3[] { offset, offset2, offset3 };

            foreach (Vector3 off in offsets)
            {
                RaycastHit2D hit = Physics2D.Raycast(
                    transform.position + off,
                    direcaoRaycast,
                    rayDistance,
                    layersToCheck.Length > 0 ? LayerMask.GetMask(layersToCheck) : Physics2D.AllLayers
                );

                Debug.DrawRay(transform.position + off, direcaoRaycast * rayDistance, Color.red);

                if (hit.collider != null)
                {
                    PlayerMoviment player = hit.collider.GetComponent<PlayerMoviment>();
                    ItemObject item = hit.collider.GetComponent<ItemObject>();

                    if (item != null)
                    {
                        Debug.LogWarning("Item detected");
                    }
                    if (player != null)
                    {
                        Debug.LogWarning("Player detected");
                    }

                    if (player != null && player.isDashing || item != null)
                    {
                        actionActive = true;
                        GetComponent<BoxCollider2D>().enabled = false;
                        animator.SetTrigger(animationTrigger);
                        Destroy(gameObject, 1f);
                        break;
                    }
                }
            }
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colidiu com algo");
        //Player logic.
        foreach (string tag in tagToCheck)
        {
            if (collision.gameObject.CompareTag(tag))
            {
                if (this.gameObject.CompareTag("Vidro"))
                {
                    Debug.Log("Logica no update");
                    return;
                }

                // Lógica para outros objetos.
                Debug.Log("Colidiu com " + tag);
                animator.SetTrigger(animationTrigger);
            }
        }
    }
}
