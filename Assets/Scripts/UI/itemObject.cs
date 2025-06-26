using TMPro;
using UnityEngine;

public class itemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TextMeshPro texto;
    [SerializeField] public ItemData itemData;
    [SerializeField] public FragmentoData fragmentoData;
    public PlayerMoviment playerMoviment;

    void Awake()
    {
        texto = GetComponentInChildren<TextMeshPro>();
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        SetupVisual();
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
        if (itemData != null)
        {
            Debug.Log("Pegou item: " + itemData.ItemName);
            inventory_System.instance.AddItem(itemData);
        }
        else if (fragmentoData != null)
        {
            Debug.Log("Pegou fragmento: " + fragmentoData.NomeFragmento);
            FragmentoSystem.instance.AddItem(fragmentoData);
        }
        else
        {
            Debug.LogWarning("Nenhum dado para pegar.");
            return;
        }

        Destroy(this.gameObject);
        GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManagerInteract.Instance.interactIcon.GetComponent<IconIdle>().startPosition = transform.position + new Vector3(0, 1.2f, 0);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);
        }
    }
}