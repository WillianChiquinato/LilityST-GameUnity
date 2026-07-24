using System.Collections;
using TMPro;
using UnityEngine;

public class ItemFragment : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rb;
    private Animator animator;
    [SerializeField] public FragmentoData fragmentoData;
    [SerializeField] private ParticleSystem idleParticleInstance;
    private LayerMask groundLayer;

    [Header("Components")]
    public bool isItemPegado = false;
    public bool itemIsGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        groundLayer = LayerMask.GetMask("Ground");
        SetupVisual();
    }

    void Update()
    {
        itemIsGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
    }

    private void SetupVisual()
    {
        if (fragmentoData != null)
        {
            GetComponent<SpriteRenderer>().sprite = fragmentoData.Icon;
            gameObject.name = "Fragmento - " + fragmentoData.NomeFragmento;

            if (fragmentoData.animatorController != null && animator != null)
            {
                animator.runtimeAnimatorController = fragmentoData.animatorController;
            }

            //if (fragmentoData.particulaIdlePrefab != null)
            //{
            //    Instantiate(fragmentoData.particulaIdlePrefab, transform);
            //}
        }
        else
        {
            Debug.LogWarning("itemObject não possui dados de item ou fragmento.");
        }
    }

    public void SetupFragmento(FragmentoData _fragmentoData, Vector2 _velocity)
    {
        fragmentoData = _fragmentoData;
        rb.linearVelocity = _velocity;
        SetupVisual();
    }

    public void PickUpItem()
    {
        if (isItemPegado) return;

        GameManagerInteract.Instance.interactIcon.transform.SetParent(GameManagerInteract.Instance.transform);
        GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);

        isItemPegado = true;

        if (fragmentoData == null)
            return;

        FragmentoSystem.instance.AddItem(fragmentoData);

        //if (fragmentoData.particulaPegarPrefab != null)
        //    Instantiate(fragmentoData.particulaPegarPrefab, transform.position, Quaternion.identity);

        // Desliga colisor/física pra não interagir de novo enquanto anima
        GetComponent<Collider2D>().enabled = false;
        if (rb != null) rb.simulated = false;

        if (animator != null)
        {
            animator.SetTrigger("Pickup");
            StartCoroutine(DestroyAfterAnimation());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        // espera o Animator entrar no state de Pickup
        yield return null;
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName("Pickup"));

        // espera a duração da animação terminar
        yield return new WaitForSeconds(
            animator.GetCurrentAnimatorStateInfo(0).length);

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && itemIsGrounded)
        {
            GameManagerInteract.Instance.interactIcon.transform.SetParent(transform);
            GameManagerInteract.Instance.interactIcon.GetComponent<IconIdle>().startPosition = transform.position + new Vector3(0, 1.2f, 0);
            GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManagerInteract.Instance.interactIcon.transform.SetParent(GameManagerInteract.Instance.transform);
            GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);
        }
    }

    void OnDrawGizmos()
    {
        //IsGround.
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector2.down * 1f);
    }
}
