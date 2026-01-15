using TMPro;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public bool isItemPegado = false;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TextMeshPro texto;
    [SerializeField] public ItemData itemData;
    [SerializeField] public FragmentoData fragmentoData;
    public bool itemIsGrounded;
    private LayerMask groundLayer;

    void Awake()
    {
        texto = GetComponentInChildren<TextMeshPro>();
        groundLayer = LayerMask.GetMask("Ground");
        SetupVisual();
    }

    void Update()
    {
        itemIsGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
    }

    private void SetupVisual()
    {
        if (itemData != null)
        {
            GetComponent<SpriteRenderer>().sprite = itemData.Icon;
            gameObject.name = "Item - " + itemData.ItemName;
        }
        else if (fragmentoData != null)
        {
            GetComponent<SpriteRenderer>().sprite = fragmentoData.Icon;
            gameObject.name = "Fragmento - " + fragmentoData.NomeFragmento;
        }
        else
        {
            Debug.LogWarning("itemObject não possui dados de item ou fragmento.");
        }
    }

    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.linearVelocity = _velocity;
        SetupVisual();
    }

    public void SetupFragmento(FragmentoData _fragmentoData, Vector2 _velocity)
    {
        fragmentoData = _fragmentoData;
        rb.linearVelocity = _velocity;
        SetupVisual();
    }

    public void PickUpItem()
    {
        GameManagerInteract.Instance.interactIcon.transform.SetParent(GameManagerInteract.Instance.transform);
        GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);
        GetComponent<RicocheteItens>().countBatidas = 0;

        isItemPegado = true;
        if (itemData != null)
        {
            inventory_System.instance.AddItem(itemData);
        }
        else if (fragmentoData != null)
        {
            FragmentoSystem.instance.AddItem(fragmentoData);
        }
        else
        {
            return;
        }

        Destroy(this.gameObject);
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