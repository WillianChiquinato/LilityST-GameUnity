using System.Collections.Generic;
using UnityEngine;

public class Throw_Item : MonoBehaviour
{
    public ItemData itemData;

    public Animator animator;
    public GameObject prefabItem;
    public GameObject[] itemArremessar;
    public int stackObjetos;
    public List<Inventory_item> itensColetaveis;
    public GameObject newDrop;
    public bool arremessar;
    public bool TimerArremessar;
    public float ForceArremesso;

    void Start()
    {
        animator = GetComponent<Animator>();

        itensColetaveis = inventory_System.instance.coletaveis;
    }

    void Update()
    {
        if (inventory_System.instance != null)
        {
            itensColetaveis = inventory_System.instance.coletaveis;

            if (itensColetaveis != null && itensColetaveis.Count > 0)
            {
                itemArremessar = new GameObject[itensColetaveis.Count];

                for (int i = 0; i < itensColetaveis.Count; i++)
                {
                    itemData = itensColetaveis[i].itemData;
                    prefabItem.GetComponent<itemObject>().itemData = itemData;
                    // Certifica-se de que cada posição do array é inicializada
                    itemArremessar[i] = prefabItem;
                    stackObjetos = itensColetaveis[0].stackSize;
                }
            }
            else
            {
                Debug.LogWarning("Nenhum item coletável encontrado no inventário.");
            }
        }

        arremessar = Input.GetKeyDown(KeyCode.Space);

        if (itemArremessar != null && stackObjetos > 0)
        {
            if (arremessar && newDrop == null)
            {
                animator.SetBool("Arremessar", true);
            }

            if (TimerArremessar)
            {
                if (newDrop == null)
                {
                    newDrop = Instantiate(itemArremessar[0], transform.position, Quaternion.identity);
                    inventory_System.instance.RemoveItem(itensColetaveis[0].itemData);

                    Rigidbody2D rb = newDrop.GetComponent<Rigidbody2D>();
                    float direction = transform.localScale.x > 0 ? 1 : -1;

                    // Aplica uma força para cima e para frente
                    Vector2 force = new Vector2(ForceArremesso * direction, ForceArremesso * 1f);
                    rb.AddForce(force, ForceMode2D.Impulse);
                    animator.SetBool("Arremessar", false);
                    Destroy(newDrop, 7f);
                }
            }

            if (newDrop != null)
            {
                Rigidbody2D rb = newDrop.GetComponent<Rigidbody2D>();

                if (rb.linearVelocity.magnitude < 0.1f)
                {
                    newDrop.tag = "ItemArremessar";
                    newDrop = null;
                }
            }
        }
        else
        {
            Debug.Log("Nenhum item coletável encontrado no inventário.");
        }
    }
}
